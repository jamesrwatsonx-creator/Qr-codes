function isURL(str) {
  const urlRegex = '^(?!mailto:)(?:(?:http|https|ftp)://)(?:\\S+(?::\\S*)?@)?(?:(?:(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}(?:\\.(?:[0-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))|(?:(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)(?:\\.(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)*(?:\\.(?:[a-z\\u00a1-\\uffff]{2,})))|localhost)(?::\\d{2,5})?(?:(/|\\?|#)[^\\s]*)?$';
  return new RegExp(urlRegex, 'i').test(str);
}

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
  navigator.clipboard.writeText(url);
  setStatus(statusElement, "📋 Copied to clipboard")
}

(function() {
  const status = document.getElementById("status-text");
  const inputUrl = document.getElementById("input-url");
  const outputUrl = document.getElementById("output-url");
  const outputQrCode = document.getElementById("output-qr-code");
  const shortenAction = document.getElementById("shorten-action");

  inputUrl.addEventListener('input', () => {
    isURL(inputUrl.value)
      ? clearInputError(inputUrl, status)
      : setInputError(inputUrl, status);
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
    
    // Hide the QR code
    outputQrCode.style.display = "none";
    outputQrCode.src = "";
    // Hide the output field
    outputUrl.style.display = "none";
    outputUrl.value = "";


    if(!isURL(inputUrl.value)) {
      setInputError(inputUrl, status);
      return;
    }
    const r = new XMLHttpRequest();
    r.open('POST', '/to', true);
    r.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
    r.onload = function() {
      if (r.status === 201) {
        clearInputError(inputUrl, status);
        const data = JSON.parse(r.responseText);
        console.log("Created ShortLink", data);
        // Display the shortened URL
        outputUrl.value = `${document.URL}to/${data['id']}`;
        outputUrl.style.display = "block";
        // Display the QR code
        outputQrCode.src = `${document.URL}qr/${data['id']}`;
        outputQrCode.style.display = "block";
        setStatus(status, "✅ URL successfully shortened")
      }
    };

    r.send(JSON.stringify({
      "uri": inputUrl.value,
      "creator": "webui",
      "preferredIdLength": 4
    }));
  })
})();
