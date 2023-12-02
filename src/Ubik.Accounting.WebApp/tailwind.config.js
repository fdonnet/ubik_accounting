/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  fontFamily: {
    'sans': [
      'Inter',
      'ui-sans-serif',
      'system-ui',
      '-apple-system',
      'system-ui',
      'Segoe UI',
      'Roboto',
      'Helvetica Neue',
      'Arial',
      'Noto Sans',
      'sans-serif',
      'Apple Color Emoji',
      'Segoe UI Emoji',
      'Segoe UI Symbol',
      'Noto Color Emoji'
    ],
    'body': [
      'Inter',
      'ui-sans-serif',
      'system-ui',
      '-apple-system',
      'system-ui',
      'Segoe UI',
      'Roboto',
      'Helvetica Neue',
      'Arial',
      'Noto Sans',
      'sans-serif',
      'Apple Color Emoji',
      'Segoe UI Emoji',
      'Segoe UI Symbol',
      'Noto Color Emoji'
    ],
  },
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

