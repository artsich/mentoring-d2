#az login
az group create -l westus -n DeployTaskGroupDocker
az appservice plan create -g DeployTaskGroupDocker -n free-plan-task-docker  --is-linux --sku F1
az webapp create -n web-app-task-docker --plan free-plan-task-docker -g DeployTaskGroupDocker -i jillydockerid/webappdeploy:latest
