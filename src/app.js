import {
    publishEvent,
    subscribeState,
    bridgeReceiveIntegerFromNative,
    bridgeReceiveBooleanFromNative,
    bridgeReceiveStringFromNative,
    bridgeReceiveObjectFromNative
} from '@crestron/ch5-crcomlib/build_bundles/cjs/cr-com-lib';

window.bridgeReceiveIntegerFromNative = bridgeReceiveIntegerFromNative;
window.bridgeReceiveBooleanFromNative = bridgeReceiveBooleanFromNative;
window.bridgeReceiveStringFromNative = bridgeReceiveStringFromNative;
window.bridgeReceiveObjectFromNative = bridgeReceiveObjectFromNative;

