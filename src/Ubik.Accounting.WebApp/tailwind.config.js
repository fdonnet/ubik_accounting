/** @type {import('tailwindcss').Config} */
module.exports = {
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
