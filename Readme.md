1. This tutorial is great to follow along: 
https://docs.microsoft.com/en-us/learn/modules/aks-workshop/04-deploy-mongodb 

2. To deploy SqlServer, need to attach it to Persistent Volume. Follow this link: 
https://docs.microsoft.com/en-us/sql/linux/tutorial-sql-server-containers-kubernetes?view=sql-server-ver15 
Make sure to apply the sql deployment yaml file to the same namespace as Platform Service and Command Service. 

3. To upload the container images to Azure Container Registry, follow this link: 
https://www.youtube.com/watch?v=O5aXcmKc1HU 

docker build -t dotnetmicroserviceregistry.azurecr.io/commandservice . 
docker push dotnetmicroserviceregistry.azurecr.io/commandservice

4. 