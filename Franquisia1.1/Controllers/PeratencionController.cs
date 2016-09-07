    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class PeratencionController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        [HttpPost]
        public JsonResult Cambiar(string codconc, string codper)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol) ) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                peratencion peratencion = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.codigo.Equals(codper) && a.situa.Equals("V")).FirstOrDefault();
                if (peratencion == null) { return Json(new { respuesta = "ERROR: No existe el c\u00F3digo del personal de atenci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                else{
                    conc conc = db.conc.Where(a => a.CODIGO.Equals(codconc)).FirstOrDefault();
                    if (conc == null) { return Json(new { respuesta = "ERROR: No existe el c\u00F3digo del consumo" }, JsonRequestBehavior.AllowGet); }
                    else{
                        conc.PERATENCION = peratencion.codigo;
                        db.SaveChanges();
                        return Json(new { respuesta = "EXITO: Personal de atenci\u00F3n cambiado" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
    }
}
