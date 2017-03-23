# Design Day Companion Code for Microservices with Service Fabric

This repository takes a basic ASP.net “monolithic” project and steps through the stages a developer might take to migrate to a microservices based architecture using Azure Service Fabric. 

The project contains a simulation API that allows requests for a Monte-Carlo simulation to be made and returns JSON representing the result set.

The project also contains an analysis controller that displays a basic chart of the results.

## Progression

### Stage 1
Base ASP.net application in “monolithic” form.

### Stage 2
Host ASP.NET application as-is inside of a Docker container on Service Fabric OR migrate to ASP.NET core

### Stage 3
ASP.NET core monolith, SimulationScheduler added as a new Microservice 

### Stage 4
ASP.NET core monolith, Simulation Scheduler and simulate feature from monolith decomposed into it’s own service

### Pre-requisites
* Azure SDK
*	Service Fabric SDK 2.4
* .NET Core 1.1
*	Azure Blob Storage account OR Azure Storage Emulator running

### Things to know
* Visual Studio should be run as Administrator when you want to deploy to the cluster on your dev machine
