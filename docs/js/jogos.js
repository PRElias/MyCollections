var app = {
    games: [],
    gameDistinctList: [],
    availableTags: []
};

app.getGames = function () {
    var xobj = new XMLHttpRequest();
    xobj.overrideMimeType("application/json");
    xobj.open('GET', './games/games.json', true);
    xobj.onreadystatechange = function () {
        if (xobj.readyState == 4 && xobj.status == "200") {
            // .open will NOT return a value but simply returns undefined in async mode so use a callback
            // console.log("Lista de jogos recuperada");
            app.renderizeGames(xobj.responseText);
        }
    }
    xobj.send(null);
}

app.renderizeGames = function (response) {
    app.games = app.tryParseJSON(response);
    app.games = onlyEnabled(app.games);
    // console.log("Total de jogos: " + app.games.length);
    app.gameDistinctList = Array.from(app.games);
    // app.gameDistinctList = onlyEnabled(app.gameDistinctList);

    //Ordenando
    app.gameDistinctList.sort(function (a, b) {
        return a.Name.toLowerCase() < b.Name.toLowerCase() ? -1 : a.Name.toLowerCase() > b.Name.toLowerCase() ? 1 : 0;
    });
    
    //Removendo propriedades pra poder fazer o distinct
    for (var index in app.gameDistinctList) {
        delete app.gameDistinctList[index].BuyDate;
        delete app.gameDistinctList[index].Disabled;
        delete app.gameDistinctList[index].GameID;
        delete app.gameDistinctList[index].IGDBId;
        delete app.gameDistinctList[index].PlayedTime;
        delete app.gameDistinctList[index].Price;
        delete app.gameDistinctList[index].Purchased;
        delete app.gameDistinctList[index].Selected;
        delete app.gameDistinctList[index].SteamApID;
        delete app.gameDistinctList[index].Store;
        delete app.gameDistinctList[index].System;
    }
    //Removendo duplicatas
    app.gameDistinctList = multiDimensionalUnique(app.gameDistinctList);

    //console.log("Jogos únicos: " + app.gameDistinctList.length);

    //Montando elementos HTML
    let items = [];
    for (let index in app.gameDistinctList) {
        let game = app.gameDistinctList[index];
        let gameName = game.Name.replace("'", "");
        items.push(
            "<span class='game col-lg-2 col-sm-6 col-md-6 col-xs-12' id='" + game.Id + "' onclick='showDetails(this.id)'>" +
            "<p class='gameName'>" + gameName + "</p>" +
            "<img class='cover lazy' data-src='" + game.LogoURL + "' data-game='" + gameName + "' alt='logo' /img>" +
            "</span>"
        );
        app.availableTags.push(gameName);
    }

    var wrapper = document.createElement('div');
    wrapper.innerHTML = items.join("");

    var main = document.querySelector('div.main_div');
    main.appendChild(wrapper);

    $('.lazy').Lazy();
};

app.renderizeDetails = function (gameName) {

    let gameCopies = app.games.filter(function (g) {
        return g.name == gameName;
    });

    //Montando elementos HTML
    var items = [];
    for (var index in gameCopies) {
        var game = gameCopies[index];
        items.push(
            "<p>" + game.system + " / " + game.store + "</p>"
        );
    }

    let wrapper = document.createElement('div');
    wrapper.innerHTML = items.join("");

    let main = document.querySelector('.modal-body');
    main.innerHTML = "";

    let steamAppID = getSteamAppID(gameCopies);

    if (steamAppID != undefined) {
        let steamLink = "https://store.steampowered.com/app/" + steamAppID;

        if (steamLink != undefined) {
            let link = document.createElement('a');
            link.href = steamLink;
            link.innerHTML = "Link do Steam";
            link.target = "_blank";
            wrapper.appendChild(link);
        }
    }

    main.appendChild(wrapper);
};

app.tryParseJSON = function (jsonString) {
    try {
        var o = JSON.parse(jsonString);
        if (o && typeof o === "object") {
            return o;
        }
    }
    catch (e) { }
    return false;
};


window.onload = function () {
    app.getGames();
}

function getSteamAppID(gameCopies) {
    for (var index in gameCopies) {
        var game = gameCopies[index];
        if (game.appID != "")
            return game.appID;
    }
}

function navigateToGame() {
    var pesquisa = $('#procurar').val();
    var jogo = document.getElementById(pesquisa);

    if (jogo !== null) {
        $('html, body').animate({
            scrollTop: $(jogo).offset().top - 35
        }, 1000);
    }
}

function multiDimensionalUnique(arr) {
    var uniques = [];
    var itemsFound = {};
    for (var i = 0, l = arr.length; i < l; i++) {
        var stringified = JSON.stringify(arr[i]);
        if (itemsFound[stringified]) { continue; }
        uniques.push(arr[i]);
        itemsFound[stringified] = true;
    }
    return uniques;
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
};

$('#procurar').focus(
    function () {
        $(this).val('');
    }
);

$('#procurar').click(
    function () {
        $(this).val('');
    }
);

//Autocomplete
$(function () {
    $("#procurar").autocomplete({
        source: app.availableTags,
        select: function (event, ui) {
            event.preventDefault();
            $('#procurar').val(ui.item.value);
            navigateToGame();
        }
    });
});

function showDetails(gameName) {
    app.renderizeDetails(gameName);
    $('.modal-title').text(gameName);
    $("#modal").modal('show');
}

function onlyEnabled(array) {
    let enabled = [];
    for (var index in array) {
        if (array[index].Disabled == false) {
            enabled.push(array[index]);
        }
    }
    return enabled;
}

function gameTotals(array, system) {
    let count = 0;
    for (var index in array) {
        if (array[index].system == system) {
            count++;
        }
    }
    return count;
}


function renderizeGeneralDetails() {

    let totalJogosUnicos = document.createElement('p');
    totalJogosUnicos.innerText = "Total de jogos únicos: " + app.gameDistinctList.length;
    let totalJogos = document.createElement('p');
    totalJogos.innerText = "Total de jogos: " + app.games.length;
    let totalPS3 = document.createElement('p');
    totalPS3.innerText = "Total de PS3: " + gameTotals(app.games, 'PS3');
    let totalPS4 = document.createElement('p');
    totalPS4.innerText = "Total de PS4: " + gameTotals(app.games, 'PS4');
    let totalXBOXOne = document.createElement('p');
    totalXBOXOne.innerText = "Total de XBOX One: " + gameTotals(app.games, 'XBOX One');


    let wrapper = document.createElement('div');
    wrapper.appendChild(totalJogosUnicos);
    wrapper.appendChild(totalJogos);
    wrapper.appendChild(totalPS3);
    wrapper.appendChild(totalPS4);
    wrapper.appendChild(totalXBOXOne);

    let main = document.querySelector('.modal-body');
    main.innerHTML = "";

    main.appendChild(wrapper);

    $('.modal-title').text("Detalhes");
    $("#modal").modal('show');
};

$(window).scroll(function () {
    if ($(this).scrollTop() > 50) {
        $('.scrolltop:hidden').stop(true, true).fadeIn();
    } else {
        $('.scrolltop').stop(true, true).fadeOut();
    }
});
$(function () { $(".scroll").click(function () { $("html,body").animate({ scrollTop: "50" }, "1000"); return false }) })
