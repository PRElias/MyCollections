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

    console.log(jsonResponse);

    window.teste = jsonResponse;

    $.each(jsonResponse, function (key) {
        items.push(
            
            "<img src='" + jsonResponse[key].store.logo + "' class='storeLogo' /img>"
        );
    });

    var wrapper = document.createElement("div");
    wrapper.innerHTML = items.join("");

    var modal = document.querySelector('div#myModalBody');
    modal.innerHTML = '';
    modal.appendChild(wrapper);
};

window.onload = function () {
    var userId = $("#hiddenUserEmail").data("value");
    $('#myModal').on('show.bs.modal', function (event) {
        var modal = $(this);
        var button = $(event.relatedTarget);
        var recipient = button.data('game');
        modal.find('.modal-title').text(recipient);
        getGameDetails(recipient, userId);
    });
};