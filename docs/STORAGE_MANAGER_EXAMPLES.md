# StorageManager Usage Examples

## Quick Start

### 1. Store and Retrieve Data

```javascript
// Store a simple string
StorageManager.set('username', 'john_doe', {
    type: StorageManager.TYPES.LOCAL
});

// Retrieve it
const username = StorageManager.get('username', {
    type: StorageManager.TYPES.LOCAL
});
console.log(username); // 'john_doe'
```

### 2. Store Complex Objects

```javascript
const userData = {
    id: 123,
    name: 'John Doe',
    email: 'john@example.com',
    preferences: {
        theme: 'dark',
        notifications: true
    }
};

// Store object (automatically JSON stringified)
StorageManager.set('userData', userData, {
    type: StorageManager.TYPES.LOCAL,
    expirationDays: 30
});

// Retrieve (automatically parsed)
const retrieved = StorageManager.get('userData', {
    type: StorageManager.TYPES.LOCAL
});
console.log(retrieved.preferences.theme); // 'dark'
```

### 3. Temporary Session Data

```javascript
// Store data that persists only for the current session
StorageManager.set('cartItems', [
    { id: 1, quantity: 2 },
    { id: 2, quantity: 1 }
], {
    type: StorageManager.TYPES.SESSION
});

// Retrieve session data
const cart = StorageManager.get('cartItems', {
    type: StorageManager.TYPES.SESSION
});
```

### 4. Cookie with Expiration

```javascript
// Store cookie that expires in 7 days
StorageManager.set('authToken', 'xyz123', {
    type: StorageManager.TYPES.COOKIE,
    expirationDays: 7,
    secure: false,
    sameSite: 'Lax'
});

// Retrieve cookie
const token = StorageManager.get('authToken', {
    type: StorageManager.TYPES.COOKIE
});
```

## Advanced Examples

### 5. User Preferences Management

```javascript
// Complete user preferences
const userPrefs = {
    theme: 'dark',
    language: 'es',
    fontSize: 'large',
    emailNotifications: true,
    dataRetention: '90days',
    syncPreferences: true
};

// Save using convenience method
StorageManager.setUserPreferences(userPrefs);

// Retrieve using convenience method
const prefs = StorageManager.getUserPreferences();

// Access specific preference
if (prefs.theme === 'dark') {
    document.documentElement.classList.add('dark-mode');
}
```

### 6. Session Data Management

```javascript
// Store session metadata
const sessionData = {
    sessionId: 'sess_' + Date.now(),
    startTime: new Date().toISOString(),
    viewedPages: [],
    interactions: 0
};

StorageManager.setSessionData(sessionData);

// Later, retrieve and update
const session = StorageManager.getSessionData();
session.interactions++;
session.viewedPages.push('/blog/post-1');
StorageManager.setSessionData(session); // Update
```

### 7. Cookie Consent Management

```javascript
// User accepts certain cookie categories
StorageManager.acceptCookies(['essential', 'analytics', 'preferences']);

// Check if specific cookie type is accepted
if (StorageManager.isCategoryAccepted('analytics')) {
    // Initialize Google Analytics or similar
    initializeAnalytics();
}

if (StorageManager.isCategoryAccepted('marketing')) {
    // Initialize marketing scripts
    initializeMarketing();
}

// Get full consent record
const consent = StorageManager.getCookieConsent();
console.log(consent);
// {
//   accepted: true,
//   categories: ['essential', 'analytics', 'preferences'],
//   timestamp: '2026-06-02T10:30:00.000Z'
// }
```

### 8. Check Storage Availability

```javascript
// Initialize and check what's available
const availability = StorageManager.init();

if (availability.localStorage) {
    StorageManager.set('preference', value, { type: StorageManager.TYPES.LOCAL });
} else if (availability.sessionStorage) {
    StorageManager.set('preference', value, { type: StorageManager.TYPES.SESSION });
} else if (availability.cookies) {
    StorageManager.set('preference', value, { type: StorageManager.TYPES.COOKIE });
} else {
    console.error('No storage available - use in-memory only');
}
```

### 9. Conditional Storage Based on Preference

