const flinkApi = new FlinkApiClient();

const previous = {
  days: [],
  hours: [],
  minutes: [],
  seconds: []
};

function getTargetTimeFromURL() {
  const params = new URLSearchParams(window.location.search);
  const ts = params.get("activeFrom");
  const timestamp = parseInt(ts, 10);
  if (!isNaN(timestamp)) {
    return timestamp * 1000;
  }
  return null;
}

function updateCountdown() {
  const now = new Date().getTime();
  const targetTimestamp = getTargetTimeFromURL();
  const distance = targetTimestamp - now;

  if (distance <= 0) {
    flinkApi
      .getShortLinkHref(document.documentURI.split('/').pop())
      .then(u => window.location.href = u);

    return;
  }

  const d = padDigits(Math.floor(distance / (1000 * 60 * 60 * 24)), 3);
  const h = padDigits(Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)));
  const m = padDigits(Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60)));
  const s = padDigits(Math.floor((distance % (1000 * 60)) / 1000));

  updateUnit('days', d, previous.days);
  updateUnit('hours', h, previous.hours);
  updateUnit('minutes', m, previous.minutes);
  updateUnit('seconds', s, previous.seconds);

  previous.days = d;
  previous.hours = h;
  previous.minutes = m;
  previous.seconds = s;
}
function padDigits(num, size = 2) {
  return num.toString().padStart(size, '0').split('');
}

function updateUnit(id, digits, previousDigits) {
  const container = document.querySelector(`#${id} .digits`);

  if (previousDigits.join('') === digits.join(''))
    return;

  container.innerHTML = '';
  digits.forEach(d => {
    const digitEl = document.createElement('div');
    digitEl.className = 'digit';
    digitEl.innerHTML = `<span>${d}</span>`;
    container.appendChild(digitEl);
  });
}

(async function () {
  setInterval(updateCountdown, 1000)
})();

