#!/bin/bash

# author: martin@affolter.net

set -e

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
echo "DIR: $DIR"

env=${1:-dev}

terraform destroy
