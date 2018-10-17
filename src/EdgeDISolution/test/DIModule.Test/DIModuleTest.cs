using System;
using Microsoft.Azure.Devices.Client;
using Xunit;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DIModule.Test
{
    public class DIModuleTest
    {
        ILogger<MyModule> logger;

        public DIModuleTest()
        {
            this.logger = new Logger<MyModule>(new NullLoggerFactory());
        }

        [Fact]
        public async Task When_Temperature_Is_Higher_Than_Default_Threshold_Forwards_To_IotHub()
        {
            var moduleClient = new ModuleClientForTest();
            var module = new MyModule(moduleClient, this.logger);
            await module.InitializeAsync();

            Assert.Equal(MessageResponse.Completed, await moduleClient.RouteMessage("input1", new DevicePayload { MachineTemperature = 30 } ));

            var actualOutputMessages = moduleClient.GetSentEvents("output1");
            Assert.Equal(1, actualOutputMessages.Count());
            Assert.True(actualOutputMessages.First().Properties.ContainsKey("alert"), "Ensure 'alert' property was created");
        }

        [Fact]
        public async Task When_Message_Is_Forwarded_Properties_Are_Copied()
        {
            var moduleClient = new ModuleClientForTest();
            var module = new MyModule(moduleClient, this.logger);
            await module.InitializeAsync();

            var message = new Message(UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new DevicePayload { MachineTemperature = 30 })));
            message.Properties.Add("MyProperty", "MyValue");
            Assert.Equal(MessageResponse.Completed, await moduleClient.RouteMessage("input1", message));

            var actualOutputMessages = moduleClient.GetSentEvents("output1");
            Assert.Equal(1, actualOutputMessages.Count());
            Assert.True(actualOutputMessages.First().Properties.ContainsKey("alert"), "Ensure 'alert' property was created");
            Assert.True(actualOutputMessages.First().Properties.ContainsKey("MyProperty"), "Ensure 'MyProperty' property was copied");
            Assert.Equal("MyValue", actualOutputMessages.First().Properties["MyProperty"]);

        
        }

        [Fact]
        public async Task When_Temperature_Is_Lesser_Than_Default_Threshold_Ignore()
        {
            var moduleClient = new ModuleClientForTest();
            var module = new MyModule(moduleClient, this.logger);
            await module.InitializeAsync();

            Assert.Equal(MessageResponse.Completed, await moduleClient.RouteMessage("input1", new DevicePayload { MachineTemperature = 20 } ));

            var actualOutputMessages = moduleClient.GetSentEvents("output1");
            Assert.Equal(0, actualOutputMessages.Count());
        }

        [Fact]
        public async Task When_Payload_Has_Different_Contract_Ignore()
        {
            var moduleClient = new ModuleClientForTest();
            var module = new MyModule(moduleClient, this.logger);
            await module.InitializeAsync();

            Assert.Equal(MessageResponse.Completed, await moduleClient.RouteMessage("input1", new { wrongProperty = 30 } ));

            var actualOutputMessages = moduleClient.GetSentEvents("output1");
            Assert.Equal(0, actualOutputMessages.Count());
        }

        [Fact]
        public async Task When_Payload_Is_Invalid_Ignore()
        {
            var moduleClient = new ModuleClientForTest();
            var module = new MyModule(moduleClient, this.logger);
            await module.InitializeAsync();

            Assert.Equal(MessageResponse.Completed, await moduleClient.RouteMessage("input1", "{ 'MachineTemperature': '30"));

            var actualOutputMessages = moduleClient.GetSentEvents("output1");
            Assert.Equal(0, actualOutputMessages.Count());
        }

        [Fact]
        public async Task When_Twin_Has_No_Value_Temperature_Threshold_Is_Default()
        {
            var moduleClient = new ModuleClientForTest();
            var module = new MyModule(moduleClient, this.logger);

            await module.InitializeAsync();
            Assert.Equal(25d, module.TemperatureThreshold);
        }

        [Fact]
        public async Task When_Twin_Has_Value_Temperature_Threshold_Is_Initialized()
        {
            var moduleClient = new ModuleClientForTest()
                .SetGetTwinResult(new { TemperatureThreshold = 45.0 });

            var module = new MyModule(moduleClient, this.logger);

            await module.InitializeAsync();
            Assert.Equal(45d, module.TemperatureThreshold);
        }

        [Fact]
        public async Task When_Twin_Is_Update_With_New_Threshold_Current_State_Is_Updated()
        {
            var moduleClient = new ModuleClientForTest();
            var module = new MyModule(moduleClient, this.logger);

            await module.InitializeAsync();

            await moduleClient.TriggerDesiredPropertyChange(new { TemperatureThreshold = 100.0 });
            Assert.Equal(100.0, module.TemperatureThreshold);
        }
    }
}
