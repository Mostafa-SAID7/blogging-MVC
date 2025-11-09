// Analytics Dashboard JavaScript

(function ($) {
    "use strict";

    let charts = {};
    let refreshInterval;

    $(document).ready(function () {
        initializeAnalytics();
    });

    function initializeAnalytics() {
        if (!$('#analyticsContainer').length) return;

        setupRealTimeUpdates();
        initializeCharts();
        setupExportFunctionality();
        setupFilters();
    }

    function setupRealTimeUpdates() {
        // Auto-refresh analytics every 5 minutes
        refreshInterval = setInterval(() => {
            refreshAnalyticsData();
        }, 5 * 60 * 1000);

        // Manual refresh button
        $('#refreshAnalytics').on('click', function () {
            refreshAnalyticsData();
            showRefreshFeedback();
        });
    }

    function refreshAnalyticsData() {
        // Refresh key metrics
        updateMetrics();

        // Refresh charts
        updateCharts();

        // Update timestamps
        $('.last-updated').text(new Date().toLocaleString());
    }

    function updateMetrics() {
        // Simulate fetching updated metrics
        // In real implementation, this would make AJAX calls
        $('.metric-value').each(function () {
            const currentValue = parseInt($(this).text().replace(/,/g, ''));
            const newValue = currentValue + Math.floor(Math.random() * 10) - 5; // Random fluctuation
            $(this).text(BloggingAgent.formatNumber(Math.max(0, newValue)));
        });
    }

    function initializeCharts() {
        // Views over time chart
        if (typeof Chart !== 'undefined') {
            initializeViewsChart();
            initializeTrafficSourcesChart();
            initializePerformanceChart();
        } else {
            console.warn('Chart.js not loaded, charts will not be displayed');
        }
    }

    function initializeViewsChart() {
        const ctx = document.getElementById('viewsChart');
        if (!ctx) return;

        const labels = getLast7Days();
        const data = generateRandomData(7, 50, 200);

        charts.views = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Page Views',
                    data: data,
                    borderColor: 'rgb(13, 110, 253)',
                    backgroundColor: 'rgba(13, 110, 253, 0.1)',
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function (value) {
                                return BloggingAgent.formatNumber(value);
                            }
                        }
                    }
                }
            }
        });
    }

    function initializeTrafficSourcesChart() {
        const ctx = document.getElementById('trafficSourcesChart');
        if (!ctx) return;

        charts.traffic = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Direct', 'Search Engines', 'Social Media', 'Referrals', 'Email'],
                datasets: [{
                    data: [35, 25, 20, 15, 5],
                    backgroundColor: [
                        'rgb(13, 110, 253)',
                        'rgb(25, 135, 84)',
                        'rgb(255, 193, 7)',
                        'rgb(220, 53, 69)',
                        'rgb(108, 117, 125)'
                    ]
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    }

    function initializePerformanceChart() {
        const ctx = document.getElementById('performanceChart');
        if (!ctx) return;

        charts.performance = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['SEO Score', 'Readability', 'Engagement', 'Conversion'],
                datasets: [{
                    label: 'Score',
                    data: [85, 78, 92, 67],
                    backgroundColor: 'rgba(25, 135, 84, 0.8)',
                    borderColor: 'rgb(25, 135, 84)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        max: 100
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    }

    function updateCharts() {
        if (charts.views) {
            const newData = generateRandomData(7, 50, 200);
            charts.views.data.datasets[0].data = newData;
            charts.views.update();
        }
    }

    function setupExportFunctionality() {
        $('#exportJson').on('click', function () {
            exportData('json');
        });

        $('#exportCsv').on('click', function () {
            exportData('csv');
        });

        $('#exportPdf').on('click', function () {
            exportAsPDF();
        });
    }

    function exportData(format) {
        const button = $(this);
        const originalText = button.html();

        button.html('<i class="fas fa-spinner fa-spin me-2"></i>Exporting...').prop('disabled', true);

        // Simulate export delay
        setTimeout(() => {
            // In real implementation, trigger download
            console.log(`Exporting analytics as ${format.toUpperCase()}`);

            button.html('<i class="fas fa-check me-2"></i>Exported!').removeClass('btn-primary').addClass('btn-success');

            setTimeout(() => {
                button.html(originalText).removeClass('btn-success').addClass('btn-primary').prop('disabled', false);
            }, 2000);
        }, 1500);
    }

    function exportAsPDF() {
        // PDF export would require additional libraries like jsPDF
        alert('PDF export functionality would be implemented with jsPDF library');
    }

    function setupFilters() {
        $('#dateRangeFilter').on('change', function () {
            const range = $(this).val();
            filterAnalyticsByDate(range);
        });

        $('#contentTypeFilter').on('change', function () {
            const type = $(this).val();
            filterAnalyticsByContent(type);
        });
    }

    function filterAnalyticsByDate(range) {
        console.log('Filtering by date range:', range);
        // Implement date filtering logic
        refreshAnalyticsData();
    }

    function filterAnalyticsByContent(type) {
        console.log('Filtering by content type:', type);
        // Implement content type filtering logic
        refreshAnalyticsData();
    }

    function showRefreshFeedback() {
        const feedback = $('#refreshFeedback');
        if (feedback.length === 0) {
            $('body').append('<div id="refreshFeedback" class="alert alert-success position-fixed" style="top: 20px; right: 20px; z-index: 9999;">Analytics refreshed!</div>');
        }

        setTimeout(() => {
            $('#refreshFeedback').fadeOut(() => $(this).remove());
        }, 3000);
    }

    function getLast7Days() {
        const dates = [];
        for (let i = 6; i >= 0; i--) {
            const date = new Date();
            date.setDate(date.getDate() - i);
            dates.push(date.toLocaleDateString());
        }
        return dates;
    }

    function generateRandomData(count, min, max) {
        const data = [];
        for (let i = 0; i < count; i++) {
            data.push(Math.floor(Math.random() * (max - min + 1)) + min);
        }
        return data;
    }

    // Post details modal functionality
    window.showPostAnalytics = function (postId) {
        const modal = new bootstrap.Modal(document.getElementById('postAnalyticsModal'));
        const content = document.getElementById('postAnalyticsContent');

        content.innerHTML = '<div class="text-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>';

        // Fetch post analytics
        BloggingAgent.ajaxRequest(`/analytics/post/${postId}`)
            .then(data => {
                content.innerHTML = generatePostAnalyticsHTML(data);
            })
            .catch(error => {
                content.innerHTML = '<div class="alert alert-danger">Error loading post analytics.</div>';
                console.error('Error:', error);
            });

        modal.show();
    };

    function generatePostAnalyticsHTML(data) {
        return `
            <div class="row">
                <div class="col-md-6">
                    <h6>Traffic Metrics</h6>
                    <div class="mb-3">
                        <strong>Views:</strong> ${BloggingAgent.formatNumber(data.views || 0)}
                    </div>
                    <div class="mb-3">
                        <strong>Unique Views:</strong> ${BloggingAgent.formatNumber(data.uniqueViews || 0)}
                    </div>
                    <div class="mb-3">
                        <strong>Bounce Rate:</strong> ${(data.bounceRate || 0).toFixed(1)}%
                    </div>
                </div>
                <div class="col-md-6">
                    <h6>Engagement</h6>
                    <div class="mb-3">
                        <strong>Avg. Read Time:</strong> ${data.averageReadTime || 0} min
                    </div>
                    <div class="mb-3">
                        <strong>Shares:</strong> ${data.shares || 0}
                    </div>
                    <div class="mb-3">
                        <strong>Comments:</strong> ${data.comments || 0}
                    </div>
                </div>
            </div>
            <hr>
            <h6>Traffic Sources</h6>
            <div class="row">
                ${Object.entries(data.trafficSources || {}).map(([source, count]) => `
                    <div class="col-md-4 mb-2">
                        <strong>${source}:</strong> ${BloggingAgent.formatNumber(count)}
                    </div>
                `).join('')}
            </div>
        `;
    }

    // Cleanup on page unload
    $(window).on('beforeunload', function () {
        if (refreshInterval) {
            clearInterval(refreshInterval);
        }
    });

})(jQuery);