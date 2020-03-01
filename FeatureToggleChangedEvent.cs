using System;

namespace azurefunctions.servicebusqueue.publishtosignalr
{
    public class FeatureToggleChangedEvent
    {
        public string SignalRConnectionStringVaultUrl {get; set;}
        public Guid ConfigurationId {get; set;}
        public string FeatureName {get; set;}
        public bool NewValue {get; set;}
    }
}
