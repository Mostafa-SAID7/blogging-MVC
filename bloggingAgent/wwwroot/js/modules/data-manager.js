/**
 * Data Manager Module
 * Centralized data utilities: formatting, storage, export
 * Usage: DataManager.formatNumber(), DataManager.saveToStorage(), etc.
 */

const DataManager = (function () {
    'use strict';

    /**
     * Format number with thousand separators
     * @param {number} num - Number to format
     * @return {string} Formatted number
     */
    function formatNumber(num) {
        try {
            return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        } catch (err) {
            console.error('Error formatting number:', err);
            return num;
        }
    }

    /**
     * Format file size bytes to human readable format
     * @param {number} bytes - Bytes value
     * @return {string} Formatted file size
     */
    function formatFileSize(bytes) {
        try {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const sizes = ['Bytes', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
        } catch (err) {
            console.error('Error formatting file size:', err);
            return 'Unknown';
        }
    }

    /**
     * Format date/time
     * @param {Date|string} date - Date to format
     * @param {string} format - Format string
     * @return {string} Formatted date
     */
    function formatDate(date, format = 'MM/DD/YYYY') {
        try {
            const d = new Date(date);
            const month = String(d.getMonth() + 1).padStart(2, '0');
            const day = String(d.getDate()).padStart(2, '0');
            const year = d.getFullYear();
            const hours = String(d.getHours()).padStart(2, '0');
            const minutes = String(d.getMinutes()).padStart(2, '0');
            const seconds = String(d.getSeconds()).padStart(2, '0');

            return format
                .replace('MM', month)
                .replace('DD', day)
                .replace('YYYY', year)
                .replace('HH', hours)
                .replace('mm', minutes)
                .replace('ss', seconds);
        } catch (err) {
            console.error('Error formatting date:', err);
            return date;
        }
    }

    /**
     * Generate random data array
     * @param {number} count - Number of data points
     * @param {number} min - Minimum value
     * @param {number} max - Maximum value
     * @return {array} Random data array
     */
    function generateRandomData(count, min, max) {
        try {
            const data = [];
            for (let i = 0; i < count; i++) {
                data.push(Math.floor(Math.random() * (max - min + 1)) + min);
            }
            return data;
        } catch (err) {
            console.error('Error generating random data:', err);
            return [];
        }
    }

    /**
     * Get last N days as date array
     * @param {number} days - Number of days
     * @return {array} Array of dates
     */
    function getLastNDays(days) {
        try {
            const dates = [];
            for (let i = days - 1; i >= 0; i--) {
                const date = new Date();
                date.setDate(date.getDate() - i);
                dates.push(date.toLocaleDateString());
            }
            return dates;
        } catch (err) {
            console.error('Error getting last N days:', err);
            return [];
        }
    }

    /**
     * Save data to localStorage
     * @param {string} key - Storage key
     * @param {*} value - Value to store
     * @return {boolean} Success status
     */
    function saveToStorage(key, value) {
        try {
            if (typeof localStorage !== 'undefined') {
                localStorage.setItem(key, JSON.stringify(value));
                return true;
            }
        } catch (err) {
            console.error('Error saving to storage:', err);
        }
        return false;
    }

    /**
     * Load data from localStorage
     * @param {string} key - Storage key
     * @param {*} defaultValue - Default value if not found
     * @return {*} Retrieved value
     */
    function loadFromStorage(key, defaultValue = null) {
        try {
            if (typeof localStorage !== 'undefined') {
                const item = localStorage.getItem(key);
                return item ? JSON.parse(item) : defaultValue;
            }
        } catch (err) {
            console.error('Error loading from storage:', err);
        }
        return defaultValue;
    }

    /**
     * Remove from localStorage
     * @param {string} key - Storage key
     * @return {boolean} Success status
     */
    function removeFromStorage(key) {
        try {
            if (typeof localStorage !== 'undefined') {
                localStorage.removeItem(key);
                return true;
            }
        } catch (err) {
            console.error('Error removing from storage:', err);
        }
        return false;
    }

    /**
     * Clear all localStorage data
     * @return {boolean} Success status
     */
    function clearStorage() {
        try {
            if (typeof localStorage !== 'undefined') {
                localStorage.clear();
                return true;
            }
        } catch (err) {
            console.error('Error clearing storage:', err);
        }
        return false;
    }

    /**
     * Export data as JSON file
     * @param {*} data - Data to export
     * @param {string} filename - File name
     */
    function exportAsJSON(data, filename = 'export.json') {
        try {
            const dataStr = JSON.stringify(data, null, 2);
            const dataBlob = new Blob([dataStr], { type: 'application/json' });
            downloadFile(dataBlob, filename);
        } catch (err) {
            console.error('Error exporting as JSON:', err);
        }
    }

    /**
     * Export data as CSV file
     * @param {array} data - Array of objects
     * @param {string} filename - File name
     */
    function exportAsCSV(data, filename = 'export.csv') {
        try {
            if (!Array.isArray(data) || data.length === 0) {
                console.warn('Invalid data for CSV export');
                return;
            }

            // Get headers
            const headers = Object.keys(data[0]);
            
            // Create CSV content
            let csv = headers.join(',') + '\n';
            data.forEach(row => {
                const values = headers.map(header => {
                    const value = row[header];
                    // Escape quotes and wrap in quotes if contains comma
                    const escaped = String(value).replace(/"/g, '""');
                    return escaped.includes(',') ? `"${escaped}"` : escaped;
                });
                csv += values.join(',') + '\n';
            });

            const dataBlob = new Blob([csv], { type: 'text/csv' });
            downloadFile(dataBlob, filename);
        } catch (err) {
            console.error('Error exporting as CSV:', err);
        }
    }

    /**
     * Download file
     * @param {Blob} blob - File blob
     * @param {string} filename - File name
     */
    function downloadFile(blob, filename) {
        try {
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = filename;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        } catch (err) {
            console.error('Error downloading file:', err);
        }
    }

    /**
     * Debounce function
     * @param {function} func - Function to debounce
     * @param {number} wait - Wait time in ms
     * @return {function} Debounced function
     */
    function debounce(func, wait) {
        try {
            let timeout;
            return function executedFunction(...args) {
                try {
                    const later = () => {
                        try {
                            clearTimeout(timeout);
                            func(...args);
                        } catch (err) {
                            console.error('Error in debounced function execution:', err);
                        }
                    };
                    clearTimeout(timeout);
                    timeout = setTimeout(later, wait);
                } catch (err) {
                    console.error('Error in debounce wrapper:', err);
                }
            };
        } catch (err) {
            console.error('Error creating debounce function:', err);
            return func;
        }
    }

    /**
     * Throttle function
     * @param {function} func - Function to throttle
     * @param {number} limit - Limit time in ms
     * @return {function} Throttled function
     */
    function throttle(func, limit) {
        try {
            let inThrottle;
            return function () {
                try {
                    const args = arguments;
                    const context = this;
                    if (!inThrottle) {
                        func.apply(context, args);
                        inThrottle = true;
                        setTimeout(() => inThrottle = false, limit);
                    }
                } catch (err) {
                    console.error('Error in throttled function execution:', err);
                }
            };
        } catch (err) {
            console.error('Error creating throttle function:', err);
            return func;
        }
    }

    // Public API
    return {
        formatNumber,
        formatFileSize,
        formatDate,
        generateRandomData,
        getLastNDays,
        saveToStorage,
        loadFromStorage,
        removeFromStorage,
        clearStorage,
        exportAsJSON,
        exportAsCSV,
        downloadFile,
        debounce,
        throttle
    };
})();

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = DataManager;
}
