using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MonteMonolith.Framework;
using MonteMonolith.Framework.Storage;
using Microsoft.ServiceFabric.Services.Client;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace MonteMonolith.NewFeature.Controllers
{
    [Route("api/[controller]")]
    public class SimulationSchedulerController : Controller
    {
        private HttpCommunicationClientFactory _CommunicationFactory;

        public IHistoryRepository HistoryRepository { get; private set; }

        public SimulationSchedulerController(IHistoryRepository historyRepository)
        {
            this.HistoryRepository = historyRepository;

            _CommunicationFactory = new HttpCommunicationClientFactory(ServicePartitionResolver.GetDefault());
        }

        [HttpGet("schedule/{hour}/{min}/{sec}/{total}/{tMin}/{tMod}/{tMax}")]
        public async Task<string> Schedule(int total, string tMin, string tMod, string tMax, int hour, int min, int sec)
        {
            var serviceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/MonteMonolithDotNetCore");

            //NOTE: this is just for demonstration purposes, not something you'd want to do in real world
            await Task.Delay(TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(min) + TimeSpan.FromSeconds(sec));

            try
            {
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
            }
            catch (Exception ex)
            {
                //TODO: log exception
                throw;
            }
        }
    }
}
