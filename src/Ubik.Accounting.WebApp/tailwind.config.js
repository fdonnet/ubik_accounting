/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    './**/*.html',
    './**/*.razor',
    '../Ubik.Accounting.WebApp.Client/**/*.razor'
  ],
  mode: 'jit',
  theme: {
    extend: {},
  },
  plugins: [],
}
