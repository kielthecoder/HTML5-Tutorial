const webXpanel = require('../node_modules/@crestron/ch5-webxpanel/dist/cjs/index.js');

const configuration = {
    host: '192.168.1.13',
    ipId: '4'
};

if (webXpanel.isActive) {
    console.log(`WebXPanel version: ${webXpanel.getVersion()}`);
    console.log(`WebXPanel build date: ${webXpanel.getBuildDate()}`);

    webXpanel.default.initialize(configuration);
}
else {
    console.log('Skipping WebXPanel since running on touchpanel');
}

const CrComLib = require('../node_modules/@crestron/ch5-crcomlib/build_bundles/cjs/cr-com-lib.js');

window.CrComLib = CrComLib;
window.bridgeReceiveIntegerFromNative = CrComLib.bridgeReceiveIntegerFromNative;
window.bridgeReceiveBooleanFromNative = CrComLib.bridgeReceiveBooleanFromNative;
window.bridgeReceiveStringFromNative = CrComLib.bridgeReceiveStringFromNative;
window.bridgeReceiveObjectFromNative = CrComLib.bridgeReceiveObjectFromNative;

CrComLib.subscribeState('s', '1', (value) => {
    const elem = document.getElementById('room-name');
    elem.innerHTML = value;
});

var activeCard = document.getElementById('card-welcome');

function showCard (nextCard) {
    if (activeCard.id != nextCard) {
        const popup = document.getElementsByClassName('popup')[0];
        activeCard.classList.remove('active');

        if (nextCard.substring(0, 6) == 'popup-') {
            popup.classList.add('active');
        }
        else {
            popup.classList.remove('active');
        }

        activeCard = document.getElementById(nextCard);
        activeCard.classList.add('active');
    }
}

function routeSource (n) {
    CrComLib.publishEvent('n', '1', n);
}

CrComLib.subscribeState('n', '1', (value) => {
    switch (value) {
        case 0:
            showCard('card-welcome');
            break;
        case 1:
            showCard('card-laptop');
            break;
        case 2:
            showCard('card-appletv');
            break;
    }
});

const btnLaptop = document.getElementById('btn-laptop');
const btnAppleTV = document.getElementById('btn-appletv');

btnLaptop.addEventListener('click', (e) => {
    showCard('card-laptop');
    routeSource(1);
});

btnAppleTV.addEventListener('click', (e) => {
    showCard('card-appletv');
    routeSource(2);
});

const btnSettings = document.getElementById('btn-settings');
const btnShutdown = document.getElementById('btn-shutdown');

btnSettings.addEventListener('click', (e) => showCard('popup-settings'));
btnShutdown.addEventListener('click', (e) => showCard('popup-shutdown'));