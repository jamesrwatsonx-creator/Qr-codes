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

  if(type === 'error')
    inputElement.classList.add("error-glow");
  if(type === 'warn')
    inputElement.classList.add("warn-glow");

  const domMessageElement = getInputElementStatusMessageElementFor(inputElement);
  domMessageElement.innerHTML = `<span aria-hidden="true">${iconMap[type]} </span><span class="font-color-${type}">${message}</span>`;
  domMessageElement.classList.remove("invisible");
  inputElement.addEventListener('keyup', () => clearInputStatus(inputElement));
  inputElement.addEventListener('change', () => clearInputStatus(inputElement));
}

function clearInputStatus(inputElement) {
  inputElement.classList.remove("error-glow");
  inputElement.classList.remove("warn-glow");
  const domMessageElement = getInputElementStatusMessageElementFor(inputElement);
  domMessageElement.classList.add("invisible");
}
