$(document).ready(function () {
    $('#tabela').DataTable();
});

$('#btn_GetFromSteam').click(function () {
    $('#btn_GetFromSteam').text("Aguarde").addClass("blink_me").css("pointer-events", "none");
    var userId = $("#hiddenUserId").data("value");
    $.ajax({
        method: "GET",
        url: "GetFromSteam/" + userId,
        success: function (results) {
            $('#btn_GetFromSteam').text("Atualizar informações do Steam").removeClass("blink_me").css("pointer-events", "none");
            alert(results);
        },
        error: function (jqXHR, error, errorThrown) {
            $('#btn_GetFromSteam').text("Atualizar informações do Steam").removeClass("blink_me").css("pointer-events", "none");
            if (jqXHR.status && jqXHR.status === 401) {
                alert(jqXHR.responseText);
            } else {
                alert("Erro ao recuperar informações do Steam");
            }
        }
    });
});


