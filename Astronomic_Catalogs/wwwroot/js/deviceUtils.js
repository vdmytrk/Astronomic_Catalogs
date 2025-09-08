"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.DeviceUtils = void 0;
class DeviceUtils {
    static isTouchDevice() {
        return 'ontouchstart' in window || navigator.maxTouchPoints > 0;
    }
    static getDeviceType() {
        const ua = navigator.userAgent;
        const uaData = navigator.userAgentData;
        if (uaData) {
            if (uaData.mobile) {
                return 'mobile';
            }
            return 'desktop';
        }
        if (/Mobi|Android/i.test(ua))
            return 'mobile';
        if (/iPad|Tablet/i.test(ua))
            return 'tablet';
        return 'desktop';
    }
    static getDevicePlatform() {
        const ua = navigator.userAgent.toLowerCase();
        if (ua.includes('windows'))
            return 'Windows';
        if (ua.includes('mac os') || ua.includes('macintosh'))
            return 'MacOS';
        if (ua.includes('android'))
            return 'Android';
        if (ua.includes('iphone') || ua.includes('ipad'))
            return 'iOS';
        if (ua.includes('linux'))
            return 'Linux';
        return 'Other';
    }
    static isDarkMode() {
        var _a, _b;
        return (_b = (_a = window.matchMedia) === null || _a === void 0 ? void 0 : _a.call(window, '(prefers-color-scheme: dark)').matches) !== null && _b !== void 0 ? _b : false;
    }
    static getScreenSize() {
        return {
            width: window.innerWidth,
            height: window.innerHeight,
        };
    }
    static isSafari() {
        return /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
    }
}
exports.DeviceUtils = DeviceUtils;
//# sourceMappingURL=deviceUtils.js.map