export function showSuccess(alertId) {
  var panel = document.getElementById(alertId);
  panel.classList.remove("hidden");
  panel.classList.add("block");
}

export function closeSuccess(alertId) {
  var panel = document.getElementById(alertId);
  if (panel.classList.contains("block")) {
    panel.classList.remove("block");
    panel.classList.add("hidden");
  }
}
