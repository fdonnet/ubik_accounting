function toggleVisibility(elementId) {
  var element = document.getElementById(elementId);

  if (element.classList.contains("block")) {
    element.classList.remove("block");
    element.classList.add("hidden");
  }
  else {
    element.classList.remove("hidden");
    element.classList.add("block");
  }
}

function showElement(elementId) {
  var element = document.getElementById(elementId);

  if (element.classList.contains("hidden")) {
    element.classList.remove("hidden");
    element.classList.add("block");
  }
}

function hideElement(elementId) {
  var element = document.getElementById(elementId);

  if (element.classList.contains("block")) {
    element.classList.remove("block");
    element.classList.add("hidden");
  }
}
