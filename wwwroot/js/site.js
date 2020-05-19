// $('#btn_GetFromSteam').click(function () {
//     $('#btn_GetFromSteam').text("Aguarde").addClass("blink_me").css("pointer-events", "none");
//     var userId = $("#hiddenUserId").data("value");
//     $.ajax({
//         method: "GET",
//         url: "GetFromSteam/" + userId,
//         success: function (results) {
//             $('#btn_GetFromSteam').text("Atualizar informações do Steam").removeClass("blink_me").css("pointer-events", "initial");
//             alert(results);
//             location.reload();
//         },
//         error: function (jqXHR, error, errorThrown) {
//             $('#btn_GetFromSteam').text("Atualizar informações do Steam").removeClass("blink_me").css("pointer-events", "initial");
//             if (jqXHR.status && jqXHR.status === 401) {
//                 alert(jqXHR.responseText);
//             } else {
//                 alert("Erro ao recuperar informações do Steam");
//             }
//         }
//     });
// });

// Opções padrão para todas as datatables
$.extend($.fn.dataTable.defaults, {
    pageLength: 50,
    ordering: true,
    responsive: true,
    language: {
        lengthMenu: "Exibe _MENU_ Registros por página",
        search: "",
        searchPlaceholder: "Digite aqui para procurar",
        paginate: {
            "previous": "<",
            "next": ">"
        },
        zeroRecords: "Nada foi encontrado",
        loadingRecords: "<span class='fa-stack fa-lg'><i class='fa fa-spinner fa-spin fa-stack-1x fa-fw'></i></span>",
        info: "Exibindo página _PAGE_ de _PAGES_",
        infoEmpty: "",
        infoFiltered: "(filtrado de _MAX_ regitros totais)",
        emptyTable: "Sem dados"
    },
    dom: 'Bfrtip',
    buttons: [{
        extend: "excelHtml5",
        text: "Download em Excel",
        className: "btn btn-primary",
        init: function (api, node, config) {
            $(node).removeClass('dt-button buttons-excel buttons-html5')
        }
    }]
});

$.extend($.fn.dataTableExt.oSort, {
    "date-range-pre": function (a) {
        var monthArr = ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];
        return monthArr.indexOf(a);
    },
    "date-range-asc": function (a, b) {
        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    },
    "date-range-desc": function (a, b) {
        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    }
});

$.fn.dataTable.render.multi = function (renderArray) {
    return function (d, type, row, meta) {
        for (var r = 0; r < renderArray.length; r++) {
            d = renderArray[r](d, type, row, meta);
        }

        return d;
    }
}

//Table
var colunas = [{
        "data": "GameID",
        "title": "Id"
    },
    {
        "data": "nome",
        "title": "Nome"
    },
    {
        "data": "logo",
        "title": "Logo"
    },
    {
        "data": "loja",
        "title": "Loja",
        "width": "7%"
    },
    {
        "data": "sistema",
        "title": "Sistema"
    },
    {
        "data": "status",
        "title": "Status"
    }
]

$(window).on('load', function(){
    // PAGE IS FULLY LOADED  
    // FADE OUT YOUR OVERLAYING DIV
    $('#overlay').fadeOut();
 });

$(document).ready(function () {
    $('#gamesTable').DataTable({
        columns: colunas,
        bStateSave: true,
        fnStateSave: function (oSettings, oData) {
            localStorage.setItem('gamesTableState', JSON.stringify(oData));
        },
        fnStateLoad: function (oSettings) {
            return JSON.parse(localStorage.getItem('gamesTableState'));
        }
    });

    $('#btn_NewGame').prependTo($('.dataTables_wrapper  .dt-buttons'));
    $('#btn_AutoNewGames').prependTo($('.dataTables_wrapper  .dt-buttons'));
    $('#btn_Commit').prependTo($('.dataTables_wrapper  .dt-buttons'));

    $('#gamesTable').on('click', 'tbody tr', function (evt) {
        var $cell = $(evt.target).closest('td');
        console.log("Linha clicada: " + gamesTable.rows[this.rowIndex]._DT_RowIndex);
        if ($cell.index() > 0) {
            window.location.href = "/Games/Edit/" + gamesTable.rows[this.rowIndex].firstElementChild.innerText;
        }
    });

    $('#selectAll').click( function () {
        $('.card input[type="checkbox"]').prop('checked', this.checked)
    })
});

function usarImagemSteam() {
    debugger
    // document.getElementById("divNewImage").style.display = "none";
    var element = document.createElement('input');
    element.type = "text";
    element.id = "usingSteamLogo";
    element.value = $("#logoSteam")[0].src;
    element.name = "SteamOriginalImageURL";
    element.hidden = true;
    console.log(element);
    $("#divNewImage").append(element);
}