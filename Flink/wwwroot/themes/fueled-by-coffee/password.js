(function () {
  const flinkApi = new FlinkApiClient();

  const domPasswordInput = document.getElementById("in-password");
  const domUnlockButton = document.getElementById("btn-unlock");

  // Submit on Enter
  domPasswordInput.addEventListener('keyup', (ev) => {
    if (ev.key === "Enter") {
      ev.preventDefault();
      domUnlockButton.click();
    }
  });

  domUnlockButton.addEventListener('click', async () => {
    clearInputStatus(domPasswordInput);
    const shortLinkId = document.documentURI.split('/').pop();
    const shortLinkHref = await flinkApi.getShortLinkHref(shortLinkId, domPasswordInput.value);
    flinkApi.checkPassword(shortLinkId, domPasswordInput.value)
      .then(isPasswordCorrect => {
        if (isPasswordCorrect) {
          window.location.href = shortLinkHref;
        } else {
          setInputElementStatus(domPasswordInput, `The password you entered is wrong`, 'error');
        }
      });
  });
})();
