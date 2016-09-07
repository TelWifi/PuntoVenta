using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Ionic.Zip;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Franquisia1._1.Controllers
{
    public class ConcController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        public JsonResult Anular(string codigo)
        {
            try
            {
                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                if (!String.IsNullOrWhiteSpace(codigo))
                {
                    codigo = codigo.Trim();
                    conc conc = db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.CODIGO.Equals(codigo)).FirstOrDefault();
                    if (conc != null)
                    {
                        List<cond> lista = db.cond.Where(a => a.CODCIA.Equals(codcia) && a.CODIGO.Equals(conc.CODIGO)).ToList();
                        foreach (var cond in lista) { db.cond.Remove(cond); }
                        db.conc.Remove(conc);
                        db.SaveChanges();
                        return Json(new { respuesta = "EXITO: Consumo anulado" }, JsonRequestBehavior.AllowGet);
                    }
                    else { return Json(new { respuesta = "ERROR: El C\u00F3digo de consumo no existe" }, JsonRequestBehavior.AllowGet); }
                }
                else { return Json(new { respuesta = "ERROR: El c\u00F3digo del consumo no puede ser nulo o vac\u00EDo" }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public JsonResult Facturar(string codconc, string nroane,string desane, string refane,string tipdoc,string formaventa, string formapago)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n" }, JsonRequestBehavior.AllowGet); }

                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                string punemi = Session["Loged_usrfile_punemi"].ToString();
                string idusr = Session["Loged_usrfile_idusr"].ToString();
                string desusr = Session["Loged_usrfile_desusr"].ToString();

                anexos anexo = AnexosController.crearObtener(codcia, nroane, desane, refane);
                if (anexo == null) { return Json(new { respuesta = "ERROR: Error al obtener o crear el anexo, asegurese que los datos del cliente esten correctamente ingresados" }, JsonRequestBehavior.AllowGet); }
                forventa fv = db.forventa.Where(a => a.codcia.Equals(codcia) && a.codigo.Equals(formaventa) && a.situa.Equals("V")).FirstOrDefault();
                if (fv == null) { return Json(new { respuesta = "ERROR: La forma de venta seleccionada no existe" }, JsonRequestBehavior.AllowGet); }
                parreg parreg = db.parreg.Where(a => a.IDCIA.Equals(codcia) && a.FORM.Equals("POS")).FirstOrDefault();
                if (parreg == null) { return Json(new { respuesta = "ERROR: Error al obtener par\u00E1metros intenernos" }, JsonRequestBehavior.AllowGet); }
                parreg prigv = db.parreg.Where(a => a.IDCIA.Equals(codcia) && a.FORM.Equals("COM")).FirstOrDefault();
                if (prigv == null) { return Json(new { respuesta = "ERROR: Error al obtener par\u00E1metros intenernos" }, JsonRequestBehavior.AllowGet); }

                string numdoc = GenerarNumDoc(tipdoc);
                if (String.IsNullOrWhiteSpace(numdoc)) { return Json(new { respuesta = "ERROR: Error al generar el n\u00FAmero del documento" }, JsonRequestBehavior.AllowGet); }
                int? tmp = generar(codcia, punemi);
                if (tmp==null){return Json(new { respuesta = "ERROR: Error al generar el c\u00F3digo "}, JsonRequestBehavior.AllowGet);}
                
                List<venpag> lista = JsonConvert.DeserializeObject<List<venpag>>(formapago);
                int index = 1;
                foreach (venpag item in lista)
                {
                    item.CODCIA = codcia;
                    item.CODIGO = punemi + tmp.ToString().PadLeft(8, '0');
                    item.FORVENTA = fv.codigo;
                    item.ITEM = index.ToString().PadLeft(3, '0');
                    item.RECIBIDO = item.VUELTO + item.IMPORTE;
                    if (String.IsNullOrWhiteSpace(item.FORPAGO)) { item.TARJETA = null; }
                    if (String.IsNullOrWhiteSpace(item.TARJETA)) { item.TARJETA = null; }
                    if (String.IsNullOrWhiteSpace(item.REFERENCIA)) { item.TARJETA = null; }
                    db.venpag.Add(item);
                    index++;
                }    
                venc venc = new venc();
                venc.CODCIA = codcia;
                venc.CODIGO = punemi + tmp.ToString().PadLeft(8, '0');
                venc.SUCURSAL = sucursal;
                venc.PUNEMI = punemi;
                venc.PROC_VENTA = "VD";
                venc.TIPO = "B";
                venc.PRG = "V0105";
                venc.PERIODO = DateTime.Now.Year.ToString();
                venc.FECMOV = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
                venc.MES = DateTime.Now.ToString("MM");
                venc.TIPANE = "C";
                venc.CODANE = anexo.codane;
                venc.DIRECCION = refane;
                venc.TIPDOC = tipdoc;
                venc.NUMDOC = numdoc;
                venc.FECDOC = DateTime.Now.ToString("dd/MM/yyyy");
                venc.FECVEN = DateTime.Now.ToString("dd/MM/yyyy");
                venc.CTIPCAM = 0;
                venc.CDATE = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
                venc.CHORA = DateTime.Now.ToString("hh:mm:ss");
                venc.CUSER = idusr;
                venc.CSIT = "RF";
                venc.IS_GR_REMITENTE = "N";
                venc.CODMON = parreg.POS_MON_REFERENCIA;

                conc conc=db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.CODIGO.Equals(codconc)).FirstOrDefault();
                if (conc==null){ return Json(new { respuesta = "ERROR: Error el c\u00F3digo del consumo no existe" }, JsonRequestBehavior.AllowGet); }
                if (conc.SITUACION.Equals("A"))
                {
                    conc.SITUACION = "C";
                    List<cond> items = db.cond.Where(a => a.CODCIA.Equals(codcia) && a.CODIGO.Equals(conc.CODIGO)).ToList();
                    index = 1;
                    decimal total=0, sumagra = 0, sumaexo = 0, sumaina = 0, sumaigv=0;
                    foreach (cond item in items)
                    {
                        vend vend = new vend();
                        vend.CODCIA = codcia;
                        vend.CODIGO = venc.CODIGO;
                        vend.ITEM = index.ToString().PadLeft(3, '0');
                        vend.IS_LOTE = "N";
                        vend.PREUNI = (decimal)item.PREUNI;
                        vend.CANTIDAD = (decimal)item.CANTIDAD;
                        vend.SUBTOTAL = (decimal)item.TOTAL;
                        vend.PORCDESC = 0;
                        vend.DESCUENTO = 0;
                        vend.TOTAL = vend.PREUNI;
                        vend.IGV = (decimal)(vend.TOTAL * prigv.COM_TASA_IGV / (100 + prigv.COM_TASA_IGV));
                        vend.CONVENTA = item.CONVENTA;
                        conventa conventa = db.conventa.Where(a => a.codcia.Equals(codcia) && a.codigo.Equals(item.CONVENTA) && a.situa.Equals("V")).FirstOrDefault();
                        vend.TIPOVALORVENTA = conventa.tipovalorventa;
                        vend.NETO = vend.TOTAL-vend.IGV;
                        sumaigv += vend.IGV;
                        total += vend.TOTAL;
                        switch (vend.TIPOVALORVENTA)
                        {
                            case "01":
                                vend.GRAVADO = vend.NETO;
                                sumagra +=(decimal)vend.GRAVADO;
                                break;
                            case "02":
                                vend.EXONERADO = vend.NETO;
                                sumaexo +=(decimal)vend.EXONERADO;
                                break;
                            case "03":
                                vend.INAFECTO = vend.NETO;
                                sumaina += (decimal)vend.INAFECTO;
                                break;
                            default:
                                break;
                        }
                        db.vend.Add(vend); index++;
                    }
                    db.venc.Add(venc);
                    db.SaveChanges();
                    string codaux = punemi + tmp.ToString().PadLeft(8, '0');
                    sucursal suc = db.sucursal.Where(a=>a.codcia.Equals(codcia) && a.codigo.Equals(sucursal) && a.situa.Equals("V")).FirstOrDefault();
                    var u =(from a in db.forpago join b in db.venpag on a.codigo equals b.FORPAGO
                           where b.CODCIA.Equals(codcia) && b.CODIGO.Equals(codaux)
                                select new {a.descripcion, a.is_tarjeta, b.IMPORTE, b.RECIBIDO, b.VUELTO  }
                                ).ToList();
                    string codper = conc.PERATENCION;
                    peratencion per = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.codigo.Equals(codper)).FirstOrDefault();
                    return Json(new { respuesta = "EXITO: Exito", gravado=sumagra, exonerado=sumaexo, inafecto = sumaina,
                        direccion=suc.direccion, igv=sumaigv,total=total, resumen=u, cajero=desusr, vendedor=per.descripcion
                    }, JsonRequestBehavior.AllowGet); 
                }
                else { return Json(new { respuesta = "ERROR: Error el consumo est\u00E1 cerrado" }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        private int? generar(string codcia, string punemi )
        {
            try
            {
                int? codigo = 1;
                numpos np =  db.numpos.Where(a => a.CODCIA.Equals(codcia) && a.PUNEMI.Equals(punemi)).FirstOrDefault();
                if (np != null) { np.NUMERO++; codigo = np.NUMERO; db.SaveChanges(); }
                else
                {
                    codigo = 1; np = new numpos();
                    np.CODCIA = codcia; np.PUNEMI = punemi; np.NUMERO = codigo;
                    db.numpos.Add(np); db.SaveChanges();
                }
                return codigo;
            }
            catch (System.Data.EntityException ex) { return null; }
            catch (Exception ex) { return null; }
        }
        private string GenerarNumDoc(string codtipdoc)
        {
            try
            {
                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                string punemi = Session["Loged_usrfile_punemi"].ToString();
                numpemi np = db.numpemi.Where(a => a.CODCIA.Equals(codcia) && a.PUNEMI.Equals(punemi) && a.CODIGO.Equals(codtipdoc)).FirstOrDefault();
                if (np == null) { return null; }
                np.NUMERO++;
                db.SaveChanges();
                return np.SERIE + "-" + np.NUMERO;
            }
            catch (System.Data.EntityException ex) { return null; }
            catch (Exception ex) { return null; }
        }
    }
}
