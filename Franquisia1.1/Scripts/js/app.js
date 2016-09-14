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
        if (!r.test(dni)) alert("ERROR: El DNI NO es v\u00E1lido, debe constar de 8 caracteres num\u00E9ricos.");
        return r.test(dni);
    },
    Anexo: function (a) {
        switch (a.tipdoc) {
            case "01":
                return Validar.DNI(a.nrodoc) && Validar.StrValido(a.apepat) && Validar.StrValido(a.apemat) && Validar.StrValido(a.nombre1);
                break;
            case "04":
                return Validar.StrValido(a.nrodoc) && Validar.StrValido(a.apepat) && Validar.StrValido(a.apemat) && Validar.StrValido(a.nombre1);
                break;
            case "07":
                return Validar.StrValido(a.nrodoc) && Validar.StrValido(a.apepat) && Validar.StrValido(a.apemat) && Validar.StrValido(a.nombre1);
                break;
            case "06":
                return Validar.RUC(a.nrodoc) && Validar.StrValido(a.desane); break;
            default:
                return Validar.StrValido(a.nrodoc) && Validar.StrValido(a.desane);
        }
    },
    StrValido: function (s) { return $.trim(s).length > 0; },
    StrLength: function (s, l) { return s.length == l; },
    StrSoloNum: function (s) { var re = /^[0-9]+$/; return re.test(s); },
    Dato: function (d) {return d != undefined && Validar.StrValido(d);},

};
var Msg = {
    UNDATENCION: "unidad de atenci\u00F3n",
    DIVATENCION: "divisi\u00F3n de atenci\u00F3n",
    APERTURAR_UND: "La unidad de atenci\u00F3n No est\u00E1 aperturada\n\u00BFDesea aperturarla?",
    ANULAR_CONSUMO: "\u00BFDesea anular el consumo?",
    CANT_NO_MENOR_A: "ERROR: La cantidad ingresada no puede ser menor que <attr>",
    CAMPOS_NULOS_O_VACIOS: "ERROR: Los campos no pueden ser nulos o vac\u00EDos",
    CAMBIAR_UND: "\u00BFSeguro que desea cambiar a otra unidad de atenci\u00F3n",
    ERROR_CAMPO_NUMERICO: "ERROR: <attr> solo debe contener caracteres num\u00E9ricos",
    ERROR_IMPRIMIR: "ERROR: Error al imprimir",
    NO_NULO_O_VACIO: "ERROR: <attr> no puede ser nulo o vac\u00EDo",
    NO_EXISTE: "ERROR: No existe <attr>",
    NRODOC_INVALIDO: "ERROR: El n\u00FAmero de documento no es valido, debe tener:\nRUC: 11 caracteres\nDNI: 8 caracteres",
    ANEXO_INVALIDO:"ERROR: Los datos del cliente son incorrectos",
    SELECCIONE_ATTR: "RECOMENDACION: Seleccione <attr>",
    SELECCION_UND: "RECOMENDACION: Seleccione <attr>".replace("<attr>", "una unidad de atenci\u00F3n"),
    SELECCION_DIV: "RECOMENDACION: Seleccione <attr>".replace("<attr>", "una unidad de atenci\u00F3n"),
    SIN_ELEMENTOS: "ERROR: No existen elementos",
    EXITO:"EXITO",
    ERROR: "ERROR",
    ADVERTENCIA: "ADVERTENCIA",
    RECOMENDACION:"RECOMENDACION",
};
var App = {
    ControlTeclado: "input[name=control-teclado-opciones]",
    CARSUBSTR: 3,
    getTipNrodoc:function(nrodoc){
        var r = /\d{11}/;
        if (r.test(nrodoc)) {
            return "06";
        }
        r = /(^([0-9]{8,8})|^)$/;
        if (r.test(nrodoc)) {
            return "01";
        }
        return "07";
    },
    getImageText: function (t, cl) {
        var fontsize = "30";
        var c = document.createElement('canvas');
        var ctx = c.getContext("2d");
        ctx.font = "bold " + fontsize + "px sans-serif";
        var arr = t.split(" ");
        for (var i = 0; i < arr.length; i++) {
            ctx.fillText(arr[i], 10, fontsize*(i+1));
        }
        var img = c.toDataURL("image/png");
        return $('<img src="' + img + '" class="img-responsive img-thumbnail '+cl+'"/>');
    },
    getImageArr: function (arr,cl) {
        return $("<img class=\"img-responsive img-thumbnail "+cl+"\" src=\"data:image/gif;base64," + decBase64(arr) + "\" />");
    },
    getPanelServ: function () {
        return $("<div class='col-xs-4 col-sm-3 col-md-2 panel-servicio'></div>")
    },
    getInputCantidad: function () {
        var i = $("<input type='number' min='1' max='1000' value='1' class='numero' />");
        if ($(App.ControlTeclado + ":checked").val() == "ACT") { tecladoNumerico(i); }
        return i;
    },
    getBtnRemove:function(){
        return $("<button class='close remove-producto red'>&times;</button>");
    },
    isExito: function (t) { return t.split(":")[0] == Msg.EXITO;},
    isError: function (t) { return t.split(":")[0] == Msg.ERROR;},
    isAdvert: function (t) { return t.split(":")[0] == Msg.ADVERTENCIA; },
    isRecomen: function (t) { return t.split(":")[0] == Msg.RECOMENDACION; },
    getStr: function (c, n) { var s = ""; for (var i = 0; i < n; i++) { s += c; } return s;},
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
function replaceAll(str, find, replace) { return str.replace(new RegExp(find, 'g'), replace); }
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
    var WIDTHPRINT = "270px", FONT_SIZE = "14px";
    var mw = window.open('', 'Imprimir Factura', 'height=500,width=' + WIDTHPRINT);
    if (!mw.print) { return false; }
    var s = "<html lang='es'><head><meta charset='utf-8'><style>*{padding: 0px;margin:2px;}	body{width:" + WIDTHPRINT + ";font-size:" + FONT_SIZE + ";text-align:center;}	pre{white-space: pre-wrap;}	table{text-align: right;width: 100%;}	table>tbody>tr>td:nth-child(1) {text-align: left;}	.text-left{		text-align: left;	}</style></head><body>";
    mw.document.write(s);
    mw.document.write($(div).html());
    mw.document.write("<script type='text/javascript'>window.onload = function() { print(); close(); } </script>");
    mw.document.write("</body></html>");
    mw.document.close(); // necessary for IE >= 10
    mw.focus(); // necessary for IE >= 10
    return true;
}
function printPreFactura(t, f) {
    var itd = t.find("tbody tr").length;
    var tp = $("<table  class='text-left'><thead><tr> <th>Cant.</th> <th>Producto</th></tr></thead><tbody></tbody></table>");
    t.find("tbody tr").each(function () {
        var nf = $("<tr></tr>");
        nf.append($("<td style='text-align:center;'></td>").append($(this).find("td input[type=number]").val()));
        nf.append($("<td></td>").append($(this).find("td").get(f.CPRODUCTO).innerHTML));
        tp.find("tbody").append(nf);
    });
    var dp = $("<pre><h4 align='center'>EL CHALAN S.A.C.</h4></pre>");
    dp.append(tp);
    var strf = "<table ><tr><td>{ITEMS}</td><td></td></tr></table>";
    strf = strf.replace("{ITEMS}",  "Items: ".concat(itd));
    taux = $(strf)
    dp.append($("<hr />"));
    dp.append(taux);
    if (!print(dp)) { alert(Msg.ERROR_IMPRIMIR); }
}
function cambiarUndatencion(cd,su, sd, nu, nd, udes){
    if (!Validar.Dato(cd) || !Validar.Dato(su.val()) || !Validar.Dato(sd.val()) || !Validar.Dato(nu) || !Validar.Dato(nd)){ return alert(Msg.CAMPOS_NULOS_O_VACIOS); }
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
function buscarConventaDescripcion(d, p, t) {
    p.empty();
    if (!Validar.Dato(d)) { return; }
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: '/Conventa/Buscar', data: { descripcion: d },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "EXITO") {
                $.each(response.lista, function (idx, obj) {
                    var img;
                    if (obj["foto64"] != null) { img = App.getImageArr(obj["foto64"], "img-servicio"); }
                    else { img = App.getImageText(obj["descripcion"], "img-servicio"); }
                    var ps = App.getPanelServ();
                    ps.data("codigo", obj["codigo"]); ps.data("precio", obj["precio"]); ps.data("tipovv", obj["tipovalorventa"]);
                    var n = $("<div class=\"nombre\">" + obj["descripcion"] + "</div>");
                    ps.append(img); ps.append(n); p.append(ps);
                    img.on("click", function () {
                        if (!Validar.Dato($(t).data("codigo"))) { alert(Msg.SELECCION_UND); } else { agregarProducto(ps); }
                    });
                });
            } else { alert(response.respuesta); }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function buscarConventaClaserv(c, p, t) {
    if (!Validar.Dato(c)) { return alert(Msg.NO_NULO_O_VACIO.replace("<attr>", "El c\u00F3digo de claserv")); }
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: '/Conventa/Obtener', data: { claserv: c },
        success: function (response, textStatus, jqXHR) {
            p.empty();
            if (response.respuesta.toString().split(":")[0] == "EXITO") {
                $.each(response.lista, function (idx, obj) {
                    var img;
                    if (obj["foto64"] != null) { img = App.getImageArr(obj["foto64"], "img-servicio"); }
                    else { img = App.getImageText(obj["descripcion"], "img-servicio"); }
                    var ps = App.getPanelServ();
                    ps.data("codigo", obj["codigo"]); ps.data("precio", obj["precio"]); ps.data("tipovv", obj["tipovalorventa"]);
                    var n = $("<div class=\"nombre\">" + obj["descripcion"] + "</div>");
                    ps.append(img); ps.append(n); p.append(ps);
                    img.on("click", function () {
                        if (!Validar.Dato($(t).data("codigo"))) { alert(Msg.SELECCION_UND); } else { agregarProducto(ps); }
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
    $(App.ControlTeclado+":checked").parent().siblings('.btn').removeClass("active");
    $(App.ControlTeclado+":checked").parent().addClass("active");  
    $("#control-teclado .btn").on("click", function () {
        $(this).siblings('.btn').removeClass("active");$(this).addClass("active");$(this).children("input[type=radio]").prop('checked', true);
        if ($(App.ControlTeclado + ":checked").val() == "ACT") {
            tecladoNumerico($("input[type=number]")); $("input[type=text]").keyboard();
        } else {
            $("input[type=number]").each(function () {var t = $(this).keyboard().getkeyboard();t.destroy();});
            $("input[type=text]").each(function () {var t = $(this).keyboard().getkeyboard();t.destroy();});
        }
    });
    $("#btn-update").on("click", function () {
        changeUndAtencion($('#undatencion'));
    });
});
