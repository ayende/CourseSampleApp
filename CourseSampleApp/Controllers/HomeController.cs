using System.Web.Mvc;
using CourseSampleApp.Models;

namespace CourseSampleApp.Controllers
{
	public class HomeController : SessionController
	{
		public ActionResult Blog(int id)
		{
			var blog = Session.Get<Blog>(id);

			return Json(blog, JsonRequestBehavior.AllowGet);
		}
	}
}