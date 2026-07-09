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
            "/api/clients": {
                target: "http://localhost:5263",
                changeOrigin: true,
                secure: false,
            },
            "/api/Advertisements": {
                target: "http://localhost:5104",
                changeOrigin: true,
                secure: false,
            },
            "/api/Companies": {
                target: "http://localhost:5287",
                changeOrigin: true,
                secure: false,
            },
            "/api/notifications": {
                target: "http://localhost:5240",
                changeOrigin: true,
                secure: false,
            },
            "/api/job-applications": {
                target: "http://localhost:5291",
                changeOrigin: true,
                secure: false,
            },
            "/api": {
                target: "http://localhost:5218",
                changeOrigin: true,
                secure: false,
            },
        },
    },
})