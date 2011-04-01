using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using CourseSampleApp.Models;
using NHibernate.Criterion;
using NHibernate.Linq;
using Expression = NHibernate.Criterion.Expression;

namespace CourseSampleApp.Controllers
{
	public class LibraryController : SessionController
	{
		public ActionResult New(string name)
		{
			var lib = new Library { Name = name };

			Session.Save(lib);

			return Json(new { LibraryId = lib.Id }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Modify(int bookId)
		{
			var book = Session.Get<Book>(bookId);

			book.Name = DateTime.Now.ToShortTimeString();


			return Json(new { Status = "OK"}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult CrateLoad(int size)
		{
			for (int i = 0; i < size; i++)
			{
				Session.Save(new Library
				{
					Name = "Library #" + i
				});
			}
			return Json(new {Status = "Done"}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult BookIt(int id, string name, string isbn, string grade)
		{
			var book = new Book
			{
				Name = name,
				Library = Session.Load<Library>(id),
				//Attributes =
				//    {
				//        ISBN = isbn,
				//        Grade = grade
				//    }
			};

			Session.Save(book);

			return Json(new { Status = "Done" }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult SwitchCase()
		{
			foreach (var library in Session.Query<Library>())
			{
				if (char.IsUpper(library.Name[0]))
					library.Name = library.Name.ToLower();
				else
					library.Name = library.Name.ToUpper();
			}
			
			return Json(new { Status = "Done" }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult LendingHistory(int bookId, int start, int pageSize)
		{
			var book = Session.Get<Book>(bookId);
			var bookLoans = Session.CreateFilter(book.Loans, "where this.LendingPeriod.DueDate < :now")
				.SetParameter("now", DateTime.Today)
				.SetMaxResults(pageSize)
				.SetFirstResult(start)
				.List()
				.OfType<BookLoan>();

			return Json(new
			{
				book.Name,
				Lenders = bookLoans.Select(x => x.LendingPeriod).ToArray()
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Books()
		{
			// this is actually filtered!
			// see CurrentLibrarySessionFilter

			var q = from book in Session.Query<Book>()
					select book.Name;

			return Json(new { Books = q.ToArray() }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult UsersReading(int bookId)
		{
			var usersReadingSpecifiedBook = DetachedCriteria.For<BookLoan>()
				.Add(Restrictions.Eq("Book.id", bookId))
				.SetProjection(Projections.Property("Member.id"));

			var q = Session.CreateCriteria<Member>()
				.Add(Subqueries.PropertyIn("id", usersReadingSpecifiedBook))
				.SetProjection(Projections.Property("Name"))
				.List();

			// not good beacuse it forces a join and may result
			// in duplicate items

			//var q = from loan in Session.Query<BookLoan>()
			//        where loan.Book.Id == bookId
			//        select loan.Member.Name;

			return Json(new
			{
				Readers = q
			}, JsonRequestBehavior.AllowGet);


		}

		public ActionResult Members(int id)
		{
			//var q = Session.CreateQuery("select m.Name from Member m where m.Library = :libraryId")
			//    .SetParameter("libraryId", 1)
			//    .List();

			//var q = (from member in Session.Query<Member>()
			//         where member.Library.Id == id
			//         select member.Name).ToArray();

			var q = Session.CreateCriteria<Member>()
				.Add(Restrictions.Eq("Library.id", id))
				.SetProjection(Projections.Property("Name"))
				.List();

			return Json(new
			{
				LibraryId = id,
				Members = q
			}, JsonRequestBehavior.AllowGet);

		}

		public ActionResult Shutdown(int id)
		{
			// because of cascades, result in a lot of queries
			// we don't really care, shutdowns are rare, and if they aren't
			// someone should be made to regret it.
			Session.Delete(Session.Get<Library>(id));

			return Json(new { Message = "Library deleted, we hate you!" }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ShutdownQuickly(int id)
		{
			Session.CreateQuery("delete BookLoan bl where bl.Book in (from Book b where b.Library = :libraryId)")
				.SetParameter("libraryId", id)
				.ExecuteUpdate();

			Session.CreateQuery("delete Book b where b.Library = :libraryId")
				.SetParameter("libraryId", id)
				.ExecuteUpdate();

			Session.CreateQuery("delete CourseSampleApp.Models.Member m where m.Library = :libraryId")
				.SetParameter("libraryId", id)
				.ExecuteUpdate();

			Session.CreateQuery("delete Library m where m.Id = :libraryId")
				.SetParameter("libraryId", id)
				.ExecuteUpdate();

			return Json(new { Message = "Library deleted, we hate you!" }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Join(int id, string memberName)
		{
			var member = new Member
			{
				Name = memberName,
				Library = Session.Load<Library>(id),
				Email = memberName +"@" + memberName + ".com",
				Attributes = new Hashtable
				{
					{"JoinDate", DateTime.Today},
					{"MembershipType", "Standard"}
				}
			};
			Session.Save(member);

			return Json(new { MemberId = member.Id }, JsonRequestBehavior.AllowGet);

		}

		public ActionResult Acquire(int id, string bookName)
		{
			var book = new Book
			{
				Name = bookName,
				Library = Session.Load<Library>(id),
			};
			Session.Save(book);

			return Json(new { BookId = book.Id }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Rent(int memberId, int bookId)
		{
			var bookLoan = new BookLoan
			{
				LendingPeriod = new LendingPeriod
				{
					DueDate = DateTime.Today.AddDays(7),
					LoanDate = DateTime.Today
				},
				Book = Session.Load<Book>(bookId),
				Member = Session.Load<Member>(memberId)
			};

			Session.Save(bookLoan);

			return Json(new { BookLoanId = bookLoan.Id }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Status(int id)
		{
			var library = Session.Load<Library>(id);
			var bookLoans = Session.Query<BookLoan>()
				.Cacheable()
				.Fetch(x => x.Member)
				.Fetch(x => x.Book)
				.Where(bl => bl.Member.Library.Id == id)
				.ToArray();

			// complexity is
			// O( numOfbooks * numOfBookLoans + numOfMembers * numOfBookLoans);
			// ALL in memory, no database access below this line!
			// Much better than anything else, since anything else will require
			// database access for queries.

			// obviously, not an approach to be used with potentially large data sets
			// if you have more than 100 items or so, use paging or your application will
			// die and the database would fall over and cry.
			return Json(new
			{
				library.Name,
				Books = library.Books.Select(b => new
				{
					b.Name,
					LoanedTo = bookLoans.Where(x => x.Book == b)
													.Select(bl => new
													{
														bl.Member.Name,
														bl.LendingPeriod
													})
				}).ToArray(),
				Members = library.Members.Select(m => new
				{
					m.Name,
					Read = bookLoans.Where(x => x.Member == m).Select(loan => new
					{
						loan.Book.Name,
						ReadAt = loan.LendingPeriod.LoanDate
					}).ToArray()
				}).ToArray(),
			}, JsonRequestBehavior.AllowGet);

		}
	}
}