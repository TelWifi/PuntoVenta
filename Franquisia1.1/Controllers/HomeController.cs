using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Franquisia1._1.Controllers
{
    
    public class HomeController : Controller    
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
           
            try
            {
                appbosaEntities db = new appbosaEntities();
                ViewBag.cantimagenes = 4;
                return View();
            }
            catch (System.Data.EntityException ex) { return RedirectToAction("ErrorBD", "Error", ex.Message); }
            catch (Exception ex) { return RedirectToAction("Error", "Error", ex.Message); }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string idusr, string passusr, string rol,string idcia, string sucur)
        {
            if (String.IsNullOrWhiteSpace(idusr)) { return Json(new { respuesta = "ERROR: El usuario no puede ser nulo o vac\u00EDo" }, JsonRequestBehavior.AllowGet); }
            if (String.IsNullOrWhiteSpace(passusr)) { return Json(new { respuesta = "ERROR: La contraseña no puede ser nulo o vac\u00EDo" }, JsonRequestBehavior.AllowGet); }
            if (String.IsNullOrWhiteSpace(idcia)) { return Json(new { respuesta = "ERROR: El c\u00F3digo de cia no puede ser nulo o vac\u00EDo" }, JsonRequestBehavior.AllowGet); }
            if (String.IsNullOrWhiteSpace(sucur)) { return Json(new { respuesta = "ERROR: La sucursal no puede ser nulo o vac\u00EDo" }, JsonRequestBehavior.AllowGet); }
            if (String.IsNullOrWhiteSpace(rol)) { return Json(new { respuesta = "ERROR: El rol no puede ser nulo o vac\u00EDo" }, JsonRequestBehavior.AllowGet); }
            try
            {
                usrfile u = new usrfile();
                u.idusr = idusr; u.passusr = passusr;
                using (appbosaEntities ae = new appbosaEntities())
                {
                    string pwd_encriptada = u.Encripta(u.passusr);
                    usrfile v = ae.usrfile.Where(a => a.idusr.Equals(u.idusr) && a.passusr.Equals(pwd_encriptada) && a.stateusr.Equals("V")).FirstOrDefault();
                    if (v != null)
                    {
                        Session["Loged_usrfile_idusr"] = v.idusr;
                        Session["Loged_usrfile_typeusr"] = v.typeusr;
                        Session["Loged_usrfile_ciafile"] = idcia;
                        Session["Loged_usrfile_sucursal"] = sucur;
                        Session["Loged_usrfile_rol"] = rol;
                        punemi punemi = ae.punemi.Where(a => a.codcia.Equals(idcia) && a.sucursal.Equals(sucur) && a.situa.Equals("V")).FirstOrDefault();
                        if (punemi != null) { Session["Loged_usrfile_punemi"] = punemi.codigo; }
                        string redir = RedireccionTiposUuario();
                        return Json(new { respuesta = redir }, JsonRequestBehavior.AllowGet);

                    }
                    else { return Json(new { respuesta = "ERROR: Credenciales inv\u00E1lidas" }, JsonRequestBehavior.AllowGet); }
                }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAjax(string idusr, string passusr)
        {
            usrfile u = new usrfile();
            try
            {
                using (appbosaEntities ae = new appbosaEntities())
                {
                    string pwd_encriptada = u.Encripta(passusr);
                    usrfile v = ae.usrfile.Where(a => a.idusr.Equals(idusr) && a.passusr.Equals(pwd_encriptada) && a.stateusr.Equals("V")).FirstOrDefault();
                    if (v != null)
                    {
                        List<rolespv>roles = ae.rolespv.Where(a=>a.idusr.Equals(idusr)).ToList();
                        List<string> ciausr = ae.ciausrfile.Where(a=>a.idusr.Equals(v.idusr)).Select(a=>a.idcia).ToList();
                        var cias = ae.ciafile.Where(a => ciausr.Contains(a.idcia)).Select(a => new { a.idcia, a.descia}).ToList();
                        return Json(new { respuesta = "EXITO: Verificaci\u00F3n exitosa", lista = roles, listacia = cias }, JsonRequestBehavior.AllowGet);
                    }
                    else { return Json(new { respuesta = "ERROR: Credenciales inv\u00E1lidas" }, JsonRequestBehavior.AllowGet); }
                }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public ActionResult OffLogin()
        {
            Session["Loged_usrfile_idusr"] = null;
            Session["Loged_usrfile_typeusr"] = null;
            Session["Loged_usrfile_sucursal"] = null;
            Session["Loged_usrfile_ciafile"] = null;
            Session["Loged_usrfile_punemi"] = null;
            Session["Loged_usrfile_rol"] = null;
            return RedirectToAction("Login", "Home");
        }

        public string RedireccionTiposUuario()
        {
            try
            {
                using (appbosaEntities db = new appbosaEntities())
                {
                    string rol = Session["Loged_usrfile_rol"].ToString();
                    if (rol.Equals("C")) { return "/UndAtencion/Facturacion"; }
                    else if (rol.Equals("M")) { return "/UndAtencion/Consumos"; }
                    else
                    {
                        Session["msg_error"] = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n";
                        return "/Home/OffLogin";
                    }
                }
            }
            catch (System.Data.EntityException ex) { return null; }
            catch (Exception ex) { return null; }
        }
    }
}
