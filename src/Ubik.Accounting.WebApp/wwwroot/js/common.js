function toggleVisibility(elementId) {
  var element = document.getElementById(elementId);

  if (element.classList.contains("block")) {
    element.classList.remove("block");
    element.classList.add("hidden");
  }
  else {
    if (element.classList.contains("hidden")) {
      element.classList.remove("hidden");
      element.classList.add("block");
    }
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

function buttonLoadingModeUp(contentId, spinnerId, buttonId) {
  var content = document.getElementById(contentId);
  var spinner = document.getElementById(spinnerId);
  var currentButton = document.getElementById(buttonId);

  currentButton.setAttribute("disabled", "disabled");
  currentButton.classList.add("cursor-not-allowed");
  content.classList.remove("visible");
  content.classList.add("invisible");
  spinner.classList.remove("hidden");
}

function buttonLoadingModeDown(contentId, spinnerId, buttonId) {
  var content = document.getElementById(contentId);
  var spinner = document.getElementById(spinnerId);
  var currentButton = document.getElementById(buttonId);

  content.classList.remove("invisible");
  content.classList.add("visible");
  spinner.classList.add("hidden");
  currentButton.classList.remove("cursor-not-allowed");
  currentButton.removeAttribute("disabled");
}

function openDialog(dialogId) {
  document.getElementById(dialogId).showModal();
}

function closeDialog(dialogId) {
  document.getElementById(dialogId).close();
}

function closeOnClickOutsideDialog(dialogId) {
  var dialog = document.getElementById(dialogId)

  dialog.addEventListener('click', function (e) {
    console.info(e.target.tagName);
    if (e.target.tagName === 'DIALOG') dialog.close()
  });
}
