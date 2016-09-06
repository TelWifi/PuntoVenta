using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class UndAtencionController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        public ActionResult Index()
        {
            string rol = Session["Loged_usrfile_rol"].ToString();
            if (rol.Equals("C") || rol.Equals("M"))
            {
                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                ViewBag.personal = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V"));
                List<divatencion> divs = db.divatencion.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal)).ToList();
                ViewBag.divs = divs;
                parreg pr= db.parreg.Where(a => a.IDCIA.Equals(codcia) && a.FORM.Equals("POS")).FirstOrDefault();
                ViewBag.parpwd = pr.POS_IS_CLAVE_PERATENCION;
                return View();
            }
            else { return RedirectToAction("ErrorPermiso", "Error"); }
            
        }
       [HttpPost]
       public JsonResult Aperturar(string codigo, string idperatencion, string divate, string pwd)
       {
           try
           {
               string codcia = Session["Loged_usrfile_ciafile"].ToString();
               parreg pr = db.parreg.Where(a => a.IDCIA.Equals(codcia) && a.FORM.Equals("POS")).FirstOrDefault();
               if (pr.POS_IS_CLAVE_PERATENCION.Equals("S"))
               {
                   usrfile u = new usrfile();
                   string newpwd = u.Encripta(pwd);
                   
                   peratencion p = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V") && a.codigo.Equals(idperatencion)).FirstOrDefault();
                   if (p == null || !p.clave.Equals(newpwd))
                   {
                       return Json(new { respuesta = "ERROR: Contrase\u00F1a incorrecta" }, JsonRequestBehavior.AllowGet);
                   }
               }
               string rol = Session["Loged_usrfile_rol"].ToString();
               if (rol.Equals("C") || rol.Equals("M")){
                   string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                   string codpunemi = Session["Loged_usrfile_punemi"].ToString();
                   string idusr = Session["Loged_usrfile_idusr"].ToString();
                   peratencion per = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.codigo.Equals(idperatencion) && a.situa.Equals("V")).FirstOrDefault();
                   if (per == null) { return Json(new { respuesta = "ERROR: El c\u00F3digo del personal de atenci\u00F3n no existe" }, JsonRequestBehavior.AllowGet); }
                   undatencion undatencion = db.undatencion.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal)
                       && a.CODIGO.Equals(codigo) && a.DIVATENCION.Equals(divate) && a.ESTADO.Equals("V")).FirstOrDefault();
                   if (undatencion == null) { return Json(new { respuesta = "ERROR: La unidad de atenci\u00F3n no existe" }, JsonRequestBehavior.AllowGet); }

                   conc cc = db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.UNDATENCION.Equals(undatencion.CODIGO)
                       && a.SITUACION.Equals("A")).FirstOrDefault();
                   if (cc != null) { return Json(new { respuesta = "EXITO: La unidad de atenci\u00F3n est\u00E1 aperturada", div = undatencion.DIVATENCION, und = undatencion.CODIGO, per=per.codigo, rol=rol }, JsonRequestBehavior.AllowGet); }

                   int? tmp = this.generar(codcia);
                   if (tmp == null) { return Json(new { respuesta = "ERROR: Error al generar el c\u00F3digo del consumo" }, JsonRequestBehavior.AllowGet); }
                   conc conc = new conc();
                   conc.CODCIA = codcia;
                   conc.CODIGO = tmp.ToString().PadLeft(10, '0');
                   conc.SUCURSAL = sucursal;
                   conc.UNDATENCION = undatencion.CODIGO;
                   conc.PERATENCION = per.codigo;
                   conc.FECHA = DateTime.Now.Date;
                   conc.HORA = DateTime.Now.ToString("hh:mm:ss");
                   conc.USER = idusr;
                   conc.SITUACION = "A";
                   db.conc.Add(conc);
                   db.SaveChanges();
                   return Json(new { respuesta = "EXITO: Unidad de atenci\u00F3n aperturada", div = undatencion.DIVATENCION, und = undatencion.CODIGO, per = per.codigo, rol=rol }, JsonRequestBehavior.AllowGet);
               }else { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }                
           }
           catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
           catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
       }

       public ActionResult Facturacion(string div, string und, string per){
           try
           {
               string rol = Session["Loged_usrfile_rol"].ToString();
               if (rol.Equals("C"))
               {
                   string codcia = Session["Loged_usrfile_ciafile"].ToString();
                   string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                   string idusr = Session["Loged_usrfile_idusr"].ToString();

                   ViewBag.div = div;
                   ViewBag.und = und;
                   ViewBag.per = per;
                   peratencion p = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V") && a.codigo.Equals(per)).FirstOrDefault();
                   if (p == null || !p.codigo.Equals(per))
                   {
                       return RedirectToAction("ErrorPermiso", "Error");
                   }
                   ViewBag.categorias = db.claserv.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V"));
                   ViewBag.divisiones = db.divatencion.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal)).ToList();
                   ViewBag.personal = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V"));
                   ViewBag.puntosemision = db.punemi.Where(a => a.codcia.Equals(codcia)&&a.sucursal.Equals(sucursal) && a.situa.Equals("V")).ToList();
                   ViewBag.cia = db.ciafile.Where(a => a.idcia.Equals(codcia)).FirstOrDefault();
                   ViewBag.sucursal = db.sucursal.Where(a => a.codigo.Equals(sucursal)).FirstOrDefault();
                   var td = (from A in db.tdprgven
                                  join B in db.maesgen on A.tipdoc equals B.clavemaesgen
                                  where A.codcia.Equals(codcia) && B.idmaesgen.Equals("110") && A.prg.Equals("V0105") orderby A.tipdoc ascending
                                  select new { clave = A.tipdoc, descripcion = B.desmaesgen }).ToList();
                   List<Models.AxuliarHash> tipdocs = new List<Models.AxuliarHash>();
                   foreach (var item in td)
                   {
                       tipdocs.Add(new Models.AxuliarHash(item.clave, item.descripcion));
                   }
                   ViewBag.tipdocs = tipdocs.ToList();
                   ViewBag.forventa = db.forventa.Where(a=>a.codcia.Equals(codcia) && a.situa.Equals("V")).ToList();
                   ViewBag.forpago = db.forpago.Where(a =>a.codcia.Equals(codcia) && a.situa.Equals("V")).ToList();
                   return View("Facturacion");
               }else { return RedirectToAction("ErrorPermiso", "Error"); }
           }
           catch (System.Data.EntityException ex) { return RedirectToAction("ErroBD", "Error", ex.Message); }
           catch (Exception ex) { return RedirectToAction("Error", "Error", ex.Message); }  
       }
       public ActionResult Consumos(string div, string und, string per)
       {
           try
           {
               string rol = Session["Loged_usrfile_rol"].ToString();
               if (rol.Equals("C") || rol.Equals("M"))
               {
                   string codcia = Session["Loged_usrfile_ciafile"].ToString();
                   string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                   string idusr = Session["Loged_usrfile_idusr"].ToString();

                   ViewBag.div = div;
                   ViewBag.und = und;
                   ViewBag.per = per;

                   ViewBag.categorias = db.claserv.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V"));
                   ViewBag.divisiones = db.divatencion.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal)).ToList();
                   ViewBag.personal = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V"));
                   ViewBag.cia = db.ciafile.Where(a => a.idcia.Equals(codcia)).FirstOrDefault();
                   ViewBag.sucursal = db.sucursal.Where(a => a.codigo.Equals(sucursal)).FirstOrDefault();
                   ViewBag.accesofacturacion = rol.Equals("C");
                   return View("Consumos");
               }else { return RedirectToAction("ErrorPermiso", "Error"); }
           }
           catch (System.Data.EntityException ex) { return RedirectToAction("ErroBD","Error",ex.Message);}
           catch (Exception ex) { return RedirectToAction("Error", "Error", ex.Message); }  
       }
       public JsonResult Obtener(string divatencion)
       {
           try
           {
               string rol = Session["Loged_usrfile_rol"].ToString();
               if (rol.Equals("C") || rol.Equals("M"))
               {
                   var codcia = Session["Loged_usrfile_ciafile"].ToString();
                   var sucursal = Session["Loged_usrfile_sucursal"].ToString();
                   var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                   List<conc> listconc = db.conc.Where(a=>a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.SITUACION.Equals("A")).ToList();
                   List<string> listund = listconc.Select(a => a.UNDATENCION).ToList();
                   List<string> listper = listconc.Select(a => a.PERATENCION).ToList();
                   
                   List<undatencion> ocupadas = db.undatencion.Where(b => b.CODCIA.Equals(codcia) && b.SUCURSAL.Equals(sucursal) && b.DIVATENCION.Equals(divatencion) && b.ESTADO.Equals("V") && listund.Contains(b.CODIGO)).ToList();
                   List<undatencion> libres = db.undatencion.Where(b => b.CODCIA.Equals(codcia) && b.SUCURSAL.Equals(sucursal) && b.DIVATENCION.Equals(divatencion) && b.ESTADO.Equals("V") && !listund.Contains(b.CODIGO)).ToList();
                   List<peratencion> peraten = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V") && listper.Contains(a.codigo)).ToList();
                   var ocp = (from a in ocupadas
                              join b in listconc on a.CODIGO equals b.UNDATENCION
                              join c in db.peratencion on b.PERATENCION equals c.codigo
                              select new { CODUND = a.CODIGO,UNDDES=a.DESCRIPCION,CODPER=c.codigo, PERCORTO = c.corto, CODDIV=a.DIVATENCION });
                   return Json(new { respuesta = "EXITO: Exito", libres = libres, ocupadas = ocp }, JsonRequestBehavior.AllowGet);
               }
               else { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
           }
           catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
           catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
       }
       public ActionResult Selector(string div, string und, string per, string pass)
       {
           try
           {
               string rol = Session["Loged_usrfile_rol"].ToString();
               if (rol.Equals("C") || rol.Equals("M"))
               {
                   usrfile u = new usrfile();
                   string newpwd = u.Encripta(pass);
                   string codcia = Session["Loged_usrfile_ciafile"].ToString();
                   peratencion p = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.situa.Equals("V") && a.codigo.Equals(per)).FirstOrDefault();
                   if (p==null || !p.clave.Equals(newpwd))
                   {
                       return JavaScript ("ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n");
                       //return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet);
                   }
                   switch (rol)
                   {
                       case "M":
                           return RedirectToAction("Consumos", new { div = div, und = und, per = per });
                       case "C":
                           return RedirectToAction("Facturacion", new { div = div, und = und, per = per });
                       default:
                           return RedirectToAction("ErrorPermiso", "Error");
                   }
               }
               else { return RedirectToAction("ErrorPermiso", "Error"); }
           }

           catch (System.Data.EntityException ex) { return RedirectToAction("ErroBD", "Error", ex.Message); }
           catch (Exception ex) { return RedirectToAction("Error", "Error", ex.Message); }
       }
       public JsonResult ObtenerLibres(string clave)
       {
           try
           {
               string rol = Session["Loged_usrfile_rol"].ToString();
               if (rol.Equals("C") || rol.Equals("M"))
               {
                   string codcia = Session["Loged_usrfile_ciafile"].ToString();
                   string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                   var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                   List<undatencion> libres = db.undatencion.Where(b => b.CODCIA.Equals(codcia) && b.SUCURSAL.Equals(sucursal) && b.DIVATENCION.Equals(clave) && !db.conc.Where(a => a.UNDATENCION.Equals(b.CODIGO) && a.SITUACION.Equals("A")).Select(a => a.UNDATENCION).Contains(b.CODIGO)).ToList();
                   return Json(new { respuesta = "EXITO: Petici\00F3n exitosa", lista = serializer.Serialize(libres) }, JsonRequestBehavior.AllowGet);
               }
               else { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
           }
           catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
           catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
       }

       public JsonResult Cambiar(string codigo, string nuevound, string nuevodiv)
       {
           try
           {
               string rol = Session["Loged_usrfile_rol"].ToString();
               if (rol.Equals("C") || rol.Equals("M"))
               {
                   string codcia = Session["Loged_usrfile_ciafile"].ToString();
                   string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                   conc conc = db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.CODIGO.Equals(codigo)).FirstOrDefault();
                   if (conc == null) { return Json(new { respuesta = "ERROR: El c\u00F3digo del consumo no existe" }, JsonRequestBehavior.AllowGet); }
                   undatencion nuevaund = db.undatencion.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.CODIGO.Equals(nuevound) && a.DIVATENCION.Equals(nuevodiv)).FirstOrDefault();
                   if (nuevaund == null) { return Json(new { respuesta = "ERROR: La nueva unidad de atenci\u00F3n no existe" }, JsonRequestBehavior.AllowGet); }
                   List<undatencion> libres = db.undatencion.Where(b => b.CODCIA.Equals(codcia) && b.SUCURSAL.Equals(sucursal) && !db.conc.Where(a => a.UNDATENCION.Equals(b.CODIGO) && a.SITUACION.Equals("A")).Select(a => a.UNDATENCION).Contains(b.CODIGO)).ToList();
                   if (libres.Contains(nuevaund))
                   {
                       conc.UNDATENCION = nuevaund.CODIGO;
                       db.SaveChanges();
                       return Json(new { respuesta = "EXITO: Unidad de atenci\u00F3n cambiada", undatencion = nuevaund }, JsonRequestBehavior.AllowGet);
                   }
                   else { return Json(new { respuesta = "ERROR: La nueva unidad de atenci\u00F3n ya est\u00E1 aperturada\nsolo se permite cambiar a unidades de atenci\u00F3n no aperturadas" }, JsonRequestBehavior.AllowGet); }
               }
               else { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
           }
           catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
           catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
       }
       private int? generar(string codcia)
       {
           try
           {
               int? codigo = 1;
               numcia nc = db.numcia.Where(a => a.CODCIA.Equals(codcia) && a.PROCESO.Equals("CON")).FirstOrDefault();
               if (nc != null) { nc.NUMERO++; codigo = nc.NUMERO; db.SaveChanges(); }
               else
               {
                   codigo = 1; nc = new numcia();
                   nc.CODCIA = codcia; nc.NUMERO = codigo;
                   db.numcia.Add(nc); db.SaveChanges();
               }
               return codigo;
           }
           catch (System.Data.EntityException ex) { return null; }
           catch (Exception ex) { return null; }
       }
       protected override void Dispose(bool disposing)
       {
            db.Dispose();
            base.Dispose(disposing);
       }
    }
}

