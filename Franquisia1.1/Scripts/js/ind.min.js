var mpa = "#modal-seleccionar-peratencion";
var cld = "class='col-xs-3 col-sm-2 col-md-1 panel";
var parpwd; var rol; var refresh = undefined;
function goTo(r, d, u, p) { if (r == "M") { window.location = "/UndAtencion/Consumos?div=" + d + "&und=" + u + "&per=" + p; } else if (r == "C") { window.location = "/UndAtencion/Facturacion?div=" + d + "&und=" + u + "&per=" + p; } }
function getUndAtencion(p, c) {
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: "/UndAtencion/Obtener", data: { divatencion: c },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.split(":")[0] == "ERROR") { return alert(response.respuesta); }
            p.empty();
            $.each(response.libres, function (idx, obj) {
                var pu = $("<div "+cld+" und und-libre'>" + obj["DESCRIPCION"] + "</div>");
                pu.data("codigo", obj["CODIGO"]);
                p.append(pu);
                pu.on("click", function () {
                    $("#und-sel").text($(this).data("codigo"));
                    $(mpa).modal("show");
                    $("#perate-pass").focus();
                });
            });
            p.append($("<div class='container row'></div>"));
            $.each(response.ocupadas, function (idx, obj) {
                var sts = obj["FACT"] == "S" ? "und-fact" : "und-ocupada";
                var pu = $("<div "+cld+" und " + sts + "'>" + obj["UNDDES"] + "<br/></div>");
                pu.data("codund", obj["CODUND"]); pu.data("coddiv", obj["CODDIV"]); pu.data("codper", obj["CODPER"]);
                pu.append($("<div class='percorto'>" + obj["PERCORTO"] + "</div>"));
                pu.on("click", function () {
                    console.log(rol);
                    if (pu.hasClass("und-fact")) {return alert("ERROR: Acceso denegado, el consumo est\u00E1 en el proceso de facturaci\u00F3n");}
                    if (parpwd == "S") {
                        var m = $("#modal-ingresar");
                        m.find("input").val("");
                        m.data("codund", obj["CODUND"]); m.data("coddiv", obj["CODDIV"]); m.data("codper", obj["CODPER"]); m.modal("show");
                        $("#perate-pass-2").focus();
                    } else {
                        var div = obj["CODDIV"]; var und = obj["CODUND"]; var per = obj["CODPER"];goTo(rol, div, und, per);
                    }
                });
                p.append(pu);
            });
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function ingresarSoloClave() {
    var pass = $("#perate-pass-2").val();
    if (Validar.Dato(pass)) {
        var m = $("#modal-ingresar");
        m.modal("hide");
        var und = m.data("codund"); var div = m.data("coddiv"); var per = m.data("codper");
        aperturarUndAtencion(div, und, per, pass, function (response) { goTo(response.rol, response.div, response.und, response.per);});
    } else { alert("Ingrese contrase\u00f1a"); }
}
function obtenerRefresh() {
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: "/Parreg/TiempoRefrescoPanel", data: {},
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.split(":")[0] == "ERROR") { alert(response.respuesta); }
            refresh = 1000 * parseInt(response.tiempo);
            console.log(refresh);
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
$(document).ready(function () {
    parpwd = $("#parpwd").text();
    rol = $("#rol").text();
    obtenerRefresh(refresh);
    tecladoNumerico($("input"), 4);
    $("#divisiones li").on("click", function () {
        var cod = $(this).data("codigo"); $("#div-sel").text(cod); getUndAtencion($("#divatencion-" + cod), cod);
    });
    $("#empleados li").on("click", function () {
        $(this).siblings("li").removeClass("element-active"); $(this).addClass("element-active"); $("#per-sel").text($(this).data("codigo"));
        if (parpwd == "S") {
            var pass = $("#perate-pass").val();
            if (Validar.Dato(pass)) {
                var div = $("#div-sel").text(); var und = $("#und-sel").text(); var per = $("#per-sel").text();
                if (!Validar.Dato(per)) { return alert("Seleccione una persona"); }
                $(mpa).modal("hide");
                aperturarUndAtencion(div, und, per, pass, function (response) { goTo(response.rol, response.div, response.und, response.per);});
            } else {$("#perate-pass").focus();}
        } else {
            var div = $("#div-sel").text(); var und = $("#und-sel").text(); var per = $("#per-sel").text();
            aperturarUndAtencion(div, und, per, "", function (response) { goTo(rol, div, und, per);});
        }
    });
    $("#btn-ingresar-2").on("click", function () { ingresarSoloClave();});
    $("#perate-pass-2").bind('accepted', function () { ingresarSoloClave(); });
});