```javascript
// Get user preference for data retention
const prefs = StorageManager.getUserPreferences();

let storageConfig = {};
if (prefs.dataRetention === 'session') {
    storageConfig.type = StorageManager.TYPES.SESSION;
} else if (prefs.dataRetention === '30days') {
    storageConfig = { type: StorageManager.TYPES.LOCAL, expirationDays: 30 };
} else if (prefs.dataRetention === 'forever') {
    storageConfig = { type: StorageManager.TYPES.LOCAL, expirationDays: 365 };
}

StorageManager.set('userHistory', historyData, storageConfig);
```

### 10. Clear Specific Item

```javascript
// Remove a specific item
StorageManager.remove('tempToken', {
    type: StorageManager.TYPES.COOKIE
});

// Or from localStorage
StorageManager.remove('oldPreference', {
    type: StorageManager.TYPES.LOCAL
});
```

### 11. Check if Key Exists

```javascript
// Check before retrieving
if (StorageManager.exists('username', { type: StorageManager.TYPES.LOCAL })) {
    const username = StorageManager.get('username', { type: StorageManager.TYPES.LOCAL });
} else {
    console.log('Username not stored');
}
```

### 12. Get All Keys

```javascript
// Get all localStorage keys
const keys = StorageManager.keys(StorageManager.TYPES.LOCAL);
console.log(keys);
// ['theme', 'language', 'userId', 'preferences']

// Get all sessionStorage keys
const sessionKeys = StorageManager.keys(StorageManager.TYPES.SESSION);
```

### 13. Clear All Storage of Type

```javascript
// Clear all localStorage
StorageManager.clear(StorageManager.TYPES.LOCAL);

// Clear all sessionStorage (usually on logout)
StorageManager.clear(StorageManager.TYPES.SESSION);
```

## Real-World Scenarios

### Scenario 1: Theme Management

```javascript
// On theme toggle button click
document.getElementById('themeToggle').addEventListener('click', function() {
    const currentTheme = StorageManager.get('theme', { type: StorageManager.TYPES.LOCAL }) || 'light';
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';
    
    // Save preference
    StorageManager.set('theme', newTheme, {
        type: StorageManager.TYPES.LOCAL,
        expirationDays: 365
    });
    
    // Apply theme
    document.documentElement.setAttribute('data-theme', newTheme);
});

// On page load, restore theme
document.addEventListener('DOMContentLoaded', function() {
    const savedTheme = StorageManager.get('theme', { type: StorageManager.TYPES.LOCAL });
    if (savedTheme) {
        document.documentElement.setAttribute('data-theme', savedTheme);
    }
});
```

### Scenario 2: Shopping Cart

```javascript
// Add to cart
function addToCart(productId, quantity) {
    let cart = StorageManager.get('cart', { type: StorageManager.TYPES.LOCAL }) || [];
    
    const existingItem = cart.find(item => item.id === productId);
    if (existingItem) {
        existingItem.quantity += quantity;
    } else {
        cart.push({ id: productId, quantity });
    }
    
    StorageManager.set('cart', cart, { type: StorageManager.TYPES.LOCAL });
    updateCartUI();
}

// Retrieve cart
function getCart() {
    return StorageManager.get('cart', { type: StorageManager.TYPES.LOCAL }) || [];
}

// Clear cart (on checkout)
function clearCart() {
    StorageManager.remove('cart', { type: StorageManager.TYPES.LOCAL });
}
```

### Scenario 3: User Analytics

```javascript
// Track user interactions with consent check
function trackPageView(pageUrl) {
    // Only track if user accepted analytics cookies
    if (!StorageManager.isCategoryAccepted('analytics')) {
        return;
    }
    
    const analytics = StorageManager.get('analytics', { type: StorageManager.TYPES.LOCAL }) || {};
    
    if (!analytics.pageViews) {
        analytics.pageViews = [];
    }
    
    analytics.pageViews.push({
        url: pageUrl,
        timestamp: new Date().toISOString()
    });
    
    StorageManager.set('analytics', analytics, {
        type: StorageManager.TYPES.LOCAL,
        expirationDays: 30
    });
}
```

### Scenario 4: Form Auto-Save

