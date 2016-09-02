using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class ConventaController : Controller
    {
        private appbosaEntities db = new appbosaEntities();

        public JsonResult Obtener(string claserv)
        {
            try
            {
                string rol = Session["Loged_usrfile_rol"].ToString();
                if (rol.Equals("C") || rol.Equals("M"))
                {
                    if (String.IsNullOrWhiteSpace(claserv)) { return Json(new { respuesta = "ERROR: El c\u00F3digo de la categor\u00EDa no puede ser nulo o vac\u00EDo" }, JsonRequestBehavior.AllowGet); }
                    else
                    {
                        claserv = claserv.Trim();
                        string codcia = Session["Loged_usrfile_ciafile"].ToString();
                        string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                        //leer parametro imagen 
                        if (true)
                        {
                            var listajson = (from result in db.conventa
                                             join a in db.convensuc on result.codigo equals a.conventa
                                             join b in db.preconven on a.conventa equals b.conventa
                                             join c in db.maesgen on result.tipovalorventa equals c.clavemaesgen
                                             where result.claserv.Equals(claserv) && a.codcia.Equals(codcia)
                                             && a.sucursal.Equals(sucursal) && b.state.Equals("V") && result.situa.Equals("V")
                                             && c.idmaesgen.Equals("502")
                                             select new
                                             {
                                                 codigo = result.codigo,
                                                 foto64 = result.foto64,
                                                 precio = b.precio,
                                                 descripcion = result.descripcion,
                                                 tipovalorventa = c.desmaesgen
                                             }).ToList();
                            return Json(new { respuesta = "EXITO: EXITO", lista = listajson }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var listajson = (from result in db.conventa
                                             join a in db.convensuc on result.codigo equals a.conventa
                                             join b in db.preconven on a.conventa equals b.conventa
                                             join c in db.maesgen on result.tipovalorventa equals c.clavemaesgen
                                             where result.claserv.Equals(claserv) && a.codcia.Equals(codcia)
                                             && a.sucursal.Equals(sucursal) && b.state.Equals("V") && result.situa.Equals("V")
                                             && c.idmaesgen.Equals("502")
                                             select new
                                             {
                                                 codigo = result.codigo,
                                                 precio = b.precio,
                                                 descripcion = result.descripcion,
                                                 tipovalorventa = c.desmaesgen
                                             }).ToList();
                            return Json(new { respuesta = "EXITO: EXITO", lista = listajson }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }else { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }                
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult Buscar(string descripcion)
        {
            try
            {
                string rol = Session["Loged_usrfile_rol"].ToString();
                if (rol.Equals("C") || rol.Equals("M"))
                {
                    if (String.IsNullOrWhiteSpace(descripcion)) { return Json(new { respuesta = "ERROR: Ingrese la descripci\u00F3n a buscar" }, JsonRequestBehavior.AllowGet); }
                    else
                    {
                        descripcion = descripcion.Trim();
                        string codcia = Session["Loged_usrfile_ciafile"].ToString();
                        string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                        //leer parametro imagen 
                        if (true)
                        {
                            var listajson = (from result in db.conventa
                                             join a in db.convensuc on result.codigo equals a.conventa
                                             join b in db.preconven on a.conventa equals b.conventa
                                             join c in db.maesgen on result.tipovalorventa equals c.clavemaesgen
                                             where a.codcia.Equals(codcia) && a.sucursal.Equals(sucursal) && b.state.Equals("V")
                                             && result.situa.Equals("V") && result.descripcion.Contains(descripcion)
                                             && c.idmaesgen.Equals("502")
                                             select new
                                             {
                                                 codigo = result.codigo,
                                                 foto64 = result.foto64,
                                                 precio = b.precio,
                                                 descripcion = result.descripcion,
                                                 tipovalorventa = c.desmaesgen
                                             }).ToList();
                            return Json(new { respuesta = "EXITO: Exito", lista = listajson }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var listajson = (from result in db.conventa
                                             join a in db.convensuc on result.codigo equals a.conventa
                                             join b in db.preconven on a.conventa equals b.conventa
                                             join c in db.maesgen on result.tipovalorventa equals c.clavemaesgen
                                             where a.codcia.Equals(codcia) && a.sucursal.Equals(sucursal) && b.state.Equals("V")
                                             && result.situa.Equals("V") && result.descripcion.Contains(descripcion)
                                             && c.idmaesgen.Equals("502")
                                             select new
                                             {
                                                 codigo = result.codigo,
                                                 precio = b.precio,
                                                 descripcion = result.descripcion,
                                                 tipovalorventa = c.desmaesgen
                                             }).ToList();
                            return Json(new { respuesta = "EXITO: Exito", lista = listajson }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }else { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }                
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
