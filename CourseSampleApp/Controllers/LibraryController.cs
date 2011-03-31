using System;
using System.Linq;
using System.Web.Mvc;
using CourseSampleApp.Models;
using NHibernate.Linq;

namespace CourseSampleApp.Controllers
{
	public class LibraryController : SessionController
	{
		public ActionResult New(string name)
		{
			var lib = new Library {Name = name};

			Session.Save(lib);

			return Json(new {LibraryId = lib.Id}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Shutdown(int id)
		{
			// because of cascades, result in a lot of queries
			// we don't really care, shutdowns are rare, and if they aren't
			// someone should be made to regret it.
			Session.Delete(Session.Get<Library>(id));

			return Json(new { Message = "Library deleted, we hate you!"}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Join(int id, string memberName)
		{
			var member = new Member
			{
				Name = memberName,
				Library = Session.Load<Library>(id),
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
				DueDate = DateTime.Today.AddDays(7),
				LoanDate = DateTime.Today,
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
				.Fetch(x=>x.Member)
				.Fetch(x=>x.Book)
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
				                                  		bl.LoanDate
				                                  	})
				}).ToArray(),
				Members = library.Members.Select(m => new
				{
					m.Name,
					Read = bookLoans.Where(x => x.Member == m).Select(loan => new
					{
						loan.Book.Name,
						ReadAt = loan.LoanDate
					}).ToArray()
				}).ToArray(),
			}, JsonRequestBehavior.AllowGet);

		}
	}
}