import { fileURLToPath, URL } from 'node:url'

import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

/**
 * The backend the dev server proxies to. Inside Docker Compose this is the
 * service name; on a developer's machine it is localhost.
 */
const backendUrl = process.env.BACKEND_URL ?? 'http://localhost:8080'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    host: true,
    port: 5173,
    // The API and the MCP route are proxied rather than called across origins,
    // so development matches production: one origin, no CORS configuration.
    proxy: {
      '/api': { target: backendUrl, changeOrigin: true },
      '/mcp': { target: backendUrl, changeOrigin: true },
    },
  },
  build: {
    // In production the backend serves these files. See ADR-0003.
    outDir: '../backend/src/Platform.Host/wwwroot',
    emptyOutDir: true,
  },
})
