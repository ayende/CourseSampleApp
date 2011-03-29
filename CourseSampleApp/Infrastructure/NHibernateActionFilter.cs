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

		public static ISession CurrentSession
		{
			get { return HttpContext.Current.Items["NHibernateSession"] as ISession; }
			set { HttpContext.Current.Items["NHibernateSession"] = value; }
		}

		private static ISessionFactory BuildSessionFactory()
		{
			return new Configuration()
				.Configure()
				.BuildSessionFactory();
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			CurrentSession = sessionFactory.OpenSession();
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{

			var session = CurrentSession;
			if (session != null)
			{
				session.Dispose();
			}
		}
	}
}