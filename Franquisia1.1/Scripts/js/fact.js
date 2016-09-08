var COLITEMS = 0; var COLPRODUCTO = 1; var COLPRECIO = 2; var COLTIPOVV = 3; var COLCANTIDAD = 4; var COLSUBTOTAL = 5; var COLBTNELIMINAR = 6; var COLFORMAPAGO = 0; var COLTIPOTAR = 1; var COLNROREF = 2; var COLIMPORTE = 3; var IGV = 18;
function actualizarTotalCheckbox(t, cs, r) {
    var s = 0;
    $(t).find("tbody tr").each(function (index) {if ($(this).find("input[type=checkbox]").prop("checked")) { s += parseFloat($(this).find("td").get(cs).innerHTML);}});
    r.text(s.toFixed(2));
}
function actualizarItemsCheckbox(t, r) {
    var i = 0;
    $(t).find("tbody tr").each(function (index) {if ($(this).find("input[type=checkbox]").prop("checked")) { i += 1;}});
    r.text(i);
}
function actualizarIGV(t) {
    var tl = parseFloat($(t + "-total").text()); var s = tl * IGV / (100 + IGV); $(t + "-igv").text(s.toFixed(2));
}
function obtenerIGV() {
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Parreg/ObtenerIGV", data: {},
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
            else if (response.respuesta.toString().split(":")[0] == "EXITO") {
                parseFloat(response.igv)!=undefined?IGV = parseFloat(response.igv):IGV=18;
            }
        },
        error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function mostrarAnexo(a, d) {
    d.find(".anexo-desane").val(a.desane); d.find(".anexo-nrodoc").val(a.nrodoc); d.find(".anexo-refane").val(a.refane);
}
function clearAnexo(d) {
    mostrarAnexo({ "desane": "", "nrodoc": "", "refane": "" }, d);
}
function obtenerAnexo(d) {
    return { desane: d.find(".anexo-desane").val(), nrodoc: d.find(".anexo-nrodoc").val(), refane: d.find(".anexo-refane").val() };
}
function actualizarAnexo(a, d) {
    if (!Validar.Anexo(a) ) {return;}
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Anexos/Actualizar",
        data: { desane: a.desane, nrodoc: a.nrodoc, refane: a.refane },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
            else if (response.respuesta.toString().split(":")[0] == "ADVERTENCIA") {
                mostrarAnexo({ "desane": "", "nrodoc": "", "refane": "" }, d);
                console.log(response.respuesta);
            }
            else if (response.respuesta.toString().split(":")[0] == "EXITO") { mostrarAnexo(response.anexo, d);}
        },
        error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function actualizarAnexoDNI(a, d) {
    if (!Validar.Anexo(a)) { return; }
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Anexos/ActualizarDNI",
        data: { nrodoc: a.nrodoc, refane: a.refane, nom1: a.nombre1, nom2: a.nombre2, apepat: a.apepat, apemat: a.apemat },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
            else if (response.respuesta.toString().split(":")[0] == "ADVERTENCIA") {
                mostrarAnexo({ "desane": "", "nrodoc": a.nroane, "refane": "" }, d);
            }
            else if (response.respuesta.toString().split(":")[0] == "EXITO") { mostrarAnexo(response.anexo, d); }
        },
        error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function actualizarAnexoRUC(a, d) {
    if (!Validar.Anexo(a)) { return; }
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Anexos/ActualizarRUC",
        data: { nrodoc: a.nrodoc, refane: a.refane, desane:a.desane},
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
            else if (response.respuesta.toString().split(":")[0] == "ADVERTENCIA") {
                mostrarAnexo({ "desane": "", "nrodoc": a.nroane, "refane": "" }, d);
            }
            else if (response.respuesta.toString().split(":")[0] == "EXITO") { mostrarAnexo(response.anexo, d); }
        },
        error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function getBDAnexo(div,m) {
    var a = obtenerAnexo(div);
    var input = "input[name=tipo-documentos]:checked";
    if ($(input).val() == "01" && !Validar.RUC(a.nrodoc)) { clearAnexo(div); return; }
    else if ($(input).val() == "03" && !Validar.DNI(a.nrodoc)) { clearAnexo(div); return; }
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Anexos/Obtener", data: { nrodoc: a.nrodoc },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
            else if (response.respuesta.toString().split(":")[0] == "ADVERTENCIA") {
                mostrarAnexo({"nrodoc":a.nrodoc, "refane":"", "desane":""}, div);
                var m = "#modal-actualizar-anexo-dni";
                if ($(input).val() == "01") {
                    m = "#modal-actualizar-anexo-ruc";
                    $(m).find("input[type='text']").val("");

                    $(m).find("#actualizar-ruc-titulo").text("Agregar cliente");
                    $(m).find("#form-actruc-nrodoc").val(a.nrodoc);
                    $(m).modal('show');
                    $(m).find("#form-actruc-refane").focus();
                } else {
                    $(m).find("input[type='text']").val("");

                    $(m).find("#actualizar-dni-titulo").text("Agregar cliente");
                    $(m).find("#form-actdni-nrodoc").val(a.nrodoc);
                    $(m).modal('show');
                    $(m).find("#form-actdni-refane").focus();
                }
            }
            else if (response.respuesta.toString().split(":")[0] == "EXITO") { mostrarAnexo(response.anexo, div); }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function crearAnexo(a, d, m) {
    if (a.tipdoc == "RUC" && a.nrodoc.length != 11) { return alert(MSG_ANEXO_NROCAR_NRODOC); }
    if (a.tipdoc == "DNI" && a.nrodoc.length != 8) { return alert(MSG_ANEXO_NROCAR_NRODOC); }
    if (!Validar.Anexo(a)) { return; }
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: '/Anexos/Crear', data: { desane: a.desane, tipdoc: a.tipdoc, nrodoc: a.nrodoc, refane: a.refane },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta); }
            if (response.respuesta.toString().split(":")[0] == "EXITO") { mostrarAnexo(response.anexo, d); m.modal('hide'); }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function resetTabla(t) {
    mostrarAnexo({ "desane": "", "nrodoc": "", "refane": "" }, $(t + "-anexo")); $(t).find("tbody tr").remove();$(t).data("codigo", "");$(t + "-items").text("-");$(t + "-igv").text("-");$(t + "-total").text("-");
}
function printFactura(t, a, r) {
    var itd = "Nro. Items: ".concat(t.find("tbody tr").length);
    var ttld = "Total: ".concat(sumar(t, COLSUBTOTAL).toFixed(2));
    var tp = $("<table width=" + ANCHO_IMPRESION + " style=\"font-size:" + TAMANO_FUENTE + ";\"><thead><tr> <th>Cant.</th> <th style=\"text-align:left;\">Producto</th><th style='text-align:right;'>Total</th></tr></thead><tbody></tbody></table>");
    
    t.find("tbody tr").each(function () {
        var nf = $("<tr></tr>");
        nf.append($("<td style='text-align:center;'></td>").append($(this).find("td input[type=number]").val()));
        nf.append($("<td></td>").append($(this).find("td").get(COLPRODUCTO).innerHTML));
        nf.append($("<td style='text-align:right;'></td>").append($(this).find("td").get(COLSUBTOTAL).innerHTML));
        tp.find("tbody").append(nf);
    });
    var separador = "<hr />";
    var dp = $("<div style='width:" + ANCHO_IMPRESION + ";'><h4 style='text-align:center;'>EL CHALAN S.A.C.</h4><h5>"+r.direccion+"</h5><div>");
    dp.append("Raz\u00F3n Social/Ape. y Nombres: " + a.desane + "<br/>RUC/DNI: " + a.nrodoc + "<br/>Direcci\u00F3n: " + a.refane + "<br/>"); dp.append(tp);
    var strf = "<table width=" + ANCHO_IMPRESION + " style=\"font-size:" + TAMANO_FUENTE + ";\" ><tr><td>Op. Gravadas</td><td style='text-align:right;'>{gravado}</td></tr><tr><td>Op. Exoneradas</td><td style='text-align:right;'>{exonerado}</td></tr><tr><td>Op. Inafectas</td><td style='text-align:right;'>{inafecto}</td></tr><tr><td>I.G.V. S./</td><td style='text-align:right;'>{IGV}</td></tr><tr><td>Importe Total a Pagar S./</td><td style='text-align:right;'>{TOTAL}</td></tr></table>";
    strf = strf.replace("{gravado}", parseFloat(r.gravado).toFixed(2));
    strf = strf.replace("{exonerado}", parseFloat(r.exonerado).toFixed(2));
    strf = strf.replace("{inafecto}", parseFloat(r.inafecto).toFixed(2));
    strf = strf.replace("{IGV}", parseFloat(r.igv).toFixed(2));
    strf = strf.replace("{TOTAL}", parseFloat(r.total).toFixed(2));
    taux = $(strf)
    $.each(r.resumen, function (idx, obj) {
        var s = "<tr><td>SON: </tr></td>";
        s += "<tr><td>" + obj["descripcion"] + " " + obj["RECIBIDO"] + " (Soles)</td></tr>";
        s += "<tr><td>Vuelto: S/." + parseFloat(obj["VUELTO"]).toFixed(2) + "</td></tr>";
        s += "<tr><td>Cajero: " + r.cajero + "</td></tr>";
        s += "<tr><td>Vendedor: " + r.vendedor + "</td></tr>";
        taux.append($(s))
    });
    dp.append($(separador));
    dp.append(taux);
    if (!print(dp)) { alert("ERROR: Error al imprimir"); }
}
function guardarFactura(t) {
    if (!Validar.Dato(t.data("codigo"))) { return alert(MSG_NO_EXISTE.replace("el c\u00F3digo del consumo")); }
    var cod = t.data("codigo"); var ar = new Array(); var cond;
    t.find("tbody tr").each(function (index) {
        cond = {
            "CANTIDAD": parseFloat($(this).find("td input[type=number]").val()), "CONVENTA": $(this).data("codigo"), "ITEM": $(this).find("td").get(COLITEMS).innerHTML,
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
    if (!Validar.StrSoloNum(sl.val())) { return alert(MSG_ERROR_SOLO_NUMEROS.replace("<attr>", "La unidad de atenci\u00F3n")); }
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Obtener", data: { codund: sl.val(), coddiv: cd },
        success: function (response, textStatus, jqXHR) {
            $(t).find("tbody tr").remove();
            if (response.respuesta.toString().split(":")[0] == "ERROR") {
                $(ua).data("codigo", "");$(ua).text("");$(pa).data("codigo", "");$(pa).text("");resetTabla(t);
                if (response.respuesta.toString().split(":")[1] == " NO EXISTE LA CABECERA DEL CONSUMO") { f(sl); }
                else { alert(response.respuesta); }
            }
            if (response.respuesta.toString().split(":")[0] == "EXITO") {
                $(ua).data("codigo", response.undatencion.CODIGO);
                $(ua).text(response.undatencion.DESCRIPCION);
                $(t).data("codigo", response.cabecera.CODIGO);
                $(pa).data("codigo", response.peratencion.codigo);
                $(pa).text(response.peratencion.descripcion);
                $.each(response.lista, function (idx, obj) {
                    var txtc = $("<input type=\"number\" min=\"1\" max=\"1000\" value=\"" + obj["CANTIDAD"] + "\" class=\"numero\"/>");
                    if ($("input[name=control-teclado-opciones]:checked").val() == "ACT") { tecladoNumerico(txtc); }
                    var btne = $("<button class='close remove-producto red'>&times;</button>");
                    var el = [$(t).find("tr").length, obj["DESCRIPCION"], parseFloat(obj["PREUNI"]).toFixed(2), obj["tipovalorventa"].substring(0, CARSUBSTR), txtc, parseFloat(obj["TOTAL"]).toFixed(2), btne];

                    var nf = addRow($(t), el);
                    nf.data("codigo", obj["CONVENTA"]);
                    btne.on('click', function () {
                        removeRow(nf); actualizarTotal(t, COLSUBTOTAL);actualizarItems(t, COLITEMS); actualizarIGV(t);
                    });
                    txtc.change(function () {
                        var cant = parseFloat($(this).val());
                        if (cant>0) {
                            actualizarSubtotal(nf, txtc, COLPRECIO, COLSUBTOTAL); actualizarTotal(t, COLSUBTOTAL); actualizarIGV(t);
                        } else { $(this).val(1); alert(MSG_NO_PUEDE_SER_MENOR_QUE.replace("{text}", "una unidad"));}
                    });
                    $(t).append(nf);
                });
                actualizarTotal(t, COLSUBTOTAL); actualizarItems(t, COLITEMS); actualizarIGV(t);
            }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function agregarProducto(pp) {
    var tf = '#tabla-factura';
    var btne = $("<button class='close remove-producto red'>&times;</button>");
    var txtc = $("<input type=\"number\" min=\"1\" max=\"1000\" value=\"1\" class=\"numero\" />");
    if ($("input[name=control-teclado-opciones]:checked").val() == "ACT") { tecladoNumerico(txtc); }
    var n = pp.find(".nombre").first().text();var p = parseFloat(pp.data("precio")).toFixed(2);
    var el = [$(tf).find("tr").length, n, p, pp.data("tipovv").substring(0, CARSUBSTR), txtc, p, btne]; var nf = addRow($(tf), el); nf.data("codigo", pp.data("codigo"));
    actualizarTotal(tf, COLSUBTOTAL);actualizarItems(tf, COLITEMS);actualizarIGV(tf);
    btne.on('click', function () {
        removeRow(nf);actualizarTotal(tf, COLSUBTOTAL); actualizarItems(tf, COLITEMS); actualizarIGV(tf);
    });
    txtc.change(function () {
        var cant = parseFloat($(this).val());
        if (cant > 0) { actualizarSubtotal(nf, txtc, COLPRECIO, COLSUBTOTAL); actualizarTotal(tf, COLSUBTOTAL); actualizarIGV(tf); actualizarIGV(tf); }
        else { $(this).val(1); alert(MSG_NO_PUEDE_SER_MENOR_QUE.replace("{text}", "una unidad")); }
    });
}
function obtenerTarjetas(p) {
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: "/Tarjetas/Obtener", data: {},
        success: function (response, textStatus, jqXHR) {
            p.empty();
            if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta);}
            if (response.respuesta.toString().split(":")[0] == "EXITO") {
                $.each(response.lista, function (idx, obj) {
                    var d = $("<div class=\"btn\" data-descripcion=\"" + obj["descripcion"] + "\" data-codigo=\"" + obj["codigo"] + "\"></div>");
                    var i = $("<input name=\"tipo-tarjeta\" value=\""+obj["codigo"]+"\" class=\"hide\" type=\"radio\">");
                    var img;
                    if (obj["foto64"]!=null) {
                        img = $("<img class=\"img-responsive img-thumbnail \" src=\"data:image/gif;base64," + decBase64(obj["foto64"]) + "\" style=\"height:40px;\" />");
                    } else {d.append(obj["descripcion"]);}
                    d.append(i);d.append(img);p.append(d);
                });
                p.find(".btn").on("click", function () {
                    $(this).siblings(".btn").removeClass("btn-success");
                    $(this).addClass("btn-success");
                    $(this).children("input[type=radio]").prop('checked', true);
                    p.parent().find(".tarjeta").text($(this).data("descripcion"));
                    p.parent().find(".tarjeta").data("codigo", $(this).data("codigo"));
                });
            }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function getFormEfectivo(p) {
    p.empty();
    var html = "<div id=\"denominaciones\"><label>Denominaciones</label><div class=\"efectivo text-center\">"+ "<div class=\"btn-group btn-group-sm btn-group-justified\"><a>S/. 200</a><a>S/. 100</a><a>S/. 50</a><a>S/. 20</a></div>"+ "<div class=\"btn-group btn-group-sm btn-group-justified\"><a>S/. 10</a><a>S/. 5</a><a>S/. 2</a><a>S/. 1</a></div>"
        + "<div class=\"btn-group btn-group-sm btn-group-justified\"><a>S/. 0.50</a><a>S/. 0.20</a><a>S/. 0.10</a><aux class=\"btn btn-danger input-reset\">"+ "Limpiar</aux></div></div></div>";
    html += " <div class=\"row\"><div class=\"col-xs-6 form-group\">"+ "<label>Importe</label><input class=\"form-control input-sm importe\" type=\"text\" disabled />"
        + "</div><div class=\"col-xs-6 form-group\"><label>Cambio</label><input class=\"form-control input-sm cambio\""+ "type=\"text\" disabled /></div></div>";
    p.append($(html));
    p.find("a").addClass("btn btn-secundario btn-secundario-bordes");
    p.find('a').on("click", function () {
        var ipI = p.find(".importe").first();var ipC = p.find(".cambio").first();var e = parseFloat($(this).text().split(' ')[1]);var ef = parseFloat(ipI.val());
        if (isNaN(ef)) { ef = 0; }var nef = e + ef;ipI.val(parseFloat(nef).toFixed(2));
        var nc = nef - (parseFloat($("#form-pagar-total").text()) - parseFloat($("#forma-pago-total").text())).toFixed(2);
        if (nc >= 0) { ipC.val(nc.toFixed(2)); }
    });
    p.find(".input-reset").on("click", function () {
        p.find(".importe").val(""); p.find(".cambio").val("");
    });
}
function getFormTarjeta(p, ist) {
    p.empty();
    var html = "<div class=\"row\"><div class=\"col-xs-6 form-group\"><label>Tarjeta: </label> <label class=\"tarjeta\"></label><br/>"+ "<label>Nro. de referencia</label><input  class=\"form-control input-sm nro-ref\" type=\"text\" />"+ "<label>Importe</label><input class=\"form-control input-sm importe\" type=\"text\" />"+ "</div><div id=\"divtarjetas\" class=\"col-xs-6 form-group\"></div></div>";
    p.append($(html));var ipI = p.find(".importe");var tp = parseFloat($("#form-pagar-total").text()) - parseFloat($("#forma-pago-total").text());
    if (tp > 0) { ipI.val(tp.toFixed(2)); }if (ist == "S") { obtenerTarjetas(p.find("#divtarjetas")); }if ($("input[name=control-teclado-opciones]:checked").val() == "ACT") { p.find("input[type=text]").keyboard(); }
}
function changeUndAtencion(txt) {
    if (Validar.Dato($("#divatencion").val())) {
        if(Validar.Dato(txt.val())){
            obtenerDetalle(txt, $("#divatencion").val(), "#tabla-factura", "#peratencion-desc", "#undatencion-desc",
                function (select) {
                    if (confirm(MSG_DESEA_APERTURAR)) { $("#modal-seleccionar-peratencion").modal('show'); }
                });
        } else { alert(MSG_SELECCIONE_UNDATENCION); }
    } else { $(this).val(""); alert(MSG_SELECCIONE_DIVATENCION); }
}
function initTipDoc(n) {
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: '/Parreg/TipDocDefault', data: { },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.split(":")[0]=="EXITO") {
                var td = $("input[name='" + n + "'][ value='"+response.codigo+"']");
                td.prop('checked', true); td.parent().addClass("active");
            }
        },
        error: function (xhr, status) { errorAjax(xhr, status); }
    });
}

$(document).ready(function () {
    IGV = obtenerIGV();
    initTipDoc("tipo-documentos");
    obtenerDetalle($("#undatencion"), $("#divatencion").val(), "#tabla-factura", "#peratencion-desc", "#undatencion-desc", function (s) {; });
    var grnt = $("input[name=naturaleza-opcion]:radio"); grnt.first().prop('checked', true); grnt.first().parent().addClass("active");
    
    $("#divatencion").on("change", function () {
        $("#undatencion").val(""); resetTabla("#tabla-factura"); clearDesc("#peratencion-desc", "#undatencion-desc");
    });
    $("#undatencion").keypress(function (e) { if (e.which == 13) { changeUndAtencion($(this)); }});
    $('#undatencion').bind('accepted', function (e, keyboard, el) { changeUndAtencion($(this));});
    if ($("input[name=control-teclado-opciones]:checked").val() == "ACT") {
        $("input[type='text']").keyboard();
        tecladoNumerico($("input[type='number']"));
    }
    $(".anexo").find(".anexo-nrodoc").bind('accepted', function (e, keyboard, el) { getBDAnexo($(this).parent().parent()); });
    $(".anexo").find(".anexo-nrodoc").keypress(function (e) { if (e.which == 13) { getBDAnexo($(this).parent().parent()); } });

    $(".anexo").find(".anexo-desane, .anexo-refane").bind('accepted', function (e, keyboard, el) { actualizarAnexo(obtenerAnexo($(this).parent()), $("tabla-factura-anexo")); });
    $(".anexo").find(".anexo-desane, .anexo-refane").keypress(function (e) { if (e.which == 13) { actualizarAnexo(obtenerAnexo($(this).parent()), $("tabla-factura-anexo")); } });

    $(".btn-seleccionar-anexo").on("click", function () {
        $(".modal").modal("hide");
        var modal = $("#modal-seleccionar-anexo");
        modal.data("anexo", $(this).data("anexo"));
        modal.modal('show');
    });
    $(".btn-actualizar-anexo").on("click", function () {
        a = obtenerAnexo($("#tabla-factura-anexo"));
        if (Validar.Anexo(a)) {
            $.ajax({
                type: "post", dataType: 'json', cache: false, url: "/Anexos/Obtener", data: { nrodoc: a.nrodoc },
                success: function (response, textStatus, jqXHR) {
                    if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
                    else if (response.respuesta.toString().split(":")[0] == "EXITO") {
                        var input = "input[name=tipo-documentos]:checked";
                        var m = "#modal-actualizar-anexo-ruc";
                        if ($(input).val() == "01" && Validar.RUC(a.nrodoc)) {
                            $(m).find("input[type='text']").val("");

                            $(m).find("#actualizar-ruc-titulo").text("Actualizar cliente");
                            $("#form-actruc-nrodoc").val(response.anexo.nrodoc);
                            $("#form-actruc-desane").val(response.anexo.desane);
                            $("#form-actruc-refane").val(response.anexo.refane);
                        }
                        else if ($(input).val() == "03" && Validar.DNI(a.nrodoc)) {
                            m = "#modal-actualizar-anexo-dni";
                            $(m).find("input[type='text']").val("");

                            $("#actualizar-dni-titulo").text("Actualizar cliente");
                            $("#form-actdni-nrodoc").val(response.anexo.nrodoc);
                            $("#form-actdni-refane").val(response.anexo.refane);
                            $("#form-actdni-ape").val(response.anexo.apepat + " " + response.anexo.apemat);
                            $("#form-actdni-nom1").val(response.anexo.nombre1);
                            $("#form-actdni-nom2").val(response.anexo.nombre2);
                        }
                        $(m).modal("show");
                    }
                }, error: function (xhr, status) { errorAjax(xhr, status); }
            });
        } else { alert("ERROR: No se puede actualizar porque no esta registrado"); }
    });
    $("#btn-actdni").on("click", function () {
        a = obtenerAnexo($("#tabla-factura-anexo"));
        a.refane = $("#form-actdni-refane").val();
        a.nombre1 = $("#form-actdni-nom1").val();
        a.nombre2 = $("#form-actdni-nom2").val();
        a.apepat = $("#form-actdni-ape").val().split(" ")[0];
        a.apemat = $("#form-actdni-ape").val().split(" ")[1];
        actualizarAnexoDNI(a, $("#tabla-factura-anexo"));
        $("#modal-actualizar-anexo-dni").modal('hide');
    });
    $("#btn-actruc").on("click", function () {
        a.nrodoc = $("#form-actruc-nrodoc").val();
        a.desane = $("#form-actruc-desane").val();
        a.refane = $("#form-actruc-refane").val();
        actualizarAnexoRUC(a, $("#tabla-factura-anexo"));
        $("#modal-actualizar-anexo-ruc").modal('hide');
    });
    $("#modal-crear-anexo").on("show.bs.modal", function () {
        $(this).find('input[type=text]').val();
    });
    $("#form-anexo-btn-crear").on("click", function () {
        var modal = $("#modal-crear-anexo");var fa = "#form-anexo";
        var a = {desane: $(fa+"-desane").val(), tipdoc: $("input[name='form-anexo-tipdoc']:checked").val(),nrodoc: $(fa+"-nrodoc").val(), refane: $(fa+"-refane").val()}
        crearAnexo(a, $(modal.data("anexo")), modal);
    });

    $("#btn-buscar-razon").on("click", function () {
        buscarAjax("/Anexos/BuscarRazon", $("#txtBuscar").val(), $("#tabla-seleccionar-anexo"), ["desane","tipdoc", "nrodoc", "refane"],
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
                mostrarAnexo(a, $(m.data("anexo")));m.modal('hide');
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
                mostrarAnexo(a, $(m.data("anexo")));
                m.modal('hide');
            });
        });
    });

    $("#factura-btn-pagar").on("click", function () {
        var t = $("#tabla-factura");
        if (!Validar.Dato($("#divatencion").val())) { return alert(MSG_SELECCIONE_DIVATENCION); }
        if (!Validar.Dato($("#tabla-factura").data("codigo"))) { return alert(MSG_SELECCIONE_UNDATENCION); }
        if (t.find("tbody tr").length <= 0) { return alert(MSG_SIN_ELEMENTOS); }
        if (!Validar.Anexo( obtenerAnexo($("#tabla-factura-anexo")))) { return;}
        var m = $("#modal-pagar");
        m.data("tabla", "#tabla-factura");m.data("anexo", "#tabla-factura-anexo");
        $("#form-pagar-total").text(parseFloat($("#tabla-factura-total").text()).toFixed(2));
        $("#forma-pago-total").text("0.00");$("#tabla-forma-pago").find("tbody tr").remove();
        m.modal('show');
    });
   
    $("#cambiar-undatencion").on("click", function () {
        if (!Validar.Dato($("#divatencion").val())) { return alert(MSG_SELECCIONE_DIVATENCION); }
        if (!Validar.Dato($("#tabla-factura").data("codigo"))) { return alert(MSG_SELECCIONE_UNDATENCION); }
        $("#modal-cambiar-undatencion").modal("show");
        $("#modal-cambiar-divatencion").val($("#divatencion").val());
        $("#modal-cambiar-divatencion").change();
    });
    $("#modal-cambiar-divatencion").on("change", function () {
        buscarAjax("/UndAtencion/ObtenerLibres", $(this).val(), $("#tabla-cambiar-undatencion"), ["CODIGO", "DESCRIPCION"],
            function (f) {
                f.on("click", function () {
                    if (confirm(MSG_DESEA_CAMBIAR_UNDATENCION)) {
                        var nuevocod = $(this).find("td").get(0).innerHTML
                        var divate = $("#modal-cambiar-divatencion").val();
                        var codigo = $("#tabla-factura").data("codigo");
                        cambiarUndatencion(codigo, $("#undatencion"), $("#divatencion"), nuevocod, divate, $("#undatencion-desc"));
                        $("#modal-cambiar-undatencion").modal("hide");
                    }
                });
            });
    });
    $("#factura-pre-factura").on("click", function () {
        if (!Validar.Dato($("#divatencion").val())) { return alert(MSG_SELECCIONE_DIVATENCION); }
        if (!Validar.Dato($("#tabla-factura").data("codigo"))) { return alert(MSG_SELECCIONE_UNDATENCION); }
        var t = $("#tabla-factura");
        if (t.find("tbody tr").length <= 0) { return alert(MSG_SIN_ELEMENTOS); }
        printPreFactura(t)
    });

    $("#anular-factura").on("click", function () {
        if (!Validar.Dato($("#divatencion").val())) { return alert(MSG_SELECCIONE_DIVATENCION); }
        if (!Validar.Dato($("#tabla-factura").data("codigo"))) { return alert(MSG_SELECCIONE_UNDATENCION); }
        if (confirm(MSG_DESEA_ANULAR)) {
            var cod = $("#tabla-factura").data("codigo");
            $.ajax({
                type: "post", dataType: 'json', cache: false, url: '/Conc/Anular', data: { codigo: cod },
                success: function (response, textStatus, jqXHR) {
                    if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta); }
                    else { resetTabla("#tabla-factura"); $("#undatencion").val(""); clearDesc("#peratencion-desc", "#undatencion-desc"); window.location = "/UndAtencion/Index";}
                }, error: function (xhr, status) { errorAjax(xhr, status); }
            });
        }
    });

    $(".seleccion-categoria").on("click", function () {
        var codigo = $(this).data("codigo");
        buscarConventaClaserv(codigo, $("#panel-productos-" + codigo));
    });

    $("#empleados li").on("click", function () {
        $("#modal-seleccionar-peratencion").modal("hide");
        var sld = $(this);
        $.ajax({
            type: "post", dataType: 'json', cache: false, url: "/UndAtencion/Aperturar",
            data: { codigo: $("#undatencion").val(), idperatencion: $(this).data("codigo"), divate: $("#divatencion").val() },
            success: function (response, textStatus, jqXHR) {
                if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
                else {
                    $("#tabla-factura").data("codigo", response.cabecera.CODIGO);
                    $("#undatencion-desc").text(response.undatencion.DESCRIPCION);
                    $("#undatencion-desc").data("codigo", response.undatencion.CODIGO);
                    $("#peratencion-desc").data("codigo", $(this).data("codigo"));
                    $("#peratencion-desc").text(sld.data("desc"));
                }
            }, error: function (xhr, status) { errorAjax(xhr, status); }
        });
    });

    $("#factura-btn-guardar").on("click", function () {
        if (!Validar.Dato($("#divatencion").val())) { return alert(MSG_SELECCIONE_DIVATENCION); }
        if (!Validar.Dato($("#tabla-factura").data("codigo"))) { return alert(MSG_SELECCIONE_UNDATENCION); }
        guardarFactura($("#tabla-factura"));
    });

    $("#tipos-comprobantes .btn").on("click", function () {
        if (!$(this).hasClass("active")) {
            $("#tipos-comprobantes .btn").removeClass("active");
            $(this).addClass("active"); $(this).children("input[type=radio]").prop('checked', true);
            clearAnexo($("#tabla-factura-anexo"));
        }   
    });
    $("#modal-pagar-naturaleza .btn").on("click", function () {
        $(this).siblings(".btn").removeClass("active");
        $(this).addClass("active");
        $(this).children("input[type=radio]").prop('checked', true);
        $("input[name=naturaleza-opcion]:checked").change();
    });

    $("input[name=naturaleza-opcion]").on("change", function () {
        o = $(this);
        if (o.val() == "C") {
            var p = $("#tab-tipo-pago-" + $("#forma-pago-agregar").data("codforma"));
            p.find(".importe").val("");
            p.find(".nro-ref").val("");
            p.find(".cambio").val("");
            $("#naturaleza-c").removeClass("hide");
            $("#forma-pago-agregar").removeClass("hide");
        } else if (o.val() == "P") {
            $("#forma-pago-agregar").addClass("hide");
            $("#naturaleza-c").addClass("hide");
            $("#tabs-forma-pago").tab();
            $("#form-pagar-total").text(parseFloat($("#tabla-factura-total").text()).toFixed(2));
            $("#forma-pago-total").text("0.00");
            $("#tabla-forma-pago").find("tbody tr").remove();
        }
    });

    $("#forma-pago-agregar").on("click", function () {
        var btn = $(this);
        var codforma = btn.data("codforma");
        if (!Validar.Dato(codforma)) {return alert("Recomendaci\u00F3n: Seleccione una forma de pago");}
        var tabla = "#tabla-forma-pago";
        var seleccion = $(".seleccion-tipo-pago[data-codigo=" + codforma + "]");
        var formapago = seleccion.children("a").first().text();
        var padre = $("#tab-tipo-pago-" + codforma);
        var importe = padre.find(".importe").val();
        var nroref = padre.find(".nro-ref").val();
        var tipotarjeta; var codtipotar="";
        if (!importe.match(/^[0-9]+([.][0-9]+)?$/)) { return alert(MSG_ERROR_SOLO_NUMEROS.replace("<attr>", "El importe")); }
        if (seleccion.data("istarjeta") == "S") {
            tipotarjeta = padre.find(".tarjeta").text(); codtipotar = padre.find(".tarjeta").data("codigo");
            if (!Validar.Dato(tipotarjeta)) {return alert(MSG_NO_NULO_VACIO.replace("<attr>","El tipo de tarjeta"));}
            if (!Validar.Dato(nroref)) { return alert(MSG_NO_NULO_VACIO.replace("<attr>", "El n\u00FAmero de referenc\u00EDa")); }
            if (!nroref.match(/^[0-9]+$/)) { return alert(MSG_ERROR_SOLO_NUMEROS.replace("<attr>","El n\u00FAmero de referenc\u00EDa")); }
        }else { nroref = ""; }
        if (!Validar.Dato(importe)) { return alert(MSG_NO_NULO_VACIO.replace("<attr>", "El importe")); }
        if (!Validar.Dato(formapago)) { return alert(MSG_NO_NULO_VACIO.replace("<attr>", "La forma de pago")); }
        var montoact = sumar($(tabla), COLIMPORTE);
        if (parseFloat($("#forma-pago-total").text()) < parseFloat($("#form-pagar-total").text())) {
            if (montoact+parseFloat(importe) - parseFloat($("#form-pagar-total").text())>0) { importe = parseFloat($("#form-pagar-total").text()) - montoact; }
            elementos = [formapago,tipotarjeta, nroref, parseFloat(importe).toFixed(2)];
            var fila = addRow($(tabla), elementos);
            if (!Validar.Dato(padre.find(".cambio").val())) { fila.data("cambio", "0"); }
            else { fila.data("cambio", padre.find(".cambio").val()); }
            fila.data("forpago", codforma); fila.data("tipotar", codtipotar);
            $("#forma-pago-total").text(sumar($(tabla), COLIMPORTE).toFixed(2));
            padre.find(".importe").val(""); padre.find(".cambio").val(""); padre.find(".nro-ref").val(""); padre.find(".tarjeta").text("");
            padre.find("#divtarjetas .btn-success").removeClass("btn-success")
        } else { alert("Error: No se puede agregar, porque el total a pagar ya coincide con los montos agregado"); }
    });

    $(".seleccion-tipo-pago").on("click", function () {
        seleccion = $(this);
        $("#forma-pago-agregar").data("codforma", seleccion.data("codigo"));
        switch (seleccion.data("codigo")) {
            case "001":
                getFormEfectivo($("#tab-tipo-pago-" + seleccion.data("codigo"))); break;
            case "002":
                getFormTarjeta($("#tab-tipo-pago-" + seleccion.data("codigo")), seleccion.data("istarjeta")); break;
        }
    });

    $("#form-pagar-btn-aceptar").on("click", function () {
        var conc = $("#tabla-factura").data("codigo");
        var anexo = obtenerAnexo($("#tabla-factura-anexo"));
        var fv = $("input[name=naturaleza-opcion]:checked").data("codigo");
        var td = $("input[name=tipo-documentos]:checked").val();
        if (!Validar.Dato(conc)) { return alert(MSG_SELECCIONE_UNDATENCION); }
        if (!Validar.Dato(td)) { return alert(MSG_NO_EXISTE.replace("<attr>", "Tipo de documento")); }
        var fp = new Array();
        $("#tabla-forma-pago").find("tbody tr").each(function (index) {
            var f = $(this);
            var i = {
                "FORPAGO": f.data("forpago"), "TARJETA": f.data("tipotar"),"REFERENCIA": f.find("td").get(COLNROREF).innerHTML, "IMPORTE": f.find("td").get(COLIMPORTE).innerHTML, "VUELTO":f.data("cambio"),
            };
            fp.push(i);
        });
        if ($("input[name=naturaleza-opcion]:checked").val() == "C" ) {
            if (sumar($("#tabla-forma-pago"), COLIMPORTE).toFixed(2) != parseFloat($("#form-pagar-total").text()).toFixed(2)) { return alert("Error: Para poder realizar la transacci\u00F3n los totales debe coincidir"); }
        }
        if ($("input[name=naturaleza-opcion]:checked").val() == "P") {
            item = {"FORVENTA":fv, "IMPORTE":$("#tabla-factura-total").text()};
            fp.push((item));
        }
        $.ajax({
            type: "post", dataType: 'json', cache: false, url: "/Conc/Facturar",
            data: { codconc: conc, nroane: anexo.nrodoc, desane: anexo.desane, refane: anexo.refane, tipdoc:td, formaventa: fv, formapago: JSON.stringify(fp) },
            success: function (response, textStatus, jqXHR) {
                if (response.respuesta.split(":")[0] == "ERROR") {alert(response.respuesta);}
                if (response.respuesta.split(":")[0] == "EXITO") {
                    printFactura($("#tabla-factura"), anexo, response);
                    $("#undatencion").val("");
                    clearDesc("#peratencion-desc", "#undatencion-desc");
                    resetTabla("#tabla-factura");
                    window.location = "/UndAtencion/Index";
                }
            }, error: function (xhr, status) { errorAjax(xhr, status); }
        });
        $("#modal-pagar").modal("hide");
    });

    




    $("#conventa-btn-buscar").on("click", function () {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"));
    });
    $("#conventa-texto").on("keyup", function (evt) {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"));
    });
    $('#conventa-texto').bind('accepted', function (e, keyboard, el) {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"));
    });
});
