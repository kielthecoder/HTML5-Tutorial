const webXpanel = require('@crestron/ch5-webxpanel/dist/cjs/index.js');

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

const CrComLib = require('@crestron/ch5-crcomlib/build_bundles/cjs/cr-com-lib.js');

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

CrComLib.subscribeState('s', '2', (value) => {
    const elem = document.getElementById('help-number');
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
        case 3:
            showCard('card-pc');
            break;
    }
});

const btnPC = document.getElementById('btn-pc');

btnPC.addEventListener('click', (e) => {
    showCard('card-pc');
    routeSource(3);
})

const btnHelp = document.getElementById('btn-help');
const btnShutdown = document.getElementById('btn-shutdown');

btnHelp.addEventListener('click', (e) => showCard('popup-help'));
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