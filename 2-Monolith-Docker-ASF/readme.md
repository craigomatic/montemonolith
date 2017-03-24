# Part 2 - Converting an ASP.net App to a Guest Container Service in Azure Service Fabric

In this example we will be taking our Monte Carlo Simulator Application from Part 1 and converting it into a MicroService in Azure Service Fabric. Service Fabric currently supports deployment of Docker containers on Linux and Windows Server containers on Windows Server 2016. Support for Hyper-V containers will be added in a future release. In the Service Fabric application model, a container represents an application host in which multiple service replicas are placed. In this scenario we will be using Guest Containers.
Guest containers: Are existing applications you can deploy in a container. Examples include Node.js, JavaScript, or any code (executables).

> Service Fabric enables you to build and manage scalable and reliable applications composed of microservices that run at very high density on a shared pool of machines, which is referred to as a cluster. It provides a runtime to build distributed, scalable, stateless and stateful microservices. It also provides comprehensive application management capabilities to provision, deploy, monitor, upgrade/patch, and delete deployed applications.

<li><a href="##Prerequisites">Prerequisites</a></li>
<li><a href="##Convert to Docker Image">Convert ASP.Net App to a Docker Image</a></li>
<li><a href="##Publish to Docker Hub">Publish Docker Image to a Container Registry</a></li>
<li><a href="##Deploy Service Fabric to Azure">Deploy a Service Fabric Cluster on Azure</a></li>
<li><a href="#install">Deploy Docker Image as a Stateless Service on Service Fabric</a></li>

<img src="https://rtwrt.blob.core.windows.net/post2-monteasf/img3.png" width="700">



## Prerequisites
  - Windows 10
