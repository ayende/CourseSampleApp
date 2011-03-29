using System.Web;
using System.Web.Mvc;
using NHibernate;

namespace CourseSampleApp.Controllers
{
	public class SessionController : Controller
	{
		public HttpSessionStateBase HttpSession
		{
			get { return base.Session;  }
		}

		public new ISession Session
		{
			get { return MvcApplication.CurrentSession; }
		}
	}
}