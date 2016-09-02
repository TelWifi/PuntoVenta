using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class SucursalController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        [HttpPost]
        public JsonResult obtener(string codcia, string idusr)
        {
            try
            {
                List<string> sucusr = db.sucusr.Where(a => a.codcia.Equals(codcia) && a.idusr.Equals(idusr)).Select(b => b.sucursal).ToList();
                List<sucursal> suc = db.sucursal.Where(c=>c.codcia.Equals(codcia) && sucusr.Contains(c.codigo)).ToList();
                return Json(new { respuesta = "EXITO: Exito", lista=suc }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
    }
}
