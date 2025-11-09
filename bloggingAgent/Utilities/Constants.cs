namespace BloggingAgent.Utilities
{
    public static class Constants
    {
        // User Roles
        public const string ROLE_ADMINISTRATOR = "Administrator";
        public const string ROLE_EDITOR = "Editor";
        public const string ROLE_AUTHOR = "Author";
        public const string ROLE_READER = "Reader";

        // Cache Keys
        public const string CACHE_BLOG_INDEX = "blog_index";
        public const string CACHE_BLOG_DETAILS = "blog_details";
        public const string CACHE_ANALYTICS = "analytics";
        public const string CACHE_USER_PROFILE = "user_profile";

        // Session Keys
        public const string SESSION_USER_ID = "UserId";
        public const string SESSION_USER_ROLE = "UserRole";

        // File Upload
        public const string UPLOAD_IMAGES_PATH = "uploads/images";
        public const string UPLOAD_DOCUMENTS_PATH = "uploads/documents";
        public const long MAX_FILE_SIZE = 10 * 1024 * 1024; // 10MB
        public const string ALLOWED_IMAGE_TYPES = ".jpg,.jpeg,.png,.gif,.webp";

        // Pagination
        public const int DEFAULT_PAGE_SIZE = 10;
        public const int MAX_PAGE_SIZE = 100;

        // Content Limits
        public const int MAX_POST_TITLE_LENGTH = 200;
        public const int MAX_POST_EXCERPT_LENGTH = 500;
        public const int MAX_COMMENT_LENGTH = 1000;
        public const int MAX_TAG_LENGTH = 50;

        // SEO Settings
        public const int SEO_TITLE_MAX_LENGTH = 60;
        public const int SEO_DESCRIPTION_MAX_LENGTH = 160;
        public const int SEO_KEYWORDS_MAX_LENGTH = 255;

        // Social Media
        public const int TWITTER_MAX_LENGTH = 280;
        public const int LINKEDIN_MAX_LENGTH = 3000;
        public const int FACEBOOK_MAX_LENGTH = 63206;

        // Email Templates
        public const string EMAIL_TEMPLATE_WELCOME = "WelcomeEmail";
        public const string EMAIL_TEMPLATE_PASSWORD_RESET = "PasswordReset";
        public const string EMAIL_TEMPLATE_COMMENT_NOTIFICATION = "CommentNotification";
        public const string EMAIL_TEMPLATE_POST_PUBLISHED = "PostPublished";

        // API Response Messages
        public const string MSG_SUCCESS = "Operation completed successfully";
        public const string MSG_ERROR = "An error occurred while processing your request";
        public const string MSG_UNAUTHORIZED = "You are not authorized to perform this action";
        public const string MSG_NOT_FOUND = "The requested resource was not found";
        public const string MSG_VALIDATION_ERROR = "Please check your input and try again";

        // Default Values
        public const string DEFAULT_AUTHOR = "AI Assistant";
        public const string DEFAULT_THEME = "default";
        public const int DEFAULT_POST_WORD_COUNT = 500;
        public const int DEFAULT_CACHE_EXPIRATION_MINUTES = 30;

        // Time Intervals
        public const int PASSWORD_RESET_TOKEN_EXPIRY_HOURS = 24;
        public const int EMAIL_CONFIRMATION_TOKEN_EXPIRY_HOURS = 48;
        public const int ACCOUNT_LOCKOUT_DURATION_MINUTES = 15;

        // Feature Flags
        public const bool ENABLE_SOCIAL_SHARING = true;
        public const bool ENABLE_COMMENTS = true;
        public const bool ENABLE_ANALYTICS = true;
        public const bool ENABLE_EMAIL_NOTIFICATIONS = true;

        // Database Constraints
        public const int DB_STRING_MAX_LENGTH = 4000;
        public const int DB_NAME_MAX_LENGTH = 100;
        public const int DB_EMAIL_MAX_LENGTH = 256;
        public const int DB_URL_MAX_LENGTH = 500;

        // Logging Categories
        public const string LOG_CATEGORY_SECURITY = "Security";
        public const string LOG_CATEGORY_PERFORMANCE = "Performance";
        public const string LOG_CATEGORY_BUSINESS = "Business";
        public const string LOG_CATEGORY_SYSTEM = "System";

        // Error Codes
        public const string ERROR_CODE_VALIDATION = "VALIDATION_ERROR";
        public const string ERROR_CODE_UNAUTHORIZED = "UNAUTHORIZED";
        public const string ERROR_CODE_NOT_FOUND = "NOT_FOUND";
        public const string ERROR_CODE_CONFLICT = "CONFLICT";
        public const string ERROR_CODE_SERVER_ERROR = "SERVER_ERROR";

        // Success Codes
        public const string SUCCESS_CODE_CREATED = "CREATED";
        public const string SUCCESS_CODE_UPDATED = "UPDATED";
        public const string SUCCESS_CODE_DELETED = "DELETED";
        public const string SUCCESS_CODE_OPERATION_SUCCESSFUL = "OPERATION_SUCCESSFUL";
    }
}