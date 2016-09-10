var MSG_SELECCIONE_UNDATENCION = "RECOMENDACION: Seleccione una unidad de atenci\u00F3n";var MSG_SELECCIONE_DIVATENCION = "RECOMENDACION: Seleccione una divisi\u00F3n de atenci\u00F3n";var MSG_DESEA_APERTURAR = "La unidad de atenci\u00F3n No est\u00E1 aperturada\n\u00BFDesea aperturarla?";var MSG_ERROR_SOLO_NUMEROS = "ERROR: <attr> solo debe contener caracteres num\u00E9ricos";var MSG_DESEA_ANULAR = "\u00BFDesea anular el consumo?";var MSG_NO_PUEDE_SER_MENOR_QUE = "ERROR: La cantidad ingresada no puede ser menor que {text}";var MSG_CAMPOS_NULOS_VACIOS = "ERROR: Los campos no pueden ser nulos o vac\u00EDos";var MSG_NO_NULO_VACIO = "ERROR: <attr> no puede ser nulo o vac\u00EDo";var MSG_SIN_ELEMENTOS = "ERROR: No existen elementos";var MSG_NO_EXISTE = "ERROR: No existe <attr>";var MSG_DESEA_CAMBIAR_UNDATENCION = "\u00BFSeguro que desea cambiar a otra unidad de atenci\u00F3n";var MSG_ANEXO_NROCAR_NRODOC = "ERROR: El n\u00FAmero de documento debe tener:\nRUC: 11 caracteres\nDNI: 8 caracteres";
var IMG_DEFAULT = "iVBORw0KGgoAAAANSUhEUgAAACUAAAAdCAYAAAAtt6XDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAA0SURBVFhH7c4xAQAwDASh+jf9tcCY4VDA20GlVClVSpVSpVQpVUqVUqVUKVVKlVKlVCmzffdHtC3tn87PAAAAAElFTkSuQmCC";
var ANCHO_IMPRESION = "264px"; var TAMANO_FUENTE = "13px"; var CARSUBSTR = 3;
var Validar = {
    RUC: function (ruc) {
        var r = /\d{11}/;
        if (r.test(ruc)) {
            var factores = new String("5432765432");
            var ultimoIndex = ruc.length - 1; var sumaTotal = 0, residuo = 0; var ultimoDigitoRUC = 0, ultimoDigitoCalc = 0;
            for (var i = 0; i < ultimoIndex; i++) { sumaTotal += (parseInt(ruc.charAt(i)) * parseInt(factores.charAt(i))); }
            residuo = sumaTotal % 11;
            var resta = (11 - residuo), digChk;
            if ((resta == 10)) { digChk = 0;}
            else if ((resta == 11)) { digChk = 1; }
            else { digChk = resta;}
            if ((ruc.charAt(ultimoIndex) == digChk)) { return true;}
            else { alert("ERROR: El RUC " + ruc + " NO es v\u00E1lido."); return false;}
        }
        alert("ERROR: El RUC NO es v\u00E1lido, debe constar de 11 caracteres num\u00E9ricos.");
        return false;
    },
    DNI: function (dni) {
        var r = /(^([0-9]{8,8})|^)$/;
        if (!r.test(dni))
            alert("ERROR: El DNI NO es v\u00E1lido, debe constar de 8 caracteres num\u00E9ricos.");
        return r.test(dni);
    },
    Anexo: function (a) {
        if (a.nrodoc.length == 11) return Validar.RUC(a.nrodoc) && Validar.StrValido(a.desane) && Validar.StrValido(a.refane);
        else if (a.nrodoc.length == 8) return Validar.DNI(a.nrodoc) && Validar.StrValido(a.desane) && Validar.StrValido(a.refane);
        alert("El formato del RUC/DNI es incorrecto");
        return false;
    },
    StrValido: function (s) { return $.trim(s).length > 0; },
    StrLength: function (s, l) { return s.length == l; },
    StrSoloNum: function (s) { var re = /^[0-9]+$/; return re.test(s); },
    Dato: function (d) {return d != undefined && Validar.StrValido(d);},

};
function decBase64(a) { var s = ""; l = a.length; for (var i = 0; i < l; i++) { s += String.fromCharCode(a[i]); } return s; }
function sumar(t, c) {var s = 0; t.find("tbody tr").each(function (index) { s += parseFloat($(this).find("td").get(c).innerHTML); }); return s;}
function clearDesc(p,u) {$(p).data("codigo", ""); $(p).text(""); $(u).data("codigo", ""); $(u).text("");}
function trim(e) { return $.trim(e);}
function errorAjax(xhr, status) { alert("ERROR: Error mientras se ejecutaba la petici\u00F3n\nVerifique su conexi\u00F3n a internet\n C\u00F3digo de estado: " + status);}
function addRow(t, e) {var nc = t.find("th").length;var nf = $("<tr data-codigo=\"\"></tr>");var td;for (var i = 0; i < nc; i++) {td = $("<td></td>");td.append(e[i]);nf.append(td);}t.append(nf);return nf;}
function addRowEvent(t, e, f) {var trs = t.find("th").length;var fila = $("<tr></tr>");for (var i = 0; i < trs; i++) {fila.append($("<td>" + e[i] + "</td>"));}t.append(fila);if (f == undefined) {return;}f(fila);}
function removeRow(fila) { fila.remove();}
function actualizarSubtotal(f, inc, clp, cls) {var p = f.find("td").get(clp);var sb = f.find("td").get(cls);sb.innerHTML = (parseFloat(p.innerHTML) * parseFloat(inc.val())).toFixed(2);}
function actualizarTotal(t, cs) {var s = 0;$(t).find("tbody tr").each(function (index) {s += parseFloat($(this).find("td").get(cs).innerHTML);});$(t + "-total").text(s.toFixed(2));}
function actualizarItems(t, ci) {$(t).find("tbody tr").each(function (index) {$(this).find("td").get(ci).innerHTML = index + 1;});$(t+"-items").text($(t).find("tbody tr").length);}
function buscarAjax(dir, clv, t, attr, f) {
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: dir,data: { clave: clv },
        success: function (response, textStatus, jqXHR) {
            t.find("tbody tr").remove(); if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta); }
            $.each(jQuery.parseJSON(response.lista), function (idx, obj) {var e = []; var l = attr.length;for (var i = 0; i < l; i++) { e.push(obj[attr[i]]); }addRowEvent(t, e, f);});
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function print(div) {
    var mw = window.open('', 'Imprimir Factura', 'height=500,width=' + ANCHO_IMPRESION);
    if (!mw.print) { return false; }
    mw.document.write('<html><head><meta charset="utf-8" /><meta name="viewport" content="width=device-width" /><title>Imprimir Factura</title>');
    mw.document.write('</head><body style=\"font-family: Arial;font-size:' + TAMANO_FUENTE + '; width:' + ANCHO_IMPRESION + ';\">');
    mw.document.write($(div).html());
    mw.document.write("<script>window.onload = function() { print(); close(); };</script>");
    mw.document.write('</body></html>');
    mw.document.close(); // necessary for IE >= 10
    mw.focus(); // necessary for IE >= 10
    return true;
}
function printPreFactura(t) {
    var itd = t.find("tbody tr").length;
    var tp = $("<table width=" + ANCHO_IMPRESION + " style=\"font-size:" + TAMANO_FUENTE + ";\"><thead><tr> <th>Cant.</th> <th style=\"text-align:left;\">Producto</th></tr></thead><tbody></tbody></table>");
    t.find("tbody tr").each(function () {
        var nf = $("<tr></tr>");
        nf.append($("<td style='text-align:center;'></td>").append($(this).find("td input[type=number]").val()));
        nf.append($("<td></td>").append($(this).find("td").get(COLPRODUCTO).innerHTML));
        tp.find("tbody").append(nf);
    });
    var separador = "<hr />";
    var dp = $("<div style='width:" + ANCHO_IMPRESION + ";'><h4 style='text-align:center;'>EL CHALAN S.A.C.</h4><div>");
    dp.append(tp);
    var strf = "<table width=" + ANCHO_IMPRESION + " style=\"font-size:" + TAMANO_FUENTE + ";\" ><tr><td>{ITEMS}</td><td></td></tr></table>";
    strf = strf.replace("{ITEMS}",  "Items: ".concat(itd));
    taux = $(strf)
    dp.append($(separador));
    dp.append(taux);
    if (!print(dp)) { alert("ERROR: Error al imprimir"); }
}
function cambiarUndatencion(cd,su, sd, nu, nd, udes){
    if (!Validar.Dato(cd) || !Validar.Dato(su.val()) || !Validar.Dato(sd.val()) || !Validar.Dato(nu) || !Validar.Dato(nd)){ return alert(MSG_CAMPOS_NULOS_VACIOS); }
  $.ajax({
        type: "post", dataType: 'json', cache: false, url: '/UndAtencion/Cambiar', data: { codigo: cd, nuevound: nu, nuevodiv: nd },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta);}
            if (response.respuesta.toString().split(":")[0] == "EXITO") {
                sd.val(response.undatencion.DIVATENCION);su.val(response.undatencion.CODIGO);udes.data("codigo", response.undatencion.CODIGO);udes.text(response.undatencion.DESCRIPCION);
            }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function buscarConventaDescripcion(d, p) {
    p.empty();
    if (!Validar.Dato(d)) { return; }
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: '/Conventa/Buscar', data: { descripcion: d },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "EXITO") {
                $.each(response.lista, function (idx, obj) {
                    var img;
                    if (obj["foto64"] != null) { img = $("<img class=\"img-responsive img-thumbnail img-servicio \" src=\"data:image/gif;base64," + decBase64(obj["foto64"]) + "\" />"); }
                    else { img = $("<img class=\"img-responsive img-thumbnail img-servicio \" src=\" data:image/png;base64,"+IMG_DEFAULT+"\"/>"); }
                    var ps = $("<div class=\"col-xs-4 col-sm-3 col-md-2 panel-servicio\"></div>");
                    ps.data("codigo", obj["codigo"]); ps.data("precio", obj["precio"]); ps.data("tipovv", obj["tipovalorventa"]);
                    var n = $("<div class=\"nombre\">" + obj["descripcion"] + "</div>");
                    ps.append(img); ps.append(n); p.append(ps);
                    img.on("click", function () {
                        if (!Validar.Dato($("#tabla-factura").data("codigo"))) { alert(MSG_SELECCIONE_UNDATENCION); } else { agregarProducto(ps); }
                    });
                });
            } else { alert(response.respuesta); }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function buscarConventaClaserv(c, p) {
    if (!Validar.Dato(c)) { return alert(MSG_NO_NULO_VACIO.replace("<attr>", "El c\u00F3digo de claserv")); }
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: '/Conventa/Obtener', data: { claserv: c },
        success: function (response, textStatus, jqXHR) {
            p.empty();
            if (response.respuesta.toString().split(":")[0] == "EXITO") {
                $.each(response.lista, function (idx, obj) {
                    var img;
                    if (obj["foto64"] != null) { img = $("<img class=\"img-responsive img-thumbnail img-servicio \" src=\"data:image/gif;base64," + decBase64(obj["foto64"]) + "\" />"); }
                    else { img = $("<img class=\"img-responsive img-thumbnail img-servicio \" src=\" data:image/png;base64,"+IMG_DEFAULT+"\"/>"); }
                    var ps = $("<div class=\"col-xs-4 col-sm-3 col-md-2 panel-servicio\"></div>");
                    ps.data("codigo", obj["codigo"]); ps.data("precio", obj["precio"]); ps.data("tipovv", obj["tipovalorventa"]);
                    var n = $("<div class=\"nombre\">" + obj["descripcion"] + "</div>");
                    ps.append(img); ps.append(n); p.append(ps);
                    img.on("click", function () {
                        if (!Validar.Dato($("#tabla-factura").data("codigo"))) { alert(MSG_SELECCIONE_UNDATENCION); } else { agregarProducto(ps); }
                    });
                });
            } else { alert(response.respuesta); }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function aperturarUndAtencion(d, u, p, c, f) {
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/UndAtencion/Aperturar",
        data: { codigo: u, idperatencion: p, divate: d, pwd:c },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
            else { f(response); }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function tecladoNumerico(s, n) {
    if (!Validar.Dato(n)) { n = 11; }

    s.keyboard({
        layout: 'custom',
        customLayout: {
            'normal': [
                '1 2 3 ',
                '4 5 6',
                '7 8 9',
                '0 {bksp}',
                '{a} {c}'
            ]
        },
        maxLength: n,
        restrictInput: true,
        useCombos: false,
    });
}
$(document).ready(function () {
    $("input[name=control-teclado-opciones]:checked").parent().siblings('.btn').removeClass("active");
    $("input[name=control-teclado-opciones]:checked").parent().addClass("active");  
    $("#control-teclado .btn").on("click", function () {
        $(this).siblings('.btn').removeClass("active");$(this).addClass("active");$(this).children("input[type=radio]").prop('checked', true);
        if ($("input[name=control-teclado-opciones]:checked").val() == "ACT") {
            tecladoNumerico($("input[type=number]")); $("input[type=text]").keyboard();
        } else {
            $("input[type=number]").each(function () {var t = $(this).keyboard().getkeyboard();t.destroy();});
            $("input[type=text]").each(function () {var t = $(this).keyboard().getkeyboard();t.destroy();});
        }
    });
    $("#btn-update").on("click", function () {
        changeUndAtencion($('#undatencion'));
    });
    $("#puntos-emision").on("change", function () {
        var v = $(this).val();
        $.ajax({
            type: "post", dataType: 'json', cache: false,
            url: '/Punemi/Cambiar', data: { codigo: v },
            success: function (response, textStatus, jqXHR) {if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
            }, error: function (xhr, status) {errorAjax(xhr,status)}
        });
    });
});
