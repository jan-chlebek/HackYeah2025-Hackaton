/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        // UKNF Primary Colors
        'primary': {
          DEFAULT: '#003366',
          blue: '#003366',
        },
        'accent': {
          DEFAULT: '#0073E6',
          blue: '#0073E6',
        },
        'light': {
          DEFAULT: '#E6F3FF',
          blue: '#E6F3FF',
        },
        // Neutral Colors
        'uknf-white': '#FFFFFF',
        'uknf-gray': {
          light: '#F5F5F5',
          DEFAULT: '#666666',
          medium: '#666666',
          dark: '#333333',
        },
      },
      fontFamily: {
        sans: ['Inter', 'Arial', 'sans-serif'],
      },
    },
  },
  plugins: [],
  // Ensure Tailwind doesn't conflict with PrimeNG
  corePlugins: {
    preflight: true,
  },
}
