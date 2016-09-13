var Form = {
    CITEMS: 0, CPRODUCTO: 1, CPRECIO: 2, CTIPOVV: 3, CCANT: 4, CSUBT: 5, CBTNELI: 6,
    CFV: 0, CFP: 1, CTTAR: 2, CNROREF: 3, CIMPORTE: 4,
    IGV: 18,
    Tabla: "#tabla-factura",
    Und: "#undatencion",
    Div: "#divatencion",
    UndDesc: Form.Und + "-desc",
    PerDesc: "#peratencion-desc",
    Anexo: Form.Tabla + "-anexo",
    GroupTipDocEmi: "input[name=tipo-documentos]",
    FormAnexo: "#modal-crear-anexo",
    FormSelPerAte: "#modal-seleccionar-peratencion",
    FormCambiarUnd: "#modal-cambiar-undatencion",
    SelectCambiarDiv: "#modal-cambiar-divatencion",
    updateTotalCheckbox: function (t, cs, r) {
        var s = 0;
        $(t).find("tbody tr").each(function (index) { if ($(this).find("input[type=checkbox]").prop("checked")) { s += parseFloat($(this).find("td").get(cs).innerHTML); } });
        r.text(s.toFixed(2));
    },
    updateItemsCheckbox: function (t, r) {
        var i = 0;
        $(t).find("tbody tr").each(function (index) { if ($(this).find("input[type=checkbox]").prop("checked")) { i += 1; } });
        r.text(i);
    },
    updateIGV: function (t) {
        var tl = parseFloat($(t + "-total").text()); var s = tl * Form.IGV / (100 + Form.IGV); $(t + "-igv").text(s.toFixed(2));
    },
    getIGV: function () {
        $.ajax({
            type: "post", dataType: 'json', cache: false, url: "/Parreg/ObtenerIGV", data: {},
            success: function (response, textStatus, jqXHR) {
                if (App.isError(response.respuesta)) { alert(response.respuesta); }
                else if (App.isExito(response.respuesta)) {
                    parseFloat(response.igv) != undefined ? Form.IGV = parseFloat(response.igv) : Form.IGV = 18;
                }
            },
            error: function (xhr, status) { errorAjax(xhr, status); }
        });
    },
    



    getInput: function (id, t, l, p) {
        return $("<div class='form-group'><label for='" + id + "'>" + l + "</label><input id='" + id + "' type='" + t + "' class='form-control' placeholder='" + p + "' /></div>");
    },
    getTextArea: function (id, l, r, p) {
        return $("<div class='form-group'><label for='" + id + "'>" + l + "</label><textarea id='" + id + "' rows='" + r + "' class='form-control' placeholder='" + p + "'></textarea></div>");
    }
}
var Anexo = {
    FormBody: "#modal-crearane-body",
    BtnSubmit: "#modal-crear-anexo-submit",
    GroupTipDoc: "#group-tipane",
    InputGroupTD:"input[name=tipo-cm]",
    BtnTipDoc: "btn-info",
    initForm: function () {
        var grnt = $(Anexo.InputGroupTD+":radio"); grnt.first().prop('checked', true); grnt.first().parent().addClass(Anexo.BtnTipDoc);
        Anexo.updateForm(grnt.first());
    },
    initForm: function(v){
        $(Anexo.InputGroupTD + ":checked").prop('checked', false);
        $(Anexo.InputGroupTD + ":checked").removeClass(Anexo.BtnTipDoc);
        $(Anexo.InputGroupTD + "[value='" + v + "']").prop('checked', true);
        $(Anexo.InputGroupTD).parent().addClass(Anexo.BtnTipDoc);
        Anexo.updateForm(grnt.first());
    },
    updateForm: function (i) {
        $(Anexo.FormBody).empty();
        $(Anexo.FormBody).append(Anexo.getFieldNrodoc(i.data("desc")));
        switch (i.data("codigo")) {
            case "01": case "04":case "07":
                $(Anexo.FormBody).append(Anexo.getFieldNombres());
                $(Anexo.FormBody).append(Anexo.getFieldApe());
                break;
            case "06":
                $(Anexo.FormBody).append(Anexo.getFieldRS());
                $(Anexo.FormBody).append(Anexo.getFieldRefane());
                break;
        }
        if ($(App.ControlTeclado+":checked").val() == "ACT") {
            $(Anexo.FormBody).find("input[type=text]").keyboard();
            $(Anexo.FormBody).find("textarea").keyboard();
        }
    },
    getFieldNrodoc: function (t) {return Form.getInput('input-nrodoc', 'text', t, 'INGRESE SU ' + t);},
    getFieldRefane: function () {return Form.getTextArea('input-refane', 'Direcci\u00F3n', 2, 'Ingrese su Direcci\u00F3n');},
    getFieldApe: function () {return Form.getInput('input-ape', 'text', 'Apellidos', 'Ingrese sus apellidos');},
    getFieldNombres: function () {
        var d = $("<div></div>"); d.append(Form.getInput('input-nom1', 'text', 'Nombre 1', 'Ingrese su primer nombre')); d.append(Form.getInput('input-nom2', 'text', 'Nombre 2', 'Ingrese su segundo nombre'));
        return d;
    },
    getFieldRS: function () { return Form.getTextArea('input-desane', 'Raz\u00F3n social', 2, '');},

    show: function (a, d) {d.find(".anexo-desane").val(a.desane); d.find(".anexo-nrodoc").val(a.nrodoc); d.find(".anexo-refane").val(a.refane);},
    clear: function (d) { Anexo.show({ "desane": "", "nrodoc": "", "refane": "" }, d); },
    get: function (d) { return { desane: d.find(".anexo-desane").val(), nrodoc: d.find(".anexo-nrodoc").val(), refane: d.find(".anexo-refane").val() }; },
    updateDB: function (a, d) {
        if (!Validar.Anexo(a)) { return; }
        $.ajax({
            type: "post", dataType: 'json', cache: false, url: "/Anexos/Actualizar",
            data: { a:JSON.stringify(a)},
            success: function (response, textStatus, jqXHR) {
                if (App.isError(response.respuesta) || App.isAdvert(response.respuesta)) { alert(response.respuesta); }
                else if (App.isExito(response.respuesta)) { Anexo.show(response.anexo, d); }
            },
            error: function (xhr, status) { errorAjax(xhr, status); }
        });
    },
    getDB: function (div, f) {
        var a = Anexo.get(div);
        var input = Form.GroupTipDocEmi+":checked";
        if (!Validar.Anexo(a)) {return alert("ERROR: Anexo incorrecto");}
        $.ajax({
            type: "post", dataType: 'json', cache: false, url: "/Anexos/Obtener", data: { nrodoc: a.nrodoc },
            success: function (response, textStatus, jqXHR) {
                if (App.isError(response.respuesta)) { alert(response.respuesta); }
                else if (App.isAdvert(response.respuesta)) {
                    Anexo.show({ "nrodoc": a.nrodoc, "refane": "", "desane": "" }, div);
                    var m = $(Form.FormAnexo);
                    m.find("input[type='text']").val("");
                    m.find("input[type='text']").first().val(a.nrodoc);
                    m.find(Anexo.BtnSubmit).text("Guardar");
                    Anexo.initForm();
                    m.modal("show");
                }
                else if (App.isExito(response.respuesta)) { Anexo.show(response.anexo, div); f == null ? "" : f(response.anexo); }
            }, error: function (xhr, status) { errorAjax(xhr, status); }
        });
    },
    createDB: function (a, d, m) {
        if (!Validar.Anexo(a)) { return alert(Msg.ANEXO_INVALIDO); }
        $.ajax({
            type: "get", dataType: 'json', cache: false, url: '/Anexos/Crear', data: { a: JSON.stringify(a) },
            success: function (response, textStatus, jqXHR) {
                if (App.isError(response.respuesta)) { return alert(response.respuesta); }
                if (App.isExito(response.respuesta) || App.isAdvert(response.respuesta)) { Anexo.show(response.anexo, d); m==null?"":m.modal('hide'); }
            }, error: function (xhr, status) { errorAjax(xhr, status); }
        });
    }
};
function resetTabla(t) { Anexo.clear($(t + "-anexo")); $(t).find("tbody tr").remove(); $(t).data("codigo", ""); $(t + "-items").text("-"); $(t + "-igv").text("-"); $(t + "-total").text("-"); }

