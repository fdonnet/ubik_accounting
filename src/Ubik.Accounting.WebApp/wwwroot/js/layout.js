var isNavOpen = false;

function toggleNav() {
  var showIcon = document.getElementById("navShowIcon");
  var closeIcon = document.getElementById("navCloseIcon");
  var topNav = document.getElementById("topNav");

  if (isNavOpen) {
    showIcon.classList.remove("hidden");
    closeIcon.classList.remove("block");
    showIcon.classList.add("block");
    closeIcon.classList.add("hidden");
    topNav.classList.remove("block")
    topNav.classList.add("hidden")
  }
  else {
    showIcon.classList.remove("block");
    closeIcon.classList.remove("hidden");
    showIcon.classList.add("hidden");
    closeIcon.classList.add("block");
    topNav.classList.remove("hidden")
    topNav.classList.add("block")
  }
  isNavOpen = !isNavOpen;
};

var themeToggleDarkIcon = document.getElementById('theme-toggle-dark-icon');
var themeToggleLightIcon = document.getElementById('theme-toggle-light-icon');
var themeToggleBtn = document.getElementById('theme-toggle');
themeToggleBtn.addEventListener('click', function () {

  // toggle icons inside button
  themeToggleDarkIcon.classList.toggle('hidden');
  themeToggleLightIcon.classList.toggle('hidden');

  // if set via local storage previously
  if (localStorage.getItem('color-theme')) {
    if (localStorage.getItem('color-theme') === 'light') {
      document.documentElement.classList.add('dark');
      localStorage.setItem('color-theme', 'dark');
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('color-theme', 'light');
    }

    // if NOT set via local storage previously
  } else {
    if (document.documentElement.classList.contains('dark')) {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('color-theme', 'light');
    } else {
      document.documentElement.classList.add('dark');
      localStorage.setItem('color-theme', 'dark');
    }
  }
});


