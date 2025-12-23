import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

import fs from 'fs'

// Check if SSL certificates exist (for local development only)
const certKeyPath = './certs/dev-cert.key'
const certPemPath = './certs/dev-cert.pem'
const hasSSLCerts = fs.existsSync(certKeyPath) && fs.existsSync(certPemPath)

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    ...(hasSSLCerts && {
      https: {
        key: fs.readFileSync(certKeyPath),
        cert: fs.readFileSync(certPemPath),
      }
    }),
    port: 5173,
    strictPort: true,
    host: 'localhost',
    hmr: {
      // Don't specify host/port - let HMR connect to same origin
      // When accessed via localhost:5000, YARP will proxy WebSocket to Vite
      protocol: 'wss',
      overlay: true
    },
    watch: {
      usePolling: true
    }
  },
  optimizeDeps: {
    force: true
  },
  build: {
    outDir: '../backend/affolterNET.TextExtractor.Web/wwwroot',
    emptyOutDir: true,
    rollupOptions: {
      output: {
        entryFileNames: 'js/[name]-[hash].js',
        chunkFileNames: 'js/[name]-[hash].js',
        assetFileNames: (assetInfo) => {
          if (/\.(css)$/.test(assetInfo.name || '')) {
            return 'css/[name]-[hash].[ext]';
          }
          if (/\.(png|jpe?g|svg|gif|tiff|bmp|ico)$/i.test(assetInfo.name || '')) {
            return 'images/[name]-[hash].[ext]';
          }
          return 'assets/[name]-[hash].[ext]';
        }
      }
    }
  }
})
