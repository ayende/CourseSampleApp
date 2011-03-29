using System.Web;
using System.Web.Mvc;
using CourseSampleApp.Infrastructure;
using NHibernate;

namespace CourseSampleApp.Controllers
{
	public class SessionController : Controller
	{
		public HttpSessionStateBase HttpSession
		{
			get { return base.Session; }
		}

		public new ISession Session { get; set; }
	}
}