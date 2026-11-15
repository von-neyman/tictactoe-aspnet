#!/usr/bin/env bash
set -e

CONTAINER_NAME="$1"
MAX_ATTEMPTS=10

for i in $(seq 1 $MAX_ATTEMPTS); do
  status=$(docker inspect -f '{{.State.Health.Status}}' "$CONTAINER_NAME" 2>/dev/null || echo "missing")
  if [ "$status" = "healthy" ]; then
    echo "Container $CONTAINER_NAME is healthy"
    exit 0
  fi
  if [ "$status" = "unhealthy" ]; then
    echo "Container $CONTAINER_NAME is unhealthy"
    docker logs "$CONTAINER_NAME"
    docker rm -f "$CONTAINER_NAME"
    exit 1
  fi
  sleep 1
done

echo "Container $CONTAINER_NAME did not become healthy in $MAX_ATTEMPTS seconds"
docker logs "$CONTAINER_NAME"
docker rm -f "$CONTAINER_NAME"
exit 1