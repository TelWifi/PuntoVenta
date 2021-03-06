﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Franquisia1._1.Models;

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
                if (!"C".Equals(rol)) { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }
                anexos anexo = JsonConvert.DeserializeObject<anexos>(a);
                string rpta = "ERROR: ";
                bool estado = true;
                if (String.IsNullOrWhiteSpace(anexo.tipdoc)) { estado = false; rpta += Msg.AttrNoNuloVacio("Tipo de documento") + "\n"; }
                if (String.IsNullOrWhiteSpace(anexo.nrodoc)) { estado = false; rpta += Msg.AttrNoNuloVacio(Msg.NUMDOC) + "\n"; }
                if (estado)
                {
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    string idusr = Session["Loged_usrfile_idusr"].ToString();
                    anexos aux = db.anexos.Where(b => b.idcia.Equals(codcia) && b.tipane.Equals("C") && b.codane.Equals(anexo.nrodoc)).FirstOrDefault();
                    if (aux == null)
                    {
                        anexo.idcia = codcia;
                        anexo.codane = anexo.nrodoc;
                        anexo.tipane = "C";
                        anexo.refane = anexo.refane != null ? anexo.refane.ToUpper() : "";
                        anexo.tipdoc = anexo.tipdoc != null ? anexo.tipdoc.ToUpper() : "";
                        anexo.nrodoc = anexo.nrodoc != null ? anexo.nrodoc.ToUpper() : "";
                        anexo.rucane = anexo.tipdoc.Equals("06")? anexo.nrodoc : "";
                        anexo.nombre1 = anexo.nombre1 != null ? anexo.nombre1.ToUpper() : "";
                        anexo.nombre2 = anexo.nombre2 != null ? anexo.nombre2.ToUpper() : "";
                        anexo.apepat = anexo.apepat != null ? anexo.apepat.ToUpper() : "";
                        anexo.apemat = anexo.apemat != null ? anexo.apemat.ToUpper() : "";
                        if (String.IsNullOrWhiteSpace(anexo.desane))
                        {
                            string[] s = { anexo.nombre1, anexo.nombre2, anexo.apepat, anexo.apemat };
                            anexo.desane = anexo.desane != null ? anexo.desane.ToUpper() : String.Join(" ", s); 
                        }
                        anexo.tipper = "";
                        if (!String.IsNullOrWhiteSpace(anexo.rucane))
                        {
                            switch (anexo.rucane.ToCharArray()[0])
                            {
                                case '1':
                                    anexo.tipper = "01";
                                    break;
                                case '2':
                                    anexo.tipper = "02";
                                    break;
                                default:
                                    
                                    break;
                            }
                        }
                        anexo.situane = "V";
                        anexo.usuario = idusr;
                        anexo.fcrea = DateTime.Now.ToString("dd/MM/yyyy");
                        anexo.fmod = anexo.fcrea;
                        db.anexos.Add(anexo);
                        db.SaveChanges();
                        anexo = db.anexos.Where(b => b.idcia.Equals(codcia) && b.tipane.Equals("C") && b.codane.Equals(anexo.nrodoc)
                            && b.situane.Equals("V")).FirstOrDefault();
                        return Json(new { respuesta = Msg.OpExitosa, anexo = anexo }, JsonRequestBehavior.AllowGet);
                        
                    }
                    else { return Json(new { respuesta = "ADVERTENCIA: El anexo ya existe", anexo = new { anexo.nrodoc, anexo.desane, anexo.refane } }, JsonRequestBehavior.AllowGet); }
                }
                return Json(new { respuesta = rpta }, JsonRequestBehavior.AllowGet);
            }catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message }, JsonRequestBehavior.AllowGet); }
        }
        public ActionResult BuscarNrodoc(string clave)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }
                
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                if (!String.IsNullOrWhiteSpace(clave))
                {
                    clave = clave.Trim();
                    var c = db.tdtipdoc.ToList().Select(a=>a.TIPDOC);
                    var lista = (from a in db.anexos
                                 join b in db.maesgen on a.tipdoc equals b.clavemaesgen
                                 where a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.nrodoc.Contains(clave) && b.idmaesgen.Equals("002") &&
                                 a.situane.Equals("V") && b.statemaesgen.Equals("V")
                                 && c.Contains(b.clavemaesgen)
                                 orderby a.nrodoc
                                 select new { desane = a.desane, tipdoc = b.parm1maesgen, nrodoc = a.nrodoc, refane = a.refane }
                                     ).ToList();
                    return Json(new { respuesta = Msg.OpExitosa, lista = serializer.Serialize(lista) }, JsonRequestBehavior.AllowGet);
                }else { return Json(new { respuesta = Msg.AttrNoNuloVacio("El valor a buscar")}, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message }, JsonRequestBehavior.AllowGet); }
        }
        public ActionResult BuscarRazon(string clave)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string codcia = Session["Loged_usrfile_ciafile"].ToString();

                if (!String.IsNullOrWhiteSpace(clave)){
                    clave = clave.Trim();
                    var c = db.tdtipdoc.ToList().Select(a => a.TIPDOC);
                    var lista = (from a in db.anexos
                                 join b in db.maesgen on a.tipdoc equals b.clavemaesgen
                                 where a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.desane.Contains(clave) && b.idmaesgen.Equals("002")
                                 && a.situane.Equals("V") && b.statemaesgen.Equals("V") && c.Contains(b.clavemaesgen)
                                 orderby a.desane
                                 select new { desane = a.desane, tipdoc = b.parm1maesgen, nrodoc = a.nrodoc, refane = a.refane }
                                     ).ToList();
                    return Json(new { respuesta=Msg.OpExitosa, lista=serializer.Serialize(lista) }, JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { respuesta = Msg.AttrNoNuloVacio("El valor a buscar")}, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex){return Json(new { respuesta = "ERROR: " + ex.Message },JsonRequestBehavior.AllowGet);}
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e.Message },JsonRequestBehavior.AllowGet); }
        }
        public JsonResult Actualizar(string a)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }
                anexos anexo = JsonConvert.DeserializeObject<anexos>(a);
                string rpta;
                if (!String.IsNullOrWhiteSpace(anexo.nrodoc))
                {
                    anexo.nrodoc = anexo.nrodoc.Trim();
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    anexos aux = db.anexos.Where(b => b.idcia.Equals(codcia) && b.tipane.Equals("C") && b.codane.Equals(anexo.nrodoc)).FirstOrDefault();
                    if (aux != null)
                    {
                        if (aux.situane.Equals("V"))
                        {
                            
                            aux.refane = anexo.refane != null ? anexo.refane.ToUpper() : "";
                            aux.tipdoc = anexo.tipdoc != null ? anexo.tipdoc.ToUpper() : "";
                            aux.nrodoc = anexo.nrodoc != null ? anexo.nrodoc.ToUpper() : "";
                            aux.rucane = anexo.tipdoc.Equals("06") ? anexo.nrodoc : "";
                            aux.nombre1 = anexo.nombre1 != null ? anexo.nombre1.ToUpper() : "";
                            aux.nombre2 = anexo.nombre2 != null ? anexo.nombre2.ToUpper() : "";
                            aux.apepat = anexo.apepat != null ? anexo.apepat.ToUpper() : "";
                            aux.apemat = anexo.apemat != null ? anexo.apemat.ToUpper() : "";
                            if (String.IsNullOrWhiteSpace(anexo.desane))
                            {
                                string[] s = { aux.nombre1, aux.nombre2, aux.apepat, aux.apemat };
                                aux.desane = anexo.desane != null ? anexo.desane.ToUpper() : String.Join(" ", s);
                            }
                            aux.fmod = DateTime.Now.ToString("dd/MM/yyyy");
                            db.SaveChanges();
                            return Json(new { respuesta = Msg.OpExitosa, anexo = new { aux.nrodoc, aux.desane, aux.refane } }, JsonRequestBehavior.AllowGet);
                        }
                        else { rpta = "ERROR: No se puede actualizar porque el registro esta deshabilitado"; }
                    }
                    else { rpta = "ADVERTENCIA: Anexo no registrado"; }
                }
                else { rpta = Msg.AttrNoNuloVacio(Msg.NUMDOC); }
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
                if (!"C".Equals(rol)) { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }
                
                string rpta;
                if (!String.IsNullOrWhiteSpace(nrodoc))
                {
                    nrodoc = nrodoc.Trim();
                    string codcia = Session["Loged_usrfile_ciafile"].ToString();
                    anexos anexo = db.anexos.Where(a => a.idcia.Equals(codcia) && a.tipane.Equals("C") && a.codane.Equals(nrodoc) && a.situane.Equals("V")).FirstOrDefault();
                    if (anexo != null) { return Json(new { respuesta = Msg.OpExitosa, anexo = anexo }, JsonRequestBehavior.AllowGet); }
                    else { rpta = "ADVERTENCIA: El Anexo no est\u00E1 registrado o se encuentra deshabilitado"; }
                }
                else { rpta = Msg.AttrNoNuloVacio(Msg.NUMDOC); }
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
