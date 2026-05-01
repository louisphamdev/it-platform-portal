# Portal Shell - Next.js React Application
# Build context: . (repo root)
# Usage: docker build -f src/portal-shell/Dockerfile .

FROM node:20-alpine AS base
WORKDIR /app
EXPOSE 3000
ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

# Install dependencies stage
FROM base AS deps
WORKDIR /app
# Copy package files and install dependencies (no lock file, so use install)
COPY src/portal-shell/package.json ./
RUN npm install

# Development image
FROM base AS dev
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY src/portal-shell/ ./
ENV NODE_ENV=development
CMD ["npm", "run", "dev"]

# Build the application
FROM deps AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY src/portal-shell/ ./
RUN npm run build

# Production image
FROM base AS runner
WORKDIR /app
ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

RUN addgroup --system --gid 1001 nodejs && \
    adduser --system --uid 1001 nextjs

COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static
COPY --from=builder /app/public ./public

USER nextjs
EXPOSE 3000
ENV PORT=3000
CMD ["node", "server.js"]