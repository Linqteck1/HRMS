using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSWeb.Models
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var urlHelper = new UrlHelper(filterContext.RequestContext);
            if (HttpContext.Current.Session["CRM_Session"] != null)
            {

                Session_CRM sess = (Session_CRM)HttpContext.Current.Session["CRM_Session"];

                var rd = HttpContext.Current.Request.RequestContext.RouteData;
                string currentController = rd.GetRequiredString("controller");
                var action = filterContext.ActionDescriptor.ActionName;
                var Allow = sess.AllPermissions.Where(x => x.AT_Pages.Controller == currentController && x.AT_PermissionActionJunc.Where(y => y.Action == action).Count() > 0).FirstOrDefault();

                HttpContext.Current.Session["CRM_Session"] = sess;
                if (Allow == null)
                {
                    if (!filterContext.HttpContext.Request.IsAjaxRequest())
                        filterContext.Result = new RedirectResult("~/Error/Permission");
                    else
                    {
                        filterContext.HttpContext.Response.StatusCode = 403;
                        filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        Error = "NotAuthorized",
                        LogOnUrl = urlHelper.Action("Permission", "Error")
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                    }


                }

            }
            else
            {

                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                    filterContext.Result = new RedirectResult("~/Login/Index");
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            Error = "SessionExpire",
                            LogOnUrl = urlHelper.Action("Index", "Login")
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }




            }

            base.OnActionExecuting(filterContext);
        }

      
      
    }
}