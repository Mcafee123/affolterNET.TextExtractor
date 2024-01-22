#!/bin/bash

# author: martin@affolter.net
set -e

. _local_infra_config.sh

docker network create "$network"

echo "run: $dockerhub_prefix/$azuritecont:$azurite_tag"
docker run -d --name="$azuritecont" --network $network -p 3000:3000 -p 10000:10000 -p 10001:10001 -p 10002:10002 "$dockerhub_prefix/$azuritecont:$azurite_tag"
