using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Error(string  msg_ex)
        {
            return View(msg_ex);
        }
        public ActionResult ErrorBD(string msg_ex)
        {
            return View(msg_ex);
        }
        public ActionResult ErrorPermiso()
        {
            return View();
        }
    }
}
