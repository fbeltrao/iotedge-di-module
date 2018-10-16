using System;
using Microsoft.Azure.Devices.Client;
using Xunit;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
