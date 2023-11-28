/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  fontFamily: {
    'body': [
      'Inter',
      'ui-sans-serif',
      'system-ui',
      // other fallback fonts
    ],
    'sans': [
      'Inter',
      'ui-sans-serif',
      'system-ui',
      // other fallback fonts
    ]
  },
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

