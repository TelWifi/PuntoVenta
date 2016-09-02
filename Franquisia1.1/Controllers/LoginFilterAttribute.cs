using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace Franquisia1._1.Controllers
{
    public class LoginFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Se ejecuta ANTES de que empiece a ejecutarse el controlador
            String metodo = filterContext.ActionDescriptor.ActionName;
            String controlador = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            if ((!controlador.Equals("Home") && !metodo.Equals("Login"))  &&
                (!controlador.Equals("Sucursal") && !metodo.Equals("Obtener")) )
            {

                if (HttpContext.Current.Session["Loged_usrfile_idusr"] == null || HttpContext.Current.Session["Loged_usrfile_typeusr"] == null
                    || HttpContext.Current.Session["Loged_usrfile_sucursal"] == null || HttpContext.Current.Session["Loged_usrfile_ciafile"] == null
                    || HttpContext.Current.Session["Loged_usrfile_rol"] == null)
                {
                    HttpContext.Current.Session["msg_error"] = "Debe iniciar Sesión";
                    filterContext.Result = new RedirectToRouteResult( new RouteValueDictionary {
                                    { "Controller", "Home" },
                                    { "Action", "OffLogin" }
                    });
                }
            }
            base.OnActionExecuting(filterContext);
        }

    }
}
