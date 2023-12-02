/** @type {import('tailwindcss').Config} */
const defaultTheme = require('tailwindcss/defaultTheme')
module.exports = {
  darkMode: 'class',
  content: [
    './**/*.html',
    './**/*.razor',
    '../Ubik.Accounting.WebApp.Client/Components/**/*.razor'
  ],
  mode: 'jit',
  theme: {
    extend: {},
  },
  plugins: [],
}

