/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: '#0F172A',
        accent: '#2563EB',
        success: '#16A34A',
        muted: '#6B7280',
        background: '#F5F5F5',
      },
      borderRadius: {
        'xl': '24px',
      },
    },
  },
  plugins: [],
}
