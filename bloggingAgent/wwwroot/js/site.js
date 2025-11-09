// BloggingAgent Site JavaScript

(function ($) {
    "use strict";

    // Initialize when document is ready
    $(document).ready(function () {
        initializeComponents();
        setupEventHandlers();
    });

    function initializeComponents() {
        // Initialize tooltips
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });

        // Initialize popovers
        var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });

        // Auto-hide alerts after 5 seconds
        setTimeout(function () {
            $('.alert').fadeOut('slow');
        }, 5000);
    }

    function setupEventHandlers() {
        // Form validation
        $('form').on('submit', function (e) {
            if (!this.checkValidity()) {
                e.preventDefault();
                e.stopPropagation();
            }
            $(this).addClass('was-validated');
        });

        // Search functionality
        setupSearch();

        // Tag filtering
        setupTagFiltering();

        // Copy to clipboard functionality
        setupCopyToClipboard();

        // Confirm delete actions
        setupConfirmActions();
    }

    function setupSearch() {
        const searchInputs = $('input[type="search"], input[name="searchQuery"]');

        searchInputs.on('input', function () {
            const query = $(this).val().trim();
            if (query.length > 0) {
                // Add loading state
                $(this).addClass('is-loading');
            } else {
                $(this).removeClass('is-loading');
            }
        });

        // Debounced search
        let searchTimeout;
        searchInputs.on('input', function () {
            clearTimeout(searchTimeout);
            const input = $(this);
            searchTimeout = setTimeout(function () {
                input.removeClass('is-loading');
                // Trigger search if needed
            }, 300);
        });
    }

    function setupTagFiltering() {
        $('.tag-filter').on('click', function (e) {
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
        });
    }

    function setupCopyToClipboard() {
        $('.copy-to-clipboard').on('click', function (e) {
            e.preventDefault();

            const textToCopy = $(this).data('clipboard-text') || $(this).text();
            const button = $(this);

            // Use modern clipboard API if available
            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(textToCopy).then(function () {
                    showCopyFeedback(button, 'Copied!');
                });
            } else {
                // Fallback for older browsers
                const textArea = document.createElement('textarea');
                textArea.value = textToCopy;
                document.body.appendChild(textArea);
                textArea.select();

                try {
                    document.execCommand('copy');
                    showCopyFeedback(button, 'Copied!');
                } catch (err) {
                    showCopyFeedback(button, 'Failed to copy', true);
                }

                document.body.removeChild(textArea);
            }
        });
    }

    function showCopyFeedback(button, message, isError = false) {
        const originalText = button.html();
        const originalClass = button.attr('class');

        button.html(`<i class="fas ${isError ? 'fa-exclamation-triangle' : 'fa-check'} me-1"></i>${message}`);
        button.removeClass('btn-primary btn-secondary').addClass(isError ? 'btn-danger' : 'btn-success');

        setTimeout(function () {
            button.html(originalText);
            button.attr('class', originalClass);
        }, 2000);
    }

    function setupConfirmActions() {
        $('.confirm-action').on('click', function (e) {
            const message = $(this).data('confirm-message') || 'Are you sure you want to proceed?';
            if (!confirm(message)) {
                e.preventDefault();
                return false;
            }
        });
    }

    // Utility functions
    window.BloggingAgent = {
        // Format numbers with commas
        formatNumber: function (num) {
            return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        },

        // Format file sizes
        formatFileSize: function (bytes) {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const sizes = ['Bytes', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
        },

        // Show loading spinner
        showLoading: function (element) {
            $(element).html('<div class="text-center"><div class="spinner-border spinner-border-sm" role="status"><span class="visually-hidden">Loading...</span></div></div>');
        },

        // Hide loading spinner
        hideLoading: function (element, content) {
            $(element).html(content);
        },

        // AJAX helper
        ajaxRequest: function (url, options) {
            const defaultOptions = {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                }
            };

            return fetch(url, Object.assign(defaultOptions, options))
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    return response.json();
                });
        }
    };

})(jQuery);

// Additional utility functions
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

function throttle(func, limit) {
    let inThrottle;
    return function () {
        const args = arguments;
        const context = this;
        if (!inThrottle) {
            func.apply(context, args);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit);
        }
    };
}

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        debounce,
        throttle
    };
}
