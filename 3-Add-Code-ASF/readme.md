# Part 3 - Add new features as Microservices

### Development continues in this new world

At this stage development is still ongoing, so instead of adding code to the original monolith, lets add new features as services.

The ficticious new feature we'll add in this stage of the project is the imaginatively named NewFeature project which includes an API for scheduling a simulation. This is a simple API endpoint that accepts 3 additional parameters as part of the GET request, the hour, minute and second to delay before running the simulation.

> Here is what we would pass to the simluation scheduler API:

```json 
http://localhost:8334/api/simulationscheduler/schedule/0/0/5/2/11,12/12.5,13/17,15 
```

Where the delay is 00:00:05

Inside that action in the controller, you'll notice that we ask the FabricRuntime for the URI of the simulate service that we'll call:

```csharp
var serviceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/MonteMonolithDotNetCore");
```

..and that we make use of a `ServicePartitionClient` from the Fabric SDK along with an implementation of `ICommunicationClient` - specifically the `HttpCommunicationClient` class in the `MonteMonolith.Framework' project:

```csharp
var partitionClient = new ServicePartitionClient<HttpCommunicationClient>(_CommunicationFactory, serviceUri);
var toReturn = string.Empty;

await partitionClient.InvokeWithRetryAsync(
    async (client) =>
    {
        var response = await client.HttpClient.GetAsync(new Uri(client.Url,
                 "/api/simulation/simulate/" + total + "/" + tMin + "/" + tMod + "/" + tMax));
        toReturn = await response.Content.ReadAsStringAsync();
     });

return toReturn;
```

The above chunk of code calls into the service hosted within the cluster that's responsible for the simulation.
