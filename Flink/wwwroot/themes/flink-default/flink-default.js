function copyURLToClipboard(statusElement, url) {
  console.debug("Clipboard")
  navigator.clipboard
    .writeText(url)
    .then(() => setStatus(statusElement, "📋 Copied to clipboard"))
    .catch(() => console.error('Could not write to clipboard.'));
}


function getInputElementStatusMessageElementFor(inputElement) {
  let el = inputElement;
  while (el && el.nextElementSibling) {
    el = el.nextElementSibling;
    if (el.tagName.toLowerCase() === 'p' && el.classList.contains("input-element-status-message")) {
      break;
    }
  }
  return el;
}

function setInputElementStatus(inputElement, message, type) {
  const iconMap = {
    error: '❌',
    warn: '⚠️',
    info: 'ℹ️',
    success: '✅'
  }

  if(type === 'error') {
      inputElement.classList.add("error-glow");
  }

  if(type === 'warning') {
    inputElement.classList.add("warning-glow");
  }

  const domMessageElement = getInputElementStatusMessageElementFor(inputElement);
  domMessageElement.innerHTML = `<span aria-hidden="true">${iconMap[type]} </span><span class="font-color-${type}">${message}</span>`;
  domMessageElement.classList.remove("invisible");
  inputElement.addEventListener('keyup', () => clearInputStatus(inputElement));
  inputElement.addEventListener('change', () => clearInputStatus(inputElement));

}

function clearInputStatus(inputElement) {
  inputElement.classList.remove("error-glow");
  inputElement.classList.remove("warning-glow");
  const domMessageElement = getInputElementStatusMessageElementFor(inputElement);
  domMessageElement.classList.add("invisible");
}

(function () {
  const flinkUi = new FlinkUi();
  const flinkApi = new FlinkApiClient();
  
  const domLongUrlInput = document.getElementById("in-long-url");
  const domAdvancedCollapsibleButton = document.getElementById("btn-advanced-collapsible");
  const domAdvancedCollapsibleContent = document.getElementById("div-advanced-collapsible-content");
  const domActiveFromInput = document.getElementById("in-dt-active-from");
  const domActiveUntilInput = document.getElementById("in-dt-active-until");
  const domPasswordInput = document.getElementById("in-password");
  const domPreferredIdInput = document.getElementById("in-preferred-id");
  const domSuffixLengthInput = document.getElementById("in-suffix-length");
  const domFlinkifyButton = document.getElementById("btn-flinkify");
  const domShortUrlInput = document.getElementById("in-short-url");
  const domQrCodeImg = document.getElementById("img-qrcode");
  const domLoadingSpinner = document.getElementById("div-embed-loading-spinner");
  const domEmbedIframe = document.getElementById("if-embed");

  const resetOutputSection = () => {
    domQrCodeImg.classList.add('invisible');
    domEmbedIframe.classList.add('invisible');
    domShortUrlInput.classList.add('invisible');
    console.log("RESET");
  };

  // Expand "Advanced Section"
  domAdvancedCollapsibleButton.addEventListener('click', () => {
    domAdvancedCollapsibleButton.classList.toggle("collapsible-active");
    domAdvancedCollapsibleContent.classList.toggle("invisible");
    const isExpanded = domAdvancedCollapsibleButton.getAttribute("aria-expanded") === "true"
    domAdvancedCollapsibleButton.setAttribute("aria-expanded", isExpanded  ? "false" : "true")
  });

  // Submit on enter (on Flink Input URL)
  domLongUrlInput.addEventListener('keyup', (longUrlKeyUpEvent) => {
    if (longUrlKeyUpEvent.key === "Enter") {
      longUrlKeyUpEvent.preventDefault();
      domFlinkifyButton.click();
    }
  });

  domFlinkifyButton.addEventListener('click', () => {
    resetOutputSection();

    if(!flinkUi.isValidURL(domLongUrlInput.value)) {
      setInputElementStatus(domLongUrlInput, `The URL is invalid and cannot be shortened`, 'error');
      return;
    }
  
    if(domPasswordInput.value.length > domPasswordInput.maxLength)
      setInputElementStatus(domPasswordInput, `Password must not be longer than ${domPasswordInput.maxLength} characters`);
    if(domPasswordInput.value.length == 0)
      setInputElementStatus(domPasswordInput, 'Link will not require a password', 'info');
    if(domPasswordInput.value.length != 0 && domPasswordInput.value.length < 12)
      setInputElementStatus(domPasswordInput, `Good passwords should have at least 12 characters`, 'warn');

    if(domPreferredIdInput.value.length > domPreferredIdInput.maxLength)
      setInputElementStatus(domPasswordInput, `Flink ID must not be longer than ${domPreferredIdInput.maxLength} characters`);
    if(domPreferredIdInput.value.length > domLongUrlInput.value.length - 7)
      setInputElementStatus(domPreferredIdInput, `Id is longer than the URL to shorten. Are you sure?`, 'warn');
    if(domPreferredIdInput.value.length == 0)
      setInputElementStatus(domPreferredIdInput, `Flink will generate a random ID for you`, 'info')

    if(domActiveFromInput?.value === "")
      setInputElementStatus(domActiveFromInput, `Link will be active immediately`, 'info');
  
    if(domActiveUntilInput?.value === "")
      setInputElementStatus(domActiveUntilInput, `Link will active forever`, 'info');

    const sanitizedUrl = flinkUi.sanitizeUrl(domLongUrlInput.value);
    domLongUrlInput.value = sanitizedUrl;

    const activeFromDateTime = flinkUi.convertToIsoDateTimeString(flinkUi.parseDateTime(domActiveFromInput));
    const activeUntilDateTime = flinkUi.convertToIsoDateTimeString(flinkUi.parseDateTime(domActiveUntilInput));
    const suffixLength = parseInt(domSuffixLengthInput.value);
    const preferredId = domPreferredIdInput.value === "" ? null : domPreferredIdInput.value;

    flinkApi
      .postLink(sanitizedUrl, "flink-ui", preferredId, suffixLength, activeFromDateTime, activeUntilDateTime)
      .then(data => {
        domShortUrlInput.value = flinkApi.getShortLinkHref(data['id']);
        domShortUrlInput.classList.remove("invisible");
        domQrCodeImg.src = flinkApi.getQrCodeHref(data['id'], 202);
        domQrCodeImg.classList.remove("invisible");
        domLoadingSpinner.classList.remove("invisible");
        flinkApi.retryGetMeta(data.id, 200)
          .then(_ => {
            domLoadingSpinner.classList.add("invisible");
            domEmbedIframe.src = flinkApi.getEmbedHref(data.id);
            domEmbedIframe.classList.remove("invisible");
          });
      });
  });
})();
