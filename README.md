# azurefunctions.servicebusqueue.publishtosignalr
Listens to feature toggle change messages published to a Service Bus queue, and sends these for onwards transmission to the SignalR hub, which will push out to any/all connected clients.
