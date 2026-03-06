import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react({
      babel: {
        plugins: [['babel-plugin-react-compiler']],
      },
    }),
  ],
  server: {
    proxy: {
      // sve zahteve ka /api prosledi na tvoj backend
      "/api": {
        target: "http://localhost:5104", // tvoj backend URL
        changeOrigin: true,
        secure: false, // dozvoljava self-signed certifikat
      },
    },
  },
})