using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class AnexosController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        public JsonResult Crear(string desane, string tipdoc,string nrodoc,string refane)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                
                string rpta = "ERROR: ";
                bool estado = true;
                if (String.IsNullOrWhiteSpace(desane)) { estado = false; rpta += "raz\u00F3n social/apellidos y nombres no puede ser nulo o vac\u00EDo\n"; }
                if (String.IsNullOrWhiteSpace(tipdoc)) { estado = false; rpta += "Tipo de documento no puede ser nulo o vac\u00EDo\n"; }
                if (String.IsNullOrWhiteSpace(nrodoc)) { estado = false; rpta += "N\u00FAmero de documento no puede ser nulo o vac\u00EDo\n"; }
                if (String.IsNullOrWhiteSpace(refane)) { estado = false; rpta += "Direcci\u00F3n no puede ser nulo o vac\u00EDo"; }
                if (estado)
                {
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    anexos anexo = anexo = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.codane.Equals(nrodoc)).FirstOrDefault();
                    if (anexo == null)
                    {
                        anexo = new anexos();
                        anexo.idcia = codcia;
                        anexo.tipane = "C";
                        anexo.desane = desane.Trim().ToUpper();
                        anexo.refane = refane.Trim().ToUpper();
                        anexo.nrodoc = nrodoc;
                        anexo.situane = "V";
                        if (tipdoc.Equals("RUC"))
                        {
                            if (nrodoc.Trim().Length == 11)
                            {
                                anexo.tipdoc = "06";
                                anexo.codane = nrodoc.Trim();
                                anexo.rucane = nrodoc;
                                db.anexos.Add(anexo);
                                db.SaveChanges();
                                anexo = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.codane.Equals(nrodoc) && a.situane.Equals("V")).FirstOrDefault();
                                return Json(new { respuesta = "EXITO: Anexo creado", anexo = anexo }, JsonRequestBehavior.AllowGet);
                            }
                            else { return Json(new { respuesta = "ERROR: N\u00FAmero de documento debe tener 11 caracteres" }, JsonRequestBehavior.AllowGet); }
                        }
                        else if (tipdoc.Equals("DNI"))
                        {
                            if (nrodoc.Trim().Length == 8)
                            {
                                anexo.tipdoc = "01";
                                anexo.codane = nrodoc.Trim();
                                db.anexos.Add(anexo);
                                db.SaveChanges();
                                anexo = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.codane.Equals(nrodoc) && a.situane.Equals("V")).FirstOrDefault();
                                return Json(new { respuesta = "EXITO: Anexo creado", anexo = anexo }, JsonRequestBehavior.AllowGet);
                            }
                            else { return Json(new { respuesta = "ERROR: N\u00FAmero de documento debe tener 8 caracteres" }, JsonRequestBehavior.AllowGet); }
                        }
                        else { return Json(new { respuesta = "ERROR: Tipo de documento incorrecto" }, JsonRequestBehavior.AllowGet); }
                    }
                    else { return Json(new { respuesta = "ERROR: Los datos ingresados pertenecen a un cliente que ya existe\nESTADO: " + anexo.situane }, JsonRequestBehavior.AllowGet); }
                }
                return Json(new { respuesta = rpta }, JsonRequestBehavior.AllowGet);
            }catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message }, JsonRequestBehavior.AllowGet); }
        }
        public ActionResult BuscarRuc(string clave)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                if (!String.IsNullOrWhiteSpace(clave))
                {
                    clave = clave.Trim();
                    string listajson = serializer.Serialize(db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C")
                    && a.nrodoc.Contains(clave) && a.situane.Equals("V")).OrderBy(a => a.nrodoc).ToList());
                    return Json(new { respuesta ="EXITO: LA PETICION SE REALIZO EXITOSAMENTE",lista = listajson }, JsonRequestBehavior.AllowGet);
                }else { return Json(new { respuesta = "ERROR: EL VALOR A BUSCAR NO PUEDE SER NULO O VACIO"}, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message }, JsonRequestBehavior.AllowGet); }
        }
        public ActionResult BuscarRazon(string clave)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string codcia = Session["Loged_usrfile_ciafile"].ToString();

                if (!String.IsNullOrWhiteSpace(clave)){
                    clave = clave.Trim();
		            string listajson = serializer.Serialize(db.anexos.Where(a =>a.idcia.Equals(codcia) && a.tipane.Equals("C")
                    && a.desane.Contains(clave) && a.situane.Equals("V")).OrderBy(a => a.desane).ToList());
                    return Json(new { respuesta="EXITO: LA PETICION SE REALIZO EXITOSAMENTE", lista=listajson }, JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { respuesta = "ERROR: EL VALOR A BUSCAR NO PUEDE SER NULO O VACIO"}, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex){return Json(new { respuesta = "ERROR: " + ex.Message },JsonRequestBehavior.AllowGet);}
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message },JsonRequestBehavior.AllowGet); }
        }
        public JsonResult Actualizar(string desane, string nrodoc, string refane)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                
                string rpta;
                if (!String.IsNullOrWhiteSpace(nrodoc))
                {
                    nrodoc = nrodoc.Trim();
                    if (nrodoc.Length == 8 || nrodoc.Length == 11)
                    {
                        string codcia = Session["Loged_usrfile_ciafile"].ToString();
                        anexos anexo = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.codane.Equals(nrodoc)).FirstOrDefault();
                        if (anexo != null)
                        {
                            if (String.IsNullOrWhiteSpace(desane) || String.IsNullOrWhiteSpace(refane)) { rpta = "ERROR: ALGUNOS CAMPOS SON VACIOS O NULOS"; }
                            else
                            {
                                if (anexo.situane.Equals("V"))
                                {
                                    anexo.desane = desane.Trim().ToUpper();
                                    anexo.refane = refane.Trim().ToUpper();
                                    db.SaveChanges();
                                    rpta = "EXITO: Anexo actualizado";
                                    return Json(new { respuesta = rpta, anexo = anexo }, JsonRequestBehavior.AllowGet);
                                }
                                else { rpta = "ERROR: No se puede actualizar porque el registro esta deshabilitado"; }
                            }
                        }
                        else { rpta = "ADVERTENCIA: El n\u00FAmero de documento no est\u00E1 registrado\nRecomendaci\u00F3n: puede registrarlo dando click en el bot\u00F3n agregar cliente"; }
                    }
                    else { rpta = "ERROR: El n\u00FAmero de documento debe tener:\nRUC: 11 caracteres\nDNI: 8 caracteres"; }
                }
                else { rpta = "ERROR: El RUC/DNI no puede ser nulo o vac\u00EDo"; }
                return Json(new { respuesta = rpta }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult Obtener(string nrodoc)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                
                string rpta;
                if (!String.IsNullOrWhiteSpace(nrodoc))
                {
                    nrodoc = nrodoc.Trim();
                    if (nrodoc.Length == 8 || nrodoc.Length == 11)
                    {
                        string codcia = Session["Loged_usrfile_ciafile"].ToString();
                        anexos anexo = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.codane.Equals(nrodoc)).FirstOrDefault();
                        if (anexo != null) { return Json(new { respuesta = "EXITO: Operaci\u00F3n exitosa", anexo = anexo }, JsonRequestBehavior.AllowGet); }
                        else { rpta = "ADVERTENCIA: El n\u00FAmero de documento no est\u00E1 registrado"; }
                    }
                    else { rpta = "ADVERTENCIA: EL n\u00FAmero de documento debe ser\nRUC: 11 caracteres\nDNI:8 caracteres"; }
                }
                else { rpta = "ERROR: EL n\u00FAmero de documento no puede ser nulo o vac\u00EDo"; }
                return Json(new { respuesta = rpta }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult ActualizarDNI(string nrodoc, string refane, string nom1, string nom2)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }

                string rpta;
                if (!String.IsNullOrWhiteSpace(nrodoc))
                {
                    nrodoc = nrodoc.Trim();
                    if (nrodoc.Length == 8 || nrodoc.Length == 11)
                    {
                        string codcia = Session["Loged_usrfile_ciafile"].ToString();
                        anexos anexo = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.codane.Equals(nrodoc)).FirstOrDefault();
                        if (anexo != null)
                        {
                            if (String.IsNullOrWhiteSpace(desane) || String.IsNullOrWhiteSpace(refane)) { rpta = "ERROR: ALGUNOS CAMPOS SON VACIOS O NULOS"; }
                            else
                            {
                                if (anexo.situane.Equals("V"))
                                {
                                    anexo.desane = desane.Trim().ToUpper();
                                    anexo.refane = refane.Trim().ToUpper();
                                    db.SaveChanges();
                                    rpta = "EXITO: Anexo actualizado";
                                    return Json(new { respuesta = rpta, anexo = anexo }, JsonRequestBehavior.AllowGet);
                                }
                                else { rpta = "ERROR: No se puede actualizar porque el registro esta deshabilitado"; }
                            }
                        }
                        else { rpta = "ADVERTENCIA: El n\u00FAmero de documento no est\u00E1 registrado\nRecomendaci\u00F3n: puede registrarlo dando click en el bot\u00F3n agregar cliente"; }
                    }
                    else { rpta = "ERROR: El n\u00FAmero de documento debe tener:\nRUC: 11 caracteres\nDNI: 8 caracteres"; }
                }
                else { rpta = "ERROR: El RUC/DNI no puede ser nulo o vac\u00EDo"; }
                return Json(new { respuesta = rpta }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message }, JsonRequestBehavior.AllowGet); }
        }
        public static anexos crearObtener(string codcia, string nrodoc, string desane, string refane)
        {
            try
            {
                appbosaEntities db = new appbosaEntities();
                if (String.IsNullOrWhiteSpace(desane) || String.IsNullOrWhiteSpace(desane) || String.IsNullOrWhiteSpace(desane) || String.IsNullOrWhiteSpace(desane))
                {return null;}
                anexos anexo = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.codane.Equals(nrodoc)).FirstOrDefault();
                if (anexo == null)
                {
                    anexo = new anexos();
                    anexo.idcia = codcia;
                    anexo.tipane = "C";
                    anexo.desane = desane.Trim().ToUpper();
                    anexo.refane = refane.Trim().ToUpper();
                    anexo.nrodoc = nrodoc;
                    anexo.situane = "V";
                    if (nrodoc.Trim().Length == 11)
                    {
                        anexo.tipdoc = "06";
                        anexo.codane = nrodoc.Trim();
                        anexo.rucane = nrodoc;
                        db.anexos.Add(anexo);
                        db.SaveChanges();
                        return anexo;
                    }
                    else if (nrodoc.Trim().Length == 8)
                    {
                        anexo.tipdoc = "01";
                        anexo.codane = nrodoc.Trim();
                        db.anexos.Add(anexo);
                        db.SaveChanges();
                        return anexo;
                    }
                    else { return null; }
            }else { return anexo; }
            }catch (Exception ex) { return null; }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
