# syntax=docker/dockerfile:1

ARG NODE_VERSION=22.13.1

# --- Build Stage ---
FROM node:${NODE_VERSION}-slim AS builder
WORKDIR /app

# Install dependencies (only package.json and package-lock.json for cache efficiency)
COPY --link package.json package-lock.json ./
RUN --mount=type=cache,target=/root/.npm \
    npm ci

# Copy the rest of the application source
COPY --link . .

# Build the app (Vite build)
RUN npm run build

# Remove dev dependencies to reduce image size
RUN npm prune --production

# --- Production Stage ---
FROM node:${NODE_VERSION}-slim AS final
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser

# Copy built app and production dependencies
COPY --from=builder /app/dist ./dist
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/package.json ./

# Copy public assets (Vite serves from /public at build time, but static servers may need it)
COPY --from=builder /app/public ./public

ENV NODE_ENV=production
ENV NODE_OPTIONS="--max-old-space-size=4096"

USER appuser

EXPOSE 4000

# Use a simple static file server for the built frontend (e.g., serve)
# Install 'serve' globally (not in node_modules) for static serving
RUN npm install -g serve@15.0.3

CMD ["serve", "-s", "dist", "-l", "4000"]
