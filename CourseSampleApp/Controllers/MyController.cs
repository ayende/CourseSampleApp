using System.Linq;
using System.Web.Mvc;
using CourseSampleApp.Models;
using NHibernate.Linq;

namespace CourseSampleApp.Controllers
{
	public class MyController : SessionController
	{
		public ActionResult Blog(int id)
		{
			var blog = Session.Get<Blog>(id);
			return Json(new
			{
				blog.Id,
				blog.Subtitle,
				blog.Title
			}, JsonRequestBehavior.AllowGet);
		}


		public ActionResult Loading(int id)
		{
			Session.Load<Blog>(id);
			return Content("Done");
		}

		public ActionResult Getting(int id)
		{
			Session.Get<Blog>(id);
			return Content("Done");
		}

		public ActionResult QueryVsGet1(int id)
		{
			Session.Get<Blog>(id);
			Session.Get<Blog>(id);
			return Content("Done");
		}

		public ActionResult QueryVsGet2(int id)
		{
			Session.Query<Blog>().Where(x => x.Id == id).SingleOrDefault();
			Session.Query<Blog>().Where(x => x.Id == id).SingleOrDefault();
			return Content("Done");
		}
		public ActionResult AddUser(int blogId, int userId)
		{
			var b = Session.Get<Blog>(blogId);
			b.Users.Add(Session.Load<User>(userId));
			return Content("Done");
		}

		public ActionResult Name(string id)
		{
			var q = (from blog in Session.Query<Blog>()
			         where blog.Title == id
			         select blog
			        ).ToArray();

			return Json(
				q.Select(b => new
				{
					b.Id,
					b.Subtitle,
					b.Title
				}), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Complex(string name, string subtitle)
		{
			var q = (from blog in Session.Query<Blog>()
					 where blog.Title == name && blog.Subtitle == subtitle
					 select blog
					).ToArray();

			return Json(
				q.Select(b => new
				{
					b.Id,
					b.Subtitle,
					b.Title
				}), JsonRequestBehavior.AllowGet);
		}


		public ActionResult AssociatedUser(string name)
		{
			var q = (from blog in Session.Query<Blog>()
					 where blog.Users.Any(x=>x.Username == name)
					 select blog
					).ToArray();

			return Json(
				q.Select(b => new
				{
					b.Id,
					b.Subtitle,
					b.Title
				}), JsonRequestBehavior.AllowGet);
		}
	}
}