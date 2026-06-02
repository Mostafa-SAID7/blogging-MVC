/**
 * Blog Generator Module
 * Consolidated blog generation functionality
 * Dependencies: UIManager, DataManager, BloggingAgent (AJAX)
 */

const BlogGeneratorModule = (function () {
    'use strict';

    const CONFIG = {
        DRAFT_STORAGE_KEY: 'blogDraft',
        AUTO_SAVE_DELAY: 2000,
        MIN_TOPIC_LENGTH: 3,
        MIN_WORD_COUNT: 100,
        MAX_WORD_COUNT: 5000
    };

    let generationInProgress = false;
    let progressInterval;
    let autoSaveTimeout;
    let isInitialized = false;

    const PROGRESS_STEPS = [
        'Analyzing topic and keywords...',
        'Generating content structure...',
        'Writing compelling introduction...',
        'Creating main content sections...',
        'Adding examples and data...',
        'Optimizing for SEO...',
        'Finalizing and formatting...',
        'Post generation complete!'
    ];

    /**
     * Initialize blog generator module
     */
    function initialize() {
        try {
            if (isInitialized) return;

            const $form = $('#generateForm');
            if (!$form.length) {
                console.warn('Generate form not found');
                return;
            }

            setupFormSubmission($form);
            setupAutoSave($form);
            setupWordCountSuggestions();
            loadDraft($form);

            isInitialized = true;
        } catch (err) {
            console.error('Error initializing blog generator module:', err);
        }
    }

    /**
     * Setup form submission
     * @param {jQuery} $form - Form element
     */
    function setupFormSubmission($form) {
        try {
            $form.on('submit', function (e) {
                e.preventDefault();

                if (generationInProgress) {
                    UIManager.showAlert('Generation already in progress', 'warning');
                    return;
                }

                const formData = new FormData(this);
                if (!validateForm(formData)) {
                    return;
                }

                startGeneration($form, formData);
            });
        } catch (err) {
            console.error('Error setting up form submission:', err);
        }
    }

    /**
     * Validate form
     * @param {FormData} formData - Form data
     * @return {boolean} Validation result
     */
    function validateForm(formData) {
        try {
            const topic = formData.get('Topic');
            if (!topic || topic.trim().length < CONFIG.MIN_TOPIC_LENGTH) {
                UIManager.showAlert(
                    `Please enter a topic with at least ${CONFIG.MIN_TOPIC_LENGTH} characters.`,
                    'danger'
                );
                $('#Topic').focus();
                return false;
            }

            const targetWordCount = parseInt(formData.get('TargetWordCount'));
            if (targetWordCount < CONFIG.MIN_WORD_COUNT || targetWordCount > CONFIG.MAX_WORD_COUNT) {
                UIManager.showAlert(
                    `Target word count must be between ${CONFIG.MIN_WORD_COUNT} and ${CONFIG.MAX_WORD_COUNT}.`,
                    'danger'
                );
                $('#TargetWordCount').focus();
                return false;
            }

            return true;
        } catch (err) {
            console.error('Error validating form:', err);
            return false;
        }
    }

    /**
     * Start generation
     * @param {jQuery} $form - Form element
     * @param {FormData} formData - Form data
     */
    function startGeneration($form, formData) {
        try {
            generationInProgress = true;
            const $submitBtn = $form.find('button[type="submit"]');
            const $progressCard = $('#progressCard');
            const $progressBar = $('#progressBar');
            const $progressText = $('#progressText');

            // Update UI
            UIManager.showFeedback($submitBtn, 'Generating...', false);
            $progressCard.removeClass('d-none');
            $progressBar.css('width', '0%');

            // Simulate progress
            simulateProgress($progressBar, $progressText, function () {
                // Submit form
                submitGenerationRequest($form, formData, $submitBtn, $progressBar, $progressText);
            });
        } catch (err) {
            console.error('Error starting generation:', err);
            generationInProgress = false;
            UIManager.showAlert('Error starting generation', 'danger');
        }
    }

    /**
     * Simulate progress
     * @param {jQuery} $bar - Progress bar element
     * @param {jQuery} $text - Progress text element
     * @param {function} callback - Callback when progress complete
     */
    function simulateProgress($bar, $text, callback) {
        try {
            let progress = 0;

            progressInterval = setInterval(() => {
                progress += Math.random() * 12;
                if (progress > 95) progress = 95;

                $bar.css('width', progress + '%');

                const stepIndex = Math.floor((progress / 100) * (PROGRESS_STEPS.length - 1));
                $text.text(PROGRESS_STEPS[stepIndex]);

                if (progress >= 95) {
                    clearInterval(progressInterval);
                    if (callback) callback();
                }
            }, 800);
        } catch (err) {
            console.error('Error simulating progress:', err);
        }
    }

    /**
     * Submit generation request
     * @param {jQuery} $form - Form element
     * @param {FormData} formData - Form data
     * @param {jQuery} $submitBtn - Submit button
     * @param {jQuery} $progressBar - Progress bar
     * @param {jQuery} $progressText - Progress text
     */
    function submitGenerationRequest($form, formData, $submitBtn, $progressBar, $progressText) {
        try {
            const url = $form.attr('action');

            BloggingAgent.post(url, Object.fromEntries(formData), { showError: true })
                .then(response => {
                    completedGeneration($progressBar, $progressText, response);
                })
                .catch(error => {
                    failedGeneration($submitBtn, $progressBar, $progressText, error);
                });
        } catch (err) {
            console.error('Error submitting generation request:', err);
            failedGeneration($submitBtn, $progressBar, $progressText, err);
        }
    }

    /**
     * Generation completed
     * @param {jQuery} $progressBar - Progress bar
     * @param {jQuery} $progressText - Progress text
     * @param {object} response - Response data
     */
    function completedGeneration($progressBar, $progressText, response) {
        try {
            clearInterval(progressInterval);
            $progressBar.css('width', '100%');
            $progressText.text('Redirecting to your new post...');

            // Clear draft
            DataManager.removeFromStorage(CONFIG.DRAFT_STORAGE_KEY);

            // Redirect
            setTimeout(() => {
                window.location.href = response.redirectUrl || '/blog';
            }, 1500);
        } catch (err) {
            console.error('Error completing generation:', err);
        }
    }

    /**
     * Generation failed
     * @param {jQuery} $submitBtn - Submit button
     * @param {jQuery} $progressBar - Progress bar
     * @param {jQuery} $progressText - Progress text
     * @param {error} error - Error object
     */
    function failedGeneration($submitBtn, $progressBar, $progressText, error) {
        try {
            clearInterval(progressInterval);
            generationInProgress = false;

            UIManager.showFeedback($submitBtn, 'Generate Post', true);
            $progressBar.addClass('bg-danger');
            $progressText.text('Generation failed');

            UIManager.showAlert('Failed to generate post. Please try again.', 'danger');
        } catch (err) {
            console.error('Error in failed generation:', err);
        }
    }

    /**
     * Setup auto-save
     * @param {jQuery} $form - Form element
     */
    function setupAutoSave($form) {
        try {
            const inputs = $form.find('input, select, textarea');

            inputs.on('input change', DataManager.debounce(function () {
                saveDraft($form);
                showAutoSaveIndicator();
            }, CONFIG.AUTO_SAVE_DELAY));
        } catch (err) {
            console.error('Error setting up auto-save:', err);
        }
    }

    /**
     * Save draft to localStorage
     * @param {jQuery} $form - Form element
     */
    function saveDraft($form) {
        try {
            const formData = {};
            $form.find('input, select, textarea').each(function () {
                const $input = $(this);
                formData[$input.attr('name')] = $input.val();
            });

            DataManager.saveToStorage(CONFIG.DRAFT_STORAGE_KEY, formData);
            console.log('Draft auto-saved');
        } catch (err) {
            console.error('Error saving draft:', err);
        }
    }

    /**
     * Load draft from localStorage
     * @param {jQuery} $form - Form element
     */
    function loadDraft($form) {
        try {
            const draft = DataManager.loadFromStorage(CONFIG.DRAFT_STORAGE_KEY);
            if (!draft) return;

            Object.keys(draft).forEach(key => {
                const $input = $form.find(`[name="${key}"]`);
                if ($input.length) {
                    $input.val(draft[key]);
                }
            });

            UIManager.showAlert('Draft loaded from your previous session.', 'info');
        } catch (err) {
            console.error('Error loading draft:', err);
        }
    }

    /**
     * Show auto-save indicator
     */
    function showAutoSaveIndicator() {
        try {
            const $indicator = $('#autoSaveIndicator');
            if ($indicator.length === 0) {
                $('body').append(
                    '<div id="autoSaveIndicator" class="alert alert-info position-fixed" ' +
                    'style="top: 20px; right: 20px; z-index: 9999; max-width: 300px;">Draft saved</div>'
                );
            }

            clearTimeout(autoSaveTimeout);
            autoSaveTimeout = setTimeout(() => {
                $('#autoSaveIndicator').fadeOut('slow', function () {
                    $(this).remove();
                });
            }, 2000);
        } catch (err) {
            console.error('Error showing auto-save indicator:', err);
        }
    }

    /**
     * Setup word count suggestions
     */
    function setupWordCountSuggestions() {
        try {
            const $toneSelect = $('#Tone');
            const $audienceSelect = $('#TargetAudience');
            const $wordCountSelect = $('#TargetWordCount');

            function updateWordCountSuggestion() {
                const tone = $toneSelect.val();
                const audience = $audienceSelect.val();

                let suggestion = 1000; // default

                // Adjust based on tone and audience
                if (tone === 'casual' && audience === 'beginners') {
                    suggestion = 800;
                } else if (tone === 'formal' && audience === 'experts') {
                    suggestion = 1500;
                } else if (audience === 'students') {
                    suggestion = 1200;
                }

                // Update select if not manually changed
                if (!$wordCountSelect.hasClass('user-changed')) {
                    $wordCountSelect.val(suggestion);
                }
            }

            $toneSelect.add($audienceSelect).on('change', updateWordCountSuggestion);
            $wordCountSelect.on('change', function () {
                $(this).addClass('user-changed');
            });
        } catch (err) {
            console.error('Error setting up word count suggestions:', err);
        }
    }

    /**
     * Destroy module
     */
    function destroy() {
        try {
            if (progressInterval) {
                clearInterval(progressInterval);
            }
            if (autoSaveTimeout) {
                clearTimeout(autoSaveTimeout);
            }
            isInitialized = false;
        } catch (err) {
            console.error('Error destroying module:', err);
        }
    }

    // Cleanup on page unload
    $(window).on('beforeunload', destroy);

    // Public API
    return {
        initialize,
        validateForm,
        saveDraft,
        loadDraft,
        destroy
    };
})();

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = BlogGeneratorModule;
}
