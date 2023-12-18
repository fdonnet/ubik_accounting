export function openCloseAccountFilters(filtersId) {
  var filters = document.getElementById(filtersId);

  if (filters.classList.contains("block"))
  {
    filters.classList.remove("block");
    filters.classList.add("hidden");
  }
  else {
    filters.classList.remove("hidden");
    filters.classList.add("block");
  }
}
