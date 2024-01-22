#!/bin/bash

# author: martin@affolter.net

set -e

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
echo "DIR: $DIR"

env=$1

if [ -z "$env" ]; then
    echo "environment not set"
    exit 99
fi

. $DIR/_func.sh

tenant_id="9c0f6304-c41a-4891-8379-ed3cbfc54535"
subscription="affolter.NET MPN"
# devops settings
proj="affolterNET.TextExtractor"
lower_basename=$(echo $(tr '[:upper:]' '[:lower:]' <<< "$proj"))
basename=${lower_basename/./-}
short_basename="${basename/affolternet/an}"
location="westeurope"
instance="https://login.microsoftonline.com/"
# SENDGRID_FROM_EMAIL="donotreply@affolter.net"
# SENDGRID_FROM_NAME="Ticket Cloud Service"

application_url="ext.affolter.net"
branch_name="main"
github_user="Mcafee123"
github_repo="affolterNET.TextExtractor"
static_site_repo="https://github.com/$github_user/$github_repo"

export GITHUB_USER_NAME="$github_user"

swaAppName="$basename-$env-swa"
logAnalyticsWorkspace="$basename-$env-law"

# rg name
resourceGroup="rg_${basename/-/_}_$env"

# storage name
storageAccountName="${short_basename/-/}${env}stor"

# # function app name
# functionsAppName="$basename-$env-fnc"
# functionsAppUrl="https://$functionsAppName.azurewebsites.net"
# functionsAppDir="../func"