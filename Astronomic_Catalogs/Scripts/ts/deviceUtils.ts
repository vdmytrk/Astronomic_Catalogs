//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
export type DeviceType = 'mobile' | 'tablet' | 'desktop' | 'unknown';
export type PlatformType = 'Windows' | 'MacOS' | 'iOS' | 'Android' | 'Linux' | 'Other';

export class DeviceUtils {
    static isTouchDevice(): boolean {
        return 'ontouchstart' in window || navigator.maxTouchPoints > 0;
    }

    static getDeviceType(): DeviceType {
        const ua = navigator.userAgent;

        const uaData = (navigator as any).userAgentData;

        if (uaData) {
            if (uaData.mobile) {
                return 'mobile'; 
            }
            return 'desktop';
        }

        if (/Mobi|Android/i.test(ua)) return 'mobile';
        if (/iPad|Tablet/i.test(ua)) return 'tablet';

        return 'desktop';
    }

    static getDevicePlatform(): PlatformType {
        const ua = navigator.userAgent.toLowerCase();

        if (ua.includes('windows')) return 'Windows';
        if (ua.includes('mac os') || ua.includes('macintosh')) return 'MacOS';
        if (ua.includes('android')) return 'Android';
        if (ua.includes('iphone') || ua.includes('ipad')) return 'iOS';
        if (ua.includes('linux')) return 'Linux';

        return 'Other';
    }

    static isDarkMode(): boolean {
        return window.matchMedia?.('(prefers-color-scheme: dark)').matches ?? false;
    }

    static getScreenSize(): { width: number; height: number } {
        return {
            width: window.innerWidth,
            height: window.innerHeight,
        };
    }

    static isSafari(): boolean {
        return /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
    }
}