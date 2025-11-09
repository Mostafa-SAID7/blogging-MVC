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
            showAlert('Please enter a topic with at least 3 characters.', 'danger');
            $('#Topic').focus();
            return false;
        }

        const targetWordCount = parseInt(formData.get('TargetWordCount'));
        if (targetWordCount < 100 || targetWordCount > 5000) {
            showAlert('Target word count must be between 100 and 5000.', 'danger');
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

        // Submit form
        $.ajax({
            url: form.attr('action'),
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                clearInterval(progressInterval);
                progressBar.css('width', '100%');
                progressText.text('Redirecting to your new post...');

                // Redirect after short delay
                setTimeout(() => {
                    window.location.href = response.redirectUrl || '/blog';
                }, 1500);
            },
            error: function (xhr, status, error) {
                clearInterval(progressInterval);
                generationInProgress = false;

                submitBtn.prop('disabled', false).html('<i class="fas fa-magic me-2"></i>Generate Post');

                let errorMessage = 'An error occurred while generating the post.';
                if (xhr.responseJSON && xhr.responseJSON.error) {
                    errorMessage = xhr.responseJSON.error.message || errorMessage;
                }

                progressText.text('Generation failed: ' + errorMessage);
                progressBar.addClass('bg-danger');

                showAlert(errorMessage, 'danger');
            }
        });
    }

    function setupAutoSave(form) {
        let autoSaveTimeout;
        const inputs = form.find('input, select, textarea');

        inputs.on('input change', function () {
            clearTimeout(autoSaveTimeout);

            // Save draft to localStorage
            const formData = {};
            inputs.each(function () {
                const input = $(this);
                formData[input.attr('name')] = input.val();
            });

            localStorage.setItem('blogDraft', JSON.stringify(formData));

            // Show auto-save indicator
            showAutoSaveIndicator();

            autoSaveTimeout = setTimeout(() => {
                // Could send to server for persistent draft saving
                console.log('Draft auto-saved');
            }, 2000);
        });

        // Load draft on page load
        loadDraft(form);
    }

    function loadDraft(form) {
        const draft = localStorage.getItem('blogDraft');
        if (draft) {
            try {
                const formData = JSON.parse(draft);
                Object.keys(formData).forEach(key => {
                    const input = form.find(`[name="${key}"]`);
                    if (input.length) {
                        input.val(formData[key]);
                    }
                });

                showAlert('Draft loaded from your previous session.', 'info');
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

    function showAutoSaveIndicator() {
        const indicator = $('#autoSaveIndicator');
        if (indicator.length === 0) {
            $('body').append('<div id="autoSaveIndicator" class="alert alert-info position-fixed" style="top: 20px; right: 20px; z-index: 9999; max-width: 300px;">Draft saved</div>');
        }

        clearTimeout(window.autoSaveTimeout);
        window.autoSaveTimeout = setTimeout(() => {
            $('#autoSaveIndicator').fadeOut(() => $(this).remove());
        }, 2000);
    }

    function showAlert(message, type = 'info') {
        const alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show position-fixed" style="top: 20px; left: 50%; transform: translateX(-50%); z-index: 9999; max-width: 500px;">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;

        $('body').append(alertHtml);

        setTimeout(() => {
            $('.alert').fadeOut(() => $(this).remove());
        }, 5000);
    }

    // Export for potential use in other scripts
    window.BlogGenerator = {
        validateForm: validateForm,
        startGeneration: startGeneration
    };

})(jQuery);