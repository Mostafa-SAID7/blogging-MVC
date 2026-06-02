/**
 * Analytics Module
 * Consolidated analytics functionality using centralized utilities
 * Dependencies: UIManager, ChartManager, DataManager, BloggingAgent (AJAX)
 */

const AnalyticsModule = (function () {
    'use strict';

    const CONFIG = {
        REFRESH_INTERVAL: 5 * 60 * 1000, // 5 minutes
        CHART_UPDATE_INTERVAL: 3000
    };

    let charts = {};
    let refreshInterval;
    let isInitialized = false;

    /**
     * Initialize analytics module
     */
    function initialize() {
        try {
            if (isInitialized) return;

            if (!$('#analyticsContainer').length) {
                console.warn('Analytics container not found');
                return;
            }

            setupRealTimeUpdates();
            initializeCharts();
            setupExportFunctionality();
            setupFilters();

            isInitialized = true;
        } catch (err) {
            console.error('Error initializing analytics module:', err);
        }
    }

    /**
     * Setup real-time updates
     */
    function setupRealTimeUpdates() {
        try {
            // Auto-refresh every 5 minutes
            refreshInterval = setInterval(() => {
                refreshAnalyticsData();
            }, CONFIG.REFRESH_INTERVAL);

            // Manual refresh button
            $(document).on('click', '#refreshAnalytics', function () {
                refreshAnalyticsData();
                showRefreshFeedback($(this));
            });
        } catch (err) {
            console.error('Error setting up real-time updates:', err);
        }
    }

    /**
     * Refresh analytics data
     */
    function refreshAnalyticsData() {
        try {
            updateMetrics();
            updateCharts();
            updateTimestamp();
        } catch (err) {
            console.error('Error refreshing analytics data:', err);
        }
    }

    /**
     * Update metrics
     */
    function updateMetrics() {
        try {
            $('.metric-value').each(function () {
                const currentValue = parseInt($(this).text().replace(/,/g, ''));
                const newValue = currentValue + Math.floor(Math.random() * 10) - 5;
                $(this).text(DataManager.formatNumber(Math.max(0, newValue)));
            });
        } catch (err) {
            console.error('Error updating metrics:', err);
        }
    }

    /**
     * Initialize charts
     */
    function initializeCharts() {
        try {
            if (!ChartManager.isChartAvailable()) {
                console.warn('Chart.js not loaded');
                return;
            }

            initViewsChart();
            initTrafficSourcesChart();
            initPerformanceChart();
        } catch (err) {
            console.error('Error initializing charts:', err);
        }
    }

    /**
     * Initialize views chart
     */
    function initViewsChart() {
        try {
            const labels = DataManager.getLastNDays(7);
            const data = DataManager.generateRandomData(7, 50, 200);

            const chart = ChartManager.initLineChart('viewsChart', {
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
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                callback: function (value) {
                                    return DataManager.formatNumber(value);
                                }
                            }
                        }
                    }
                }
            });

            charts.views = chart;
        } catch (err) {
            console.error('Error initializing views chart:', err);
        }
    }

    /**
     * Initialize traffic sources chart
     */
    function initTrafficSourcesChart() {
        try {
            const chart = ChartManager.initDoughnutChart('trafficSourcesChart', {
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
                }
            });

            charts.traffic = chart;
        } catch (err) {
            console.error('Error initializing traffic chart:', err);
        }
    }

    /**
     * Initialize performance chart
     */
    function initPerformanceChart() {
        try {
            const chart = ChartManager.initBarChart('performanceChart', {
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
                    }
                }
            });

            charts.performance = chart;
        } catch (err) {
            console.error('Error initializing performance chart:', err);
        }
    }

    /**
     * Update charts
     */
    function updateCharts() {
        try {
            if (charts.views) {
                const newData = DataManager.generateRandomData(7, 50, 200);
                ChartManager.updateChart('viewsChart', null, newData);
            }
        } catch (err) {
            console.error('Error updating charts:', err);
        }
    }

    /**
     * Update timestamp
     */
    function updateTimestamp() {
        try {
            $('.last-updated').text(new Date().toLocaleString());
        } catch (err) {
            console.error('Error updating timestamp:', err);
        }
    }

    /**
     * Setup export functionality
     */
    function setupExportFunctionality() {
        try {
            $(document).on('click', '#exportJson', function () {
                exportData('json');
            });

            $(document).on('click', '#exportCsv', function () {
                exportData('csv');
            });

            $(document).on('click', '#exportPdf', function () {
                exportAsPDF();
            });
        } catch (err) {
            console.error('Error setting up export:', err);
        }
    }

    /**
     * Export data
     * @param {string} format - Export format
     */
    function exportData(format) {
        try {
            // Get data from page
            const tableData = [];
            $('table tbody tr').each(function () {
                const rowData = {};
                $(this).find('td').each(function (index) {
                    rowData[`Column${index}`] = $(this).text();
                });
                tableData.push(rowData);
            });

            if (format === 'json') {
                DataManager.exportAsJSON(tableData, 'analytics.json');
                UIManager.showAlert('Data exported as JSON', 'success');
            } else if (format === 'csv') {
                DataManager.exportAsCSV(tableData, 'analytics.csv');
                UIManager.showAlert('Data exported as CSV', 'success');
            }
        } catch (err) {
            console.error('Error exporting data:', err);
            UIManager.showAlert('Export failed', 'danger');
        }
    }

    /**
     * Export as PDF
     */
    function exportAsPDF() {
        try {
            UIManager.showAlert('PDF export requires jsPDF library', 'info');
        } catch (err) {
            console.error('Error exporting PDF:', err);
        }
    }

    /**
     * Setup filters
     */
    function setupFilters() {
        try {
            $(document).on('change', '#dateRangeFilter', function () {
                const range = $(this).val();
                filterAnalyticsByDate(range);
            });

            $(document).on('change', '#contentTypeFilter', function () {
                const type = $(this).val();
                filterAnalyticsByContent(type);
            });
        } catch (err) {
            console.error('Error setting up filters:', err);
        }
    }

    /**
     * Filter by date range
     * @param {string} range - Date range
     */
    function filterAnalyticsByDate(range) {
        try {
            console.log('Filtering by date range:', range);
            refreshAnalyticsData();
        } catch (err) {
            console.error('Error filtering by date:', err);
        }
    }

    /**
     * Filter by content type
     * @param {string} type - Content type
     */
    function filterAnalyticsByContent(type) {
        try {
            console.log('Filtering by content type:', type);
            refreshAnalyticsData();
        } catch (err) {
            console.error('Error filtering by content:', err);
        }
    }

    /**
     * Show refresh feedback
     * @param {jQuery} $button - Button element
     */
    function showRefreshFeedback($button) {
        try {
            UIManager.showFeedback($button, 'Refreshing...', false);
        } catch (err) {
            console.error('Error showing refresh feedback:', err);
        }
    }

    /**
     * Show post analytics
     * @param {number} postId - Post ID
     */
    function showPostAnalytics(postId) {
        try {
            const $modal = UIManager.showModal('postAnalyticsModal');
            const $content = $('#postAnalyticsContent');

            UIManager.showLoading($content, 'Loading post analytics...');

            // Fetch post analytics
            BloggingAgent.get(`/analytics/post/${postId}`)
                .then(data => {
                    $content.html(generatePostAnalyticsHTML(data));
                })
                .catch(error => {
                    $content.html('<div class="alert alert-danger">Error loading post analytics.</div>');
                });
        } catch (err) {
            console.error('Error showing post analytics:', err);
        }
    }

    /**
     * Generate post analytics HTML
     * @param {object} data - Analytics data
     * @return {string} HTML content
     */
    function generatePostAnalyticsHTML(data) {
        try {
            return `
                <div class="row">
                    <div class="col-md-6">
                        <h6>Traffic Metrics</h6>
                        <div class="mb-3">
                            <strong>Views:</strong> ${DataManager.formatNumber(data.views || 0)}
                        </div>
                        <div class="mb-3">
                            <strong>Unique Views:</strong> ${DataManager.formatNumber(data.uniqueViews || 0)}
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
                            <strong>${source}:</strong> ${DataManager.formatNumber(count)}
                        </div>
                    `).join('')}
                </div>
            `;
        } catch (err) {
            console.error('Error generating HTML:', err);
            return '<div class="alert alert-danger">Error generating content</div>';
        }
    }

    /**
     * Destroy analytics module
     */
    function destroy() {
        try {
            if (refreshInterval) {
                clearInterval(refreshInterval);
            }
            Object.keys(charts).forEach(key => {
                ChartManager.destroyChart(key);
            });
            charts = {};
            isInitialized = false;
        } catch (err) {
            console.error('Error destroying analytics module:', err);
        }
    }

    // Cleanup on page unload
    $(window).on('beforeunload', destroy);

    // Public API
    return {
        initialize,
        refreshAnalyticsData,
        showPostAnalytics,
        destroy
    };
})();

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AnalyticsModule;
}
