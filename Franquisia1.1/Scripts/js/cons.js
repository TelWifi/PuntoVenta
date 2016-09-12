var Form = {
    CITEMS: 0, CPRODUCTO: 1, CPRECIO: 2, CCANT: 3, CSUBT: 4, CBTNELI: 5,
    Tabla: "#tabla-factura",
    Und: "#undatencion",
    Div: "#divatencion",
    UndDesc: Form.Und + "-desc",
    PerDesc: "#peratencion-desc",
    FormCambiarUnd: Form.FormCambiarUnd,
    FormSelectDiv:Form.FormSelectDiv,
    getIGV: function () {
        $.ajax({
            type: "post", dataType: 'json', cache: false, url: "/Parreg/ObtenerIGV", data: {},
            success: function (response, textStatus, jqXHR) {
                if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
                else if (response.respuesta.toString().split(":")[0] == "EXITO") {
                    parseFloat(response.igv) != undefined ? Form.IGV = parseFloat(response.igv) : Form.IGV = 18;
                }
            },
            error: function (xhr, status) { errorAjax(xhr, status); }
        });
    },
};
function removeRow(fila, t) {
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Eliminar",
        data: { codigo: $(Form.Tabla).data("codigo"), item: fila.data("item") },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta); }
            fila.remove();
            actualizarTotal(t, Form.CSUBT); actualizarItems(t, Form.CITEMS);
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function updateRow(f, t) {
    var txtc = f.find("td input[type=number]");
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Actualizar",
        data: { codigo: $(Form.Tabla).data("codigo"), item: f.data("item"), cantidad: txtc.val(), },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta); }
            actualizarSubtotal(f, txtc, Form.CPRECIO, Form.CSUBT); actualizarTotal(t, Form.CSUBT);
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function resetTabla(t) { $(t).find("tbody tr").remove(); $(t).data("codigo", ""); $(t + "-items").text("-"); $(t + "-igv").text("-"); $(t + "-total").text("-"); $(t + "-sel-items").text("-"); $(t + "-sel-total").text("-"); $(t + "-sel-igv").text("-"); }
function guardarFactura(t) {
    if (!Validar.Dato(t.data("codigo"))) { return alert(Msg.NO_EXISTE.replace("el c\u00F3digo del consumo")); }
    var cod = t.data("codigo"); var arr = new Array();
    t.find("tbody tr").each(function (index) {
        var cond = {"CANTIDAD": parseFloat($(this).find("td input[type=number]").val()), "CONVENTA": $(this).data("codigo"), "ITEM": $(this).find("td").get(Form.CITEMS).innerHTML,};
        arr.push((cond));
    });
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: '/Cond/Guardar', data:  { codigo: cod, items: JSON.stringify(arr) },
        success: function (response, textStatus, jqXHR) { alert(response.respuesta); },
        error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function agregarProducto(pp) {
    var txtc = App.getInputCantidad();
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Crear",
        data: { codigo: $(Form.Tabla).data("codigo"), cantidad: txtc.val() , conventa: pp.data("codigo")  },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.toString().split(":")[0] == "ERROR") { return alert(response.respuesta); }
            var tf = '#tabla-factura';
            var btn_eliminar = App.getBtnRemove();
            var n = pp.find(".nombre").first().text();
            var p = parseFloat(pp.data("precio")).toFixed(2);
            var el = [$(tf).find("tr").length, n, p, txtc, p, btn_eliminar];
            var nf = addRow($(tf), el); nf.data("item", response.item);
            nf.data("codigo", pp.data("codigo")); actualizarTotal(tf, Form.CSUBT); actualizarItems(tf, Form.CITEMS);
            btn_eliminar.on('click', function () { removeRow(nf, tf);});
            txtc.change(function () {
                var cant = parseFloat($(this).val());
                if (cant > 0) { updateRow(nf, Form.Tabla); } else { $(this).val(1); updateRow(nf, Form.Tabla); alert(Msg.CANT_NO_MENOR_A.replace("{text}", "una unidad")); }
            });
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function obtenerDetalle(sl, cd, t, pa, ua, f) {
    if (!Validar.Dato(sl.val()) || !Validar.Dato(cd) || !Validar.Dato(t)) { return; }
    if (!Validar.StrSoloNum(sl.val())) { return alert(Msg.ERROR_CAMPO_NUMERICO.replace("<attr>", "La unidad de atenci\u00F3n")); }
    $.ajax({
        type: "post", dataType: 'json', cache: false, url: "/Cond/Obtener", data: { codund: sl.val(), coddiv: cd },
        success: function (response, textStatus, jqXHR) {
            clearDesc(pa, ua); resetTabla(t);
            if (App.isError(response.respuesta)) {
                alert(response.respuesta);
            }
            else if (App.isExito(response.respuesta)) {
                $(ua).data("codigo", response.undatencion.CODIGO);
                $(ua).text(response.undatencion.DESCRIPCION);
                $(t).data("codigo", response.cabecera.CODIGO);
                $(pa).data("codigo", response.peratencion.codigo);
                $(pa).text(response.peratencion.descripcion);
                $.each(response.lista, function (idx, obj) {
                    var txtc = App.getInputCantidad();
                    var btne = App.getBtnRemove();
                    var el = [$(t).find("tr").length, obj["DESCRIPCION"], parseFloat(obj["PREUNI"]).toFixed(2), txtc, parseFloat(obj["TOTAL"]).toFixed(2), btne];
                    var nf = addRow($(t), el); nf.data("codigo", obj["CONVENTA"]); nf.data("item", obj["ITEM"]);
                    btne.on('click', function () {removeRow(nf, t);});
                    txtc.change(function () {
                        var cant = parseFloat($(this).val());
                        if (cant > 0) { updateRow(nf, t); } else { $(this).val(1); updateRow(nf, t); alert(Msg.CANT_NO_MENOR_A.replace("{text}", "una unidad")); }
                    });
                    $(t).append(nf);
                });
                actualizarTotal(t, Form.CSUBT); actualizarItems(t, Form.CITEMS);
            } else if (App.isAdvert(response.respuesta)) { f(sl); }
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}

function changeUndAtencion(txt) {
    if (Validar.Dato($(Form.Div).val())) {
        if (Validar.Dato(txt.val())) {
            obtenerDetalle(txt, $(Form.Div).val(), Form.Tabla, Form.PerDesc, Form.UndDesc,
                function (select) {
                    if (confirm(MSG_DESEA_APERTURAR)) { $("#modal-seleccionar-peratencion").modal('show'); }
                });
        } else { alert(Msg.SELECCION_UND); }
    } else { $(this).val(""); alert(Msg.SELECCION_DIV); }
}
$(document).ready(function () {
    $(Form.Div).on("change", function () {
        $(Form.Und).val("");
        resetTabla(Form.Tabla);
        clearDesc(Form.PerDesc, Form.UndDesc);
    });
    $(Form.Und).keypress(function (e) { if (e.which == 13) { changeUndAtencion($(this));}});
    $('#undatencion').bind('accepted', function (e, keyboard, el) { changeUndAtencion($(this));});
    obtenerDetalle($(Form.Und), $(Form.Div).val(), Form.Tabla, Form.PerDesc, Form.UndDesc, function (s) { console.log("sdsa"); });
    if ($(App.ControlTeclado + ":checked").val() == "ACT") {
        $("input[type='text']").keyboard();
        tecladoNumerico($("input[type='number']"));
    }
    $("#cambiar-undatencion").on("click", function () {
        if (!Validar.Dato($(Form.Div).val())) { return alert(Msg.SELECCION_DIV); }
        if (!Validar.Dato($(Form.Tabla).data("codigo"))) { return alert(Msg.SELECCION_UND); }
        $(Form.FormCambiarUnd).modal("show");
        $(Form.FormSelectDiv).val($(Form.Div).val());
        $(Form.FormSelectDiv).change();
    });
    $(Form.FormSelectDiv).on("change", function () {
        buscarAjax("/UndAtencion/ObtenerLibres", $(this).val(), $("#tabla-cambiar-undatencion"), ["CODIGO", "DESCRIPCION"],
            function (fila) {
                fila.on("click", function () {
                    if (confirm(MSG_DESEA_CAMBIAR_UNDATENCION)) {
                        var nuevocod = $(this).find("td").get(0).innerHTML
                        var divate = $(Form.FormSelectDiv).val();
                        var codigo = $(Form.Tabla).data("codigo");
                        cambiarUndatencion(codigo, $(Form.Und), $(Form.Div), nuevocod, divate, $(Form.UndDesc));
                        $(Form.FormCambiarUnd).modal("hide");
                    }
                });
            });
    });

    $("#factura-pre-factura").on("click", function () {
        if (!Validar.Dato($(Form.Div).val())) { return alert(Msg.SELECCION_DIV); }
        if (!Validar.Dato($(Form.Tabla).data("codigo"))) { return alert(Msg.SELECCION_UND); }
        var t = $(Form.Tabla);
        if (t.find("tbody tr").length <= 0) { return alert(Msg.SIN_ELEMENTOS); }
        printPreFactura(t)
    });

    $("#empleados li").on("click", function () {
        $("#modal-seleccionar-peratencion").modal("hide");
        var selected = $(this);
        $.ajax({
            type: "post", dataType: 'json', cache: false, url: "/UndAtencion/Aperturar",
            data: { codigo: $(Form.Und).val(), idperatencion: $(this).data("codigo"), divate: $(Form.Div).val() },
            success: function (response, textStatus, jqXHR) {
                if (response.respuesta.toString().split(":")[0] == "ERROR") { alert(response.respuesta); }
                else {
                    $(Form.Tabla).data("codigo", response.cabecera.CODIGO);
                    $(Form.UndDesc).text(response.undatencion.DESCRIPCION);
                    $(Form.UndDesc).data("codigo", response.undatencion.CODIGO);
                    $(Form.PerDesc).data("codigo", $(this).data("codigo"));
                    $(Form.PerDesc).text(selected.data("desc"));
                }
            }, error: function (xhr, status) { errorAjax(xhr, status); }
        });
    });

    $("#factura-btn-guardar").on("click", function () {
        if (!Validar.Dato($(Form.Div).val())) { return alert(Msg.SELECCION_DIV); }
        if (!Validar.Dato($(Form.Tabla).data("codigo"))) { return alert(Msg.SELECCION_UND); }
        guardarFactura($(Form.Tabla));
    });
    $(".seleccion-categoria").on("click", function () {
        var codigo = $(this).data("codigo");
        buscarConventaClaserv(codigo, $("#panel-productos-" + codigo));
    });
    $("#conventa-btn-buscar").on("click", function () {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"), Form.Tabla);
    });
    $("#conventa-texto").on("keyup", function (evt) {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"), Form.Tabla);
    });
    $('#conventa-texto').bind('accepted', function (e, keyboard, el) {
        buscarConventaDescripcion($("#conventa-texto").val(), $("#panel-buscar"), Form.Tabla);
    });
});