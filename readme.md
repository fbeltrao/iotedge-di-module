# Dependency Injection in IoT Edge Module

[IoT Edge](https://docs.microsoft.com/en-us/azure/iot-edge/about-iot-edge) is an Azure service that enables building IoT projects where smart capabilities are available at the edge. One of the capabilities is to provide custom code packaged into IoT Edge modules which execute in field devices. IoT Edge modules can be implemented in multiple languages (C#, Python, JavaScript, Java, C).

This sample implementation illustrates a way to add dependency injection (DI) to an C# IoT Edge module. DI improves the code testability and maintainability by removing strict dependencies.

## IoT Edge Module in C#

[Developing a IoT Edge module in C#](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-develop-csharp-module) will get you started with a .NET Core Console app that connects to the IoT Edge Module and pipes messages upstream (IoT Hub). The provided code is simple to understand, using only static methods and variables.

As your IoT Edge modules become more complex it is a good idea to refactor the code to improve maintenance through dependency injection.

Adding dependency injection requires the following changes to the Program.cs file (besides adding the package `Microsoft.Extensions.DependencyInjection`):

```C#
static void Main(string[] args)
{
    var serviceCollection = new ServiceCollection();
    ConfigureServices(serviceCollection);

    // Build the our IServiceProvider and set our static reference to it
    ServiceProvider = serviceCollection.BuildServiceProvider();

    // Initialize module
    ServiceProvider.GetRequiredService<MyModule>()
        .InitializeAsync()
        .GetAwaiter()
        .GetResult();

    // Wait until the app unloads or is cancelled
    var cts = new CancellationTokenSource();
    AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
    Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
    WhenCancelled(cts.Token).Wait();
}

private static void ConfigureServices(ServiceCollection serviceCollection)
{
    serviceCollection.AddModuleClient(new AmqpTransportSettings(TransportType.Amqp_Tcp_Only));
    serviceCollection.AddSingleton<MyModule>();
}
```

## Abstracting ModuleClient dependency

An IoT Edge module communicates with IoT Hub through an instance of the [ModuleClient](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.devices.client.moduleclient?view=azure-dotnet) class.

In a newly created IoT Edge module the ModuleClient instance is created at the startup using `ModuleClient.CreateFromEnvironmentAsync`. This class is implemented in package `Microsoft.Azure.Devices.Client`. None of the class members (methods and properties) are virtual, which makes it hard to write unit test against. In the sample code an `IModuleClient` interface was introduced, having a default implementation that uses a concrete ModuleClient. Moreover, our module does not create a ModuleClient, but receives a IModuleClient in the constructor:

```C#
public class MyModule
{
    private readonly IModuleClient moduleClient;
  
    public MyModule(IModuleClient moduleClient)
    {
        this.moduleClient = moduleClient;
    }
}

public interface IModuleClient : IDisposable
{
    ...
    Task<Twin> GetTwinAsync();
    ...
}

public class ModuleClientAdapter : IModuleClient
{
    private readonly ModuleClient moduleClient;

    public ModuleClientAdapter(ModuleClient moduleClient)
    {
       this.moduleClient = moduleClient ?? throw new System.ArgumentNullException(nameof(moduleClient));
    }

    ...

    public Task<Twin> GetTwinAsync() => this.moduleClient.GetTwinAsync();

    ...
}
```

The sample unit test project contains a IModuleClient implementation (ModuleClientForTest) serving as a quick start to build tests interacting with the module client (pipe message, receive twin updates, set results for twin queries).

```C#
// Module client implementation for testing purposes
public class ModuleClientForTest : IModuleClient
{
    // Route message to modules
    public async Task<MessageResponse> RouteMessage(string inputName, object payload) { ... }
}
```

This abstraction makes simpler to write unit test that verifies if the module is handling twin changes properly:

```C#
[Fact]
public async Task When_Twin_Is_Update_With_New_Threshold_Current_State_Is_Updated()
{
    var moduleClient = new ModuleClientForTest();
    var module = new MyModule(moduleClient, this.logger);

    await module.InitializeAsync();

    await moduleClient.TriggerDesiredPropertyChange(new { TemperatureThreshold = 100.0 });
    Assert.Equal(100.0, module.TemperatureThreshold);
}
```

Asserting message upstream is simpler using the provided `ModuleClientForTest`:

```C#
[Fact]
public async Task When_Temperature_Is_Higher_Than_Default_Threshold_Forwards_To_IotHub()
{
    var moduleClient = new ModuleClientForTest();
    var module = new MyModule(moduleClient, this.logger);
    await module.InitializeAsync();

    Assert.Equal(MessageResponse.Completed, await moduleClient.RouteMessage("input1", new DevicePayload { Machine = new MachineTelemetry { Temperature = 30 } } ));

    var actualOutputMessages = moduleClient.GetSentEvents("output1");
    Assert.Single(actualOutputMessages);
    Assert.True(actualOutputMessages.First().Properties.ContainsKey("alert"), "Ensure 'alert' property was created");
}
```

## Adding Serilog

The sample code also demonstrate how to setup [Serilog](https://serilog.net/) as the logging framework, leveraging DI to provide an ILogger implementation to classes that require it. Serilog makes it simpler to log to different targets by providing multiple [sinks](https://github.com/serilog/serilog/wiki/Provided-Sinks).

Adding logging to MyModule class requires adding a new parameter in the constructor `public MyModule(IModuleClient moduleClient, ILogger<MyModule> logger)`.