// Blog Generation JavaScript

(function ($) {
    "use strict";

    let generationInProgress = false;
    let progressInterval;

    $(document).ready(function () {
        initializeBlogGenerator();
    });

    function initializeBlogGenerator() {
        const generateForm = $('#generateForm');
        const progressCard = $('#progressCard');
        const progressBar = $('#progressBar');
        const progressText = $('#progressText');

        if (!generateForm.length) return;

        generateForm.on('submit', function (e) {
            e.preventDefault();

            if (generationInProgress) {
                return;
            }

            const formData = new FormData(this);

            // Validate required fields
            if (!validateForm(formData)) {
                return;
            }

            startGeneration(generateForm, progressCard, progressBar, progressText, formData);
        });

        // Auto-save draft functionality
        setupAutoSave(generateForm);

        // Dynamic word count suggestions
        setupWordCountSuggestions();
    }

    function validateForm(formData) {
        const topic = formData.get('Topic');
        if (!topic || topic.trim().length < 3) {
            UIManager.showAlert('Please enter a topic with at least 3 characters.', 'danger');
            $('#Topic').focus();
            return false;
        }

        const targetWordCount = parseInt(formData.get('TargetWordCount'));
        if (targetWordCount < 100 || targetWordCount > 5000) {
            UIManager.showAlert('Target word count must be between 100 and 5000.', 'danger');
            $('#TargetWordCount').focus();
            return false;
        }

        return true;
    }

    function startGeneration(form, progressCard, progressBar, progressText, formData) {
        generationInProgress = true;
        const submitBtn = form.find('button[type="submit"]');

        // Update UI
        submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Generating...');
        progressCard.removeClass('d-none');
        progressBar.css('width', '0%');

        // Progress simulation
        let progress = 0;
        const progressSteps = [
            'Analyzing topic and keywords...',
            'Generating content structure...',
            'Writing compelling introduction...',
            'Creating main content sections...',
            'Adding examples and data...',
            'Optimizing for SEO...',
            'Finalizing and formatting...',
            'Post generation complete!'
        ];

        progressInterval = setInterval(() => {
            progress += Math.random() * 12;
            if (progress > 95) progress = 95;

            progressBar.css('width', progress + '%');

            const stepIndex = Math.floor((progress / 100) * (progressSteps.length - 1));
            progressText.text(progressSteps[stepIndex]);

            if (progress >= 95) {
                clearInterval(progressInterval);
            }
        }, 800);

        // Submit form using unified BloggingAgent API with FormData support
        const formDataToSend = new FormData(form[0]);
        
        // BloggingAgent.post() handles CSRF token and error handling automatically
        BloggingAgent.post(form.attr('action'), formDataToSend, {
            headers: {
                'Content-Type': undefined // Let browser set proper content-type for FormData
            }
        })
        .then(response => {
            clearInterval(progressInterval);
            progressBar.css('width', '100%');
            progressText.text('Redirecting to your new post...');

            // Redirect after short delay
            setTimeout(() => {
                window.location.href = response.redirectUrl || '/blog';
            }, 1500);
        })
        .catch(error => {
            clearInterval(progressInterval);
            generationInProgress = false;

            submitBtn.prop('disabled', false).html('<i class="fas fa-magic me-2"></i>Generate Post');

            let errorMessage = error.message || 'An error occurred while generating the post.';

            progressText.text('Generation failed: ' + errorMessage);
            progressBar.addClass('bg-danger');

            UIManager.showAlert(errorMessage, 'danger');
        });
    }

    function setupAutoSave(form) {
        let autoSaveTimeout;
        const inputs = form.find('input, select, textarea');

        inputs.on('input change', function () {
            clearTimeout(autoSaveTimeout);

            // Save draft to localStorage using centralized DataManager
            const formData = {};
            inputs.each(function () {
                const input = $(this);
                formData[input.attr('name')] = input.val();
            });

            DataManager.saveToStorage('blogDraft', formData);

            // Show auto-save indicator using UIManager
            UIManager.showAlert('Draft saved', 'info', { duration: 2000, position: 'bottom-right' });

            autoSaveTimeout = setTimeout(() => {
                // Could send to server for persistent draft saving
                console.log('Draft auto-saved to localStorage');
            }, 2000);
        });

        // Load draft on page load
        loadDraft(form);
    }

    function loadDraft(form) {
        // Load draft from localStorage using centralized DataManager
        const draft = DataManager.loadFromStorage('blogDraft', null);
        if (draft) {
            try {
                Object.keys(draft).forEach(key => {
                    const input = form.find(`[name="${key}"]`);
                    if (input.length) {
                        input.val(draft[key]);
                    }
                });

                UIManager.showAlert('Draft loaded from your previous session.', 'info');
            } catch (e) {
                console.error('Error loading draft:', e);
            }
        }
    }

    function setupWordCountSuggestions() {
        const toneSelect = $('#Tone');
        const audienceSelect = $('#TargetAudience');
        const wordCountSelect = $('#TargetWordCount');

        function updateWordCountSuggestion() {
            const tone = toneSelect.val();
            const audience = audienceSelect.val();

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
            if (!wordCountSelect.hasClass('user-changed')) {
                wordCountSelect.val(suggestion);
            }
        }

        toneSelect.add(audienceSelect).on('change', updateWordCountSuggestion);
        wordCountSelect.on('change', function () {
            $(this).addClass('user-changed');
        });
    }

    // Export for potential use in other scripts
    window.BlogGenerator = {
        validateForm: validateForm,
        startGeneration: startGeneration
    };

})(jQuery);