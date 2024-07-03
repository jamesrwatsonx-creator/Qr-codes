var currentDimensions = null;

function getDimensions() {
  return {
    'width': window.innerWidth,
    'height': Math.max(document.body.scrollHeight, document.documentElement.scrollHeight)
  }
}

function onPotentialDimensionChange() {
  const newDimensions = getDimensions();
  //if(newDimensions.height === currentDimensions?.height && newDimensions.width === currentDimensions?.width) return;

  const messageForParentWindow = {
    'messageType': 'frameDimensions', 'messageContent': newDimensions
  };

  currentDimensions = newDimensions;
  window.parent.postMessage(messageForParentWindow, "*");
}

window.addEventListener('load', function () {
  onPotentialDimensionChange();
  if (window.MutationObserver) {
    const mutationObserver = new MutationObserver(onPotentialDimensionChange);
    const mutationObserverConfiguration = {
      attributes: true, attributeOldValue: false, characterData: true, characterDataOldValue: false, childList: true, subtree: true
    };
    mutationObserver.observe(document.body, mutationObserverConfiguration);
  } else {
    window.setInterval(onPotentialDimensionChange, 100);
  }
});
