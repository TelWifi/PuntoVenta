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
                    $("#modal-seleccionar-peratencion").modal("show");
                });
            });
            $.each(response.ocupadas, function (idx, obj) {
                var pu = $("<div class='col-xs-3 col-sm-2 col-md-1 panel undatencion-ocupada'>" + obj["UNDDES"] + "<br/></div>");
                pu.data("codund", obj["CODUND"]);
                pu.data("coddiv", obj["CODDIV"]);
                pu.data("codper", obj["CODPER"]);
                pu.append($("<div class='percorto'>" + obj["PERCORTO"] + "</div>"));
                pu.on("click", function () {
                    var m = $("#modal-ingresar");
                    m.data("codund", obj["CODUND"]); m.data("coddiv", obj["CODDIV"]); m.data("codper", obj["CODPER"]);
                    m.modal("show");
                });
                p.append(pu);
            });
        }, error: function (xhr, status) { errorAjax(xhr, status); }
    });
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

    $("#btn-ingresar").on("click", function () {
        var pass = $("#perate-pass").val();
        if (!isNullOrWhiteSpace(pass)) {
            var div = $("#div-sel").text();
            var und = $("#und-sel").text();
            var per = $("#per-sel").text();
            window.location = "/UndAtencion/Selector?div=" + div + "&und=" + und + "&per=" + per + "&pass=" + pass;
            $("#modal-seleccionar-peratencion").modal("hide");
        } else {
            alert("Ingrese contraseña");
        }
    });
    $("#btn-ingresar-2").on("click", function () {
        var pass = $("#perate-pass-2").val();
        if (!isNullOrWhiteSpace(pass)) {
            var m = $("#modal-ingresar");
            var und = m.data("codund");
            var div = m.data("coddiv");
            var per = m.data("codper");
            m.modal("hide");

            window.location = "/UndAtencion/Selector?div=" + div + "&und=" + und + "&per=" + per + "&pass=" + pass;


        } else {
            alert("Ingrese contraseña");
        }
    });
});