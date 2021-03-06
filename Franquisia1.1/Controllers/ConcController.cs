﻿using System;
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
using AppAccounting;
using Franquisia1._1.Models;
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
                if (!"C".Equals(rol)) { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }
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
                        return Json(new { respuesta = Msg.OpExitosa }, JsonRequestBehavior.AllowGet);
                    }
                    else { return Json(new { respuesta = Msg.AttrNoExiste(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
                }
                else { return Json(new { respuesta = Msg.AttrNoNuloVacio(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public JsonResult Facturar(string codconc, string nroane,string desane, string refane,string tipdoc, string vp)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if (!"C".Equals(rol)) { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }

                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                string punemi = Session["Loged_usrfile_punemi"].ToString();
                string idusr = Session["Loged_usrfile_idusr"].ToString();
                string desusr = Session["Loged_usrfile_desusr"].ToString();

                anexos anexo = AnexosController.crearObtener(codcia, nroane, desane, refane);
                if (anexo == null) { return Json(new { respuesta = "ERROR: Error al obtener o crear el anexo, asegurese que los datos del cliente esten correctamente ingresados" }, JsonRequestBehavior.AllowGet); }
                parreg parreg = db.parreg.Where(a => a.IDCIA.Equals(codcia) && a.FORM.Equals("POS")).FirstOrDefault();
                if (parreg == null) { return Json(new { respuesta = Msg.ErrParam }, JsonRequestBehavior.AllowGet); }
                parreg prigv = db.parreg.Where(a => a.IDCIA.Equals(codcia) && a.FORM.Equals("COM")).FirstOrDefault();
                if (prigv == null) { return Json(new { respuesta = Msg.ErrParam }, JsonRequestBehavior.AllowGet); }

                string numdoc = GenCod.NumDoc(db, codcia, punemi, tipdoc);
                if (String.IsNullOrWhiteSpace(numdoc)) { return Json(new { respuesta = Msg.ErrGenerar(Msg.NUMDOC) }, JsonRequestBehavior.AllowGet); }
                string codvenc = GenCod.CodVenc(db, codcia, punemi);
                if (codvenc == null) { return Json(new { respuesta = Msg.ErrGenerar(Msg.CODIGO + " de la venta") }, JsonRequestBehavior.AllowGet); }
                
                List<venpag> lista = JsonConvert.DeserializeObject<List<venpag>>(vp);
                int index = 1;
                foreach (venpag item in lista)
                {
                    item.CODCIA = codcia;
                    item.CODIGO = codvenc;
                    item.ITEM = index.ToString().PadLeft(3, '0');
                    item.RECIBIDO = item.VUELTO + item.IMPORTE;
                    if (String.IsNullOrWhiteSpace(item.FORPAGO)) { item.TARJETA = null; }
                    if (String.IsNullOrWhiteSpace(item.TARJETA)) { item.TARJETA = null; }
                    if (String.IsNullOrWhiteSpace(item.REFERENCIA)) { item.TARJETA = null; }
                    db.venpag.Add(item);
                    index++;
                }    
                
                conc conc=db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.CODIGO.Equals(codconc)).FirstOrDefault();
                if (conc == null) { return Json(new { respuesta = Msg.AttrNoExiste(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
                venc venc = new venc();
                venc.CODCIA = codcia;
                venc.CODIGO = codvenc;
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
                venc.SERIE = numdoc.ToString().Split('-')[0];
                venc.NRODOC = numdoc.ToString().Split('-')[1];
                venc.CONSUMO = conc.CODIGO;
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
                        vend.TOTAL = vend.SUBTOTAL;
                        vend.IGV = (decimal)(vend.TOTAL * prigv.COM_TASA_IGV / (100 + prigv.COM_TASA_IGV));
                        vend.CONVENTA = item.CONVENTA;
                        conventa conventa = db.conventa.Where(a => a.codcia.Equals(codcia) && a.codigo.Equals(item.CONVENTA) && a.situa.Equals("V")).FirstOrDefault();
                        vend.CODARTI = conventa.codigo;
                        vend.DESARTI = conventa.descripcion;
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
                    ciafile ciafile = db.ciafile.Where(a=>a.idcia.Equals(codcia)).FirstOrDefault();
                    sucursal suc = db.sucursal.Where(a => a.codcia.Equals(codcia) && a.codigo.Equals(sucursal)).FirstOrDefault();
                    var u = (from a in db.venpag join b in db.forventa on a.FORVENTA equals b.codigo
                             from c in db.forpago.Where(d=>d.codigo.Equals(a.FORPAGO)).DefaultIfEmpty()
                             from d in db.tarjetas.Where(t=>t.codigo.Equals(a.TARJETA)).DefaultIfEmpty()
                             where a.CODCIA.Equals(codcia) && a.CODIGO.Equals(codvenc)
                             select new{codigo=a.CODIGO, importe=a.IMPORTE, recibido=a.RECIBIDO,
                             vuelto=a.VUELTO, forventa=b.descripcion, forpago=c.descripcion, tarjeta=d.descripcion                             }
                                 ).ToList();
                    peratencion per = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.codigo.Equals(conc.PERATENCION)).FirstOrDefault();
                    maesgen mgmoneda = db.maesgen.Where(a=>a.idmaesgen.Equals("015") && a.clavemaesgen.Equals(venc.CODMON)).FirstOrDefault();
                    maesgen emision = db.maesgen.Where(a => a.idmaesgen.Equals("110") && a.clavemaesgen.Equals(tipdoc)).FirstOrDefault();
                    maesgen mgtd = db.maesgen.Where(a => a.idmaesgen.Equals("002") && a.clavemaesgen.Equals(anexo.tipdoc)).FirstOrDefault();
                    NumLetras nl = new NumLetras();

                    return Json(new
                    {
                        respuesta = Msg.OpExitosa,
                        cia = ciafile,
                        suc = suc,
                        fecha = venc.FECDOC,
                        hora = DateTime.Now.ToString("hh:mm tt"),
                        venc = venc,
                        anexo = anexo,
                        tipdoc = mgtd.parm1maesgen,
                        docemi = emision.parm6maesgen,
                        moneda = mgmoneda.parm1maesgen,
                        gravado = sumagra,
                        exonerado = sumaexo,
                        inafecto = sumaina,
                        igv = sumaigv,
                        total = total,
                        totalstr = nl.Numero_to_Letras(mgmoneda.clavemaesgen, total),
                        resumen = u,
                        cajero = desusr,
                        cod1 = venc.SERIE,
                        cod2 = venc.NRODOC,
                        abrevia = mgmoneda.abrevia,
                    }, JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { respuesta = "ERROR: Error el consumo est\u00E1 cerrado" }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex.Message }, JsonRequestBehavior.AllowGet); }
        }
    }
}
