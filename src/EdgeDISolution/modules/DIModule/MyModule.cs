using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace DIModule
{

    public class MyModule 
    {
        private readonly IModuleClient moduleClient;
        int counter;
        double temperatureThreshold = 25;

        public MyModule(IModuleClient moduleClient)
        {
            this.moduleClient = moduleClient;
        }

        public async Task InitializeAsync()
        {
            await this.moduleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            // Register callback to be called when a message is received by the module
            await this.moduleClient.SetInputMessageHandlerAsync("input1", PipeMessage, null);
        }

        private async Task<MessageResponse> PipeMessage(Message message, object userContext)
        {
            int counterValue = Interlocked.Increment(ref counter);

            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);
            
            Console.WriteLine($"Received message: {counterValue}, Body: [{messageString}]");

            if (!string.IsNullOrEmpty(messageString))
            {
                var devicePayload = JsonConvert.DeserializeObject<DevicePayload>(messageString);
                if (devicePayload != null && devicePayload.MachineTemperature >= this.temperatureThreshold)
                {
                    var pipeMessage = new Message(messageBytes);
                    foreach (var prop in message.Properties)
                    {
                        pipeMessage.Properties.Add(prop.Key, prop.Value);
                    }
                    pipeMessage.Properties.Add("alert", "1");

                    await this.moduleClient.SendEventAsync("output1", pipeMessage);
                    Console.WriteLine("Received message sent");
                }
            }
            return MessageResponse.Completed;
        }
    }
}