using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class PunemiController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        [HttpPost]
        public ActionResult Cambiar(string codigo)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                
                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                punemi punemi = db.punemi.Where(a =>a.codcia.Equals(codcia) && a.sucursal.Equals(sucursal) && a.codigo.Equals(codigo) && a.situa.Equals("V")).FirstOrDefault();
                if (punemi == null) { return Json(new { respuesta = "ERROR: El punto de emisi\u00F3n no existe" }, JsonRequestBehavior.AllowGet); }
                else{
                    Session["Loged_usrfile_punemi"] = punemi.codigo;
                    return Json(new { respuesta = "EXITO: Punto de emisi\u00F3 cambiado" }, JsonRequestBehavior.AllowGet); 
                }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
    }
}
