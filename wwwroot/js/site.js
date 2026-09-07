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
    var gamesTableStateKey = 'gamesTableState:' + window.location.pathname + window.location.search;

    $('#gamesTable').DataTable({
        columns: colunas,
        bStateSave: true,
        fnStateSave: function (oSettings, oData) {
            localStorage.setItem(gamesTableStateKey, JSON.stringify(oData));
        },
        fnStateLoad: function (oSettings) {
            return JSON.parse(localStorage.getItem(gamesTableStateKey));
        }
    });

    $('#btn_NewGame').prependTo($('.dataTables_wrapper  .dt-buttons'));
    $('#btn_AutoNewGames').prependTo($('.dataTables_wrapper  .dt-buttons'));
    $('#btn_Commit').prependTo($('.dataTables_wrapper  .dt-buttons'));
    $('#btn_Todos').prependTo($('.dataTables_wrapper  .dt-buttons'));
    $('#btn_SemLogo').prependTo($('.dataTables_wrapper  .dt-buttons'));
    $('#btn_AtualizarLogosSteam').prependTo($('.dataTables_wrapper  .dt-buttons'));

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

function pesquisarImagensJogo(gameId) {
    var term = $('#imageSearchTerm').val();
    var $status = $('#imageSearchStatus');
    var $results = $('#imageSearchResults');

    if (!term) {
        $status.text('Informe um termo para pesquisar.');
        return;
    }

    $status.text('Pesquisando...');
    $results.empty();

    $.getJSON('/Games/PesquisarImagens', { termo: term })
        .done(function (images) {
            if (!images || images.length === 0) {
                $status.text('Nenhuma imagem encontrada.');
                return;
            }

            $status.text('Clique em uma imagem para salvar como logo.');
            images.forEach(function (image) {
                var imageUrl = image.imageUrl || image.ImageUrl;
                var thumbnailUrl = image.thumbnailUrl || image.ThumbnailUrl || imageUrl;
                var title = image.title || image.Title || '';
                var sourceUrl = image.sourceUrl || image.SourceUrl || '';

                var $button = $('<button type="button" class="btn btn-light border m-1 internet-image-option"></button>');
                $button.attr('title', title);
                $button.append($('<img />').attr('src', thumbnailUrl).attr('alt', title).css({ width: '160px', height: '90px', objectFit: 'cover' }));
                $button.on('click', function () {
                    salvarLogoInternet(gameId, imageUrl, thumbnailUrl, sourceUrl);
                });
                $results.append($button);
            });
        })
        .fail(function () {
            $status.text('Não foi possível pesquisar imagens agora.');
        });
}

function atualizarLinksPesquisaImagem(term) {
    var query = encodeURIComponent(term + ' game cover');
    $('#duckDuckGoImageSearch').attr('href', 'https://duckduckgo.com/?q=' + query + '&iax=images&ia=images');
    $('#bingImageSearch').attr('href', 'https://www.bing.com/images/search?q=' + query);
}

function salvarLogoManual(gameId) {
    var imageUrl = $('#manualImageUrl').val();
    if (!imageUrl) {
        $('#imageSearchStatus').text('Cole uma URL de imagem para salvar.');
        return;
    }

    salvarLogoInternet(gameId, imageUrl, '', '');
}
function salvarLogoInternet(gameId, imageUrl, thumbnailUrl, sourceUrl) {
    var $form = $('<form method="post" action="/Games/SalvarLogoInternet"></form>');
    $form.append($('<input type="hidden" name="gameId" />').val(gameId));
    $form.append($('<input type="hidden" name="imageUrl" />').val(imageUrl));
    $form.append($('<input type="hidden" name="thumbnailUrl" />').val(thumbnailUrl));
    $form.append($('<input type="hidden" name="sourceUrl" />').val(sourceUrl));
    $('body').append($form);
    $form.submit();
}

function deleteGame(id) {
    var xobj = new XMLHttpRequest();
    xobj.open('POST', '../../Games/Delete/' + id, true);
    xobj.onreadystatechange = function () {
        if (xobj.readyState == 4 && xobj.status == "200") {
            // .open will NOT return a value but simply returns undefined in async mode so use a callback
            // console.log("Lista de jogos recuperada");
            window.location.href = '../../Games';
        }
    }
    xobj.send(null);
}
