import '@mantine/core/styles.css'
import '@mantine/notifications/styles.css'
import './index.css'

import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

import { App } from '@/app/App'

const container = document.getElementById('root')

if (container === null) {
  throw new Error('index.html has no #root element to mount into.')
}

createRoot(container).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
