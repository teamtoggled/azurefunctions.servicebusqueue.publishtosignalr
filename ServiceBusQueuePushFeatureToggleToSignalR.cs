using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace azurefunctions.servicebusqueue.publishtosignalr
{
    public static class ServiceBusQueuePushFeatureToggleToSignalR
    {
        [FunctionName("ServiceBusQueuePushFeatureToggleToSignalR")]
        public static async Task Run([ServiceBusTrigger("%ServiceBusQueueName%", Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            var signalRConnectionString = Environment.GetEnvironmentVariable("SignalRConnectionString");
            var signalRHubName = Environment.GetEnvironmentVariable("SignalRHubName");

            log.LogInformation($"Going to push message now, to hub {signalRHubName}");

            var signalRServerHandler = new SignalRServerHandler(signalRConnectionString, signalRHubName);            
            await signalRServerHandler.Broadcast("This is a message from Azure Functions!");

            log.LogInformation("Pushed the message.");
        }
    }
}
