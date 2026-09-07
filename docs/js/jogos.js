var app = {
    games: [],
    tags: [],
    hiddens: []
};

var sPlataforma, total_geral, total_repetidos;
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
    app.hiddens = [];
    for (let index in app.games) {
        if (app.games[index].Disabled == false) {
            if (app.games[index].Name == lastName) {
                hidden = " hidden";
                app.hiddens.push(app.games[index]);
            }
            else {
                hidden = "";
            }

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

app.renderizeDetails = function (gameId) {
    let game = app.games.find(function (item) {
        return item.GameID === Number(gameId);
    });
    let main = document.querySelector('.modal-body');

    main.innerHTML = '';

    if (!game) {
        main.textContent = 'Jogo não encontrado.';
        return;
    }

    let copies = app.games.filter(function (item) {
        return item.Disabled === false && item.FriendlyName === game.FriendlyName;
    });

    $('.modal-title').text(game.Name);

    let cover = document.createElement('img');
    cover.src = game.LogoURL;
    cover.alt = game.Name;
    cover.className = 'cover';
    main.appendChild(cover);

    let details = document.createElement('p');
    details.textContent = copies.length > 1 ? 'Cópias: ' + getCopyDescriptions(copies).join('; ') : getCopyDescription(game);
    main.appendChild(details);

    let steamCopy = copies.find(function (item) {
        return item.SteamApID;
    });

    if (steamCopy) {
        let link = document.createElement('a');
        link.href = 'https://store.steampowered.com/app/' + steamCopy.SteamApID;
        link.textContent = 'Abrir na Steam';
        link.target = '_blank';
        link.rel = 'noopener noreferrer';
        main.appendChild(link);
    }
};

function getCopyDescriptions(copies) {
    return copies.map(getCopyDescription).filter(function (description, index, descriptions) {
        return description && descriptions.indexOf(description) === index;
    });
}

function getCopyDescription(game) {
    return [getSystemLabel(game.System), game.Store].filter(Boolean).join(' / ');
}

function getSystemLabel(system) {
    var systemLabels = {
        Android: 'Android',
        PC: 'PC',
        XBOX360: 'Xbox 360',
        XBOXOne: 'Xbox One',
        XBOXSeries: 'Xbox Series',
        PS3: 'PS3',
        PS4: 'PS4',
        PS5: 'PS5',
        Wii: 'Wii',
        Switch: 'Switch'
    };

    return systemLabels[system] || system;
}

window.onload = function () {
    app.getGames();
}

function calculaTotais() {
    sPlataforma = document.getElementById("sPlataforma");
    if (!sPlataforma) {
        return;
    }


    var totalsBySystem = {};
    app.games.forEach(function (game) {
        if (game.Disabled === false && game.System) {
            totalsBySystem[game.System] = (totalsBySystem[game.System] || 0) + 1;
        }
    });

    total_repetidos = app.hiddens.length;
    total_geral = Object.keys(totalsBySystem).reduce(function (total, system) {
        return total + totalsBySystem[system];
    }, 0);

    var selectedValues = Array.prototype.slice.call(sPlataforma.options)
        .filter(function (option) { return option.selected; })
        .map(function (option) { return option.value; });

    sPlataforma.innerHTML = "";
    sPlataforma.appendChild(createPlatformOption("", "Todos (" + total_geral + ")(" + (total_geral - total_repetidos) + " únicos)", selectedValues.indexOf("") !== -1));

    Object.keys(totalsBySystem).sort(function (a, b) {
        var labelA = getSystemLabel(a);
        var labelB = getSystemLabel(b);
        return labelA.localeCompare(labelB);
    }).forEach(function (system) {
        sPlataforma.appendChild(createPlatformOption(system, getSystemLabel(system) + " (" + totalsBySystem[system] + ")", selectedValues.indexOf(system) !== -1));
    });
}

function createPlatformOption(value, text, selected) {
    var option = document.createElement("option");
    option.value = value;
    option.text = text;
    option.selected = selected;
    return option;
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

function showDetails(gameId) {
    app.renderizeDetails(gameId);
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