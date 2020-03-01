using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azurefunctions.servicebusqueue.publishtosignalr
{
    public static class ServiceBusQueuePushFeatureToggleToSignalR
    {
        [FunctionName("ServiceBusQueuePushFeatureToggleToSignalR")]
        public static async Task Run([ServiceBusTrigger("%ServiceBusQueueName%", Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            var featureToggleChangedEvent = JsonConvert.DeserializeObject<FeatureToggleChangedEvent>(myQueueItem);

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            var keyVaultSecret = await keyVaultClient.GetSecretAsync(featureToggleChangedEvent.SignalRConnectionStringVaultUrl);
            var signalRConnectionString = keyVaultSecret.Value;

            var signalRHubName = featureToggleChangedEvent.ConfigurationId.ToString().ToLower();
            var signalRServerHandler = new SignalRServerHandler(signalRConnectionString, signalRHubName); 

            var broadcastObject = new{
                featureToggleName = featureToggleChangedEvent.FeatureName,
                newValue = featureToggleChangedEvent.NewValue
            };

            var broadcastJson = JsonConvert.SerializeObject(broadcastObject);
            log.LogInformation("Broadcasting event: " + broadcastJson);

            try {
                await signalRServerHandler.Broadcast(broadcastJson);
                log.LogInformation("Event broadcasst successfully");
            } 
            catch(Exception e) {
                log.LogError("Error broadcasting event", e);
            }            
        }
    }
}
