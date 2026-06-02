/**
 * Storage Manager Module
 * Handles cookies, sessionStorage, and localStorage with unified interface
 * Provides secure storage management and expiration handling
 */

const StorageManager = (function() {
    'use strict';

    // Storage type constants
    const STORAGE_TYPES = {
        COOKIE: 'cookie',
        LOCAL: 'localStorage',
        SESSION: 'sessionStorage'
    };

    // Default configuration
    const DEFAULT_CONFIG = {
        type: STORAGE_TYPES.LOCAL,
        expirationDays: 30,
        secure: false,
        sameSite: 'Lax',
        path: '/'
    };

    // Private methods
    const _parseCookie = function(name) {
        const nameEQ = name + '=';
        const cookies = document.cookie.split(';');
        for (let i = 0; i < cookies.length; i++) {
            let cookie = cookies[i].trim();
            if (cookie.indexOf(nameEQ) === 0) {
                return decodeURIComponent(cookie.substring(nameEQ.length));
            }
        }
        return null;
    };

    const _setCookie = function(name, value, config = {}) {
        const finalConfig = { ...DEFAULT_CONFIG, ...config };
        
        let cookieString = `${name}=${encodeURIComponent(value)}`;
        
        if (finalConfig.expirationDays > 0) {
            const expirationDate = new Date();
            expirationDate.setDate(expirationDate.getDate() + finalConfig.expirationDays);
            cookieString += `; expires=${expirationDate.toUTCString()}`;
        }
        
        cookieString += `; path=${finalConfig.path}`;
        
        if (finalConfig.secure) {
            cookieString += '; Secure';
        }
        
        cookieString += `; SameSite=${finalConfig.sameSite}`;
        
        document.cookie = cookieString;
    };

    const _deleteCookie = function(name) {
        _setCookie(name, '', { expirationDays: -1 });
    };

    const _getFromStorage = function(storageType, key) {
        try {
            if (storageType === STORAGE_TYPES.LOCAL) {
                return localStorage.getItem(key);
            } else if (storageType === STORAGE_TYPES.SESSION) {
                return sessionStorage.getItem(key);
            }
        } catch (e) {
            console.warn(`Error accessing ${storageType}:`, e);
        }
        return null;
    };

    const _setToStorage = function(storageType, key, value) {
        try {
            if (storageType === STORAGE_TYPES.LOCAL) {
                localStorage.setItem(key, value);
            } else if (storageType === STORAGE_TYPES.SESSION) {
                sessionStorage.setItem(key, value);
            }
        } catch (e) {
            console.warn(`Error setting ${storageType}:`, e);
        }
    };

    const _removeFromStorage = function(storageType, key) {
        try {
            if (storageType === STORAGE_TYPES.LOCAL) {
                localStorage.removeItem(key);
            } else if (storageType === STORAGE_TYPES.SESSION) {
                sessionStorage.removeItem(key);
            }
        } catch (e) {
            console.warn(`Error removing from ${storageType}:`, e);
        }
    };

    // Public API
    return {
        // Storage Types
        TYPES: STORAGE_TYPES,

        /**
         * Set an item in storage
         * @param {string} key - The storage key
         * @param {*} value - The value to store (will be stringified if object)
         * @param {Object} config - Configuration (type, expirationDays, etc.)
         */
        set: function(key, value, config = {}) {
            const finalConfig = { ...DEFAULT_CONFIG, ...config };
            const stringValue = typeof value === 'string' ? value : JSON.stringify(value);
            
            if (finalConfig.type === STORAGE_TYPES.COOKIE) {
                _setCookie(key, stringValue, finalConfig);
            } else {
                _setToStorage(finalConfig.type, key, stringValue);
            }
        },

        /**
         * Get an item from storage
         * @param {string} key - The storage key
         * @param {Object} config - Configuration (type, etc.)
         * @param {boolean} parseJson - Whether to parse JSON (default: true)
         * @returns {*} The stored value or null
         */
        get: function(key, config = {}, parseJson = true) {
            const finalConfig = { ...DEFAULT_CONFIG, ...config };
            let value = null;

            if (finalConfig.type === STORAGE_TYPES.COOKIE) {
                value = _parseCookie(key);
            } else {
                value = _getFromStorage(finalConfig.type, key);
            }

            if (value && parseJson) {
                try {
                    return JSON.parse(value);
                } catch (e) {
                    return value; // Return as string if not JSON
                }
            }

            return value;
        },

        /**
         * Remove an item from storage
         * @param {string} key - The storage key
         * @param {Object} config - Configuration (type, etc.)
         */
        remove: function(key, config = {}) {
            const finalConfig = { ...DEFAULT_CONFIG, ...config };

            if (finalConfig.type === STORAGE_TYPES.COOKIE) {
                _deleteCookie(key);
            } else {
                _removeFromStorage(finalConfig.type, key);
            }
        },

        /**
         * Clear all items of a specific type
         * @param {string} type - Storage type to clear
         */
        clear: function(type = STORAGE_TYPES.LOCAL) {
            try {
                if (type === STORAGE_TYPES.LOCAL) {
                    localStorage.clear();
                } else if (type === STORAGE_TYPES.SESSION) {
                    sessionStorage.clear();
                }
            } catch (e) {
                console.warn(`Error clearing ${type}:`, e);
            }
        },

        /**
         * Check if a key exists
         * @param {string} key - The storage key
         * @param {Object} config - Configuration (type, etc.)
         * @returns {boolean} Whether the key exists
         */
        exists: function(key, config = {}) {
            const finalConfig = { ...DEFAULT_CONFIG, ...config };
            let value = null;

            if (finalConfig.type === STORAGE_TYPES.COOKIE) {
                value = _parseCookie(key);
            } else if (finalConfig.type === STORAGE_TYPES.LOCAL) {
                value = localStorage.getItem(key);
            } else if (finalConfig.type === STORAGE_TYPES.SESSION) {
                value = sessionStorage.getItem(key);
            }

            return value !== null && value !== undefined;
        },

        /**
         * Get all keys for a storage type
         * @param {string} type - Storage type
         * @returns {Array} Array of keys
         */
        keys: function(type = STORAGE_TYPES.LOCAL) {
            try {
                if (type === STORAGE_TYPES.LOCAL) {
                    return Object.keys(localStorage);
                } else if (type === STORAGE_TYPES.SESSION) {
                    return Object.keys(sessionStorage);
                }
            } catch (e) {
                console.warn(`Error getting keys from ${type}:`, e);
            }
            return [];
        },

        /**
         * Store user preferences
         * @param {Object} preferences - User preference object
         */
        setUserPreferences: function(preferences) {
            this.set('userPreferences', preferences, {
                type: STORAGE_TYPES.LOCAL,
                expirationDays: 365
            });
        },

        /**
         * Get user preferences
         * @returns {Object} User preference object
         */
        getUserPreferences: function() {
            return this.get('userPreferences', { type: STORAGE_TYPES.LOCAL }) || {};
        },

        /**
         * Store user session data
         * @param {Object} sessionData - Session data object
         */
        setSessionData: function(sessionData) {
            this.set('sessionData', sessionData, { type: STORAGE_TYPES.SESSION });
        },

        /**
         * Get user session data
         * @returns {Object} Session data object
         */
        getSessionData: function() {
            return this.get('sessionData', { type: STORAGE_TYPES.SESSION }) || {};
        },

        /**
         * Accept cookies
         * @param {Array} categories - Cookie categories to accept (analytics, marketing, etc.)
         */
        acceptCookies: function(categories = ['essential', 'analytics']) {
            const cookieConsent = {
                accepted: true,
                categories: categories,
                timestamp: new Date().toISOString()
            };
            
            this.set('cookieConsent', cookieConsent, {
                type: STORAGE_TYPES.COOKIE,
                expirationDays: 365,
                secure: false,
                sameSite: 'Lax'
            });
        },

        /**
         * Get cookie consent
         * @returns {Object} Cookie consent object
         */
        getCookieConsent: function() {
            return this.get('cookieConsent', { type: STORAGE_TYPES.COOKIE }) || null;
        },

        /**
         * Check if specific cookie category is accepted
         * @param {string} category - Cookie category
         * @returns {boolean} Whether category is accepted
         */
        isCategoryAccepted: function(category) {
            const consent = this.getCookieConsent();
            if (!consent || !consent.categories) {
                return false;
            }
            return consent.categories.includes(category);
        },

        /**
         * Initialize storage manager
         * Sets up event listeners and validates storage availability
         */
        init: function() {
            // Check storage availability
            const storageAvailability = {
                localStorage: this._isStorageAvailable(STORAGE_TYPES.LOCAL),
                sessionStorage: this._isStorageAvailable(STORAGE_TYPES.SESSION),
                cookies: this._areCookiesEnabled()
            };

            console.log('Storage Availability:', storageAvailability);
            return storageAvailability;
        },

        /**
         * Check if storage type is available
         * @private
         * @param {string} type - Storage type
         * @returns {boolean} Whether storage is available
         */
        _isStorageAvailable: function(type) {
            try {
                const storage = type === STORAGE_TYPES.LOCAL ? localStorage : sessionStorage;
                const testKey = '__storage_test__';
                storage.setItem(testKey, 'true');
                storage.removeItem(testKey);
                return true;
            } catch (e) {
                return false;
            }
        },

        /**
         * Check if cookies are enabled
         * @private
         * @returns {boolean} Whether cookies are enabled
         */
        _areCookiesEnabled: function() {
            const testCookie = '__cookies_enabled__';
            _setCookie(testCookie, 'true', { expirationDays: 0 });
            const result = _parseCookie(testCookie) === 'true';
            _deleteCookie(testCookie);
            return result;
        }
    };
})();

// Auto-initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => StorageManager.init());
} else {
    StorageManager.init();
}
