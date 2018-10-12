var teste;

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
            "<img src='" + jsonResponse[key].store.logo + "' class='storeLogo' /img>"
        );
    });

    var wrapper = document.createElement("div");
    wrapper.innerHTML = items.join("");

    //Recupera detalhes
    var detalhes = JSON.parse(jsonResponse[0].details);

    //Caso haja detalhes, cria os elementos HTML
    if (detalhes !== null)
    {
        var summary = detalhes[0].summary;
        var rating = detalhes[0].aggregated_rating;
        var screenshots = detalhes[0].screenshots;

        window.teste = screenshots;

        wrapper.innerHTML += "<p>" + summary + "</p>";
        wrapper.innerHTML += "<p>Rating: " + rating + "</p>";

        var prints = [];

        if (screenshots !== "undefined") {
            $.each(screenshots, function (key) {
                prints.push(
                    "<a href='" + screenshots[key].url + "' target='_blank'>" +
                        "<img src='" + screenshots[key].url + "' class='' /img>" +
                    "</a>"
                );
            });

            wrapper.innerHTML += prints;
        }
    }

    var modal = document.querySelector('div#myModalBody');
    modal.innerHTML = '';
    modal.appendChild(wrapper);
}

window.onload = function () {
    var userId = $("#hiddenUserEmail").data("value");
    $('#myModal').on('show.bs.modal', function (event) {
        var modal = $(this);
        var button = $(event.relatedTarget);
        var recipient = button.data('game');
        modal.find('.modal-title').text(recipient);
        getGameDetails(recipient, userId);
    });
}

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

