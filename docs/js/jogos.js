var app = {
    games: []
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
    app.games = tryParseJSON(response);
    //Ordenando
    app.games.sort(function (a, b) {
        return a.FriendlyName < b.FriendlyName ? -1 : a.FriendlyName > b.FriendlyName ? 1 : 0;
    });

    let items = [];
    let lastName = "";
    let hidden = "";
    for (let index in app.games) {
        if (app.games[index].Name == lastName) {
            hidden = " hidden";
        }
        else {
            hidden = "";
        }
        lastName = app.games[index].Name;
        if (app.games[index].Disabled == false) {
            items.push(
                "<span " + hidden + " class='game col-lg-2 col-sm-6 col-md-6 col-xs-12 " + app.games[index].System + " " + app.games[index].Store + "' id='" + app.games[index].GameID + 
                "' name='" + app.games[index].FriendlyName +
                "' onclick='showDetails(" + app.games[index].GameID + ")'>" +
                "<p class='gameName'>" + app.games[index].Name + "</p>" +
                "<img class='cover lazy' data-src='" + app.games[index].LogoURL + "' data-game='" + app.games[index].FriendlyName + "' alt='logo' /img>" +
                "</span>"
            );
        }
    }

    var wrapper = document.createElement('div');
    wrapper.innerHTML = items.join("");

    var main = document.querySelector('div.main_div');
    main.appendChild(wrapper);

    $('.lazy').Lazy();
};

app.renderizeDetails = function (gameName) {

    // let gameCopies = app.games.filter(function (g) {
    //     return g.name == gameName;
    // });

    //Montando elementos HTML
    // var items = [];
    // for (var index in gameCopies) {
    //     var game = gameCopies[index];
    //     items.push(
    //         "<p>" + game.system + " / " + game.store + "</p>"
    //     );
    // }

    // let wrapper = document.createElement('div');
    // wrapper.innerHTML = items.join("");

    // let main = document.querySelector('.modal-body');
    // main.innerHTML = "";

    // let steamAppID = getSteamAppID(gameCopies);

    // if (steamAppID != undefined) {
    //     let steamLink = "https://store.steampowered.com/app/" + steamAppID;

    //     if (steamLink != undefined) {
    //         let link = document.createElement('a');
    //         link.href = steamLink;
    //         link.innerHTML = "Link do Steam";
    //         link.target = "_blank";
    //         wrapper.appendChild(link);
    //     }
    // }

    // main.appendChild(wrapper);
};

window.onload = function () {
    app.getGames();
}

// function getSteamAppID(gameCopies) {
//     for (var index in gameCopies) {
//         var game = gameCopies[index];
//         if (game.appID != "")
//             return game.appID;
//     }
// }

function navigateToGame() {
    var pesquisa = $('#procurar').val();
    var jogo = document.getElementsByName(pesquisa);

    if (jogo !== null) {
        $('html, body').animate({
            scrollTop: $(jogo).offset().top - 35
        }, 1000);
    }
}

/* When the user scrolls down, hide the navbar. When the user scrolls up, show the navbar */
var prevScrollpos = window.pageYOffset;

window.onscroll = function () {
    var currentScrollPos = window.pageYOffset;

    if (prevScrollpos > currentScrollPos) {
        document.getElementById("navbar").style.top = "0";
        document.getElementById("main_div").style.marginTop = "64px";
    } else {
        document.getElementById("navbar").style.top = "-64px";
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
        source: app.games.FriendlyName,
        select: function (event, ui) {
            event.preventDefault();
            $('#procurar').val(ui.item.value);
            navigateToGame();
        }
    });
});

function showDetails(gameName) {
    //app.renderizeDetails(gameName);
    $('.modal-title').text(gameName);
    $("#modal").modal('show');
}

function renderizeGeneralDetails() {

    let texto = document.createElement('p');
    texto.innerHTML = "Em contrução";
    let wrapper = document.createElement('div');
    wrapper.appendChild(texto);
    
    let main = document.querySelector('.modal-body');
    main.innerHTML = "";

    main.appendChild(wrapper);

    $('.modal-title').text("Detalhes");
    $("#modal").modal('show');
};

$(window).scroll(function () {
    if ($(this).scrollTop() > 64) {
        $('.scrolltop:hidden').stop(true, true).fadeIn();
    } else {
        $('.scrolltop').stop(true, true).fadeOut();
    }
});
$(function () { $(".scroll").click(function () { $("html,body").animate({ scrollTop: "64" }, "1000"); return false }) })

