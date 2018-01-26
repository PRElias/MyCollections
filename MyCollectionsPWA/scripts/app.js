console.log("arquivo carregado");

document.getElementById('butRefresh').addEventListener('click', function () {
    console.log("botão clicado");
    app.getGames();
});

//(function () {
//'use strict';

var app = {
    isLoading: true,
    visibleCards: {},
    selectedGames: [],
    spinner: document.querySelector('.loader'),
    cardTemplate: document.querySelector('.cardTemplate'),
    container: document.querySelector('.main')
};

document.addEventListener("DOMContentLoaded", function () {

});

app.getGames = function () {
    var game;
    var items = [];
    var url = 'http://mycollectionsapi.paulorobertoelias.com.br/api/Games';
    var request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (request.readyState === XMLHttpRequest.DONE) {
            if (request.status === 200) {

                var response = JSON.parse(request.response);
                for (var index in response) {
                    var game = response[index];

                    items.push(
                        "<div class='card cardTemplate' id='" + game.gameID + "'><p>" + game.name + "</p>" +
                        "</div > "
                    );
                }

                debugger;

                var wrapper = document.createElement('div');
                wrapper.innerHTML = items.join("");

                var main = document.getElementById("main_div");
                main.appendChild(wrapper);
                
                app.spinner.setAttribute('hidden', true);
            }
        };
    };
    request.open('GET', url);
    request.send();

    // TODO add saveSelectedGames function here
    // Save list of cities to localStorage.
    app.saveSelectedGames = function () {
        var selectedGames = JSON.stringify(app.selectedGames);
        localStorage.selectedGames = selectedGames;
    };
};



app.selectedGames = localStorage.selectedGames;
if (app.selectedGames) {
    app.selectedGames = JSON.parse(app.selectedGames);
    app.selectedGames.forEach(function (city) {
        //app.getGames();
    });
} else {
    /* The user is using the app for the first time, or the user has not
     * saved any cities, so show the user some fake data. A real app in this
     * scenario could guess the user's location via IP lookup and then inject
     * that data into the page.
     */
    //app.updateForecastCard(initialWeatherForecast);
    //app.selectedCities = [
    //    { key: initialWeatherForecast.key, label: initialWeatherForecast.label }
    //];
    //app.saveSelectedCities();
}

// TODO add service worker code here
if ('serviceWorker' in navigator) {
    navigator.serviceWorker
        .register('./service-worker.js')
        .then(function () { console.log('Service Worker Registered'); });
}
//});
