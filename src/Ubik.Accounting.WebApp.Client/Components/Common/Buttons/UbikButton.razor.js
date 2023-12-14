export function loadingModeUp(contentId,spinnerId,buttonId) {
  var content = document.getElementById(contentId);
  var spinner = document.getElementById(spinnerId);
  var currentButton = document.getElementById(buttonId);

  currentButton.setAttribute("disabled", "disabled");
  currentButton.classList.add("cursor-not-allowed");
  content.classList.remove("visible");
  content.classList.add("invisible");
  spinner.classList.remove("hidden");
}

export function loadingModeDown(contentId, spinnerId, buttonId) {
  var content = document.getElementById(contentId);
  var spinner = document.getElementById(spinnerId);
  var currentButton = document.getElementById(buttonId);

  content.classList.remove("invisible");
  content.classList.add("visible");
  spinner.classList.add("hidden");
  currentButton.classList.remove("cursor-not-allowed");
  currentButton.removeAttribute("disabled");
}
