import CrComLib from '@crestron/ch5-crcomlib/build_bundles/umd/cr-com-lib';
import WebXPanel from '@crestron/ch5-webxpanel';

const configuration = {
    host: '192.168.1.13',
    ipId: '0x04',
};

WebXPanel.initialize(configuration);

