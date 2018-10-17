using System;
using Microsoft.Azure.Devices.Client;
using Xunit;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Moq;

namespace DIModule.Test
{
    public class DIModuleTest
    {
        [Fact]
        public async Task When_Temperature_Is_Higher_Than_Default_Threshold_Forwards_To_IotHub()
        {
            var moduleClient = new TestModuleClient();
            var module = new MyModule(moduleClient);
            await module.InitializeAsync();

            moduleClient.RouteMessage("input1", new DevicePayload { MachineTemperature = 30 } );

            var actualOutputMessages = moduleClient.GetSentEvents("output1");
            Assert.Equal(1, actualOutputMessages.Count());
        }

        [Fact]
        public async Task When_Temperature_Is_Lesser_Than_Default_Threshold_Ignore()
        {
            var moduleClient = new TestModuleClient();
            var module = new MyModule(moduleClient);
            await module.InitializeAsync();

            moduleClient.RouteMessage("input1", new DevicePayload { MachineTemperature = 20 } );

            var actualOutputMessages = moduleClient.GetSentEvents("output1");
            Assert.Equal(0, actualOutputMessages.Count());
        }

        [Fact]
        public async Task When_Payload_Has_Different_Contract_Ignore()
        {
            var moduleClient = new TestModuleClient();
            var module = new MyModule(moduleClient);
            await module.InitializeAsync();

            moduleClient.RouteMessage("input1", new { wrongProperty = 30 } );

            var actualOutputMessages = moduleClient.GetSentEvents("output1");
            Assert.Equal(0, actualOutputMessages.Count());
        }

        [Fact]
        public async Task When_Payload_Is_Invalid_Ignore()
        {
            var moduleClient = new TestModuleClient();
            var module = new MyModule(moduleClient);
            await module.InitializeAsync();

            moduleClient.RouteMessage("input1", "{ 'MachineTemperature': '30");

            var actualOutputMessages = moduleClient.GetSentEvents("output1");
            Assert.Equal(0, actualOutputMessages.Count());
        }

        [Fact]
        public async Task When_Twin_Has_No_Value_Temperature_Threshold_Is_Default()
        {
            var moduleClient = new TestModuleClient();
            var module = new MyModule(moduleClient);

            await module.InitializeAsync();
            Assert.Equal(25d, module.TemperatureThreshold);
        }

        [Fact]
        public async Task When_Twin_Has_Value_Temperature_Threshold_Is_Initialized()
        {
            var moduleClient = new TestModuleClient()
                .SetGetTwinResult(new { TemperatureThreshold = 45.0 });

            var module = new MyModule(moduleClient);

            await module.InitializeAsync();
            Assert.Equal(45d, module.TemperatureThreshold);
        }

        [Fact]
        public async Task When_Twin_Is_Update_With_New_Threshold_Current_State_Is_Updated()
        {
            var moduleClient = new TestModuleClient();
            var module = new MyModule(moduleClient);

            await module.InitializeAsync();

            await moduleClient.TriggerDesiredPropertyChange(new { TemperatureThreshold = 100.0 });
            Assert.Equal(100.0, module.TemperatureThreshold);
        }
    }
}
