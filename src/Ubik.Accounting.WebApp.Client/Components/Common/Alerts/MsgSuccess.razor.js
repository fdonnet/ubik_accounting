export function showSuccess(alertId) {
  var panel = document.getElementById(alertId);
  panel.classList.remove("hidden");
  panel.classList.add("block");
}
