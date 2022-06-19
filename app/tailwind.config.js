const colors = require('tailwindcss/colors')

module.exports = {
  content: [
    './src/components/*.{html,js,tsx,ts}',
    './src/pages/*.{html,js,tsx,ts}',
    './src/index.html',
    './src/pages/Home.tsx',
    './src/*.tsx',
  ],
  theme: {
    maxWidth: {
      '1/4': '25%',
      '1/2': '50%',
      '200px': '200px',
    },
    screens: {
      'xs': '250px',
      'sm': '500px',
      'md': '768px',
      'lg': '1024px',
      'xl': '1280px',
    },
    extend: {
      fontFamily: {
        'poppins': ['Poppins', 'sans-serif'],
        'comforter': ['Comforter', 'cursive'], 
      },
      spacing:{
        '1px': '1px',
        '10%': '10%',
        '95%': '95%',
        '18': '4.5rem',
      },
      opacity: {
        '1': '.01',
        '2': '.02',
        '3': '.03',
        '4': '.04',
        '96': '.96',
        '97': '.97',
        '98': '.98',
        '99': '.99',
      },
      brightness: {
        20: '.20',
        25: '.25',
        30: '.30',
        35: '.35',
        40: '.40',
        45: '.45',
        50: '.50',
        55: '.55',
        60: '.60',
        65: '.65',
        70: '.70',
        75: '.75',
        80: '.80',
        175: '1.75',
      },
      boxShadow: {
        '4sides': '0 0px 5px 5px rgba(0, 0, 0, 0.15)',
      }
    },
    colors: {
      transparent: 'transparent',
      current: 'currentColor',
      black: {
          100: "#F3F6FB",
          200: "#999999",
          300: "#666666",
          400: "#333333",
          500: "#222222",
          600: "#111111",
          700: "#000000",
          800: "#000000",
          900: "#000000"
        },
      white: {
          100: "#ffffff",
          200: "#ffffff",
          300: "#ffffff",
          400: "#ffffff",
          500: "#ffffff",
          600: "#cccccc",
          700: "#999999",
          800: "#666666",
          900: "#333333"
      },
      red: {
          100: "#f8d3d7",
          200: "#f2a6af",
          300: "#eb7a87",
          400: "#e54d5f",
          500: "#de2137",
          600: "#b21a2c",
          700: "#851421",
          800: "#590d16",
          900: "#2c070b"
      },
      green: {
          100: "#d4edda",
          200: "#a9dcb5",
          300: "#7eca8f",
          400: "#53b96a",
          500: "#28a745",
          600: "#208637",
          700: "#186429",
          800: "#10431c",
          900: "#08210e"
      },
      blue: {
          100: "#ccccf5",
          200: "#9999eb",
          300: "#6666e1",
          400: "#3333d7",
          500: "#0000cd",
          600: "#0000a4",
          700: "#00007b",
          800: "#000052",
          900: "#000029"
},
    },
  },
  plugins: [],
}
