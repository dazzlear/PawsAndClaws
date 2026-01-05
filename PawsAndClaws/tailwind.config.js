/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./Pages/**/*.cshtml",
    "./wwwroot/**/*.html",
    "./Program.cs",
    "./**/*.razor",
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ["Poppins", "sans-serif"],
        lilita: ["Lilita One", "cursive"],
      },
    },
  },
  plugins: [],
};
