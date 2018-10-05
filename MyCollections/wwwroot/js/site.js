$(document).ready(function () {
    $('#tabela').DataTable();
});

var userId = $("#hiddenUserId").data("value");

if (userId === null) {
    $('#btn_GetFromSteam').css("pointer-events", "none");
}


$('#btn_GetFromSteam').click(function () {
    $('#btn_GetFromSteam').text("Aguarde").addClass("blink_me").css("pointer-events", "none");
    var userId = $("#hiddenUserId").data("value");
    $.ajax({
        method: "GET",
        url: "GetFromSteam/" + userId,
        success: function (results) {
            $('#btn_GetFromSteam').text("Atualizar informações do Steam").removeClass("blink_me").css("pointer-events", "initial");
            alert(results);
            location.reload();
        },
        error: function (jqXHR, error, errorThrown) {
            $('#btn_GetFromSteam').text("Atualizar informações do Steam").removeClass("blink_me").css("pointer-events", "initial");
            if (jqXHR.status && jqXHR.status === 401) {
                alert(jqXHR.responseText);
            } else {
                alert("Erro ao recuperar informações do Steam");
            }
        }
    });
});


