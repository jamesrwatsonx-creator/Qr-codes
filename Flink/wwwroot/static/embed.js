let currentDimensions = null;

function getDimensions() {
  return {
    'width': Math.min(window.innerWidth, 600),
    'height': Math.min(Math.max(document.body.scrollHeight, document.documentElement.scrollHeight), 600)
  }
}

function onPotentialDimensionChange() {
  const newDimensions = getDimensions();
  if(newDimensions.height === currentDimensions?.height && newDimensions.width === currentDimensions?.width) return;

  const messageForParentWindow = {
    'messageType': 'frameDimensions', 'messageContent': newDimensions
  };

  console.log("New Dimensions from flink Embed:", newDimensions);

  currentDimensions = newDimensions;
  window.parent.postMessage(messageForParentWindow, "*");
}

window.onload = () => onPotentialDimensionChange();
window.onresize = () => onPotentialDimensionChange();
document.getElementById('short-link').onresize = () => onPotentialDimensionChange();
