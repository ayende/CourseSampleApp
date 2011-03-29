using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NHibernate;
using NHibernate.Cfg;

namespace CourseSampleApp
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		private static readonly ISessionFactory sessionFactory = BuildSessionFactory();

		public static ISession CurrentSession
		{
			get{ return HttpContext.Current.Items["NHibernateSession"] as ISession;}
			set { HttpContext.Current.Items["NHibernateSession"] = value; }
		}

		public MvcApplication()
		{
			BeginRequest += (sender, args) =>
			{
				CurrentSession = sessionFactory.OpenSession();
			};
			EndRequest += (o, eventArgs) =>
			{
				var session = CurrentSession;
				if (session != null)
				{
					session.Dispose();
				}
			};
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		private static ISessionFactory BuildSessionFactory()
		{
			return new Configuration()
				.Configure()
				.BuildSessionFactory();
		}
	}
}