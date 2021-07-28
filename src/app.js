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
