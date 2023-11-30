export function openDialog(dialogId) {
  document.getElementById(dialogId).showModal();
}

export function closeDialog(dialogId) {
  document.getElementById(dialogId).close();
}

export function clickOutside(dialogId, insideDialogId) {
  var dialog = document.getElementById(dialogId)
  var insideDialog = document.getElementById(insideDialogId)
  dialog.addEventListener('click', function (e) {
    console.info(e.target.tagName);
    if (e.target.tagName === 'DIALOG') dialog.close()
  });
}




