using System.Web;
using System.Web.Mvc;
using CourseSampleApp.Controllers;
using NHibernate;
using NHibernate.Cfg;

namespace CourseSampleApp.Infrastructure
{
	public class NHibernateActionFilter : ActionFilterAttribute
	{
		private static readonly ISessionFactory sessionFactory = BuildSessionFactory();

		private static ISessionFactory BuildSessionFactory()
		{
			return new Configuration()
				.Configure()
				.BuildSessionFactory();
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var sessionController = filterContext.Controller as SessionController;

			if (sessionController == null)
				return;

			sessionController.Session = sessionFactory.OpenSession();
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var sessionController = filterContext.Controller as SessionController;

			if (sessionController == null)
				return;
			
			var session = sessionController.Session;
			if (session != null)
			{
				session.Dispose();
			}
		}
	}
}