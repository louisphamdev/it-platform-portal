# Staging E2E Test Configuration
# Usage: 
#   STAGING=true npm run test:staging
#   or
#   PORTAL_URL=https://staging-portal.itplatform.example.com \
#   API_GATEWAY_URL=https://staging-api.itplatform.example.com \
#   npx playwright test

import { defineConfig, devices } from '@playwright/test';

const stagingConfig = defineConfig({
  testDir: './tests/e2e',
  fullyParallel: false,  // Run sequentially on staging
  retries: 2,
  workers: 1,
  reporter: process.env.CI ? 'github' : 'html',
  use: {
    baseURL: process.env.PORTAL_URL || 'https://staging-portal.itplatform.example.com',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    // Override defaults for staging
    navigationTimeout: 30000,
    actionTimeout: 10000,
  },
  projects: [
    // Only run Chromium on staging to save time
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
  // No webServer - tests run against already-deployed staging
});

export default stagingConfig;
