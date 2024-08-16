#!/bin/bash

# author: martin@affolter.net

# DEPLOY INFRASTRUCTURE FOR TICKET APP
set -e

env=${1:-prod}

. _config.sh $env

# az_login
set_subscription

token=$(az staticwebapp secrets list --name affolternet-textextractor-prod-swa --query "properties.apiKey")
echo "token: $token"

pushd .

cd ../frontend
npm run build
# swa deploy --env prod --api-language csharp --api-version 8.0
swa deploy affolternet-textextractor \
  --env preview \
  -V silly

popd