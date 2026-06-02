# Client-Side State Management Documentation

## Overview

The BloggingAgent application implements comprehensive client-side state management with integrated support for **cookies**, **sessionStorage**, and **localStorage**. This allows for persistent user preferences, session management, and cookie consent tracking.

## Components

### 1. StorageManager Module (`storage-manager.js`)

A unified JavaScript module for managing all client-side storage types with a consistent API.

#### Features

- **Unified Interface**: Same API for cookies, localStorage, and sessionStorage
- **Automatic Serialization**: JSON serialization/deserialization
- **Expiration Handling**: Support for cookie expiration and automatic cleanup
- **Storage Detection**: Automatic detection of available storage types
- **Error Handling**: Graceful fallback with console warnings

#### Storage Types

```javascript
StorageManager.TYPES = {
    COOKIE: 'cookie',
    LOCAL: 'localStorage',
    SESSION: 'sessionStorage'
};
```

#### Core Methods

```javascript
// Set a value
StorageManager.set(key, value, config);
// config: { type: 'localStorage', expirationDays: 30, secure: false }

// Get a value
const value = StorageManager.get(key, config, parseJson = true);

// Remove a value
StorageManager.remove(key, config);

// Clear all storage of a type
StorageManager.clear(type);

// Check if key exists
StorageManager.exists(key, config);

// Get all keys
const keys = StorageManager.keys(type);
```

#### Preference Management

```javascript
// Store user preferences (auto-expires in 365 days)
StorageManager.setUserPreferences(preferencesObject);

// Retrieve user preferences
const prefs = StorageManager.getUserPreferences();

// Store session data (expires with session)
StorageManager.setSessionData(sessionDataObject);

// Retrieve session data
const sessionData = StorageManager.getSessionData();
```

#### Cookie Consent

```javascript
// Accept cookies with categories
StorageManager.acceptCookies(['essential', 'analytics', 'marketing']);

// Get cookie consent record
const consent = StorageManager.getCookieConsent();

// Check if category is accepted
if (StorageManager.isCategoryAccepted('analytics')) {
    // Initialize analytics
}
```

#### Example Usage

```javascript
// Store user theme preference (persists for 30 days)
StorageManager.set('userTheme', 'dark', {
    type: StorageManager.TYPES.LOCAL,
    expirationDays: 30
});

// Retrieve and apply theme
const theme = StorageManager.get('userTheme', {
    type: StorageManager.TYPES.LOCAL
});
if (theme) {
    document.documentElement.setAttribute('data-theme', theme);
}

// Store temporary session data
StorageManager.setSessionData({
    viewedPosts: [1, 2, 3],
    lastActive: new Date().toISOString()
});
```

### 2. Cookie Consent Partial (`_CookieNotice.cshtml`)

A comprehensive cookie consent banner with category management.

#### Features

- **Sliding Banner**: Appears at bottom of page on first visit
- **Category Selection**: Users can choose which cookie categories to accept
  - Essential (required, always enabled)
  - Analytics
  - Marketing
  - Preferences
- **Expandable Details**: Learn more about each cookie category
- **Persistent Storage**: Consent preference stored in cookies for 365 days
- **Responsive Design**: Mobile-friendly layout

#### Cookie Categories

| Category | Description | Required |
|----------|-------------|----------|
| Essential | Site functionality and security | Yes |
| Analytics | Traffic analysis and user behavior | No |
| Marketing | Personalized advertising | No |
| Preferences | User preferences and settings | No |

#### Events

```javascript
// Listen for cookie consent
window.addEventListener('cookieConsent', function(e) {
    const { categories, action } = e.detail;
    // categories: array of accepted categories
    // action: 'acceptAll' | 'rejectAll'
});
```

#### Styling

The banner includes:
- Gradient purple background
- Semi-transparent overlay for details panel
- Smooth animations
- Mobile responsive layout
- Accessibility features (ARIA labels)

### 3. User Preferences Partial (`_UserPreferences.cshtml`)

A side panel for managing user preferences.

#### Features

- **Theme Selection**: Light / Dark / Auto
- **Language Selection**: Multiple language options
- **Font Size Adjustment**: Small to Extra Large
- **Notification Settings**: Email notification toggle
- **Data Retention**: How long to keep preferences
- **Cross-Device Sync**: Optional preference synchronization
- **Clear Preferences**: Reset all saved preferences

#### Preferences Object

```javascript
{
    theme: 'light' | 'dark' | 'auto',
    language: 'en' | 'es' | 'fr' | 'de' | 'it',
    fontSize: 'small' | 'medium' | 'large' | 'xlarge',
    emailNotifications: boolean,
    dataRetention: 'session' | '30days' | '90days' | 'forever',
    syncPreferences: boolean,
    lastUpdated: ISO timestamp
}
```

#### Events

```javascript
// Listen for preferences applied
window.addEventListener('preferencesApplied', function(e) {
    const preferences = e.detail;
    // Apply preferences to UI
});

// Listen for preferences saved
window.addEventListener('preferencesSaved', function(e) {
    const preferences = e.detail;
    // Update UI to reflect new preferences
});

// Listen for preferences cleared
window.addEventListener('preferencesCleared', function(e) {
    // Reset to defaults
});
```

