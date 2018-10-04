$(document).ready(function () {
    $('#tabela').DataTable();
});

$('#btn_GetFromSteam').click(function () {
    $(this).attr("disabled", true);
    var userId = $("#hiddenUserId").data("value");
    $.ajax({
        method: "GET",
        url: "GetFromSteam/" + userId,
        success: function (results) {
            $(this).attr("disabled", false);
            alert(results);
        }
    });
});


