// BloggingAgent Site JavaScript

(function ($) {
    "use strict";

    // Configuration
    const CONFIG = {
        ALERT_DURATION: 5000,
        FEEDBACK_DURATION: 2000,
        SEARCH_DEBOUNCE: 300
    };

    // Initialize when document is ready
    $(document).ready(function () {
        initializeComponents();
        setupEventHandlers();
    });

    function initializeComponents() {
        try {
            initializeBootstrapComponents();
            initializeAlerts();
        } catch (err) {
            console.error('Error initializing components:', err);
        }
    }

    function initializeBootstrapComponents() {
        try {
            // Initialize tooltips and popovers
            [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]')).forEach(el => {
                new bootstrap.Tooltip(el);
            });

            [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]')).forEach(el => {
                new bootstrap.Popover(el);
            });
        } catch (err) {
            console.error('Error initializing Bootstrap components:', err);
        }
    }

    function initializeAlerts() {
        try {
            $('.alert').each(function () {
                autoHideAlert($(this));
            });
        } catch (err) {
            console.error('Error initializing alerts:', err);
        }
    }

    function autoHideAlert($element) {
        try {
            setTimeout(function () {
                try {
                    $element.fadeOut('slow', function () {
                        $(this).remove();
                    });
                } catch (err) {
                    console.error('Error fading out alert:', err);
                }
            }, CONFIG.ALERT_DURATION);
        } catch (err) {
            console.error('Error in autoHideAlert:', err);
        }
    }

    function setupEventHandlers() {
        try {
            setupFormValidation();
            setupSearch();
            setupTagFiltering();
            setupCopyToClipboard();
            setupConfirmActions();
        } catch (err) {
            console.error('Error setting up event handlers:', err);
        }
    }

    function setupFormValidation() {
        try {
            $('form').on('submit', function (e) {
                try {
                    const $form = $(this);
                    if (!this.checkValidity()) {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                    $form.addClass('was-validated');
                } catch (err) {
                    console.error('Error in form validation:', err);
                    e.preventDefault();
                }
            });
        } catch (err) {
            console.error('Error setting up form validation:', err);
        }
    }

    function setupSearch() {
        try {
            const searchInputs = $('input[type="search"], input[name="searchQuery"]');

            searchInputs.on('input', DataManager.debounce(function () {
                try {
                    const $input = $(this);
                    const hasQuery = $input.val().trim().length > 0;
                    
                    UIManager.setLoadingState($input, hasQuery);
                    // Trigger search if needed
                } catch (err) {
                    console.error('Error in search handler:', err);
                }
            }, 300));
        } catch (err) {
            console.error('Error setting up search:', err);
        }
    }

    function setupTagFiltering() {
        try {
            $('.tag-filter').on('click', function (e) {
                try {
                    e.preventDefault();
                    const tag = $(this).data('tag');

                    // Update URL without page reload
                    const url = new URL(window.location);
                    if (tag) {
                        url.searchParams.set('tag', tag);
                    } else {
                        url.searchParams.delete('tag');
                    }

                    window.location.href = url.toString();
                } catch (err) {
                    console.error('Error in tag filter click handler:', err);
                }
            });
        } catch (err) {
            console.error('Error setting up tag filtering:', err);
        }
    }

    function setupCopyToClipboard() {
        try {
            $('.copy-to-clipboard').on('click', function (e) {
                try {
                    e.preventDefault();

                    const textToCopy = $(this).data('clipboard-text') || $(this).text();
                    const button = $(this);

                    copyToClipboard(textToCopy, button);
                } catch (err) {
                    console.error('Error in copy to clipboard click handler:', err);
                }
            });
        } catch (err) {
            console.error('Error setting up copy to clipboard:', err);
        }
    }

    function copyToClipboard(text, $button) {
        try {
            // Modern Clipboard API - primary method
            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(text)
                    .then(() => UIManager.showFeedback($button, 'Copied!', false))
                    .catch(err => {
                        console.error('Clipboard API failed:', err);
                        copyToClipboardFallback(text, $button);
                    });
            } else {
                // Fallback for non-secure contexts (HTTP, not HTTPS)
                copyToClipboardFallback(text, $button);
            }
        } catch (err) {
            console.error('Error in copyToClipboard:', err);
            UIManager.showFeedback($button, 'Failed to copy', true);
        }
    }

    function copyToClipboardFallback(text, $button) {
        try {
            const textarea = document.createElement('textarea');
            Object.assign(textarea.style, {
                position: 'fixed',
                top: '0',
                left: '0',
                opacity: '0',
                pointerEvents: 'none'
            });
            
            textarea.value = text;
            document.body.appendChild(textarea);

            try {
                textarea.focus();
                textarea.select();

                if (navigator.clipboard && navigator.clipboard.writeText) {
                    navigator.clipboard.writeText(text)
                        .then(() => UIManager.showFeedback($button, 'Copied!', false))
                        .catch(() => UIManager.showFeedback($button, 'Failed to copy', true))
                        .finally(() => UIManager.removeElement(textarea));
                } else {
                    UIManager.showFeedback($button, 'Please copy manually', true);
                    UIManager.removeElement(textarea);
                }
            } catch (err) {
                console.error('Error in fallback copy method:', err);
                UIManager.showFeedback($button, 'Failed to copy', true);
                UIManager.removeElement(textarea);
            }
        } catch (err) {
            console.error('Error in copyToClipboardFallback:', err);
            UIManager.showFeedback($button, 'Failed to copy', true);
        }
    }

    // NOTE: removeElement and showFeedback have been moved to UIManager module
    // Use: UIManager.showFeedback() and UIManager.removeElement()
    // This preserves backward compatibility for any references to these functions

    function setupConfirmActions() {
        try {
            $('.confirm-action').on('click', function (e) {
                try {
                    const message = $(this).data('confirm-message') || 'Are you sure you want to proceed?';
                    if (!confirm(message)) {
                        e.preventDefault();
                        return false;
                    }
                } catch (err) {
                    console.error('Error in confirm action handler:', err);
                    e.preventDefault();
                }
            });
        } catch (err) {
            console.error('Error setting up confirm actions:', err);
        }
    }

    // Utility functions
    window.BloggingAgent = {
        // NOTE: formatNumber and formatFileSize have been moved to DataManager
        // Use: DataManager.formatNumber() and DataManager.formatFileSize()
        
        // Show loading spinner
        showLoading: function (element) {
            try {
                UIManager.showLoading(element);
            } catch (err) {
                console.error('Error showing loading spinner:', err);
            }
        },

        // Hide loading spinner
        hideLoading: function (element, content) {
            try {
                UIManager.hideLoading(element, content);
            } catch (err) {
                console.error('Error hiding loading spinner:', err);
            }
        },

        // Unified AJAX utility
        request: function (url, options = {}) {
            const requestOptions = this._buildRequestOptions(url, options);
            
            return this._executeRequest(url, requestOptions)
                .then(response => this._handleResponse(response, options))
                .catch(error => this._handleError(error, options));
        },

        // GET request
        get: function (url, options = {}) {
            return this.request(url, { ...options, method: 'GET' });
        },

        // POST request
        post: function (url, data, options = {}) {
            return this.request(url, { ...options, method: 'POST', body: data });
        },

        // PUT request
        put: function (url, data, options = {}) {
            return this.request(url, { ...options, method: 'PUT', body: data });
        },

        // DELETE request
        delete: function (url, options = {}) {
            return this.request(url, { ...options, method: 'DELETE' });
        },

        // PATCH request
        patch: function (url, data, options = {}) {
            return this.request(url, { ...options, method: 'PATCH', body: data });
        },

        // Internal: Build request options
        _buildRequestOptions: function (url, options) {
            try {
                const headers = {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() || '',
                    ...options.headers
                };

                const requestInit = {
                    method: options.method || 'GET',
                    headers,
                    timeout: options.timeout || 30000
                };

                // Add body for non-GET requests
                if (options.body && requestInit.method !== 'GET') {
                    requestInit.body = typeof options.body === 'string' 
                        ? options.body 
                        : JSON.stringify(options.body);
                }

                // Add credentials if specified
                if (options.credentials) {
                    requestInit.credentials = options.credentials;
                }

                return requestInit;
            } catch (err) {
                console.error('Error building request options:', err);
                throw err;
            }
        },

        // Internal: Execute fetch with timeout
        _executeRequest: function (url, requestInit) {
            try {
                const timeoutMs = requestInit.timeout;
                const controller = new AbortController();
                const timeoutId = setTimeout(() => controller.abort(), timeoutMs);

                return fetch(url, { ...requestInit, signal: controller.signal })
                    .finally(() => clearTimeout(timeoutId));
            } catch (err) {
                console.error('Error executing request:', err);
                throw err;
            }
        },

        // Internal: Handle response
        _handleResponse: function (response, options) {
            try {
                // Log response for debugging
                if (options.logResponse) {
                    console.log(`Response from ${response.url}:`, response.status);
                }

                // Handle non-OK responses
                if (!response.ok) {
                    return this._handleHttpError(response, options);
                }

                // Parse response based on content type
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    return response.json();
                } else {
                    return response.text();
                }
            } catch (err) {
                console.error('Error handling response:', err);
                throw err;
            }
        },

        // Internal: Handle HTTP errors
        _handleHttpError: function (response, options) {
            try {
                const errorData = {
                    status: response.status,
                    statusText: response.statusText
                };

                // Map common HTTP status codes to user messages
                const statusMessages = {
                    400: 'Invalid request. Please check your input.',
                    401: 'Unauthorized. Please log in.',
                    403: 'Access denied. You do not have permission.',
                    404: 'Resource not found.',
                    408: 'Request timeout. Please try again.',
                    429: 'Too many requests. Please wait a moment.',
                    500: 'Server error. Please try again later.',
                    503: 'Service unavailable. Please try again later.'
                };

                const userMessage = statusMessages[response.status] || 
                    `An error occurred (${response.status}). Please try again.`;

                errorData.message = userMessage;

                // Show user-friendly error message if requested
                if (options.showError !== false) {
                    this._showErrorMessage(userMessage, response.status);
                }

                // Call error callback if provided
                if (options.onError) {
                    options.onError(errorData);
                }

                throw new Error(`HTTP ${response.status}: ${userMessage}`);
            } catch (err) {
                console.error('Error handling HTTP error:', err);
                throw err;
            }
        },

        // Internal: Handle network/other errors
        _handleError: function (error, options) {
            try {
                let errorMessage = 'An unexpected error occurred. Please try again.';
                let status = 'NETWORK_ERROR';

                // Determine error type
                if (error.name === 'AbortError') {
                    errorMessage = 'Request timeout. Please try again.';
                    status = 408;
                } else if (error instanceof TypeError) {
                    errorMessage = 'Network error. Please check your connection.';
                    status = 'NETWORK_ERROR';
                } else {
                    errorMessage = error.message || errorMessage;
                }

                const errorData = {
                    message: errorMessage,
                    status: status,
                    originalError: error
                };

                // Show user-friendly error message if requested
                if (options.showError !== false) {
                    this._showErrorMessage(errorMessage, status);
                }

                // Call error callback if provided
                if (options.onError) {
                    options.onError(errorData);
                }

                // Log error for debugging
                console.error('AJAX request error:', errorData);

                throw new Error(errorMessage);
            } catch (err) {
                console.error('Error handling request error:', err);
                throw err;
            }
        },

        // Internal: Show error message to user
        _showErrorMessage: function (message, statusCode) {
            try {
                // Create alert element
                const alertId = 'ajax-error-' + Date.now();
                const $alert = $(`
                    <div id="${alertId}" class="alert alert-danger alert-dismissible fade show" role="alert">
                        <strong>Error:</strong> ${this._escapeHtml(message)}
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    </div>
                `);

                // Insert at top of body if no container found
                const $container = $('.alerts-container').length ? $('.alerts-container') : $('body');
                $container.prepend($alert);

                // Auto-hide after configured duration
                autoHideAlert($alert);
            } catch (err) {
                console.error('Error showing error message:', err);
            }
        },

        // Internal: Escape HTML to prevent XSS
        _escapeHtml: function (text) {
            try {
                const map = {
                    '&': '&amp;',
                    '<': '&lt;',
                    '>': '&gt;',
                    '"': '&quot;',
                    "'": '&#039;'
                };
                return text.replace(/[&<>"']/g, m => map[m]);
            } catch (err) {
                console.error('Error escaping HTML:', err);
                return text;
            }
        }
    };

})(jQuery);

// NOTE: debounce and throttle functions have been moved to DataManager module
// Use: DataManager.debounce() and DataManager.throttle()
// Import data-manager.js in _Layout.cshtml to use them
