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

