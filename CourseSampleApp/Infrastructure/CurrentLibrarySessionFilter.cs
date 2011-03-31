using System;
using System.Web.Mvc;
using CourseSampleApp.Controllers;

namespace CourseSampleApp.Infrastructure
{
	public class CurrentLibrarySessionFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var sessionController = filterContext.Controller as SessionController;
			if (sessionController == null)
				return;

			object libraryId;
			if (filterContext.RouteData.Values.TryGetValue("id", out libraryId))
			{
				sessionController.Session.EnableFilter("CurrentLibrary")
					.SetParameter("libraryId", Convert.ToInt32(libraryId));
			}
		}
	}
}