/**
 * UI Manager Module
 * Centralized UI utilities: alerts, feedback, loading states, modals
 * Usage: UIManager.showAlert(), UIManager.showFeedback(), etc.
 */

const UIManager = (function () {
    'use strict';

    const CONFIG = {
        ALERT_DURATION: 5000,
        FEEDBACK_DURATION: 2000
    };

    /**
     * Show alert message to user
     * @param {string} message - Alert message
     * @param {string} type - Alert type: 'success', 'danger', 'warning', 'info'
     * @param {object} options - Additional options
     */
    function showAlert(message, type = 'info', options = {}) {
        try {
            const {
                duration = CONFIG.ALERT_DURATION,
                position = 'top-center',
                dismissible = true,
                container = '.alerts-container'
            } = options;

            const alertId = 'alert-' + Date.now();
            const dismissButton = dismissible 
                ? '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>'
                : '';

            const $alert = $(`
                <div id="${alertId}" class="alert alert-${type} alert-dismissible fade show" role="alert">
                    ${message}
                    ${dismissButton}
                </div>
            `);

            // Add position classes
            const positionClasses = {
                'top-center': 'position-fixed top-0 start-50 translate-middle-x',
                'top-right': 'position-fixed top-0 end-0',
                'top-left': 'position-fixed top-0 start-0',
                'bottom-right': 'position-fixed bottom-0 end-0',
                'bottom-left': 'position-fixed bottom-0 start-0'
            };

            $alert.addClass(positionClasses[position] || positionClasses['top-center']);
            $alert.css('z-index', 9999);

            // Add to container or body
            const $container = $(container).length ? $(container) : $('body');
            $container.append($alert);

            // Auto-dismiss if duration specified
            if (duration > 0) {
                setTimeout(() => {
                    $alert.fadeOut('slow', function () {
                        $(this).remove();
                    });
                }, duration);
            }

            return alertId;
        } catch (err) {
            console.error('Error showing alert:', err);
        }
    }

    /**
     * Show feedback message on element (e.g., button feedback)
     * @param {jQuery|HTMLElement|string} element - Element to update
     * @param {string} message - Feedback message
     * @param {boolean} isError - Whether to show error state
     * @param {object} options - Additional options
     */
    function showFeedback(element, message, isError = false, options = {}) {
        try {
            const { duration = CONFIG.FEEDBACK_DURATION } = options;
            const $element = $(element);

            const originalText = $element.html();
            const originalClass = $element.attr('class');
            const iconClass = isError ? 'fa-exclamation-triangle' : 'fa-check';
            const buttonClass = isError ? 'btn-danger' : 'btn-success';

            $element
                .html(`<i class="fas ${iconClass} me-1"></i>${message}`)
                .removeClass('btn-primary btn-secondary btn-success btn-danger')
                .addClass(buttonClass);

            setTimeout(() => {
                try {
                    $element.html(originalText).attr('class', originalClass);
                } catch (err) {
                    console.error('Error restoring element state:', err);
                }
            }, duration);
        } catch (err) {
            console.error('Error in showFeedback:', err);
        }
    }

    /**
     * Show loading spinner
     * @param {jQuery|HTMLElement|string} element - Element to update
     * @param {string} message - Optional loading message
     */
    function showLoading(element, message = 'Loading...') {
        try {
            const $element = $(element);
            const html = message 
                ? `<div class="text-center"><div class="spinner-border spinner-border-sm me-2" role="status"></div><span>${message}</span></div>`
                : '<div class="text-center"><div class="spinner-border spinner-border-sm" role="status"><span class="visually-hidden">Loading...</span></div></div>';
            
            $element.html(html);
            setLoadingState($element, true);
        } catch (err) {
            console.error('Error showing loading:', err);
        }
    }

    /**
     * Hide loading spinner
     * @param {jQuery|HTMLElement|string} element - Element to update
     * @param {string|HTMLElement} content - Content to display
     */
    function hideLoading(element, content = '') {
        try {
            const $element = $(element);
            $element.html(content);
            setLoadingState($element, false);
        } catch (err) {
            console.error('Error hiding loading:', err);
        }
    }

    /**
     * Set loading state class
     * @param {jQuery} $element - jQuery element
     * @param {boolean} isLoading - Loading state
     */
    function setLoadingState($element, isLoading) {
        try {
            if (isLoading) {
                $element.addClass('is-loading');
            } else {
                $element.removeClass('is-loading');
            }
        } catch (err) {
            console.error('Error setting loading state:', err);
        }
    }

    /**
     * Show confirmation dialog
     * @param {string} message - Confirmation message
     * @param {function} onConfirm - Callback on confirm
     * @param {function} onCancel - Callback on cancel
     */
    function showConfirm(message, onConfirm, onCancel = null) {
        try {
            const result = confirm(message);
            if (result && onConfirm) {
                onConfirm();
            } else if (!result && onCancel) {
                onCancel();
            }
            return result;
        } catch (err) {
            console.error('Error showing confirm:', err);
        }
    }

    /**
     * Show modal
     * @param {string|HTMLElement} modalId - Modal ID or element
     */
    function showModal(modalId) {
        try {
            const element = typeof modalId === 'string' 
                ? document.getElementById(modalId)
                : modalId;
            
            if (element && typeof bootstrap !== 'undefined') {
                const modal = new bootstrap.Modal(element);
                modal.show();
                return modal;
            }
        } catch (err) {
            console.error('Error showing modal:', err);
        }
    }

    /**
     * Hide modal
     * @param {string|HTMLElement|bootstrap.Modal} modal - Modal ID, element, or instance
     */
    function hideModal(modal) {
        try {
            if (typeof modal === 'string') {
                const element = document.getElementById(modal);
                const bsModal = bootstrap.Modal.getInstance(element);
                if (bsModal) bsModal.hide();
            } else if (modal instanceof bootstrap.Modal) {
                modal.hide();
            } else {
                const bsModal = bootstrap.Modal.getInstance(modal);
                if (bsModal) bsModal.hide();
            }
        } catch (err) {
            console.error('Error hiding modal:', err);
        }
    }

    /**
     * Escape HTML to prevent XSS
     * @param {string} text - Text to escape
     * @return {string} Escaped text
     */
    function escapeHtml(text) {
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

    /**
     * Remove element safely
     * @param {HTMLElement} element - Element to remove
     */
    function removeElement(element) {
        try {
            if (element && element.parentNode) {
                element.parentNode.removeChild(element);
            }
        } catch (err) {
            console.error('Error removing element:', err);
        }
    }

    // Public API
    return {
        showAlert,
        showFeedback,
        showLoading,
        hideLoading,
        setLoadingState,
        showConfirm,
        showModal,
        hideModal,
        escapeHtml,
        removeElement,
        CONFIG
    };
})();

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = UIManager;
}
