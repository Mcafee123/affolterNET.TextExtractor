#!/bin/bash

# author: martin@affolter.net
set -e

. _local_infra_config.sh

# stop and remove containers and network
docker container stop $azuritecont

docker rm $azuritecont
docker network rm "$network"
