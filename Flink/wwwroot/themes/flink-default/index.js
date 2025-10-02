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
  let lockInputUrl = false;

  const resetOutputSection = () => {
    domQrCodeImg.classList.add('invisible');
    domEmbedIframe.classList.add('invisible');
    domShortUrlInput.classList.add('invisible');
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
    if (longUrlKeyUpEvent.key === "Enter" && lockInputUrl === false) {
      longUrlKeyUpEvent.preventDefault();
      domFlinkifyButton.click();
    } else {
      lockInputUrl = false;
    }
  });

  domFlinkifyButton.addEventListener('click', () => {
    if(lockInputUrl === true)
      return;

    lockInputUrl = true;
    resetOutputSection();

    if(!flinkUi.isValidURL(domLongUrlInput.value)) {
      setInputElementStatus(domLongUrlInput, `The URL is invalid and cannot be shortened`, 'error');
      return;
    }

    if(domPasswordInput.value.length > domPasswordInput.maxLength)
      setInputElementStatus(domPasswordInput, `Password must not be longer than ${domPasswordInput.maxLength} characters`);
    if(domPasswordInput?.value?.length === 0)
      setInputElementStatus(domPasswordInput, 'Link will not require a password', 'info');
    if(domPasswordInput?.value?.length !== 0 && domPasswordInput?.value?.length < 12)
      setInputElementStatus(domPasswordInput, `Good passwords should have at least 12 characters`, 'warn');

    if(domPreferredIdInput.value.length > domPreferredIdInput.maxLength)
      setInputElementStatus(domPreferredIdInput, `Flink ID must not be longer than ${domPreferredIdInput.maxLength} characters`);
    if(domPreferredIdInput.value.length > domLongUrlInput.value.length - 7)
      setInputElementStatus(domPreferredIdInput, `Id is longer than the URL to shorten. Are you sure?`, 'warn');
    if(domPreferredIdInput?.value?.length === 0)
      setInputElementStatus(domPreferredIdInput, `Flink will generate a random ID for you`, 'info');

    if(domActiveFromInput?.value === "")
      setInputElementStatus(domActiveFromInput, `Link will be active immediately`, 'info');

    if(domActiveUntilInput?.value === "")
      setInputElementStatus(domActiveUntilInput, `Link will active forever`, 'info');

    const sanitizedUrl = flinkUi.sanitizeUrl(domLongUrlInput.value);
    domLongUrlInput.value = sanitizedUrl;

    const activeFromDateTime = flinkUi.convertToIsoDateTimeString(flinkUi.parseDateTime(domActiveFromInput.value));
    const activeUntilDateTime = flinkUi.convertToIsoDateTimeString(flinkUi.parseDateTime(domActiveUntilInput.value));
    const suffixLength = parseInt(domSuffixLengthInput.value);
    const preferredId = domPreferredIdInput.value.trim() === "" ? null : domPreferredIdInput.value;
    const password = domPasswordInput.value.trim() === "" ? null : domPasswordInput.value;

    flinkApi
      .postLink(sanitizedUrl, "flink-ui", preferredId, suffixLength, activeFromDateTime, activeUntilDateTime, password)
      .then(async data => {
        domShortUrlInput.value = await flinkApi.getShortLinkHref(data['id'], null);
        domShortUrlInput.classList.remove("invisible");
        domQrCodeImg.src = await flinkApi.getQrCodeHref(data['id'], 202);
        domQrCodeImg.classList.remove("invisible");
        domLoadingSpinner.classList.remove("invisible");
        flinkApi.retryGetMeta(data.id, password, 200)
          .then(async _ => {
            domLoadingSpinner.classList.add("invisible");
            domEmbedIframe.src = await flinkApi.getEmbedHref(data.id, password);
            domEmbedIframe.classList.remove("invisible");
          });
      });
  });
})();
