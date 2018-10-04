$(document).ready(function () {
    $('#tabela').DataTable();
});

$('#btn_GetFromSteam').click(function () {
    $.ajax({
        method: "GET",
        url: "Games/GetFromSteam",
        complete: function (results) {
            alert(results);
        }
    });
});


