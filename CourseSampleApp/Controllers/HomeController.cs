using System.Web.Mvc;
using CourseSampleApp.Models;
using System.Linq;

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
				Users = blog.Users.Select(user => new
				{
					user.Id,
					user.Email,
					user.Username,
					user.Bio
				}).ToArray()
			}, JsonRequestBehavior.AllowGet);
		}
	}
}