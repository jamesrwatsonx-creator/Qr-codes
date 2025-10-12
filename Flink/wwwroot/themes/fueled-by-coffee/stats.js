(function () {
  const shortLinkMetricsMap = new Map();
  const fetchAndRender = () => {
    fetchShortLinkMetrics().then(shortLinkMetrics => {
      parseShortLinkMetrics(shortLinkMetrics).forEach(sl => shortLinkMetricsMap.set(sl.id, sl));

      const newMetricsDiv = document.createElement('div');
      newMetricsDiv.id = 'metrics';
      newMetricsDiv.className = 'block-container';

      Array.from(shortLinkMetricsMap)
        .map(kv => kv[1])
        .sort((a1, a2) => a2.count - a1.count)
        .forEach(m => newMetricsDiv.appendChild(createBlock(m.id, m.count)));

      document.getElementById('metrics').replaceWith(newMetricsDiv);
    });
  };

  fetchAndRender();
  setInterval(() => fetchAndRender(), 30000);
})();

async function fetchShortLinkMetrics() {
  try {
    const response = await fetch('/metrics');
    return await response.text();
  } catch (error) {
    console.error('Error fetching metrics:', error);
  }
}

function parseShortLinkMetrics(metricsData) {
  return metricsData
    .split('\n')
    .filter(l => l.startsWith('ok_get_requests_total{otel_scope_name="shortlinks"'))
    .map(l => l.match(/ok_get_requests_total\{([^}]*)} (\d+)/))
    .filter(m => m)
    .map(m => {
      const tags = m[1];
      const count = m[2];
      const idMatch = tags.match(/id="([^"]+)"/);
      return {'id': idMatch ? idMatch[1] : 'Unknown', 'count': count};
    });
}

function createBlock(shortLinkId, count) {
  const div = document.createElement('div');
  div.className = 'metric-block';
  div.innerHTML = `
            <div class="metric-block-head">
              <div class="metric-block-head-left">
                  <span>🦔 <a href="/links/${shortLinkId}">${shortLinkId}</a></span>
              </div>

              <div class="metric-block-head-right">
                  <span>👁️ ${count}</span>
              </div>
            </div>

            <iframe src="/links/${shortLinkId}/embed"></iframe>`;
  return div;
}