> With the Windows 10 anniversay edition you may need to turn off Secure Boot in the BIOS of your machine in order for the SF cluster to run locally
  - [Visual Studio 2015 with Update 2 or Later](http://www.microsoft.com/web/handlers/webpi.ashx?command=getinstallerredirect&appid=MicrosoftAzure-ServiceFabric-VS2015)
  - [Service Fabric Runtime](https://www.microsoft.com/web/handlers/webpi.ashx?command=getinstallerredirect&appid=MicrosoftAzure-ServiceFabric-VS2015) & [SDK](http://www.microsoft.com/web/handlers/webpi.ashx?command=getinstallerredirect&appid=MicrosoftAzure-ServiceFabric-VS2015)
  - Windows Powershell
  - Visual Studio Team Services Account
  - Microsoft Azure Subscription
  - [Docker for windows](https://store.docker.com/editions/community/docker-ce-desktop-windows?tab=description)


## Convert to Docker Image

### Publish Profiles

In this section we'll be wrapping our pushing our application into a ASP Docker Image.

We'll begin by opening up the original Monolith application in Part 1. Open the 1-Original-Monolith.sln file in Visual Studio.

Once the solution is open, **Build** the projects inside the IDE.

Next right click the MonteMonolith project and select **Publish**.

<img src="https://rtwrt.blob.core.windows.net/post2-monteasf/img4.png" width="700">

Inside the Publish Profile window select the **Custom** option. This will alow us to publish artifacts to a directory for our Docker Container to reference.

<img src="https://rtwrt.blob.core.windows.net/post2-monteasf/img5.PNG" width="700">

Enter an arbitrary profile name, in this example we simply called it *Containerimage*.

In the next window for Connection, Select **File System** as the Publish Method.

<img src="https://rtwrt.blob.core.windows.net/post2-monteasf/img6.PNG" width="700">

In the Target location, Create a new folder called 'PublishOutput' and use this as the target for your web application artifacts. Your target location should look similar to the following:

```gulp 
*\1-Original-Monolith\MonteMonolith\PublishOutput 
```
Open the File Publish Options section of the Settings tab. Select **Precompile during publishing**. This optimization means that you'll be compiling views in the Docker container, you are copying the precompiled views.

<img src="https://docs.microsoft.com/en-us/aspnet/mvc/overview/deployment/media/aspnetmvc/publishsettings.png" width="700">

Select **Next** and Click **Publish** in that window.

This packages and compresses your application dlls, and environmet parameters to a directory.

``` Powershell
1>------ Build started: Project: MonteMonolith.Framework, Configuration: Release Any CPU ------
1>  MonteMonolith.Framework -> C:\Users\naros\Documents\GitHub\MicroservicesMonolith\1-Original-Monolith\MonteMonolith.Framework\bin\Release\MonteMonolith.Framework.dll
2>------ Build started: Project: MonteMonolith, Configuration: Release Any CPU ------
2>  MonteMonolith -> C:\Users\naros\Documents\GitHub\MicroservicesMonolith\1-Original-Monolith\MonteMonolith\bin\MonteMonolith.dll
3>------ Publish started: Project: MonteMonolith, Configuration: Release Any CPU ------
3>Connecting to C:\Users\naros\Documents\GitHub\montemonolith\1-Original-Monolith\MonteMonolith\Bin...
3>Transformed Web.config using C:\Users\naros\Documents\GitHub\MicroservicesMonolith\1-Original-Monolith\MonteMonolith\Web.Release.config into obj\Release\TransformWebConfig\transformed\Web.config.
3>Copying all files to temporary location below for package/publish:
3>obj\Release\Package\PackageTmp.
3>Publishing folder /...
3>Publishing folder Areas...
3>Publishing folder Areas/HelpPage...
3>Publishing folder Areas/HelpPage/Views...
3>Publishing folder Areas/HelpPage/Views/Help...
3>Publishing folder Areas/HelpPage/Views/Help/DisplayTemplates...
3>Publishing folder Areas/HelpPage/Views/Shared...
3>Publishing folder bin...
3>Publishing folder bin/roslyn...
3>Publishing folder Content...
3>Publishing folder fonts...
3>Publishing folder Scripts...
3>Publishing folder Service References...
3>Publishing folder Service References/Application Insights...
3>Publishing folder Views...
3>Publishing folder Views/Analysis...
3>Publishing folder Views/Home...
3>Publishing folder Views/Shared...
3>Web App was published successfully file:///C:/Users/naros/Documents/GitHub/montemonolith/1-Original-Monolith/MonteMonolith/Bin
3>
========== Build: 2 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========
========== Publish: 1 succeeded, 0 failed, 0 skipped ==========

```

### Docker Containers



Now that we have our publish profile, we will create a docker image from the app parameters.

After installing and starting Docker, right-click on the tray icon and select Switch to Windows containers. This is required to run Docker images based on Windows. This command takes a few seconds to execute:

<img src="https://docs.microsoft.com/en-us/aspnet/mvc/overview/deployment/media/aspnetmvc/switchcontainer.png" width="300">


Return back to explorer.

> Define your Docker image in a Dockerfile. The Dockerfile contains instructions for the base image, additional components, the app you want to run, and other configuration images. The Dockerfile is the input to the docker build command, which creates the image.

> You will build an image based on the microsft/aspnet image located on Docker Hub. The base image, microsoft/aspnet, is a Windows Server image. In contains Windows Server Core, IIS and ASP.NET 4.6.2. When you run this image in your container, it will automatically start IIS and installed websites.

Open *Visual Studio Code* or a text editor.

Copy the following DockerFile and save and name your file as DockerFile.

```Docker
# The `FROM` instruction specifies the base image. You are
# extending the `microsoft/aspnet` image.

FROM microsoft/aspnet

# Next, this Dockerfile creates a directory for your application
RUN mkdir C:\monteDocker

# configure the new site in IIS.
RUN powershell -NoProfile -Command \
    Import-module IISAdministration; \
    New-IISSite -Name "ASPNET" -PhysicalPath C:\monteDocker -BindingInformation "*:8000:"

# This instruction tells the container to listen on port 8000.
EXPOSE 8000

# The final instruction copies the site you published earlier into the container.
ADD . /monteDocker
```

Save the Dockerfile inside your Publish Output directory.

Run the Docker build command to create the image that runs your ASP.NET app. To do this, open a PowerShell window in the directory of your project and type the following command in the solution directory:

``` console
docker build -t montedocker .
```
Here is what your output should look like:

``` Powershell

Step 1/5 : FROM microsoft/aspnet
 ---> e761eca2f8df
Step 2/5 : RUN mkdir C:\monteDocker
 ---> Using cache
 ---> 5399a1cabd29
Step 3/5 : RUN powershell -NoProfile -Command     Import-module IISAdministration;     New-IISSite -Name "ASPNET" -Physi
calPath C:\monteDocker -BindingInformation "*:8000:"
 ---> Using cache
 ---> cca8bedb7b6d
Step 4/5 : EXPOSE 8000
 ---> Using cache
 ---> 76a8c917354b
Step 5/5 : ADD . /monteDocker
 ---> 19b7c64ce388
Removing intermediate container 5f12c940e17b
Successfully built 19b7c64ce388

```

Once that command completes, you can run the **docker images** command to see information on the new image.

Start a container by executing the following docker run command.

``` docker
docker run -d --name aspmontedocker montedocker
```

We now have a working Docker image with our ASP.NET application deployed inside the container.


## Publish to Docker Hub

Now that we have our image, we want to deploy our image to a container registy for our Azure Service Fabric Cluster to pull the image and deploy it into the runtime. You have the choice of using Docker Hub as a Docker container Registry, Azure Container Registry or a local Docker Registry for Service fabric to pull your image. (We use Docker Hub in this example)

Sign up for Docker Hub.

<img src="https://rtwrt.blob.core.windows.net/post2-monteasf/img7.PNG" width="700">

 > Be sure you create a namespace for your Docker images

 First, login to your docker account using the **docker login command**.

 ``` Docker

docker login

Login with your Docker ID to push and pull images from Docker Hub. If you don't have a Docker ID, head over to https://hub.docker.com to create one.

Username: user
Password: Password

Login Succeeded

```

Once logged in, the container image can be pushed to Docker Hub. To do so, use the **docker push** command. Replace 'user' with your Docker ID.

```Docker

docker push <user>/montedocker

```

The container image can now be downloaded from Docker Hub onto any Windows container host using **docker pull**

<img src="https://rtwrt.blob.core.windows.net/post2-monteasf/img8.PNG" width="700">



## Deploy Service Fabric to Azure

Now that we have our Container image in our Docker Hub Repo, we will now deploy a Service Fabric cluster for us to later deploy a container as a Guest Executable.

> At the time of this write-up, Windows 10 image VMs or local machines are unable to support Guest Containers in the Service Fabric Runtime.

Navigate to this directory \*2-Monolith-Docker-ASF\MonteMicroservice

Inside the directory open the parameters.json folder.

Switch out the following two parameters with others of your choosing
``` json
 "adminUserName": {
            "value": "nmrose"
        },
 "adminPassword": {
            "value": "azureIsAw3s0me!"
            ```
