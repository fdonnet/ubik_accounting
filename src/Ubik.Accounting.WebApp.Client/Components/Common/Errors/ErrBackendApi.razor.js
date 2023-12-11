export function showError(alertId) {
  var panel = document.getElementById(alertId);
  panel.classList.remove("block");
  panel.classList.add("hidden");
}
export function dismissError(alertId) {
  var panel = document.getElementById(alertId);
  panel.classList.remove("block");
  panel.classList.add("hidden");
}

export function showErrorDetails(detailId) {
  var panel = document.getElementById(detailId);

  if (panel.classList.contains("hidden")) {
    panel.classList.remove("hidden");
    panel.classList.add("block");
  }
  else {
    panel.classList.remove("block");
    panel.classList.add("hidden");
  }
  
}
