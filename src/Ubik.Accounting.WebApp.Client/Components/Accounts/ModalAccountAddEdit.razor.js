export function openDialog(dialogId) {
  document.getElementById(dialogId).showModal();
}

export function closeDialog(dialogId) {
  document.getElementById(dialogId).close();
}

export function clickOutside(dialogId) {
  var dialog = document.getElementById(dialogId);

  dialog.addEventListener('click', (event) => {
    if (event.target.id !== 'inside-modal') {
      dialog.close();
    }
  });
}




