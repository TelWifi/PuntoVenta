using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using System.Net.Mail;
using System.Net;
using System.IO;


namespace Franquisia1._1.Controllers
{
    
    public class HomeController : Controller    
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Consulta()
        {
            try
            {
                appbosaEntities db = new appbosaEntities();
                List<string> tdtipdoc = db.tdtipdoc.ToList().Select(b => b.TIPDOC).ToList();
                var cm = db.maesgen.Where(a => a.idmaesgen.Equals("002") && tdtipdoc.Contains(a.clavemaesgen)).ToList();
                ViewBag.tipdocs = cm.ToList();
                return View();
            }
            catch (System.Data.EntityException ex) { return RedirectToAction("ErrorBD", "Error", ex.Message); }
            catch (Exception ex) { return RedirectToAction("Error", "Error", ex.Message); }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Consulta(string tipdoc, string serie, string correlativo, string fecha, string importe)
        {
            try
            {
                appbosaEntities db = new appbosaEntities();
                DateTime cdate;
                List<string> tdtipdoc = db.tdtipdoc.ToList().Select(b => b.TIPDOC).ToList();
                var cm = db.maesgen.Where(a => a.idmaesgen.Equals("002") && tdtipdoc.Contains(a.clavemaesgen)).ToList();
                ViewBag.tipdocs = cm.ToList();
                ViewBag.td = tipdoc;
                if (DateTime.TryParse(fecha, out cdate))
                {
                    venc vc = db.venc.Where(a => a.SERIE.Equals(serie) && a.NRODOC.Equals(correlativo)
                   ).FirstOrDefault();
                    if (vc!=null && vc.CDATE.Equals(cdate))
                    {
                        decimal impIn;
                        if (Decimal.TryParse(importe.Replace(',','.'), out impIn))
                        {
                            decimal impDB = db.vend.Where(a => a.CODCIA.Equals(vc.CODCIA) && a.CODIGO.Equals(vc.CODIGO)).Sum(a => a.TOTAL);
                            if (impIn == impDB)
                            {
                                List<vend> vds = db.vend.Where(a => a.CODCIA.Equals(vc.CODCIA) && a.CODIGO.Equals(vc.CODIGO)).ToList();
                                ciafile cia = db.ciafile.Where(a => a.idcia.Equals(vc.CODCIA)).FirstOrDefault();
                                sucursal suc = db.sucursal.Where(a => a.codcia.Equals(cia.idcia) && a.codigo.Equals(vc.SUCURSAL)).FirstOrDefault();
                                anexos anexo = db.anexos.Where(a => a.idcia.Equals(cia.idcia) && a.tipane.Equals(vc.TIPANE) && a.codane.Equals(vc.CODANE)).FirstOrDefault();
                                if (anexo.tipdoc.Equals(tipdoc))
                                {
                                    maesgen emision = db.maesgen.Where(a => a.idmaesgen.Equals("110") && a.clavemaesgen.Equals(vc.TIPDOC)).FirstOrDefault();
                                    maesgen mgtd = db.maesgen.Where(a => a.idmaesgen.Equals("002") && a.clavemaesgen.Equals(anexo.tipdoc)).FirstOrDefault();
                                    maesgen mgmoneda = db.maesgen.Where(a => a.idmaesgen.Equals("015") && a.clavemaesgen.Equals(vc.CODMON)).FirstOrDefault();
                                    var u = (from a in db.venpag
                                             join b in db.forventa on a.FORVENTA equals b.codigo
                                             from c in db.forpago.Where(d => d.codigo.Equals(a.FORPAGO)).DefaultIfEmpty()
                                             from d in db.tarjetas.Where(t => t.codigo.Equals(a.TARJETA)).DefaultIfEmpty()
                                             where a.CODCIA.Equals(vc.CODCIA) && a.CODIGO.Equals(vc.CODIGO)
                                             select new
                                             {
                                                 codigo = a.CODIGO,
                                                 importe = a.IMPORTE,
                                                 recibido = a.RECIBIDO,
                                                 vuelto = a.VUELTO,
                                                 forventa = b.descripcion,
                                                 forpago = c.descripcion,
                                                 tarjeta = d.descripcion
                                             }
                                 ).ToList();
                                    decimal total = 0, sumagra = 0, sumaina = 0, sumaexo = 0, sumaigv = 0;
                                    foreach (vend item in vds)
                                    {
                                        sumaigv += item.IGV;
                                        total += item.TOTAL;
                                        switch (item.TIPOVALORVENTA)
                                        {
                                            case "01":
                                                item.GRAVADO = item.NETO;
                                                sumagra += (decimal)item.GRAVADO;
                                                break;
                                            case "02":
                                                item.EXONERADO = item.NETO;
                                                sumaexo += (decimal)item.EXONERADO;
                                                break;
                                            case "03":
                                                item.INAFECTO = item.NETO;
                                                sumaina += (decimal)item.INAFECTO;
                                                break;
                                        }
                                    }
                                    parreg prigv = db.parreg.Where(a => a.IDCIA.Equals(cia.idcia) && a.FORM.Equals("COM")).FirstOrDefault();
                

                                    AppAccounting.NumLetras nl = new AppAccounting.NumLetras();

                                    ViewBag.cia = cia;
                                    ViewBag.suc = suc;
                                    ViewBag.fecha = cdate.ToString("dd/MM/yyyy");
                                    ViewBag.venc = vc;
                                    ViewBag.vends = vds;
                                    ViewBag.anexo = anexo;
                                    ViewBag.tipdoc = mgtd.parm1maesgen;
                                    ViewBag.tipdocdesc = mgtd.desmaesgen;
                                    ViewBag.docemi = emision.parm6maesgen;
                                    ViewBag.moneda = mgmoneda.parm1maesgen;
                                    ViewBag.gravado = sumagra;
                                    ViewBag.exonerado = sumaexo;
                                    ViewBag.inafecto = sumaina;
                                    ViewBag.igv = sumaigv;
                                    ViewBag.com_tasa_igv = prigv.COM_TASA_IGV;
                                    ViewBag.total = total;
                                    ViewBag.totalstr = nl.Numero_to_Letras(mgmoneda.clavemaesgen, total);
                                    ViewBag.resumen = u;
                                    ViewBag.serie = vc.SERIE;
                                    ViewBag.correlativo = vc.NRODOC;
                                    ViewBag.abrevia = mgmoneda.abrevia;
                                    return View("Documento");
                                }
                            }
                        }
                        else
                        {
                            ViewBag.msg_error = "El monto total debe ser un n\u00FAmero v\u00E1lido";
                            return View("Consulta");
                        }
                    }
                    ViewBag.msg_error = "Los datos ingresados no coinciden con ninguno de nuestros registros";
                    return View("Consulta");
                }
                else
                {
                    ViewBag.msg_error = "Formato de fecha incorrecto";
                    return View("Consulta");
                }
            }
            catch (Exception ex) { return RedirectToAction("Error", "Error", ex.Message); }
        }
        public ActionResult Respuesta(string tipdoc, string serie, string correlativo, string fecha, string importe)
        {
            try
            {
                appbosaEntities db = new appbosaEntities();
                DateTime cdate;
                List<string> tdtipdoc = db.tdtipdoc.ToList().Select(b => b.TIPDOC).ToList();
                var cm = db.maesgen.Where(a => a.idmaesgen.Equals("002") && tdtipdoc.Contains(a.clavemaesgen)).ToList();
                ViewBag.tipdocs = cm.ToList();
                ViewBag.td = tipdoc;
                ViewBag.print = true;
                if (DateTime.TryParse(fecha, out cdate))
                {
                    venc vc = db.venc.Where(a => a.SERIE.Equals(serie) && a.NRODOC.Equals(correlativo)
                   ).FirstOrDefault();
                    if (vc != null && vc.CDATE.Equals(cdate))
                    {
                        decimal impIn;
                        if (Decimal.TryParse(importe.Replace(',', '.'), out impIn))
                        {
                            decimal impDB = db.vend.Where(a => a.CODCIA.Equals(vc.CODCIA) && a.CODIGO.Equals(vc.CODIGO)).Sum(a => a.TOTAL);
                            if (impIn == impDB)
                            {
                                List<vend> vds = db.vend.Where(a => a.CODCIA.Equals(vc.CODCIA) && a.CODIGO.Equals(vc.CODIGO)).ToList();
                                ciafile cia = db.ciafile.Where(a => a.idcia.Equals(vc.CODCIA)).FirstOrDefault();
                                sucursal suc = db.sucursal.Where(a => a.codcia.Equals(cia.idcia) && a.codigo.Equals(vc.SUCURSAL)).FirstOrDefault();
                                anexos anexo = db.anexos.Where(a => a.idcia.Equals(cia.idcia) && a.tipane.Equals(vc.TIPANE) && a.codane.Equals(vc.CODANE)).FirstOrDefault();
                                if (anexo.tipdoc.Equals(tipdoc))
                                {
                                    maesgen emision = db.maesgen.Where(a => a.idmaesgen.Equals("110") && a.clavemaesgen.Equals(vc.TIPDOC)).FirstOrDefault();
                                    maesgen mgtd = db.maesgen.Where(a => a.idmaesgen.Equals("002") && a.clavemaesgen.Equals(anexo.tipdoc)).FirstOrDefault();
                                    maesgen mgmoneda = db.maesgen.Where(a => a.idmaesgen.Equals("015") && a.clavemaesgen.Equals(vc.CODMON)).FirstOrDefault();
                                    var u = (from a in db.venpag
                                             join b in db.forventa on a.FORVENTA equals b.codigo
                                             from c in db.forpago.Where(d => d.codigo.Equals(a.FORPAGO)).DefaultIfEmpty()
                                             from d in db.tarjetas.Where(t => t.codigo.Equals(a.TARJETA)).DefaultIfEmpty()
                                             where a.CODCIA.Equals(vc.CODCIA) && a.CODIGO.Equals(vc.CODIGO)
                                             select new
                                             {
                                                 codigo = a.CODIGO,
                                                 importe = a.IMPORTE,
                                                 recibido = a.RECIBIDO,
                                                 vuelto = a.VUELTO,
                                                 forventa = b.descripcion,
                                                 forpago = c.descripcion,
                                                 tarjeta = d.descripcion
                                             }
                                 ).ToList();
                                    decimal total = 0, sumagra = 0, sumaina = 0, sumaexo = 0, sumaigv = 0;
                                    foreach (vend item in vds)
                                    {
                                        sumaigv += item.IGV;
                                        total += item.TOTAL;
                                        switch (item.TIPOVALORVENTA)
                                        {
                                            case "01":
                                                item.GRAVADO = item.NETO;
                                                sumagra += (decimal)item.GRAVADO;
                                                break;
                                            case "02":
                                                item.EXONERADO = item.NETO;
                                                sumaexo += (decimal)item.EXONERADO;
                                                break;
                                            case "03":
                                                item.INAFECTO = item.NETO;
                                                sumaina += (decimal)item.INAFECTO;
                                                break;
                                        }
                                    }
                                    parreg prigv = db.parreg.Where(a => a.IDCIA.Equals(cia.idcia) && a.FORM.Equals("COM")).FirstOrDefault();


                                    AppAccounting.NumLetras nl = new AppAccounting.NumLetras();

                                    ViewBag.cia = cia;
                                    ViewBag.suc = suc;
                                    ViewBag.fecha = cdate.ToString("dd/MM/yyyy");
                                    ViewBag.venc = vc;
                                    ViewBag.vends = vds;
                                    ViewBag.anexo = anexo;
                                    ViewBag.tipdoc = mgtd.parm1maesgen;
                                    ViewBag.tipdocdesc = mgtd.desmaesgen;
                                    ViewBag.docemi = emision.parm6maesgen;
                                    ViewBag.moneda = mgmoneda.parm1maesgen;
                                    ViewBag.gravado = sumagra;
                                    ViewBag.exonerado = sumaexo;
                                    ViewBag.inafecto = sumaina;
                                    ViewBag.igv = sumaigv;
                                    ViewBag.com_tasa_igv = prigv.COM_TASA_IGV;
                                    ViewBag.total = total;
                                    ViewBag.totalstr = nl.Numero_to_Letras(mgmoneda.clavemaesgen, total);
                                    ViewBag.resumen = u;
                                    ViewBag.serie = vc.SERIE;
                                    ViewBag.correlativo = vc.NRODOC;
                                    ViewBag.abrevia = mgmoneda.abrevia;
                                    return View("Documento");
                                }
                            }
                        }
                        else
                        {
                            ViewBag.msg_error = "El monto total debe ser un n\u00FAmero v\u00E1lido";
                            return View("Consulta");
                        }
                    }
                    ViewBag.msg_error = "Los datos ingresados no coinciden con ninguno de nuestros registros";
                    return View("Consulta");
                }
                else
                {
                    ViewBag.msg_error = "Formato de fecha incorrecto";
                    return View("Consulta");
                }
            }
            catch (Exception ex) { return RedirectToAction("Error", "Error", ex.Message); }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Download(string tipdoc, string serie, string correlativo, string fecha, string importe)
        {
            return new ActionAsPdf("Respuesta", new { tipdoc = tipdoc, serie = serie, correlativo = correlativo, fecha = fecha, importe = importe }) 
            { 
                FileName = String.Format("{0} - {1}.pdf","Documento Electrónico", DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss"))
            };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SendEmail(string email, string tipdoc, string serie, string correlativo, string fecha, string importe)
        {
            try
            {
                appbosaEntities db = new appbosaEntities();
                setupmail stmail = db.setupmail.ToList().First();
                usrfile u = new usrfile();
                string FromEmail = stmail.DIRMAIL;
                string FromDisplayName = stmail.NOMMAIL;
                string FromPassword = u.Desencripta(stmail.PASSMAIL);
                string subject = "Documento Electrónico";
                string body = "";
                string host = stmail.SMTPMAIL;
                int port = int.Parse(stmail.PUERTO_SMTP);
                string SslAct = stmail.CONEXION_SSL;

                var fromAddress = new MailAddress(FromEmail, FromDisplayName);
                var toAddress = new MailAddress(email);
                var smtp = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = SslAct.Equals("S"),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, FromPassword)
                };

                var root = Server.MapPath("~/PDF/");
                if (!System.IO.Directory.Exists(@root))
                {
                    System.IO.Directory.CreateDirectory(@root);
                }
                var pdfname = String.Format("{0} - {1}.pdf","Documento Electrónico", DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss"));
                var path = Path.Combine(root, pdfname);
                path = Path.GetFullPath(path);
                var something = new Rotativa.ActionAsPdf("Respuesta", new { tipdoc = tipdoc, serie = serie, correlativo = correlativo, fecha = fecha, importe = importe }) {
                    FileName = pdfname,
                };
                var binary = something.BuildPdf(this.ControllerContext);
                System.IO.File.Create(path).Close();
                System.IO.File.WriteAllBytes(@path, binary);
                
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject, Body = body 
                })
                {
                    message.Attachments.Add(new Attachment(path));
                    smtp.Send(message);
                }
                System.IO.File.Delete(@path);
                return Json(new { respuesta = "EXITO: Email enviado a " + email }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
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
                        Session["Loged_usrfile_desusr"] = v.desusr;
                        punemi punemi = ae.punemi.Where(a => a.codcia.Equals(idcia) && a.sucursal.Equals(sucur) && a.situa.Equals("V")).FirstOrDefault();
                        if (punemi != null) { Session["Loged_usrfile_punemi"] = punemi.codigo; }
                        string redir = "/UndAtencion/Index";//RedireccionTiposUuario();
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
