var parpwd = $("#parpwd").text();
var rol = $("#rol").text();
function getUndAtencion(p, c) {
    $.ajax({
        type: "get", dataType: 'json', cache: false, url: "/UndAtencion/Obtener", data: { divatencion: c },
        success: function (response, textStatus, jqXHR) {
            if (response.respuesta.split(":")[0] == "ERROR") { return alert(response.respuesta); }
            p.empty();
            $.each(response.libres, function (idx, obj) {
                var pu = $("<div class='col-xs-3 col-sm-2 col-md-1 panel undatencion'>" + obj["DESCRIPCION"] + "</div>");
                pu.data("codigo", obj["CODIGO"]);
                p.append(pu);
                pu.on("click", function () {
                    $("#und-sel").text($(this).data("codigo"));
                        var m = $("#modal-seleccionar-peratencion");
                        m.modal("show");
                        $("#perate-pass").focus();
                });
            });
            $.each(response.ocupadas, function (idx, obj) {
                var pu = $("<div class='col-xs-3 col-sm-2 col-md-1 panel undatencion-ocupada'>" + obj["UNDDES"] + "<br/></div>");
                pu.data("codund", obj["CODUND"]); pu.data("coddiv", obj["CODDIV"]); pu.data("codper", obj["CODPER"]);
                pu.append($("<div class='percorto'>" + obj["PERCORTO"] + "</div>"));
                pu.on("click", function () {
                    if (parpwd == "S") {
                        var m = $("#modal-ingresar");
                        m.find("input").val("");
                        m.data("codund", obj["CODUND"]); m.data("coddiv", obj["CODDIV"]); m.data("codper", obj["CODPER"]); m.modal("show");
                        $("#perate-pass-2").focus();
                    } else {
                        var div = obj["CODDIV"]; var und = obj["CODUND"]; var per = obj["CODPER"];
                        if (rol == "M") {
                            window.location = "/UndAtencion/Consumos?div=" + div + "&und=" + und + "&per=" + per;
                        } else if (rol == "C") {
                            window.location = "/UndAtencion/Facturacion?div=" + div + "&und=" + und + "&per=" + per;
                        }
                    }
                });
                p.append(pu);
            });
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
}
function ingresarSoloClave() {
    var pass = $("#perate-pass-2").val();
    if (!isNullOrWhiteSpace(pass)) {
        var m = $("#modal-ingresar");
        m.modal("hide");
        var und = m.data("codund"); var div = m.data("coddiv"); var per = m.data("codper");
        aperturarUndAtencion(div, und, per, pass, function (response) {
            if (response.rol == "M") {
                window.location = "/UndAtencion/Consumos?div=" + response.div + "&und=" + response.und + "&per=" + response.per;
            } else if (response.rol = "C") {
                window.location = "/UndAtencion/Facturacion?div=" + response.div + "&und=" + response.und + "&per=" + response.per;
            }
        });
    } else { alert("Ingrese contrase\u00f1a"); }
}

$(document).ready(function () {
    $("input").keyboard();
    $("#divisiones li").on("click", function () {
        var cod = $(this).data("codigo");
        $("#div-sel").text(cod);
        getUndAtencion($("#divatencion-" + cod), cod);
    });
    $("#empleados li").on("click", function () {
        $(this).siblings("li").removeClass("element-active");
        $(this).addClass("element-active");
        $("#per-sel").text($(this).data("codigo"));
    });
    $("#empleados li").on("click", function () {
        if (parpwd == "S") {
            var pass = $("#perate-pass").val();
            if (!isNullOrWhiteSpace(pass)) {
                var div = $("#div-sel").text(); var und = $("#und-sel").text(); var per = $("#per-sel").text();
                if (isNullOrWhiteSpace(per)) { return alert("Seleccione una persona");}
                $("#modal-seleccionar-peratencion").modal("hide");
                aperturarUndAtencion(div, und, per, pass, function (response) {
                    if (response.rol == "M") {
                        window.location = "/UndAtencion/Consumos?div=" + response.div + "&und=" + response.und + "&per=" + response.per;
                    } else if (response.rol = "C") {
                        window.location = "/UndAtencion/Facturacion?div=" + response.div + "&und=" + response.und + "&per=" + response.per;
                    }
                });
            } else { alert("Ingrese contrase\u00F1a"); }
        } else {
            var div = $("#div-sel").text(); var und = $("#und-sel").text(); var per = $("#per-sel").text();
            aperturarUndAtencion(div, und, per, "", function (response) {
                if (response.rol == "M") {
                    window.location = "/UndAtencion/Consumos?div=" + response.div + "&und=" + response.und + "&per=" + response.per;
                } else if (response.rol = "C") {
                    window.location = "/UndAtencion/Facturacion?div=" + response.div + "&und=" + response.und + "&per=" + response.per;
                }
            });
        }
    });

    $("#btn-ingresar-2").on("click", function () { ingresarSoloClave();});
    $("#perate-pass-2").bind('accepted', function (e, keyboard, el) { ingresarSoloClave(); });
});