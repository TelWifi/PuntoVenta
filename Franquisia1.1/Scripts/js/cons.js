var COLITEMS = 0; var COLPRODUCTO = 1; var COLPRECIO = 2; var COLCANTIDAD = 3; var COLSUBTOTAL = 4; var COLBTNELIMINAR = 5;
function removeRow(fila, t) {
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Eliminar",
        data: { codigo: $("#tabla-factura").data("codigo"), item: fila.data("item") },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta); }
            fila.remove();
            actualizarTotal(t, COLSUBTOTAL); actualizarItems(t, COLITEMS);
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function updateRow(f, t) {
    var txtc = f.find("td input[type=number]");
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Actualizar",
        data: { codigo: $("#tabla-factura").data("codigo"), item: f.data("item"), cantidad: txtc.val(), },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta); }
            actualizarSubtotal(f, txtc, COLPRECIO, COLSUBTOTAL); actualizarTotal(t, COLSUBTOTAL);
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function resetTabla(t) { $(t).find("tbody tr").remove(); $(t).data("codigo", ""); $(t + "-items").text("-"); $(t + "-igv").text("-"); $(t + "-total").text("-"); $(t + "-sel-items").text("-"); $(t + "-sel-total").text("-"); $(t + "-sel-igv").text("-"); }
function guardarFactura(t) {
    if (isNullOrWhiteSpace(t.data("codigo"))) { return alert(MSG_NO_EXISTE.replace("el c\u00F3digo del consumo")); }
    var cod = t.data("codigo"); var arr = new Array();
    t.find("tbody tr").each(function (index) {
        var cond = {"CANTIDAD": parseFloat($(this).find("td input[type=number]").val()), "CONVENTA": $(this).data("codigo"), "ITEM": $(this).find("td").get(COLITEMS).innerHTML,};
        arr.push((cond));
    });
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: '/Cond/Guardar', data:  { codigo: cod, items: JSON.stringify(arr) },
        success: function (response, textStatus, jqXHR) { alert(response.respuesta); },
        error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function agregarProducto(pp) {
    var txtc = $("<input type=\"number\"  min=\"1\" max=\"1000\" value=\"1\" class=\"numero\" />");
    if ($("input[name=control-teclado-opciones]:checked").val() == "ACT") { txtc.keyboard(); }
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Crear",
        data: { codigo: $("#tabla-factura").data("codigo"), cantidad: txtc.val() , conventa: pp.data("codigo")  },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta); }
            var tf = '#tabla-factura';
            var btn_eliminar = $("<button class='close remove-producto red'>&times;</button>");
            var n = pp.find(".nombre").first().text();
            var p = parseFloat(pp.data("precio")).toFixed(2);
            var el = [$(tf).find("tr").length, n, p, txtc, p, btn_eliminar];
            var nf = addRow($(tf), el); nf.data("item", response.item);
            nf.data("codigo", pp.data("codigo")); actualizarTotal(tf, COLSUBTOTAL); actualizarItems(tf, COLITEMS);
            btn_eliminar.on('click', function () { removeRow(nf, tf);});
            txtc.change(function () {
                var cant = parseFloat($(this).val());
                if (cant > 0) { updateRow(nf, "#tabla-factura"); } else { $(this).val(1); updateRow(nf, "#tabla-factura"); alert(MSG_NO_PUEDE_SER_MENOR_QUE.replace("{text}", "una unidad")); }
            });
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function obtenerDetalle(sl, cd, t, pa, ua, f) {
    if (isNullOrWhiteSpace(sl.val()) || isNullOrWhiteSpace(cd) || isNullOrWhiteSpace(t)) { return; }
    if (!sl.val().match(/^[0-9]+$/)) { return alert(MSG_ERROR_SOLO_NUMEROS.replace("<attr>", "La unidad de atenci\u00F3n")); }
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Obtener", data: { codund: sl.val(), coddiv: cd },
        success: function (response, textStatus, jqXHR) {
            $(t).find("tbody tr").remove();
            if (response.respuesta.toString().split(":")[0] == "ERROR") {
                clearDesc(pa, ua);resetTabla(t);if (response.respuesta.toString().split(":")[1] == " NO EXISTE LA CABECERA DEL CONSUMO") {f(sl);} else { alert(response.respuesta); }
            }
            if (response.respuesta.toString().split(":")[0] == "EXITO") {
                $(ua).data("codigo", response.undatencion.CODIGO);
                $(ua).text(response.undatencion.DESCRIPCION);
                $(t).data("codigo", response.cabecera.CODIGO);
                $(pa).data("codigo", response.peratencion.codigo);
                $(pa).text(response.peratencion.descripcion);
                $.each(response.lista, function (idx, obj) {
                    var txtc = $("<input type=\"number\" min=\"1\" max=\"1000\" value=\"" + obj["CANTIDAD"] + "\" class=\"numero\" />");
                    if ($("input[name=control-teclado-opciones]:checked").val() == "ACT") { txtc.keyboard(); }
                    var btne = $("<button class='close remove-producto red'>&times;</button>");
                    var el = [$(t).find("tr").length, obj["DESCRIPCION"], parseFloat(obj["PREUNI"]).toFixed(2), txtc, parseFloat(obj["TOTAL"]).toFixed(2), btne];
                    var nf = addRow($(t), el); nf.data("codigo", obj["CONVENTA"]); nf.data("item", obj["ITEM"]);
                    btne.on('click', function () {removeRow(nf, t);});
                    txtc.change(function () {
                        var cant = parseFloat($(this).val());
                        if (cant > 0) { updateRow(nf, t); } else { $(this).val(1); updateRow(nf, t); alert(MSG_NO_PUEDE_SER_MENOR_QUE.replace("{text}", "una unidad")); }
                    });
                    $(t).append(nf);
                });
                actualizarTotal(t, COLSUBTOTAL); actualizarItems(t, COLITEMS);
            }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}

function changeUndAtencion(txt) {
    if (isNullOrWhiteSpace(txt.val())) {
        resetTabla("#tabla-factura"); clearDesc("#peratencion-desc", "#undatencion-desc");
    } else {
        if (!isNullOrWhiteSpace($("#divatencion").val())) {
            if (!isNullOrWhiteSpace($("#undatencion").val())) {
                obtenerDetalle(txt, $("#divatencion").val(), "#tabla-factura", "#peratencion-desc", "#undatencion-desc",
                function (select) {
                    if (confirm(MSG_DESEA_APERTURAR)) { $("#modal-seleccionar-peratencion").modal('show'); }
                });
            } else { alert(MSG_SELECCIONE_UNDATENCION); }
        } else { $(this).val(""); alert(MSG_SELECCIONE_DIVATENCION); $("#divatencion").focus(); }
    }
}
function verificarUndAperturada() {
    var div = $("#divate").text();
    var und = $("#undate").text();
    var per = $("#perate").text();
    if (!isNullOrWhiteSpace(div) && !isNullOrWhiteSpace(und) && !isNullOrWhiteSpace(per)) {
        aperturarUndAtencion(div, und, per);
        $("#undatencion").val(und);
        $("#divatencion").val(div);
    }
    console.log("aqui");
}
function undAperturada() {
    obtenerDetalle($("#undatencion"), $("#divatencion").val(), "#tabla-factura", "#peratencion-desc", "#undatencion-desc");
}
$(document).ready(function () {
    $("#divatencion").on("change", function () {
        $("#undatencion").val("");
        resetTabla("#tabla-factura");
        clearDesc("#peratencion-desc", "#undatencion-desc");
    });
    $("#undatencion").keypress(function (e) { if (e.which == 13) { changeUndAtencion($(this));}});
    $('#undatencion').bind('accepted', function (e, keyboard, el) { changeUndAtencion($(this));});
    //verificarUndAperturada();
    obtenerDetalle($("#undatencion"), $("#divatencion").val(), "#tabla-factura", "#peratencion-desc", "#undatencion-desc", function (s) { console.log("sdsa"); });

    if ($("input[name=control-teclado-opciones]:checked").val() == "ACT") {$("input[type='text']").keyboard();$("input[type='number']").keyboard();}
    $("#cambiar-undatencion").on("click", function () {
        if (isNullOrWhiteSpace($("#divatencion").val())) { return alert(MSG_SELECCIONE_DIVATENCION); }
        if (isNullOrWhiteSpace($("#tabla-factura").data("codigo"))) { return alert(MSG_SELECCIONE_UNDATENCION); }
        $("#modal-cambiar-undatencion").modal("show");
        $("#modal-cambiar-divatencion").val($("#divatencion").val());
        $("#modal-cambiar-divatencion").change();
    });
    $("#modal-cambiar-divatencion").on("change", function () {
        buscarAjax("/UndAtencion/ObtenerLibres", $(this).val(), $("#tabla-cambiar-undatencion"), ["CODIGO", "DESCRIPCION"],
            function (fila) {
                fila.on("click", function () {
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
        if (isNullOrWhiteSpace($("#divatencion").val())) { return alert(MSG_SELECCIONE_DIVATENCION); }
        if (isNullOrWhiteSpace($("#tabla-factura").data("codigo"))) { return alert(MSG_SELECCIONE_UNDATENCION); }
        var t = $("#tabla-factura");
        if (t.find("tbody tr").length <= 0) { return alert(MSG_SIN_ELEMENTOS); }
        printPreFactura(t)
    });

    $(".seleccion-categoria").on("click", function () {
        var codigo = $(this).data("codigo");
        buscarConventaClaserv(codigo, $("#panel-productos-" + codigo));
    });

    $("#empleados li").on("click", function () {
        $("#modal-seleccionar-peratencion").modal("hide");
        var selected = $(this);
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
                    $("#peratencion-desc").text(selected.data("desc"));
                }
            }, error: function (xhr, status) { errorAjax(xhr, status); }
        });
    });

    $("#factura-btn-guardar").on("click", function () {
        if (isNullOrWhiteSpace($("#divatencion").val())) { return alert(MSG_SELECCIONE_DIVATENCION); }
        if (isNullOrWhiteSpace($("#tabla-factura").data("codigo"))) { return alert(MSG_SELECCIONE_UNDATENCION); }
        guardarFactura($("#tabla-factura"));
    });
    
    $("#conventa-btn-buscar").on("click", function () {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"));
    });
    $("#conventa-texto").on("keyup", function (evt) {
        if (!isNullOrWhiteSpace($("#conventa-texto").val())) {
            buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"));
        } else { $("#panel-buscar").empty(); }
    });
    $('#conventa-texto').bind('accepted', function (e, keyboard, el) {
        if (!isNullOrWhiteSpace($("#conventa-texto").val())) {
            buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"));
        } else { $("#panel-buscar").empty(); }
    });
});
