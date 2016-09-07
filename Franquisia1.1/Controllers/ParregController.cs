using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class ParregController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        
        public JsonResult ObtenerIGV()
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if ("C".Equals(rol) || "M".Equals(rol))
                {
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                    string punemi = Session["Loged_usrfile_punemi"].ToString();
                    parreg prg = db.parreg.Where(a => a.IDCIA.Equals(codcia) && a.FORM.Equals("COM")).FirstOrDefault();
                    if (prg == null) { return Json(new { respuesta = "ERROR: El par\u00E1metro IGV no existe" }, JsonRequestBehavior.AllowGet); }
                    return Json(new { respuesta = "EXITO: ", igv = prg.COM_TASA_IGV }, JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult TipDocDefault()
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if ("C".Equals(rol) || "M".Equals(rol))
                {
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                    string punemi = Session["Loged_usrfile_punemi"].ToString();
                    parreg prg = db.parreg.Where(a => a.IDCIA.Equals(codcia) && a.FORM.Equals("POS")).FirstOrDefault();
                    if (prg == null) { return Json(new { respuesta = "ERROR: El par\u00E1metro IGV no existe" }, JsonRequestBehavior.AllowGet); }
                    return Json(new { respuesta = "EXITO: ", codigo = prg.POS_TIPDOC_DEFAULT }, JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult TiempoRefrescoPanel()
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if ("C".Equals(rol) || "M".Equals(rol))
                {
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                    string punemi = Session["Loged_usrfile_punemi"].ToString();
                    parreg prg = db.parreg.Where(a => a.IDCIA.Equals(codcia) && a.FORM.Equals("POS")).FirstOrDefault();
                    if (prg == null) { return Json(new { respuesta = "ERROR: El par\u00E1metro IGV no existe" }, JsonRequestBehavior.AllowGet); }
                    return Json(new { respuesta = "EXITO: ", tiempo = prg.POS_TIEMPO_REFRESCO_PANEL }, JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }

    }
}
