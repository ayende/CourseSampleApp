using System.Web.Mvc;
using CourseSampleApp.Models;

namespace CourseSampleApp.Controllers
{
	public class HomeController : SessionController
	{
		public ActionResult Blog(int id)
		{
			var blog = Session.Get<Blog>(id);

			return Json(new
			{
				blog.AllowsComments,
				blog.CreatedAt,
				blog.Id,
				blog.Subtitle,
			}, JsonRequestBehavior.AllowGet);
		}
	}
}