using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Franquisia1._1.Controllers
{
    public class AnexosController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        public JsonResult Crear(string a)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                anexos anexo = JsonConvert.DeserializeObject<anexos>(a);
                string rpta = "ERROR: ";
                bool estado = true;
                if (String.IsNullOrWhiteSpace(anexo.desane)) { estado = false; rpta += "raz\u00F3n social/apellidos y nombres no puede ser nulo o vac\u00EDo\n"; }
                if (String.IsNullOrWhiteSpace(anexo.tipdoc)) { estado = false; rpta += "Tipo de documento no puede ser nulo o vac\u00EDo\n"; }
                if (String.IsNullOrWhiteSpace(anexo.nrodoc)) { estado = false; rpta += "N\u00FAmero de documento no puede ser nulo o vac\u00EDo\n"; }
                if (String.IsNullOrWhiteSpace(anexo.refane)) { estado = false; rpta += "Direcci\u00F3n no puede ser nulo o vac\u00EDo"; }
                if (estado)
                {
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    anexos aux = anexo = db.anexos.Where(b => b.idcia.Equals(codcia) && b.tipane.Equals("C") && b.codane.Equals(anexo.nrodoc)).FirstOrDefault();
                    if (aux == null)
                    {
                        anexo.idcia = codcia;
                        anexo.tipane = "C";
                        aux.refane = anexo.refane != null ? anexo.refane.ToUpper() : "";
                        aux.tipdoc = anexo.tipdoc != null ? anexo.tipdoc.ToUpper() : "";
                        aux.nrodoc = anexo.nrodoc != null ? anexo.nrodoc.ToUpper() : "";
                        aux.rucane = anexo.rucane != null ? anexo.rucane.ToUpper() : "";
                        aux.nombre1 = anexo.desane != null ? anexo.nombre1.ToUpper() : "";
                        aux.nombre2 = anexo.desane != null ? anexo.nombre2.ToUpper() : "";
                        aux.apepat = anexo.desane != null ? anexo.apepat.ToUpper() : "";
                        aux.apemat = anexo.desane != null ? anexo.apemat.ToUpper() : "";
                        aux.desane = anexo.desane != null ? anexo.desane.ToUpper() : String.Join(" ", new { aux.nombre1, aux.nombre2, aux.apepat, aux.apemat });
                        anexo.situane = "V";
                        db.anexos.Add(anexo);
                        db.SaveChanges();
                        anexo = db.anexos.Where(b => b.idcia.Equals(codcia) && b.tipane.Equals("C") && b.codane.Equals(anexo.nrodoc)
                            && b.situane.Equals("V")).FirstOrDefault();
                        return Json(new { respuesta = "EXITO: Anexo creado", anexo = anexo }, JsonRequestBehavior.AllowGet);
                        
                    }
                    else { return Json(new { respuesta = "ADVERTENCIA: El anexo ya existe\n", anexo = aux}, JsonRequestBehavior.AllowGet); }
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
                    List<anexos> lista = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C")
                    && a.nrodoc.Contains(clave) && a.situane.Equals("V")).OrderBy(a => a.nrodoc).ToList();
                    foreach (var item in lista)
                    {
                        switch (item.tipdoc)
                        {
                            case "01":
                                item.tipdoc = "DNI";
                                break;
                            case "06": item.tipdoc = "RUC";
                                break;
                        }
                    }
                    return Json(new { respuesta ="EXITO: LA PETICION SE REALIZO EXITOSAMENTE",lista = serializer.Serialize(lista) }, JsonRequestBehavior.AllowGet);
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
                    List<anexos> lista = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C")
                    && a.desane.Contains(clave) && a.situane.Equals("V")).OrderBy(a => a.desane).ToList();
                    foreach (var item in lista)
                    {
                        switch (item.tipdoc)
                        {
                            case "01":
                                item.tipdoc = "DNI";
                                break;
                            case "06": item.tipdoc = "RUC";
                                break;
                        }
                    }
                    return Json(new { respuesta="EXITO: LA PETICION SE REALIZO EXITOSAMENTE", lista=serializer.Serialize(lista) }, JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { respuesta = "ERROR: EL VALOR A BUSCAR NO PUEDE SER NULO O VACIO"}, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex){return Json(new { respuesta = "ERROR: " + ex.Message },JsonRequestBehavior.AllowGet);}
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message },JsonRequestBehavior.AllowGet); }
        }
        public JsonResult Actualizar(string a)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                anexos anexo = JsonConvert.DeserializeObject<anexos>(a);
                string rpta;
                if (!String.IsNullOrWhiteSpace(anexo.nrodoc))
                {
                    anexo.nrodoc = anexo.nrodoc.Trim();
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    anexos aux = db.anexos.Where(b => b.idcia.Equals(codcia) && b.tipane.Equals("C") && b.codane.Equals(anexo.nrodoc)).FirstOrDefault();
                    if (aux != null)
                    {
                        if (anexo.situane.Equals("V"))
                        {
                            
                            aux.refane = anexo.refane != null ? anexo.refane.ToUpper() : "";
                            aux.tipdoc = anexo.tipdoc != null ? anexo.tipdoc.ToUpper() : "";
                            aux.nrodoc = anexo.nrodoc != null ? anexo.nrodoc.ToUpper() : "";
                            aux.rucane = anexo.rucane != null ? anexo.rucane.ToUpper() : "";
                            aux.nombre1 = anexo.desane != null ? anexo.nombre1.ToUpper() : "";
                            aux.nombre2 = anexo.desane != null ? anexo.nombre2.ToUpper() : "";
                            aux.apepat = anexo.desane != null ? anexo.apepat.ToUpper() : "";
                            aux.apemat = anexo.desane != null ? anexo.apemat.ToUpper() : "";
                            aux.desane = anexo.desane != null ? anexo.desane.ToUpper() : String.Join(" ", new { aux.nombre1, aux.nombre2, aux.apepat, aux.apemat });
                            db.SaveChanges();
                            rpta = "EXITO: Anexo actualizado";
                            return Json(new { respuesta = rpta, anexo = anexo }, JsonRequestBehavior.AllowGet);
                        }
                        else { rpta = "ERROR: No se puede actualizar porque el registro esta deshabilitado"; }
                    }
                    else { rpta = "ADVERTENCIA: Anexo no registrado"; }
                }
                else { rpta = "ERROR: El n\u00FAmero de documento no puede ser nulo o vac\u00EDo"; }
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
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    anexos anexo = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.codane.Equals(nrodoc) && a.situane.Equals("V")).FirstOrDefault();
                    if (anexo != null) { return Json(new { respuesta = "EXITO: Anexo obtenido", anexo = anexo }, JsonRequestBehavior.AllowGet); }
                    else { rpta = "ADVERTENCIA: El Anexo no est\u00E1 registrado o se encuentra deshabilitado"; }
                }
                else { rpta = "ERROR: EL n\u00FAmero de documento no puede ser nulo o vac\u00EDo"; }
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
