using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class TarjetasController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        public JsonResult Obtener()
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                List<tarjetas> tar = db.tarjetas.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V")).ToList();
                return Json(new { respuesta = "EXITO: Petici\u00F3n exitosa", lista = tar}, JsonRequestBehavior.AllowGet); 
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
    }
}
