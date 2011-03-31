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
			sessionController.Session.BeginTransaction();

		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var sessionController = filterContext.Controller as SessionController;

			if (sessionController == null)
				return;

			using (var session = sessionController.Session)
			{
				if (session == null)
					return;

				if (!session.Transaction.IsActive) 
					return;

				if (filterContext.Exception != null)
					session.Transaction.Rollback();
				else
					session.Transaction.Commit();
			}
		}
	}
}