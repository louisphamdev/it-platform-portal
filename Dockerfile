FROM alpine:latest
WORKDIR /app
COPY src/portal-shell/ ./src/
COPY src/services/ ./services/
COPY infra/ ./infra/
RUN echo "placeholder build complete"
