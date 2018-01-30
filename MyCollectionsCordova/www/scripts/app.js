//document.getElementById('butRefresh').addEventListener('click', function () {
//    console.log("botão clicado");
//});

//localStorage.clear();


var app = {
    isLoading: true,
    visibleCards: {},
    games: [],
    spinner: document.querySelector('.loader'),
    cardTemplate: document.querySelector('.cardTemplate'),
    container: document.querySelector('.main')
};

//document.addEventListener("DOMContentLoaded", function () {

//});


app.getGames = function () {
    //debugger;
    app.spinner.setAttribute('hidden', false);
    app.games = localStorage.getItem("games");
    if (app.games === undefined || app.games === null) {

        var url = 'http://mycollectionsapi.paulorobertoelias.com.br/api/Games';
        var request = new XMLHttpRequest();
        request.onreadystatechange = function () {
            if (request.readyState === XMLHttpRequest.DONE) {
                if (request.status === 200) {
                    app.games = request.response;
                    localStorage.setItem("games", app.games);
                }
            }
        };
        request.open('GET', url, false);
        request.send();
    }

    app.renderizeGames();
    app.spinner.setAttribute('hidden', true);

};

app.renderizeGames = function () {
    //debugger
    var items = [];
    var games = JSON.parse(app.games);
    for (var index in games) {
        var game = games[index];

        items.push(
            "<div class='row' id='" + game.gameID + "'>" +
            //"<p>" + game.name + "</p > " +
            "<img class='col cover' src='" + game.cover + "' alt='logo' /img>" +
            "</div > "
        );
    }

    var wrapper = document.createElement('div');
    wrapper.innerHTML = items.join("");

    var main = document.getElementById("main_div");
    main.appendChild(wrapper);
};

app.getGames();

//if ('serviceWorker' in navigator) {
//    navigator.serviceWorker
//        .register('./service-worker.js')
//        .then(function () { console.log('Service Worker Registered'); });
//}