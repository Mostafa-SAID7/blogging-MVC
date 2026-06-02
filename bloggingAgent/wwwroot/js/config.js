/**
 * Centralized Application Configuration
 * All application-wide constants are defined here
 * Load this FIRST before any other scripts
 */

const AppConfig = {
    // UI Configuration
    UI: {
        ALERT_DURATION: 5000,        // Alert auto-close duration in ms
        FEEDBACK_DURATION: 2000,     // Button feedback duration in ms
        SEARCH_DEBOUNCE: 300,        // Search input debounce in ms
    },

    // Storage Configuration
    STORAGE: {
        BLOG_DRAFT_KEY: 'blogDraft',  // localStorage key for blog drafts
    },

    // Blog Generation Configuration
    BLOG_GENERATION: {
        AUTO_SAVE_DELAY: 2000,        // Auto-save draft delay in ms
        PROGRESS_UPDATE_INTERVAL: 800 // Progress bar update interval in ms
    },

    // Analytics Configuration
    ANALYTICS: {
        REFRESH_INTERVAL: 5 * 60 * 1000,  // Auto-refresh interval in ms (5 minutes)
        CHART_UPDATE_INTERVAL: 3000,      // Chart update interval in ms
        CHART_ANIMATION_DURATION: 300     // Chart animation duration in ms
    },

    // API Configuration
    API: {
        TIMEOUT: 30000,               // Default API request timeout in ms
        RETRY_ATTEMPTS: 3,            // Number of retry attempts for failed requests
        RETRY_DELAY: 1000             // Delay between retry attempts in ms
    },

    // Validation Configuration
    VALIDATION: {
        MIN_TOPIC_LENGTH: 3,          // Minimum topic name length
        MIN_WORD_COUNT: 100,          // Minimum word count for blog posts
        MAX_WORD_COUNT: 5000          // Maximum word count for blog posts
    },

    // Feature Flags
    FEATURES: {
        AUTO_SAVE_ENABLED: true,      // Enable draft auto-save
        ANALYTICS_ENABLED: true,      // Enable analytics dashboard
        EXPORT_ENABLED: true          // Enable data export functionality
    }
};

/**
 * Legacy Compatibility Layer
 * These are deprecated - use AppConfig instead
 * Maintained for backward compatibility only
 */
const CONFIG = AppConfig;

// Log initialization
console.log('✅ AppConfig initialized - All application constants loaded');
