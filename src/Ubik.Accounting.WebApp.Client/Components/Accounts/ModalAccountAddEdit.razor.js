export function openDialog(dialogId) {
  document.getElementById(dialogId).showModal();
}

export function closeDialog(dialogId) {
  document.getElementById(dialogId).close();
}
