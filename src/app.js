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
