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

const moment = require('moment');

CrComLib.subscribeState('s', '1', (value) => {
    const elem = document.getElementById('room-name');
    elem.innerHTML = value;
});

var activeCard = document.getElementById('card-welcome');
var prevCard;

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

        prevCard = activeCard;
        activeCard = document.getElementById(nextCard);
        activeCard.classList.add('active');

        const name = activeCard.id.substring(activeCard.id.indexOf('-') + 1);
        const bottom = document.getElementById('bottom');
        Array.from(bottom.getElementsByTagName('BUTTON')).forEach((btn) => {
            if (btn.id == `btn-${name}`) {
                btn.classList.add('active');
            }
            else {
                btn.classList.remove('active');
            }
        });
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

function previousCard() {
    if (prevCard !== undefined) {
        showCard(prevCard.id);
        prevCard = undefined;
    }
}

const btnShutdownShutdown = document.getElementById('btn-shutdown-shutdown');
btnShutdownShutdown.addEventListener('click', (e) => {
    showCard('card-welcome');
    routeSource(0);
});

const btnSettingsApply = document.getElementById('btn-settings-apply');
btnSettingsApply.addEventListener('click', (e) => previousCard());

const btnsCancel = Array.from(document.getElementsByClassName('cancel'))
btnsCancel.forEach((btn) => {
    btn.addEventListener('click', (e) => previousCard());
});

const lblTime = document.getElementById('time');
const lblDate = document.getElementById('date');

setInterval(() => {
    lblTime.innerText = moment().format('h:mm A');
    lblDate.innerText = moment().format('dddd, MMMM Do, YYYY');
}, 5000);