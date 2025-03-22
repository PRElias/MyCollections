var app = {
    games: [],
    tags: [],
    hiddens: []
};

var sPlataforma, total_android, total_pc, total_xbox360, total_xboxone, total_ps3, total_p4, total_wii, total_geral, total_repetidos;
var all = false;
var navHeight = 56;

// if ($(window).width() <= 555) {
//     navHeight = 152;
// }

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
    xobj.onloadend = function() {
        calculaTotais();
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
            app.hiddens.push(app.games[index]);
        }
        else {
            hidden = "";
        }

        if (app.games[index].Disabled == false) {
            lastName = app.games[index].Name;
            app.tags.indexOf(app.games[index].FriendlyName) === -1 ? app.tags.push(app.games[index].FriendlyName) : null;
            items.push(
                "<span " + hidden + " class='game col-lg-2 col-sm-6 col-md-6 col-xs-12 " + app.games[index].System + " " + app.games[index].Store + "' id='" + app.games[index].GameID + 
                "' name='" + app.games[index].FriendlyName +
                "' onclick='showDetails(" + app.games[index].GameID + ")'>" +
                "<p class='gameName'>" + app.games[index].Name + "</p>" +
                "<img class='cover lazy' data-src='" + app.games[index].LogoURL + "' data-game='" + app.games[index].FriendlyName + "' /img>" +
                "</span>"
            );
        }
    }

    let wrapper = document.createElement('div');
    wrapper.innerHTML = items.join("");

    let main = document.querySelector('div.main_div');
    main.appendChild(wrapper);

    $('.lazy').Lazy({
        //TODO: não funcionou o delay no lugar do scroll
        delay: 1100,
        onFinishedAll: function() {
            $("img").removeClass("lazy");
            $('#loading').hide();
        }
    });
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

function calculaTotais() {
    
    total_repetidos = app.hiddens.length;
    total_android = document.querySelectorAll('.Android').length;
    total_pc = document.querySelectorAll('.PC').length;
    total_xbox360 = document.querySelectorAll('.XBOX360').length;
    total_xboxone = document.querySelectorAll('.XBOXOne').length;
    total_ps3 = document.querySelectorAll('.PS3').length;
    total_ps4 = document.querySelectorAll('.PS4').length;
    total_wii = document.querySelectorAll('.Wii').length;
    total_geral = total_android + total_pc + total_xbox360 + total_xboxone + total_ps3 + total_ps4 + total_wii;

    sPlataforma = document.getElementById("sPlataforma");
    sPlataforma.options[0].text+= " (" + total_geral + ")(" + (total_geral - total_repetidos) + " únicos)";
    sPlataforma.options[1].text+= " (" + total_android + ")";
    sPlataforma.options[2].text+= " (" + total_pc + ")";
    sPlataforma.options[3].text+= " (" + total_xbox360 + ")";
    sPlataforma.options[4].text+= " (" + total_xboxone + ")";
    sPlataforma.options[5].text+= " (" + total_ps3 + ")";
    sPlataforma.options[6].text+= " (" + total_ps4 + ")";
    sPlataforma.options[7].text+= " (" + total_wii + ")";
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
        //descendo
        document.getElementById("navbar").style.top = "0";
        document.getElementById("main_div").style.marginTop = navHeight + "px";
    } else {
        //subindo
        $("#navbarSupportedContent").removeClass("show");
        document.getElementById("navbar").style.top =  (navHeight * -1) + "px";
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
        source: app.tags,
        select: function (event, ui) {
            event.preventDefault();
            $('#procurar').val(ui.item.value);
            all = true;
            changePlataforma();
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

function changePlataforma() {
    if (sPlataforma.options[0].selected === true) {
        all = true;
    }
    for (i = 1; i < sPlataforma.length; i++) {
        let system = "." + sPlataforma.options[i].value;
        $(system).show();
        if (all) { 
            for (h = 0; h < app.hiddens.length; h++) {
                var game = document.getElementById(app.hiddens[h].GameID);
                if (game != null){ game.hidden = true };
            }
            continue; 
        }
        else {
            for (h = 0; h < app.hiddens.length; h++) {
                var game = document.getElementById(app.hiddens[h].GameID);
                if (game != null){ game.hidden = false };
            }
        }
        if (sPlataforma.options[i].selected === false) {
            $(system).hide();
        }
    }
    all = false;
    $("#navbarSupportedContent").removeClass("show");
}