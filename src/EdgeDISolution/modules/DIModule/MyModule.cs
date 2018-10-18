using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DIModule
{

    public class MyModule 
    {
        private readonly IModuleClient moduleClient;
        private readonly ILogger logger;
        int counter;
        double temperatureThreshold = 25;
        public double TemperatureThreshold => this.temperatureThreshold;

        public MyModule(IModuleClient moduleClient, ILogger<MyModule> logger)
        {
            this.moduleClient = moduleClient;
            this.logger = logger;
        }

        public async Task InitializeAsync()
        {
            await this.moduleClient.OpenAsync();
            Console.WriteLine("My module client initialized.");

            // Resolve temperature thresold from module twin
            var moduleTwin = await this.moduleClient.GetTwinAsync();
            if (moduleTwin.Properties.Desired != null && moduleTwin.Properties.Desired.Contains("TemperatureThreshold")) 
            {
                var tempThreshold = moduleTwin.Properties.Desired["TemperatureThreshold"]?.ToString() ?? string.Empty;
                if (double.TryParse(tempThreshold, out double newTemperatureThreshold))
                {
                    this.logger.LogInformation("Using temperature threshold from module twin: {newTemperatureThreshold}", newTemperatureThreshold);
                    this.temperatureThreshold = newTemperatureThreshold;
                }
                
            } 

            // Register callback for twin changes
            await this.moduleClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, null);
           

            // Register callback to be called when a message is received by the module
            await this.moduleClient.SetInputMessageHandlerAsync("input1", PipeMessage, null);
        }

        private Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            if (desiredProperties != null && desiredProperties.Contains("TemperatureThreshold")) 
            {
                var tempThreshold = desiredProperties["TemperatureThreshold"]?.ToString() ?? string.Empty;
                if (double.TryParse(tempThreshold, out double newTemperatureThreshold))
                {
                    this.logger.LogInformation("Temperature threshold updated from {actualTemperature} to {newTemperature}", this.temperatureThreshold, newTemperatureThreshold);
                    this.temperatureThreshold = newTemperatureThreshold;
                }            
            }

            return Task.FromResult(0); 
        }

        private async Task<MessageResponse> PipeMessage(Message message, object userContext)
        {
            int counterValue = Interlocked.Increment(ref counter);

            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);
            
            this.logger.LogDebug("Received message: {messageCounter}, {messageBody}", counterValue, messageString);

            if (!string.IsNullOrEmpty(messageString))
            {
                DevicePayload devicePayload = null;
                try 
                {
                    devicePayload = JsonConvert.DeserializeObject<DevicePayload>(messageString);
                }
                catch (JsonReaderException)
                {
                }
                
                if (devicePayload != null && devicePayload.Machine != null && devicePayload.Machine.Temperature >= this.temperatureThreshold)
                {
                    var pipeMessage = new Message(messageBytes);
                    foreach (var prop in message.Properties)
                    {
                        pipeMessage.Properties.Add(prop.Key, prop.Value);
                    }
                    pipeMessage.Properties.Add("alert", "1");

                    await this.moduleClient.SendEventAsync("output1", pipeMessage);
                    this.logger.LogDebug("Received message sent");
                }
            }

            return MessageResponse.Completed;
        }
    }
}