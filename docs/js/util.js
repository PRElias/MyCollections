if ('serviceWorker' in navigator) {
    window.addEventListener('load', function () {
        var hadController = Boolean(navigator.serviceWorker.controller);
        navigator.serviceWorker.register('./service-worker.js', {
            scope: './',
            updateViaCache: 'none'
        }).then(function (reg) {
            console.log('Service worker registrado para o escopo: ' + reg.scope);
            return reg.update();
        }).catch(function (error) {
            console.error('Falha ao registrar o service worker:', error);
        });

        navigator.serviceWorker.addEventListener('controllerchange', function () {
            if (hadController) {
                window.location.reload();
            }
        });
    });
}
else {
    console.log("Navegador não aceita serviceWorker");
}

function copyObject(o) {
    var output, v, key;
    output = Array.isArray(o) ? [] : {};
    for (key in o) {
        v = o[key];
        output[key] = (typeof v === "object") ? copyObject(v) : v;
    }
    return output;
}

function tryParseJSON(jsonString) {
    try {
        var o = JSON.parse(jsonString);
        if (o && typeof o === "object") {
            return o;
        }
    }
    catch (e) { }
    return false;
};