function printFactura(t, a, r) {

    var s1 = "<pre>{cia-desc}\n{slogan}\n\n{cia-nom}\nRUC {ruc}\nCentral: {dir-central}\n{telefono}\nBOLETA DE VENTA ELECTRÓNICA\n{cod1}-{cod2}</pre>";
    var s2 = "<pre class='text-left'>Tienda {tienda}\n{direccion}\n{distrito}-{departamento}\nFecha: {fecha}   Hora: {hora}\n************************************\nCORRELATIVO\t: {correlativo}\nCAJA\t\t: {caja}\nTIPO DE MONEDA\t: {moneda}\nCLIENTE\t\t: {cliente}\nDOC. IDENTIDAD	: {tipdoc} : {nro-doc}\n************************************\nARTICULO\t|CANT|PRECIO|IMPORTE\n************************************\n<table id='t-i'><thead><tr><th></th><th style='width:30px;'></th><th style='width:50px;'></th><th style='width:50px;'></th></tr></thead></table>====================================<table><tr><td>\t\tTOTAL	S/.</td><td id='total'></td></tr><tr></tr><tr><td>Op. Exonerada\tS/.</td><td id='exonerada'></td></tr><tr><td>Op. Inafecta\tS/.</td><td id='inafecta'></td></tr><tr><td>Op. Gravada\tS/.</td><td id='gravada'></td></tr><tr><td>IGV\t\tS/.</td><td id='igv'></td></tr><tr><td>Importe Total\tS/.</td><td id='importe-total'></td></tr></table><div id='resumen'></div></pre>";
    var s3 = "<pre>{cod-gen}\n\nPresentación impresa del Comprobante de Venta Electrónica, esta puede ser consultada en {pag-web} autorizado mediante resolución de intendencia {resol-inten} / SUNAT\n\n{pag-web}\nGRACIAS POR SU COMPRA</pre>";
    var su = $("<div></div>");
    s1 = s1.replace("{cia-desc}", r.cia.descia);
    s1 = s1.replace("{slogan}", r.cia.slogan);
    s1 = s1.replace("{cia-nom}", r.cia.shortcia);
    s1 = s1.replace("{ruc}", r.cia.ruccia);
    s1 = s1.replace("{dir-central}", r.cia.dircia);
    s1 = s1.replace("{telefono}", r.cia.telefonos);
    s1 = s1.replace("{cod1}", r.cod1);
    s1 = s1.replace("{cod2}", r.cod2);

    s2 = s2.replace("{tienda}", r.suc.descripcion);
    s2 = s2.replace("{direccion}", r.suc.direccion);
    s2 = s2.replace("{distrito}", r.suc.distrito);
    s2 = s2.replace("{departamento}", r.suc.departamento);
    s2 = s2.replace("{fecha}", r.fecha);
    s2 = s2.replace("{hora}", r.hora);
    s2 = s2.replace("{correlativo}", r.venc.CODIGO);
    s2 = s2.replace("{caja}", r.venc.PUNEMI);
    s2 = s2.replace("{moneda}", r.venc.CODMON);
    s2 = s2.replace("{cliente}", r.anex.desane);
    s2 = s2.replace("{tipdoc}", r.tipdoc);
    s2 = s2.replace("{nro-doc}", r.anexo.nrodoc);

    s3 = s3.replace("{cod-gen}", "");
    s3 = replaceAll(s3, "{pag-web}", r.cia.website);
    s3 = s3.replace("{resol-inten}", r.cia.resintendencia);

    s2 = $(s2);

    var ti = s2.find("#t-i");
    t.find("tbody tr").each(function () {
        var nf = $("<tr></tr>");
        nf.append($("<td></td>").append($(this).find("td").get(Form.CPRODUCTO).innerHTML));
        nf.append($("<td></td>").append($(this).find("td input[type=number]").val()));
        nf.append($("<td></td>").append($(this).find("td").get(Form.CPRECIO).innerHTML));
        nf.append($("<td></td>").append($(this).find("td").get(Form.CSUBT).innerHTML));
        ti.append(nf);
    });
    var res = s2.find("#resumen");
    $.each(r.resumen, function (idx, obj) {
        s = "SON: "+"\n";
        s += obj["descripcion"] + " " + obj["RECIBIDO"] + " NUEVOS SOLES\n";
        if (obj["tarjeta"] != "S") { s += "VUELTO: " + parseFloat(obj["VUELTO"]).toFixed(2) + " NUEVOS SOLES";}
        res.append(s);
    });
    res.append(r.cajero);
    s2.find("#total").append(r.total);
    s2.find("#gravada").append(r.gravado);
    s2.find("#inafecta").append(r.inafecto);
    s2.find("#exonerada").append(r.exonerado);
    s2.find("#igv").append(r.igv);
    s2.find("#importe-total").append(r.total);
    su.append($(s1)); su.append(s2); su.append($(s3));
    if (!print(su)) { alert(Msg.ERROR_IMPRIMIR); }
}
function guardarFactura(t) {
    if (!Validar.Dato(t.data("codigo"))) { return alert(Msg.NO_EXISTE.replace("el c\u00F3digo del consumo")); }
    var cod = t.data("codigo"); var ar = new Array(); var cond;
    t.find("tbody tr").each(function (index) {
        var f = $(this);
        cond = {
            "CANTIDAD": parseFloat(f.find("td input[type=number]").val()), "CONVENTA": f.data("codigo"), "ITEM": f.find("td").get(Form.CITEMS).innerHTML,
        };
        ar.push((cond));
    });
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: '/Cond/Guardar', data: { codigo: cod, items: JSON.stringify(ar) },
        success: function (response, textStatus, jqXHR) { alert(response.respuesta);},
        error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function obtenerDetalle(sl, cd, t, pa, ua, f) {
    if (!Validar.Dato(sl.val()) || !Validar.Dato(cd) || !Validar.Dato(t)) { return; }
    if (!Validar.StrSoloNum(sl.val())) { return alert(Msg.ERROR_CAMPO_NUMERICO.replace("<attr>", Msg.UNDATENCION)); }
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Obtener", data: { codund: sl.val(), coddiv: cd },
        success: function (response, textStatus, jqXHR) {
            resetTabla(t); clearDesc(pa, ua);
            if (App.isError(response.respuesta)) { alert(response.respuesta);}
            else if (App.isExito(response.respuesta)) {
                $(ua).data("codigo", response.undatencion.CODIGO);
                $(ua).text(response.undatencion.DESCRIPCION);
                $(t).data("codigo", response.cabecera.CODIGO);
                $(pa).data("codigo", response.peratencion.codigo);
                $(pa).text(response.peratencion.descripcion);
                $.each(response.lista, function (idx, obj) {
                    var txtc = App.getInputCantidad();
                    var btne = App.getBtnRemove();
                    var el = [$(t).find("tr").length, obj["DESCRIPCION"], parseFloat(obj["PREUNI"]).toFixed(2), obj["tipovalorventa"].substring(0, App.CARSUBSTR), txtc, parseFloat(obj["TOTAL"]).toFixed(2), btne];
                    var nf = addRow($(t), el);
                    nf.data("codigo", obj["CONVENTA"]);
                    btne.on('click', function () {removeRow(nf); actualizarTotal(t, Form.CSUBT);actualizarItems(t, Form.CITEMS); actualizarIGV(t);});
                    txtc.change(function () {
                        var cant = parseFloat($(this).val());
                        if (cant>0) {
                            actualizarSubtotal(nf, txtc, Form.CPRECIO, Form.CSUBT); actualizarTotal(t, Form.CSUBT); actualizarIGV(t);
                        } else { $(this).val(1); alert(Msg.CANT_NO_MENOR_A.replace("<attr>", "una unidad")); }
                    });
                    $(t).append(nf);
                });
                actualizarTotal(t, Form.CSUBT); actualizarItems(t, Form.CITEMS); actualizarIGV(t);
            } else if (App.isAdvert(response.respuesta)) { f(sl);}
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function agregarProducto(pp) {
    var tf = Form.Tabla;
    var btne = App.getBtnRemove();
    var txtc = App.getInputCantidad();
    var n = pp.find(".nombre").first().text();var p = parseFloat(pp.data("precio")).toFixed(2);
    var el = [$(tf).find("tr").length, n, p, pp.data("tipovv").substring(0, App.CARSUBSTR), txtc, p, btne]; var nf = addRow($(tf), el); nf.data("codigo", pp.data("codigo"));
    actualizarTotal(tf, Form.CSUBT); actualizarItems(tf, Form.CITEMS); actualizarIGV(tf);
    btne.on('click', function () {
        removeRow(nf); actualizarTotal(tf, Form.CSUBT); actualizarItems(tf, Form.CITEMS); actualizarIGV(tf);
    });
    txtc.change(function () {
        var cant = parseFloat($(this).val());
        if (cant > 0) { actualizarSubtotal(nf, txtc, Form.CPRECIO, Form.CSUBT); actualizarTotal(tf, Form.CSUBT); actualizarIGV(tf); actualizarIGV(tf); }
        else { $(this).val(1); alert(Msg.CANT_NO_MENOR_A.replace("<attr>", "una unidad")); }
    });
}
function changeUndAtencion(txt) {
    if (Validar.Dato($(Form.Div).val())) {
        if(Validar.Dato(txt.val())){
            obtenerDetalle(txt, $(Form.Div).val(), Form.Tabla, Form.PerDesc, Form.UndDesc,
                function (select) {
                    if (confirm(MSG_DESEA_APERTURAR)) { $(Form.FormSelPerAte).modal('show'); }
                });
        } else { alert(Msg.SELECCION_UND); }
    } else { $(this).val(""); alert(Msg.SELECCION_DIV); }
}
function initTipDoc(n) {
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: '/Parreg/TipDocDefault', data: { },
        success: function (response, textStatus, jqXHR) {
            if (App.isExito(response.respuesta)) {
                var td = $("input[name='" + n + "'][ value='"+response.codigo+"']"); td.prop('checked', true); td.parent().addClass("active");
            }
        },
        error: function (xhr, status) { errorAjax(xhr, status); }
    });
}

var FormPago = {
    Name: "#modal-pagar",
    DivP: FormPago.Name + "-principal",
    DivS: FormPago.Name + "-secundario",
    GroupFV:"#group-forventa",
    InputGroupFV: "input[name='forma-venta']",
    BtnFV: "btn-warning",
    LblFV: "#forventa",
    GroupFP: "#group-forpago",
    InputGroupFP: "input[name='forma-pago']",
    BtnFP: "btn-primary",
    LblFP: "#forpago",
    Importe: "#input-importe",
    TotalPagar: "#form-pagar-total",
    TotalPagado: "#forma-pago-total",
    Tabla:"#tabla-forma-pago",
    initForm: function() {
        var grnt = $(FormPago.InputGroupFV+":radio"); grnt.first().parent().siblings("div").removeClass(FormPago.BtnFV);
        grnt.first().prop('checked', true); grnt.first().parent().addClass(FormPago.BtnFV);
        $(FormPago.LblFV).text(grnt.first().data("desc"));
        this.updateForm();
    },
    updateForm: function (i) {
        var o = $(this.InputGroupFV+":checked");
        var p = $(this.DivP);
        var s = $(this.DivS);
        p.empty(); s.empty();
        $(this.Importe).val("00.00");
        switch (o.val()) {
            case "C":
                $(this.GroupFP+", "+this.LblFP).removeClass("hide");
                $(this.LblFP).siblings("label").removeClass("hide");
                if (!Validar.Dato(i) || !Validar.Dato(i.val())) {
                    i = $(this.InputGroupFP).first(); i.prop('checked', true); i.parent().addClass(this.BtnFP);
                }
                if (i.data("istarjeta") == "S") { this.isTarjeta(p, s); }
                if (i.data("isreferencia") == "S") { this.isNroRef(p, s); }
                if (i.data("isefectivo") == "S") { this.isEfectivo(p, s); }
                $(this.LblFP).text(i.data("desc"));
                break;
            case "P":
                $(this.GroupFP+", "+this.LblFP).addClass("hide");
                $(this.LblFP).siblings("label").addClass("hide");
                $(this.Importe).prop("disabled", false);
                i.prop('checked', false);
                i.parent().removeClass(this.BtnFP);
                var aux = (parseFloat($(this.TotalPagar).text()) - parseFloat($(this.TotalPagado).text()));
                if (aux > 0) { $(this.Importe).val(aux.toFixed(2)); }
                break;
        }
    },
    isTarjeta: function (p,s) {
        var html = $("<div class='form-group'><label>Tarjeta:</label> <label class='tarjeta'></label></div>");
        p.append(html);
        this.getTarjetas(s);
        var aux = (parseFloat($(this.TotalPagar).text()) - parseFloat($(this.TotalPagado).text()));
        if (aux > 0) { $(this.Importe).val(aux.toFixed(2)); }
        $(this.Importe).attr('disabled', false);
    },
    isNroRef: function (p,s) {
        var i = $("<input type='number' placeholder='Ingrese Nro. de referencia' class='txt-nro-ref form-control-static'>");
        if ($(App.ControlTeclado + ":checked").val() == "ACT") { tecladoNumerico(i); }
        p.append($("<div class='form-group'><label>Nro. referencia: </label></div>").append(i));
        var aux = (parseFloat($(this.TotalPagar).text()) - parseFloat($(this.TotalPagado).text()));
        if (aux > 0) { $(this.Importe).val(aux.toFixed(2)); }
        $(this.Importe).attr('disabled', false);
    },
    isEfectivo: function (p, s) {
        var BtnGroupJust = "class='btn-group btn-group-sm btn-group-justified'";
        html = $("<div id='denominaciones'><label>Nominaciones</label><div class='efectivo text-center'><div "+BtnGroupJust+"><a>S/. 200</a><a>S/. 100</a><a>S/. 50</a><a>S/. 20</a></div><div "+BtnGroupJust+"><a>S/. 10</a><a>S/. 5</a><a>S/. 2</a><a>S/. 1</a></div><div "+BtnGroupJust+"><a>S/. 0.50</a><a>S/. 0.20</a><a>S/. 0.10</a><aux class='btn btn-danger input-reset'>Limpiar</aux></div></div></div>;");
        html.find("a").addClass("btn btn-secundario btn-secundario-bordes");
        s.append(html);
        p.append("<div class='form-group'><label>Vuelto: </label> <label id='lbl-vuelto'>00.00</label></div>");
        $(this.Importe).val("0.00");
        html.find('a').on("click", function () {
            var ipI = $(FormPago.Importe); var ipC = p.find("#lbl-vuelto"); var e = parseFloat($(this).text().split(' ')[1]); var ef = parseFloat(ipI.val());
            if (isNaN(ef)) { ef = 0; } var nef = e + ef; ipI.val(parseFloat(nef).toFixed(2));
            var nc = nef - (parseFloat($(FormPago.TotalPagar).text()) - parseFloat($(FormPago.TotalPagado).text())).toFixed(2);
            if (nc >= 0) { ipC.text(nc.toFixed(2)); }
        });
        html.find(".input-reset").on("click", function () {$(FormPago.Importe).val("00.00"); p.find("#lbl-vuelto").text("00.00");});
        var aux = (parseFloat($(this.TotalPagar).text()) - parseFloat($(this.TotalPagado).text()));
        if (aux > 0) { $(this.Importe).val(aux.toFixed(2)); }
        $(this.Importe).attr('disabled', true);
    },
    getTarjetas:function (p) {
        $.ajax({
            type: "get", dataType: 'json', cache: false, url: "/Tarjetas/Obtener", data: {},
            success: function (response, textStatus, jqXHR) {
                p.empty();
                if (App.isError(response.respuesta)) { return alert(response.respuesta); }
                if (App.isExito(response.respuesta)) {
                    $.each(response.lista, function (idx, obj) {
                        var d = $("<div class=\"btn\"></div>"); d.data("desc", obj["descripcion"]); d.data("codigo", obj["codigo"]);
                        var i = $("<input name=\"tipo-tarjeta\" value=\"" + obj["codigo"] + "\" class=\"hide\" type=\"radio\">");
                        var img;
                        if (obj["foto64"] != null) {
                            img = $("<img class=\"img-responsive img-thumbnail \" src=\"data:image/gif;base64," + decBase64(obj["foto64"]) + "\" style=\"height:40px;\" />");
                        } else { d.append(obj["descripcion"]); }
                        d.append(i); d.append(img); p.append(d);
                    });
                    p.find(".btn").on("click", function () {
                        $(this).siblings(".btn").removeClass("btn-success");
                        $(this).addClass("btn-success");
                        $(this).children("input[type=radio]").prop('checked', true);
                        p.parent().find(".tarjeta").text($(this).data("desc"));
                        p.parent().find(".tarjeta").data("codigo", $(this).data("codigo"));
                    });
                }
            }, error: function (xhr, status) { errorAjax(xhr, status); }
        });
    },
    add:function () {
        var ifv = $(this.InputGroupFV+":checked");
        var ifp = $(this.InputGroupFP+":checked");
        if (!Validar.Dato(ifv.val())) { return alert(Msg.NO_NULO_O_VACIO.replace("<attr>", "La forma de venta")); }
        var t = $(this.Tabla);
        var e = new Array();
        var aux = (parseFloat($(this.TotalPagar).text()) - parseFloat($(this.TotalPagado).text()));
        e.push(ifv.data("desc"));
        switch (ifv.val()) {
            case "C":
                if (ifv.val() == "C" && !Validar.Dato(ifp.val())) { return alert(Msg.NO_NULO_O_VACIO.replace("<attr>", "La forma de pago")); }
                if (aux > 0) {
                    e.push(ifp.data("desc"));
                    var pp = $(this.DivP);
                    if (ifp.data("istarjeta") == "S") {
                        var tt = pp.find(".tarjeta");
                        if (!Validar.Dato(tt.text())) { return alert("ERROR: Seleccione un tipo de tarjeta"); }
                        e.push(tt.text()); tt.text("");
                    }
                    else { e.push(""); }
                    if (ifp.data("isreferencia") == "S") {
                        var nroref = pp.find(".txt-nro-ref"); e.push(nroref.val()); nroref.val("");
                    }
                    else { e.push(""); }
                    var importe = parseFloat($(this.Importe).val().replace(',', '.'));
                    if (importe == 0) { return alert("ERROR: El Importe No puede ser 00.00"); }
                    if (importe > parseFloat($(this.TotalPagar).text()) - parseFloat($(this.TotalPagado).text())) {
                        importe = parseFloat($(this.TotalPagar).text()) - parseFloat($(this.TotalPagado).text());
                    }
                    e.push(importe.toFixed(2));
                    $(this.Importe).val("00.00");
                    var f = addRow(t, e);
                    if (ifp.data("isefectivo") == "S") { f.data("cambio", $("#lbl-vuelto").text()); $("#lbl-vuelto").text("00.00"); }
                    else { f.data("cambio", "0"); }
                    f.data("fv", ifv.data("codigo")); f.data("fp", ifp.val()); f.data("tt", pp.find(".tarjeta").data("codigo"));
                    $(this.TotalPagado).text(sumar(t, Form.CIMPORTE).toFixed(2));
                } else { alert("ERROR: No se puede agregar, porque el total a pagar ya coincide con la suma de los montos agregado"); }
                break;
            case "P":
                if (aux > 0) {
                    e.push(""); e.push(""); e.push("");
                    e.push($(this.Importe).val());
                    var f = addRow(t, e);
                    f.data("cambio", "0");
                    $(this.TotalPagado).text(sumar(t, Form.CIMPORTE).toFixed(2));
                    $(this.Importe).val("00.00");
                } else { alert("ERROR: No se puede agregar, porque el total a pagar ya coincide con la suma de los montos agregado"); }
                break;
        }
    },
    aceptar: function () {
        var ifv = $(this.InputGroupFV+":checked");
        var ifp = $(this.InputGroupFP+":checked");
        if (parseFloat($(this.TotalPagar).text()) == parseFloat($(this.TotalPagado).text())) {
            var conc = $(Form.Tabla).data("codigo");
            var anexo = Anexo.get($(Form.Anexo));
            var td = $("input[name=tipo-documentos]:checked").val();
            var fp = new Array();
            $(this.Tabla).find("tbody tr").each(function (index) {
                var f = $(this);
                var i = {
                    "FORVENTA": f.data("fv"), "FORPAGO": f.data("fp"), "TARJETA": f.data("tt"), "REFERENCIA": f.find("td").get(Form.CNROREF).innerHTML, "IMPORTE": f.find("td").get(Form.CIMPORTE).innerHTML, "VUELTO": f.data("cambio"),
                };
                fp.push(i);
            });
            $.ajax({
                type: "post", dataType: 'json', cache: false, url: "/Conc/Facturar",
                data: { codconc: conc, nroane: anexo.nrodoc, desane: anexo.desane, refane: anexo.refane, tipdoc: td, vp: JSON.stringify(fp) },
                success: function (response, textStatus, jqXHR) {
                    if (App.isError(response.respuesta)) { alert(response.respuesta); }
                    if (App.isError(response.respuesta)) {
                        printFactura($(Form.Tabla), anexo, response);
                        $(Form.Und).val("");
                        clearDesc(Form.PerDesc, Form.UndDesc);
                        resetTabla(Form.Tabla);
                        window.location = "/UndAtencion/Index";
                    }
                }, error: function (xhr, status) { errorAjax(xhr, status); }
            });
            $(FormPago.Name).modal("hide");
        } else { alert("ERROR: Para poder realizar la transacci\u00F3n los totales debe coincidir"); }
    }
};
$(document).ready(function () {
    Form.IGV = obtenerIGV();
    initTipDoc("tipo-documentos");
    obtenerDetalle($(Form.Und), $(Form.Div).val(), Form.Tabla, Form.PerDesc, Form.UndDesc, function (s) {; });
    Anexo.initForm();
    $(Form.Div).on("change", function () { $(Form.Und).val(""); resetTabla(Form.Tabla); clearDesc(Form.PerDesc, Form.UndDesc); });
    $(Form.Und).keypress(function (e) { if (e.which == 13) { changeUndAtencion($(this)); }});
    $(Form.Und).bind('accepted', function (e, keyboard, el) { changeUndAtencion($(this));});
    if ($(App.ControlTeclado + ":checked").val() == "ACT") { $("input[type='text']").keyboard(); tecladoNumerico($("input[type='number']")); }
    $(".anexo").find(".anexo-nrodoc").bind('accepted', function (e, keyboard, el) { Anexo.getDB($(this).parent().parent()); });
    $(".anexo").find(".anexo-nrodoc").keypress(function (e) { if (e.which == 13) { Anexo.getDB($(this).parent().parent()); } });

    //$(".anexo").find(".anexo-desane, .anexo-refane").bind('accepted', function (e, keyboard, el) { Anexo.updateDB(Anexo.get($(this).parent()), $(Form.Anexo)); });
    //$(".anexo").find(".anexo-desane, .anexo-refane").keypress(function (e) { if (e.which == 13) { Anexo.updateDB(Anexo.get($(this).parent()), $(Form.Anexo)); } });

    $(".btn-seleccionar-anexo").on("click", function () {
        $(".modal").modal("hide");var modal = $("#modal-seleccionar-anexo");modal.data("anexo", $(this).data("anexo"));modal.modal('show');
    });
    $(".btn-actualizar-anexo").on("click", function () {
        Anexo.getDB($(Form.Anexo), function (a) {
            Anexo.initForm(a.tipdoc);
            var m = $(Form.FormAnexo);
            m.find("#input-nrodoc").val(a.nrodoc);
            m.find("#input-refane").val(a.refane);
            m.find("#input-ape").val(a.ape);
            m.find("#input-nom1").val(a.nom1);
            m.find("#input-nom2").val(a.nom2);
            m.find("#input-desane").val(a.desane);
            m.find(Anexo.BtnSubmit).text("Actualizar");
            m.modal("show");
        } );
    });
    $("#btn-buscar-razon").on("click", function () {
        buscarAjax("/Anexos/BuscarRazon", $("#txtBuscar").val(), $("#tabla-seleccionar-anexo"), ["desane","tipdoc", "nrodoc", "refane"],
        function (f) {
            var m = $("#modal-seleccionar-anexo");
            f.on("click", function () {
                var input = "input[name=tipo-documentos]:checked";
                var aux = "";
                switch ($(input).val()) {
                    case "01": aux = "DNI"; break;
                    case "03": aux = "RUC"; break;
                }
                if (aux != f.find('td').get(1).innerHTML) {
                    return alert("ERROR: No puede colocar un " + f.find('td').get(1).innerHTML + " cuando se emite una " + $(input).data("desc"));
                }

                var a = {desane: f.find('td').get(0).innerHTML, nrodoc: f.find('td').get(2).innerHTML, refane: f.find('td').get(3).innerHTML};
                Anexo.show(a, $(m.data("anexo"))); m.modal('hide');
            });
        });
    });
    $("#btn-buscar-ruc").on("click", function () {
        buscarAjax("/Anexos/BuscarRuc", $("#txtBuscar").val(), $("#tabla-seleccionar-anexo"), ["desane", "tipdoc", "nrodoc", "refane"],
        function (f) {
            var m = $("#modal-seleccionar-anexo");
            f.on("click", function () {
                var input = "input[name=tipo-documentos]:checked";
                var aux = "";
                switch ($(input).val()) {
                    case "01":aux = "RUC";break;
                    case "03":aux = "DNI";break;
                }
                if (aux != f.find('td').get(1).innerHTML) {
                    return alert("ERROR: No puede colocar un " + f.find('td').get(1).innerHTML + " cuando el se emite una " + $(input).data("desc"));
                }
                var a = {desane: f.find('td').get(0).innerHTML, nrodoc: f.find('td').get(2).innerHTML, refane: f.find('td').get(3).innerHTML};
                Anexo.show(a, $(m.data("anexo")));
                m.modal('hide');
            });
        });
    });

    $("#factura-btn-pagar").on("click", function () {
        var t = $(Form.Tabla);
        if (!Validar.Dato($(Form.Div).val())) { return alert(Msg.SELECCION_UND); }
        if (!Validar.Dato($(Form.Tabla).data("codigo"))) { return alert(Msg.SELECCION_DIV); }
        if (t.find("tbody tr").length <= 0) { return alert(Msg.SIN_ELEMENTOS); }
        if (!Validar.Anexo(Anexo.get($(Form.Anexo)))) { return; }
        var m = $(FormPago.Name);
        m.data("tabla", Form.Tabla); m.data("anexo", Form.Anexo);
        $("#form-pagar-total").text(parseFloat($("#tabla-factura-total").text()).toFixed(2));
        $("#forma-pago-total").text("0.00"); $("#tabla-forma-pago").find("tbody tr").remove();
        FormPago.initForm();
        m.modal('show');
    });
   
    $("#cambiar-undatencion").on("click", function () {
        if (!Validar.Dato($(Form.Div).val())) { return alert(Msg.SELECCIONE_DIV); }
        if (!Validar.Dato($(Form.Tabla).data("codigo"))) { return alert(Msg.SELECCION_UND); }
        $(Form.FormCambiarUnd).modal("show");
        $(Form.SelectCambiarDiv).val($(Form.Div).val());
        $(Form.SelectCambiarDiv).change();
    });
    $(Form.SelectCambiarDiv).on("change", function () {
        buscarAjax("/UndAtencion/ObtenerLibres", $(this).val(), $("#tabla-cambiar-undatencion"), ["CODIGO", "DESCRIPCION"],
            function (f) {
                f.on("click", function () {
                    if (confirm(MSG_DESEA_CAMBIAR_UNDATENCION)) {
                        var nuevocod = $(this).find("td").get(0).innerHTML
                        var divate = $(Form.SelectCambiarDiv).val();
                        var codigo = $(Form.Tabla).data("codigo");
                        cambiarUndatencion(codigo, $(Form.Und), $(Form.Div), nuevocod, divate, $(Form.UndDesc));
                        $(Form.FormCambiarUnd).modal("hide");
                    }
                });
            });
    });
    $("#factura-pre-factura").on("click", function () {
        if (!Validar.Dato($(Form.Div).val())) { return alert(Msg.SELECCION_DIV); }
        var t = $(Form.Tabla);
        if (!Validar.Dato(t.data("codigo"))) { return alert(Msg.SELECCION_UND); }
        if (t.find("tbody tr").length <= 0) { return alert(Msg.SIN_ELEMENTOS); }
        printPreFactura(t, Form)
    });

    $("#anular-factura").on("click", function () {
        if (!Validar.Dato($(Form.Div).val())) { return alert(Msg.SELECCION_DIV); }
        if (!Validar.Dato($(Form.Tabla).data("codigo"))) { return alert(Msg.SELECCION_UND); }
        if (confirm(MSG_DESEA_ANULAR)) {
            var cod = $(Form.Tabla).data("codigo");
            $.ajax({
                type: "post", dataType: 'json', cache: false, url: '/Conc/Anular', data: { codigo: cod },
                success: function (response, textStatus, jqXHR) {
                    if (App.isError(response.respuesta)) { return alert(response.respuesta); }
                    else { resetTabla(Form.Tabla); $(Form.Und).val(""); clearDesc(Form.PerDesc, Form.UndDesc); window.location = "/UndAtencion/Index"; }
                }, error: function (xhr, status) { errorAjax(xhr, status); }
            });
        }
    });

    $("#empleados li").on("click", function () {
        $(Form.FormSelPerAte).modal("hide");
        var sld = $(this);
        $.ajax({
            type: "post", dataType: 'json', cache: false, url: "/UndAtencion/Aperturar",
            data: { codigo: $(Form.Und).val(), idperatencion: $(this).data("codigo"), divate: $(Form.Div).val() },
            success: function (response, textStatus, jqXHR) {
                if (App.isError(response.respuesta)) { alert(response.respuesta); }
                else {
                    $(Form.Tabla).data("codigo", response.cabecera.CODIGO);
                    $(Form.UndDesc).text(response.undatencion.DESCRIPCION);
                    $(Form.UndDesc).data("codigo", response.undatencion.CODIGO);
                    $(Form.PerDesc).data("codigo", $(this).data("codigo"));
                    $(Form.PerDesc).text(sld.data("desc"));
                }
            }, error: function (xhr, status) { errorAjax(xhr, status); }
        });
    });

    $("#factura-btn-guardar").on("click", function () {
        if (!Validar.Dato($(Form.Div).val())) { return alert(Msg.SELECCION_DIV); }
        if (!Validar.Dato($(Form.Tabla).data("codigo"))) { return alert(Msg.SELECCION_UND); }
        guardarFactura($(Form.Tabla));
    });

    $("#tipos-comprobantes .btn").on("click", function () {
        if (!$(this).hasClass("active")) {
            $("#tipos-comprobantes .btn").removeClass("active");
            $(this).addClass("active"); $(this).children("input[type=radio]").prop('checked', true);
            Anexo.clear($(Form.Anexo));
        }   
    });
    $(FormPago.GroupFV+" .btn").on("click", function () {
        $(this).siblings(".btn").removeClass(FormPago.BtnFV);
        $(this).addClass(FormPago.BtnFV);
        var input = $(this).children("input[type=radio]");input.prop('checked', true);
        $(FormPago.LblFV).text(input.data("desc"));
        FormPago.updateForm($(FormPago.InputGroupFP+":checked"));
    });
    $(FormPago.GroupFP + " .btn").on("click", function () {
        $(this).siblings(".btn").removeClass(FormPago.BtnFP);
        $(this).addClass(FormPago.BtnFP);
        var input = $(this).children("input[type=radio]"); input.prop('checked', true);
        FormPago.updateForm(input);
    });
    $(Anexo.GroupTipDoc+" .btn").on("click", function () {
        $(this).siblings(".btn").removeClass(Anexo.BtnTipDoc);
        $(this).addClass(Anexo.BtnTipDoc);
        var input = $(this).children("input[type=radio]"); input.prop('checked', true);
        Anexo.updateForm(input);
    });
    $("#form-pagar-btn-aceptar").on("click", function () {
        var faltante = parseFloat($("#forma-pago-total").text()) < parseFloat($("#form-pagar-total").text());
        if (faltante) { FormPago.add(); }
        var faltante = parseFloat($("#forma-pago-total").text()) < parseFloat($("#form-pagar-total").text());
        if (!faltante) { FormPago.aceptar();}
    });
    $("#puntos-emision").on("change", function () {
        var v = $(this).val();
        $.ajax({
            type: "post", dataType: 'json', cache: false,
            url: '/Punemi/Cambiar', data: { codigo: v },
            success: function (response, textStatus, jqXHR) {
                if (App.isError(response.respuesta)) { alert(response.respuesta); }
            }, error: function (xhr, status) { errorAjax(xhr, status) }
        });
    });

    $(Anexo.BtnSubmit).on("click", function () {
        var anexo = {};
        anexo.nrodoc = Validar.Dato(m.find("#input-nrodoc"))?m.find("#input-nrodoc").val():alert(Msg.NRODOC_INVALIDO);
        anexo.refane =  Validar.Dato(m.find("#input-refane"))?m.find("#input-refane").val():"";
        anexo.apepat = Validar.Dato(m.find("#input-ape"))?m.find("#input-ape").split(" ")[0]:"";
        anexo.apemat = m.find("#input-ape").val().split(" ")[1];
        anexo.nom1 = m.find("#input-nom1").val();
        anexo.nom2 = m.find("#input-nom2").val();
        anexo.desane = m.find("#input-desane").val();
        switch ($(this).text()) {
            case "Guardar":
                Anexo.createDB(anexo,$(Form.Anexo));
                break;
            case "Actualizar":
                Anexo.updateDB(anexo,$(Form.Anexo))
                break;
        }
        $(Form.FormAnexo).modal("hide");
    });
    $("#conventa-btn-buscar").on("click", function () {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"), Form.Tabla);
    });
    $("#conventa-texto").on("keyup", function (evt) {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar", Form.Tabla));
    });
    $('#conventa-texto').bind('accepted', function (e, keyboard, el) {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"), Form.Tabla);
    });
    $(".seleccion-categoria").on("click", function () {
        var codigo = $(this).data("codigo");
        buscarConventaClaserv(codigo, $("#panel-productos-" + codigo), Form.Tabla);
    });
});
