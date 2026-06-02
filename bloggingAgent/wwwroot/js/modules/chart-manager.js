/**
 * Chart Manager Module
 * Centralized chart initialization and management
 * Usage: ChartManager.initLineChart(), ChartManager.updateChart(), etc.
 */

const ChartManager = (function () {
    'use strict';

    const charts = {};

    /**
     * Check if Chart.js is available
     * @return {boolean} True if Chart.js is loaded
     */
    function isChartAvailable() {
        return typeof Chart !== 'undefined';
    }

    /**
     * Initialize line chart
     * @param {string} canvasId - Canvas element ID
     * @param {object} config - Chart configuration
     * @return {Chart} Chart instance
     */
    function initLineChart(canvasId, config) {
        try {
            if (!isChartAvailable()) {
                console.warn('Chart.js not loaded');
                return null;
            }

            const ctx = document.getElementById(canvasId);
            if (!ctx) return null;

            const defaultConfig = {
                type: 'line',
                data: {
                    labels: [],
                    datasets: [{
                        label: 'Data',
                        data: [],
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
                            display: true
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            };

            const mergedConfig = deepMerge(defaultConfig, config || {});
            const chart = new Chart(ctx, mergedConfig);
            charts[canvasId] = chart;

            return chart;
        } catch (err) {
            console.error('Error initializing line chart:', err);
            return null;
        }
    }

    /**
     * Initialize bar chart
     * @param {string} canvasId - Canvas element ID
     * @param {object} config - Chart configuration
     * @return {Chart} Chart instance
     */
    function initBarChart(canvasId, config) {
        try {
            if (!isChartAvailable()) {
                console.warn('Chart.js not loaded');
                return null;
            }

            const ctx = document.getElementById(canvasId);
            if (!ctx) return null;

            const defaultConfig = {
                type: 'bar',
                data: {
                    labels: [],
                    datasets: [{
                        label: 'Data',
                        data: [],
                        backgroundColor: 'rgba(25, 135, 84, 0.8)',
                        borderColor: 'rgb(25, 135, 84)',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            };

            const mergedConfig = deepMerge(defaultConfig, config || {});
            const chart = new Chart(ctx, mergedConfig);
            charts[canvasId] = chart;

            return chart;
        } catch (err) {
            console.error('Error initializing bar chart:', err);
            return null;
        }
    }

    /**
     * Initialize doughnut/pie chart
     * @param {string} canvasId - Canvas element ID
     * @param {object} config - Chart configuration
     * @return {Chart} Chart instance
     */
    function initDoughnutChart(canvasId, config) {
        try {
            if (!isChartAvailable()) {
                console.warn('Chart.js not loaded');
                return null;
            }

            const ctx = document.getElementById(canvasId);
            if (!ctx) return null;

            const defaultConfig = {
                type: 'doughnut',
                data: {
                    labels: [],
                    datasets: [{
                        data: [],
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
            };

            const mergedConfig = deepMerge(defaultConfig, config || {});
            const chart = new Chart(ctx, mergedConfig);
            charts[canvasId] = chart;

            return chart;
        } catch (err) {
            console.error('Error initializing doughnut chart:', err);
            return null;
        }
    }

    /**
     * Update chart data
     * @param {string} canvasId - Canvas element ID
     * @param {array} labels - New labels
     * @param {array} data - New data array(s)
     */
    function updateChart(canvasId, labels, data) {
        try {
            const chart = charts[canvasId];
            if (!chart) {
                console.warn(`Chart '${canvasId}' not found`);
                return;
            }

            if (labels) {
                chart.data.labels = labels;
            }

            if (data) {
                if (Array.isArray(data[0])) {
                    // Multiple datasets
                    data.forEach((dataset, index) => {
                        if (chart.data.datasets[index]) {
                            chart.data.datasets[index].data = dataset;
                        }
                    });
                } else {
                    // Single dataset
                    chart.data.datasets[0].data = data;
                }
            }

            chart.update();
        } catch (err) {
            console.error('Error updating chart:', err);
        }
    }

    /**
     * Update chart dataset
     * @param {string} canvasId - Canvas element ID
     * @param {number} datasetIndex - Dataset index
     * @param {string} property - Property name
     * @param {*} value - New value
     */
    function updateDataset(canvasId, datasetIndex, property, value) {
        try {
            const chart = charts[canvasId];
            if (!chart || !chart.data.datasets[datasetIndex]) {
                console.warn(`Chart or dataset '${canvasId}[${datasetIndex}]' not found`);
                return;
            }

            chart.data.datasets[datasetIndex][property] = value;
            chart.update();
        } catch (err) {
            console.error('Error updating dataset:', err);
        }
    }

    /**
     * Destroy chart
     * @param {string} canvasId - Canvas element ID
     */
    function destroyChart(canvasId) {
        try {
            const chart = charts[canvasId];
            if (chart) {
                chart.destroy();
                delete charts[canvasId];
            }
        } catch (err) {
            console.error('Error destroying chart:', err);
        }
    }

    /**
     * Get chart instance
     * @param {string} canvasId - Canvas element ID
     * @return {Chart} Chart instance
     */
    function getChart(canvasId) {
        return charts[canvasId] || null;
    }

    /**
     * Deep merge objects
     * @param {object} target - Target object
     * @param {object} source - Source object
     * @return {object} Merged object
     */
    function deepMerge(target, source) {
        try {
            const output = Object.assign({}, target);
            if (isObject(target) && isObject(source)) {
                Object.keys(source).forEach(key => {
                    if (isObject(source[key])) {
                        if (!(key in target)) {
                            Object.assign(output, { [key]: source[key] });
                        } else {
                            output[key] = deepMerge(target[key], source[key]);
                        }
                    } else {
                        Object.assign(output, { [key]: source[key] });
                    }
                });
            }
            return output;
        } catch (err) {
            console.error('Error in deepMerge:', err);
            return target;
        }
    }

    /**
     * Check if value is object
     * @param {*} item - Item to check
     * @return {boolean} True if object
     */
    function isObject(item) {
        return item && typeof item === 'object' && !Array.isArray(item);
    }

    // Public API
    return {
        isChartAvailable,
        initLineChart,
        initBarChart,
        initDoughnutChart,
        updateChart,
        updateDataset,
        destroyChart,
        getChart
    };
})();

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ChartManager;
}