#### Accessing Preferences Manager

```javascript
// Get current preferences
const prefs = UserPreferencesManager.getPreferences();

// Open preferences panel
UserPreferencesManager.openPanel();
```

## Backend Configuration

### Cookie Configuration (`CookieConfiguration.cs`)

```csharp
// Configured for development (relaxed security)
services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});
```

### Session Configuration (`SessionConfiguration.cs`)

```csharp
// Session timeout set to 30 minutes
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
```

## Implementation in Views

### Including Storage Manager

The storage manager is automatically loaded in `_Layout.cshtml`:

```html
<!-- Storage Manager Module -->
<script src="~/js/modules/storage-manager.js"></script>

<!-- Partial Views -->
@await Html.PartialAsync("_CookieNotice")
@await Html.PartialAsync("_UserPreferences")
```

### Preferences Button

A settings icon in the navbar opens the user preferences panel:

```html
<button class="nav-link btn btn-link" id="preferencesToggleBtn" 
    title="Open preferences">
    <i class="fas fa-sliders-h"></i>
</button>
```

## JavaScript Integration

### Initialize on Page Load

Both modules auto-initialize when the DOM is ready:

```javascript
// StorageManager
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => StorageManager.init());
} else {
    StorageManager.init();
}

// UserPreferencesManager
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => UserPreferencesManager.init());
} else {
    UserPreferencesManager.init();
}
```

### Custom Events

```javascript
// Cookie consent event
window.addEventListener('cookieConsent', (e) => {
    console.log('Cookie consent:', e.detail);
});

// Preferences applied event
window.addEventListener('preferencesApplied', (e) => {
    console.log('Preferences applied:', e.detail);
});

// Preferences saved event
window.addEventListener('preferencesSaved', (e) => {
    console.log('Preferences saved:', e.detail);
});
```

## Storage Structure

### Cookies

```javascript
// Cookie consent (expires in 365 days)
{
    accepted: true,
    categories: ['essential', 'analytics'],
    timestamp: '2026-06-02T...'
}
```

### LocalStorage

```javascript
// User preferences (persists until manually cleared)
{
    theme: 'dark',
    language: 'en',
    fontSize: 'medium',
    emailNotifications: true,
    dataRetention: '30days',
    syncPreferences: true,
    lastUpdated: '2026-06-02T...'
}
```

### SessionStorage

```javascript
// Session data (cleared on browser close)
{
    viewedPosts: [1, 2, 3],
    lastActive: '2026-06-02T...',
    sessionCart: []
}
```

## Security Considerations

### Secure Storage

⚠️ **Important**: Do NOT store sensitive data in client-side storage:
- Password tokens (use HTTP-only cookies instead)
- API keys
- Personal identification information
- Credit card information

### HTTPS in Production

Update cookie configuration for production:

```csharp
options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
options.Cookie.SameSite = SameSiteMode.Strict; // More restrictive
```

### GDPR Compliance

- Cookie consent banner displays before any tracking cookies are set
- Users can opt out of non-essential cookies
- Preferences can be cleared anytime
- Consent is logged with timestamp

## Best Practices

1. **Use appropriate storage type**:
   - Cookies: Cross-domain tracking, server-side needs
   - LocalStorage: Long-term client preferences
   - SessionStorage: Temporary data for current session

2. **Serialize complex objects**:
   ```javascript
   StorageManager.set('data', {name: 'John'}, {type: 'localStorage'});
   // Automatically serialized to JSON
   ```

3. **Handle storage errors gracefully**:
   ```javascript
   if (StorageManager.init().localStorage) {
       // Use localStorage
   } else {
       // Use in-memory fallback
   }
   ```

4. **Respect user preferences**:
   ```javascript
   if (StorageManager.isCategoryAccepted('analytics')) {
       // Initialize analytics only if user accepted
   }
   ```

5. **Clear sensitive data**:
   ```javascript
   // Clear sensitive session data on logout
   StorageManager.clear(StorageManager.TYPES.SESSION);
   ```

## Troubleshooting

### Storage Not Working

Check availability:
```javascript
const availability = StorageManager.init();
console.log(availability);
// {localStorage: true, sessionStorage: true, cookies: true}
```

### Preferences Not Persisting

Verify storage type:
```javascript
// Check if localStorage is available
if (!StorageManager._isStorageAvailable(StorageManager.TYPES.LOCAL)) {
    console.warn('localStorage not available');
}
```

### Cookie Consent Not Showing

Ensure partial is rendered:
```html
@await Html.PartialAsync("_CookieNotice")
```

Check if consent already given:
```javascript
const consent = StorageManager.getCookieConsent();
console.log('Existing consent:', consent);
```

## Future Enhancements

- [ ] IndexedDB support for larger data
- [ ] Preference synchronization across tabs
- [ ] Analytics integration with consent
- [ ] A/B testing preference management
- [ ] Dark mode auto-detection
- [ ] Accessibility preferences (high contrast, reduced motion)

## References

- [MDN: Web Storage API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Storage_API)
- [MDN: Cookies](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/API/cookies)
- [GDPR Cookie Law](https://www.gdpreu.org/the-regulation/key-concepts/cookies/)
- [Bootstrap Documentation](https://getbootstrap.com/)
