#!/bin/bash

# author: martin@affolter.net

# application insights: az extension add -n application-insights
# brew tap azure/functions
# brew install azure-functions-core-tools@3

# DEPLOY DOMAIN NAME FOR TICKET APP
set -e

env=${1:-prod}

. _config.sh $env

# add custom domain name
az staticwebapp hostname set  -n "$swaAppName" --hostname "$application_url"
