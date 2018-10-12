var teste;
var igdbURL;

function getGameDetails(gameName, userEmail) {
    //debugger;
    var items = [];
    var gameDetails = [];
    var url = '/api/Games/GetGame/' + encodeURIComponent(userEmail) + '/' + encodeURIComponent(gameName);
    var request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (request.readyState === XMLHttpRequest.DONE) {
            if (request.status === 200) {
                gameDetails = request.response;
            }
        }
    };
    request.open('GET', url, false);
    request.send();

    var jsonResponse = JSON.parse(gameDetails);

    //console.log(jsonResponse);

   window.teste = jsonResponse;

    $.each(jsonResponse, function (key) {
        items.push(
            "<img src='" + jsonResponse[key].store.logo + "' class='storeLogo' /img>" +
            "<img src='" + jsonResponse[key].system.logo + "' class='storeLogo' /img>"
        );
    });

    var wrapper = document.createElement("div");
    wrapper.innerHTML = items.join("");

    //Recupera detalhes
    var detalhes = JSON.parse(jsonResponse[0].details);

    if (detalhes !== null)
    {
        igdbURL = detalhes[0].url;
    }

    var modal = document.querySelector('div#myModalBody');
    modal.innerHTML = '';
    modal.appendChild(wrapper);
}

$(document).ready(function () {
    var userId = $("#hiddenUserEmail").data("value");
    $('#myModal').on('show.bs.modal', function (event) {
        var modal = $(this);
        var button = $(event.relatedTarget);
        var recipient = button.data('game');
        getGameDetails(recipient, userId);
        if (igdbURL !== null) {
            modal.find('.modal-header').empty().append("<a href='" + igdbURL + "' target='_blank'>" + recipient + "</a>");
        }
        else {
            modal.find('.modal-title').text(recipient);
        }
    });


    $(document).keyup(function () {
        $('#goToGame').attr('href', '#' + encodeURIComponent($('#procurar').val().toUpperCase()));
        $("#goToGame")[0].click();

        //$('html, body').animate({
        //    scrollTop: $("mydiv").offset().top
        //}, 1000);
    });
});


/* When the user scrolls down, hide the navbar. When the user scrolls up, show the navbar */
var prevScrollpos = window.pageYOffset;
window.onscroll = function () {
    var currentScrollPos = window.pageYOffset;

    if (prevScrollpos > currentScrollPos) {
        document.getElementById("navbar").style.top = "0";
        document.getElementById("main_div").style.marginTop = "50px";
    } else {
        document.getElementById("navbar").style.top = "-50px";
        document.getElementById("main_div").style.marginTop = "0";
    }
    prevScrollpos = currentScrollPos;
}


