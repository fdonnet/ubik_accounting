export function openDialog(dialogId) {
  document.getElementById(dialogId).showModal();
}

export function closeDialog(dialogId) {
  document.getElementById(dialogId).close();
}

export function clickOutside(dialogId) {
  var dialog = document.getElementById(dialogId)
  
  dialog.addEventListener('click', function (e) {
    console.info(e.target.tagName);
    if (e.target.tagName === 'DIALOG') dialog.close()
  });
}