```javascript
// Auto-save form data every 10 seconds
const form = document.getElementById('myForm');
const formInputs = form.querySelectorAll('input, textarea, select');

setInterval(function() {
    const formData = {};
    formInputs.forEach(input => {
        formData[input.name] = input.value;
    });
    
    StorageManager.set('formDraft', formData, {
        type: StorageManager.TYPES.SESSION
    });
}, 10000);

// Restore form on load
document.addEventListener('DOMContentLoaded', function() {
    const savedData = StorageManager.get('formDraft', { type: StorageManager.TYPES.SESSION });
    if (savedData) {
        Object.keys(savedData).forEach(key => {
            const input = form.elements[key];
            if (input) input.value = savedData[key];
        });
    }
});

// Clear draft on successful submit
form.addEventListener('submit', function(e) {
    e.preventDefault();
    // Submit form...
    StorageManager.remove('formDraft', { type: StorageManager.TYPES.SESSION });
});
```

### Scenario 5: Recently Viewed Items

```javascript
// Track recently viewed blog posts
function trackBlogView(postId, postTitle) {
    let viewed = StorageManager.get('recentlyViewed', { type: StorageManager.TYPES.LOCAL }) || [];
    
    // Remove if already in list (to move to front)
    viewed = viewed.filter(item => item.id !== postId);
    
    // Add to front
    viewed.unshift({
        id: postId,
        title: postTitle,
        viewedAt: new Date().toISOString()
    });
    
    // Keep only last 10
    viewed = viewed.slice(0, 10);
    
    StorageManager.set('recentlyViewed', viewed, {
        type: StorageManager.TYPES.LOCAL,
        expirationDays: 30
    });
}

// Display recently viewed
function displayRecentlyViewed() {
    const viewed = StorageManager.get('recentlyViewed', { type: StorageManager.TYPES.LOCAL }) || [];
    return viewed.slice(0, 5);
}
```

## Event Handling

### Listen for Cookie Consent

```javascript
window.addEventListener('cookieConsent', function(e) {
    const { categories, action } = e.detail;
    console.log(`User ${action}: ${categories.join(', ')}`);
    
    // Initialize appropriate scripts based on acceptance
    if (categories.includes('analytics')) {
        loadAnalyticsScript();
    }
    if (categories.includes('marketing')) {
        loadMarketingScript();
    }
});
```

### Listen for Preferences Changes

```javascript
window.addEventListener('preferencesApplied', function(e) {
    const prefs = e.detail;
    
    // Apply font size
    const fontSizeMap = {
        'small': '12px',
        'medium': '14px',
        'large': '16px',
        'xlarge': '18px'
    };
    
    if (fontSizeMap[prefs.fontSize]) {
        document.documentElement.style.fontSize = fontSizeMap[prefs.fontSize];
    }
});
```

## Error Handling

```javascript
// Safe wrapper with error handling
function safeStorageGet(key, defaultValue = null) {
    try {
        return StorageManager.get(key, { type: StorageManager.TYPES.LOCAL }) || defaultValue;
    } catch (error) {
        console.error('Storage read error:', error);
        return defaultValue;
    }
}

function safeStorageSet(key, value) {
    try {
        StorageManager.set(key, value, { type: StorageManager.TYPES.LOCAL });
        return true;
    } catch (error) {
        console.error('Storage write error:', error);
        return false;
    }
}
```

## Performance Tips

1. **Batch updates**: Combine multiple storage operations
   ```javascript
   const data = {
       setting1: value1,
       setting2: value2,
       setting3: value3
   };
   StorageManager.set('settings', data, { type: StorageManager.TYPES.LOCAL });
   ```

2. **Check availability first**: Don't waste time on unavailable storage
   ```javascript
   const availability = StorageManager.init();
   if (!availability.localStorage) return;
   ```

3. **Use appropriate expiration**: Don't set unnecessary long expirations
   ```javascript
   // Temporary data: 1 day
   // User preferences: 30-365 days
   // Analytics: 90 days
   ```

4. **Serialize only when needed**: Use `parseJson: false` for raw strings
   ```javascript
   const raw = StorageManager.get('key', {}, false);
   ```
