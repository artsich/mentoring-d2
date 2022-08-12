# az login
az group create -l westus -n DeployTaskGroup
az appservice plan create --name free-plan-task --resource-group DeployTaskGroup --sku F1
Set-Location ../WebAppDeploy
az webapp up --name web-app-task --plan free-plan-task --resource-group DeployTaskGroup
