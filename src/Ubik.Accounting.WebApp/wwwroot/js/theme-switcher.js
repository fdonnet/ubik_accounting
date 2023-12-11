// Change the icons inside the button based on previous settings
export function onLoad() {

}
export function onUpdate() {
  if (localStorage.getItem('color-theme') === 'dark' || (!('color-theme' in localStorage))) {
    document.documentElement.classList.add('dark');
  } else {
    document.documentElement.classList.remove('dark')
  }

  if (localStorage.getItem('color-theme') === 'dark' || (!('color-theme' in localStorage))) {
    themeToggleLightIcon.classList.remove('hidden');
    themeToggleLightIcon.classList.add('block');
    themeToggleDarkIcon.classList.remove('block');
    themeToggleDarkIcon.classList.add('hidden');
  } else {
    themeToggleDarkIcon.classList.remove('hidden');
    themeToggleDarkIcon.classList.add('block');
    themeToggleLightIcon.classList.remove('block');
    themeToggleLightIcon.classList.add('hidden');
  }
}

export function onDispose() {

}


