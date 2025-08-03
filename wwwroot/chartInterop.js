window.weatherChart = {
    chart: null,
    render: function (canvasId, labels, data) {
        const ctx = document.getElementById(canvasId).getContext('2d');
        if (window.weatherChart.chart) {
            window.weatherChart.chart.destroy();
        }
        window.weatherChart.chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Temperature',
                    data: data,
                    borderColor: 'rgb(120, 233, 126)',
                    backgroundColor: 'rgba(26, 223, 42, 0.2)',
                    fill: false,
                    tension: 0.1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    x: { display: true, title: { display: true, text: 'Time' } },
                    y: { display: true, title: { display: true, text: 'Temperature (°C)' } }
                }
            }
        });
    }
};
window.weatherChartComparison = {
    chart: null,
    render: function (canvasId, labels, datasets) {
        const ctx = document.getElementById(canvasId).getContext('2d');
        if (window.weatherChartComparison.chart) {
            window.weatherChartComparison.chart.destroy();
        }
        window.weatherChartComparison.chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: datasets
            },
            options: {
                responsive: true,
                scales: {
                    x: { display: true, title: { display: true, text: 'Time' } },
                    y: { display: true, title: { display: true, text: 'Temperature (°C)' } }
                }
            }
        });
    }
};