﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Franquisia1._1.Models;
namespace Franquisia1._1.Controllers
{
    public class CondController : Controller
    {
        private appbosaEntities db = new appbosaEntities();
        [HttpPost]
        public JsonResult Guardar(string codigo, string items)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if ("C".Equals(rol) || "M".Equals(rol))
                {
                    if (!String.IsNullOrWhiteSpace(codigo))
                    {
                        if (!String.IsNullOrWhiteSpace(items))
                        {
                            string codcia = Session["Loged_usrfile_ciafile"].ToString();
                            string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                            List<cond> listaItems;
                            try { listaItems = JsonConvert.DeserializeObject<List<cond>>(items); }
                            catch (JsonException ex) { return Json(new { respuesta = "ERROR: El detalle de consumo no tinene un formato json v\u00E1lido\nDetalle de error:" + ex.Message }, JsonRequestBehavior.AllowGet); }
                            codigo = codigo.Trim();
                            conc conc = db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.CODIGO.Equals(codigo)).FirstOrDefault();
                            if (conc == null) { return Json(new { respuesta =Msg.AttrNoExiste(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
                            else
                            {
                                var lista = db.cond.Where(a => a.CODCIA.Equals(codcia) && a.CODIGO.Equals(conc.CODIGO)).ToList();
                                foreach (var cond in lista) { db.cond.Remove(cond); }
                                foreach (var item in listaItems)
                                {
                                    if (item != null)
                                    {
                                        item.ITEM = item.ITEM.PadLeft(4, '0');
                                        item.CODCIA = codcia;
                                        item.CODIGO = codigo;
                                        preconven preconven = db.preconven.Where(a => a.conventa.Equals(item.CONVENTA) && a.codcia.Equals(codcia) &&
                                            a.sucursal.Equals(sucursal) && a.state.Equals("V")).FirstOrDefault();
                                        if (preconven == null) { return Json(new { respuesta = "ERROR: No existe un precio asignado" }, JsonRequestBehavior.AllowGet); }
                                        item.PREUNI = preconven.precio;
                                        item.TOTAL = item.PREUNI * item.CANTIDAD;
                                        db.cond.Add(item);
                                    }
                                    else { return Json(new { respuesta = "ERROR: Detalle de consumo nulo, operaci\u00F3n cancelada" }, JsonRequestBehavior.AllowGet); }
                                }
                                db.SaveChanges();
                                return Json(new { respuesta = Msg.OpExitosa }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else { return Json(new { respuesta = Msg.AttrNoNuloVacio("El detalle del consumo") }, JsonRequestBehavior.AllowGet); }
                    }
                    else { return Json(new { respuesta = Msg.AttrNoNuloVacio(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
                }else { return Json(new { respuesta =Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }                
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult Eliminar(string codigo, string item)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if ("C".Equals(rol) || "M".Equals(rol))
                {
                    if (!String.IsNullOrWhiteSpace(codigo))
                    {
                        string codcia = Session["Loged_usrfile_ciafile"].ToString();
                        string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                        conc conc = db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.CODIGO.Equals(codigo)).FirstOrDefault();
                        if (conc == null) { return Json(new { respuesta = Msg.AttrNoExiste(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
                        if (conc.FACTURANDO.Equals("N"))
                        {
                            item = item.PadLeft(4, '0');
                            cond cond = db.cond.Where(a => a.CODCIA.Equals(codcia) && a.CODIGO.Equals(conc.CODIGO) && a.ITEM.Equals(item)).FirstOrDefault() ;
                            if (cond!=null)
                            {
                                db.cond.Remove(cond);
                                db.SaveChanges();
                                return Json(new { respuesta = Msg.OpExitosa }, JsonRequestBehavior.AllowGet);
                            }
                            else { return Json(new { respuesta = Msg.AttrNoExiste("El item") }, JsonRequestBehavior.AllowGet); }
                        }
                        else { return Json(new { respuesta = Msg.ADCPF }, JsonRequestBehavior.AllowGet); }
                    }
                    else { return Json(new { respuesta = Msg.AttrNoNuloVacio(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
                }
                else { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public JsonResult Obtener(string codund, string coddiv){
            try {
                var rol = Session["Loged_usrfile_rol"];
                if ("C".Equals(rol) || "M".Equals(rol))
                {
                    if (!String.IsNullOrWhiteSpace(codund))
                    {
                        codund = codund.Trim();
                        if (!String.IsNullOrWhiteSpace(coddiv))
                        {
                            coddiv = coddiv.Trim();
                            try
                            {
                                string codcia = Session["Loged_usrfile_ciafile"].ToString();
                                string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                                undatencion und = db.undatencion.Where(a => a.CODCIA.Equals(codcia) && a.CODIGO.Equals(codund) && a.SUCURSAL.Equals(sucursal) && a.DIVATENCION.Equals(coddiv) && a.ESTADO.Equals("V")).FirstOrDefault();
                                if (und != null)
                                {
                                    conc conc = db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.UNDATENCION.Equals(codund) && a.SITUACION.Equals("A")).FirstOrDefault();
                                    if (conc == null) { return Json(new { respuesta = "ADVERTENCIA: NO EXISTE EL CONSUMO" }, JsonRequestBehavior.AllowGet); }
                                    else
                                    {
                                        peratencion per = db.peratencion.Where(a => a.codcia.Equals(codcia) && a.codigo.Equals(conc.PERATENCION) && a.situa.Equals("V")).FirstOrDefault();
                                        if (per != null)
                                        {
                                            var cond = (from a in db.cond
                                                        join b in db.conventa on a.CONVENTA equals b.codigo
                                                        join c in db.maesgen on b.tipovalorventa equals c.clavemaesgen
                                                        where a.CODIGO.Equals(conc.CODIGO) && a.CODCIA.Equals(conc.CODCIA) && b.codcia.Equals(conc.CODCIA)
                                                        && c.idmaesgen.Equals("502")
                                                        select new
                                                        {
                                                            CONVENTA = a.CONVENTA,
                                                            ITEM = a.ITEM,
                                                            DESCRIPCION = b.descripcion,
                                                            PREUNI = a.PREUNI,
                                                            CANTIDAD = a.CANTIDAD,
                                                            TOTAL = a.TOTAL,
                                                            tipovalorventa = c.desmaesgen
                                                        }).OrderBy(a => a.ITEM).ToList();
                                            return Json(new { respuesta = Msg.OpExitosa, lista = cond, cabecera = conc, peratencion = per, undatencion = und }, JsonRequestBehavior.AllowGet);
                                        }
                                        else { return Json(new { respuesta = "ERROR: No exite una persona asignada a atender la unidad de atenci\u00F3n" }, JsonRequestBehavior.AllowGet); }
                                    }
                                }
                                else { return Json(new { respuesta = "ERROR:  La unidad de atenci\u00F3n no existe en la divisi\u00F3n seleccionada" }, JsonRequestBehavior.AllowGet); }
                            }
                            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex }, JsonRequestBehavior.AllowGet); }
                            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex }, JsonRequestBehavior.AllowGet); }
                        }
                        else { return Json(new { respuesta = Msg.AttrNoNuloVacio(Msg.CODDIV)}, JsonRequestBehavior.AllowGet); }
                    }
                    else { return Json(new { respuesta = Msg.AttrNoNuloVacio(Msg.CODUND) }, JsonRequestBehavior.AllowGet); }
                }
                else { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex }, JsonRequestBehavior.AllowGet); }
            catch (Exception ex) { return Json(new { respuesta = "ERROR: " + ex }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult Crear(string codigo, string cantidad, string conventa )
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if ("C".Equals(rol) || "M".Equals(rol))
                {
                    if (!String.IsNullOrWhiteSpace(codigo))
                    {
                        string codcia = Session["Loged_usrfile_ciafile"].ToString();
                        string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                        conc conc = db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.CODIGO.Equals(codigo)).FirstOrDefault();
                        if (conc == null) { return Json(new { respuesta = Msg.AttrNoExiste(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
                        if (conc.FACTURANDO.Equals("N"))
                        {
                            string coditem = GenCod.CodItem(db, codcia, conc.CODIGO);
                            cond cond = new cond();
                            cond.ITEM = coditem;
                            cond.CODCIA = codcia;
                            cond.CODIGO = codigo;
                            cond.CONVENTA = conventa;
                            cond.CANTIDAD = Convert.ToDecimal(cantidad);
                            preconven preconven = db.preconven.Where(a => a.conventa.Equals(cond.CONVENTA) && a.codcia.Equals(codcia) &&
                                a.sucursal.Equals(sucursal) && a.state.Equals("V")).FirstOrDefault();
                            if (preconven == null) { return Json(new { respuesta = "ERROR: No existe un precio asignado" }, JsonRequestBehavior.AllowGet); }
                            cond.PREUNI = preconven.precio;
                            cond.TOTAL = cond.PREUNI * cond.CANTIDAD;
                            db.cond.Add(cond);
                            db.SaveChanges();
                            return Json(new { respuesta = Msg.OpExitosa, item = cond.ITEM }, JsonRequestBehavior.AllowGet);
                        }
                        else { return Json(new { respuesta = Msg.ADCPF }, JsonRequestBehavior.AllowGet); }
                    }
                    else { return Json(new { respuesta = Msg.AttrNoNuloVacio(Msg.CODUND) }, JsonRequestBehavior.AllowGet); }
                }
                else { return Json(new { respuesta = Msg.PermisoDenegado }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult Actualizar(string codigo, string item, string cantidad)
        {
            try
            {
                var rol = Session["Loged_usrfile_rol"];
                if ("C".Equals(rol) || "M".Equals(rol))
                {
                    if (!String.IsNullOrWhiteSpace(codigo))
                    {
                        string codcia = Session["Loged_usrfile_ciafile"].ToString();
                        string sucursal = Session["Loged_usrfile_sucursal"].ToString();
                        conc conc = db.conc.Where(a => a.CODCIA.Equals(codcia) && a.SUCURSAL.Equals(sucursal) && a.CODIGO.Equals(codigo)).FirstOrDefault();
                        if (conc == null) { return Json(new { respuesta =Msg.AttrNoExiste(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
                        if (conc.FACTURANDO.Equals("N"))
                        {
                            cond cond = db.cond.Where(a => a.CODCIA.Equals(codcia) && a.CODIGO.Equals(codigo) && a.ITEM.Equals(item)).FirstOrDefault();
                            cond.CANTIDAD = Convert.ToDecimal(cantidad);
                            cond.TOTAL = cond.PREUNI * cond.CANTIDAD;
                            db.SaveChanges();
                            return Json(new { respuesta = Msg.OpExitosa }, JsonRequestBehavior.AllowGet);
                        }
                        else { return Json(new { respuesta = Msg.ADCPF }, JsonRequestBehavior.AllowGet); }
                    }
                    else { return Json(new { respuesta = Msg.AttrNoNuloVacio(Msg.CODCONSUMO) }, JsonRequestBehavior.AllowGet); }
                }
                else { return Json(new { respuesta = Msg.OpExitosa }, JsonRequestBehavior.AllowGet); }
            }
            catch (System.Data.EntityException ex) { return Json(new { respuesta = "ERROR: " + ex }, JsonRequestBehavior.AllowGet); }
            catch (Exception e) { return Json(new { respuesta = "ERROR: " + e }, JsonRequestBehavior.AllowGet); }
        }
    }
}
