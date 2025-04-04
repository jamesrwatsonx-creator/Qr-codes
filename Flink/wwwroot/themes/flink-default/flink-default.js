function setStatus(element, text) {
  element.innerHTML = text;
}

function setInputError(inputUrlElement, statusElement) {
  inputUrlElement.classList.add("error-glow");
  setStatus(statusElement, "❌ The entered URL is not valid");
}

function clearInputError(inputUrlElement, statusElement) {
  inputUrlElement.classList.remove("error-glow");
  setStatus(statusElement, "");
}

function copyURLToClipboard(statusElement, url) {
  console.debug("Clipboard")
  navigator.clipboard
    .writeText(url)
    .then(() => setStatus(statusElement, "📋 Copied to clipboard"))
    .catch(() => console.error('Could not write to clipboard.'));
}

(function () {
  const flinkUi = new FlinkUi();
  const flinkApi = new FlinkApiClient();

  const status = document.getElementById("status-text");
  const inputUrl = document.getElementById("input-url");
  const outputUrl = document.getElementById("output-url");
  const outputQrCode = document.getElementById("output-qr-code");
  const shortenAction = document.getElementById("shorten-action");
  const embedIframe = document.getElementById("embed-output");
  const embedLoader = document.getElementById("embed-loader");

  let inputDebounceTimer;
  let flinkifyButtonLocked;

  inputUrl.addEventListener('input', () => {
    flinkifyButtonLocked = false;
    clearTimeout(inputDebounceTimer);
    clearInputError(inputUrl, status);

    inputDebounceTimer = setTimeout(() => {
      flinkUi.isValidURL(inputUrl.value)
        ? clearInputError(inputUrl, status)
        : setInputError(inputUrl, status);
    }, 1000);
  });

  // Submit on Enter
  inputUrl.addEventListener('keyup', (event) => {
    if (event.key === "Enter") {
      event.preventDefault();
      shortenAction.click();
    }
  });

  outputUrl.addEventListener('dblclick', () => copyURLToClipboard(outputUrl.value));

  shortenAction.addEventListener('click', () => {
    if (flinkifyButtonLocked === true)
      return;
    flinkifyButtonLocked = true;

    outputQrCode.classList.add("invisible");
    embedIframe.classList.add("invisible");
    outputUrl.classList.add("invisible");

    if (!flinkUi.isValidURL(inputUrl.value)) {
      setInputError(inputUrl, status);
      return;
    }

    flinkApi
      .postLink(flinkUi.sanitizeUrl(inputUrl.value), "flink-ui", null, 4)
      .then(data => {
        clearInputError(inputUrl, status);
        outputUrl.value = flinkApi.getShortLinkHref(data['id']);
        outputUrl.classList.remove("invisible");
        outputQrCode.src = flinkApi.getQrCodeHref(data['id'], 202);
        outputQrCode.classList.remove("invisible");
        embedLoader.classList.remove("invisible");
        flinkApi.retryGetMeta(data.id, 300)
          .then(_ => {
            embedLoader.classList.add("invisible");
            embedIframe.src = flinkApi.getEmbedHref(data.id);
            embedIframe.classList.remove("invisible");
            setStatus(status, "✅ URL successfully shortened");
          });
      });
  })
})();
