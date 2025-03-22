if ('serviceWorker' in navigator) {
    //Add this below content to your HTML page, or add the js file to your page at the very top to register service worker
    if (navigator.serviceWorker.controller) {
        console.log('Service worker ativo encontrando')
    } else {
        //Register the ServiceWorker
        window.addEventListener('load', function () {
            navigator.serviceWorker.register('service-worker.js', {
                scope: './'
            }).then(function (reg) {
                console.log('Service worker registrado para o escopo:' + reg.scope);
            });
        });
    }
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