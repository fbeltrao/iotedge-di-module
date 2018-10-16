using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace DIModule
{
    public interface IModuleClient : IDisposable
    {
        string ProductInfo { get; set; }
        int DiagnosticSamplingPercentage { get; set; }
        
        uint OperationTimeoutInMilliseconds { get; set; }

        
        
        //
        // Summary:
        //     Puts a received message back onto the device queue
        //
        // Returns:
        //     The lock identifier for the previously received message
        Task AbandonAsync(Message message);
        //
        // Summary:
        //     Puts a received message back onto the device queue
        //
        // Returns:
        //     The previously received message
        Task AbandonAsync(string lockToken);
        //
        // Summary:
        //     Close the DeviceClient instance
        Task CloseAsync();
        //
        // Summary:
        //     Deletes a received message from the device queue
        //
        // Returns:
        //     The previously received message
        Task CompleteAsync(Message message);
        //
        // Summary:
        //     Deletes a received message from the device queue
        //
        // Returns:
        //     The lock identifier for the previously received message
        Task CompleteAsync(string lockToken);
  
        //
        // Summary:
        //     Retrieve a device twin object for the current device.
        //
        // Returns:
        //     The device twin object for the current device
        Task<Twin> GetTwinAsync();
        //
        // Summary:
        //     Interactively invokes a method on module
        //
        // Parameters:
        //   deviceId:
        //     Device Id
        //
        //   moduleId:
        //     Module Id
        //
        //   methodRequest:
        //     Device method parameters (passthrough to device)
        //
        //   cancellationToken:
        //     Cancellation Token
        //
        // Returns:
        //     Method result
        Task<MethodResponse> InvokeMethodAsync(string deviceId, string moduleId, MethodRequest methodRequest, CancellationToken cancellationToken);
        
        //
        // Summary:
        //     Interactively invokes a method on module
        //
        // Parameters:
        //   deviceId:
        //     Device Id
        //
        //   moduleId:
        //     Module Id
        //
        //   methodRequest:
        //     Device method parameters (passthrough to device)
        //
        // Returns:
        //     Method result
        Task<MethodResponse> InvokeMethodAsync(string deviceId, string moduleId, MethodRequest methodRequest);
        //
        // Summary:
        //     Interactively invokes a method on device
        //
        // Parameters:
        //   deviceId:
        //     Device Id
        //
        //   methodRequest:
        //     Device method parameters (passthrough to device)
        //
        //   cancellationToken:
        //     Cancellation Token
        //
        // Returns:
        //     Method result
        Task<MethodResponse> InvokeMethodAsync(string deviceId, MethodRequest methodRequest, CancellationToken cancellationToken);
        //
        // Summary:
        //     Interactively invokes a method on device
        //
        // Parameters:
        //   deviceId:
        //     Device Id
        //
        //   methodRequest:
        //     Device method parameters (passthrough to device)
        //
        // Returns:
        //     Method result
        Task<MethodResponse> InvokeMethodAsync(string deviceId, MethodRequest methodRequest);
        //
        // Summary:
        //     Explicitly open the DeviceClient instance.
        Task OpenAsync();
        Task SendEventAsync(string outputName, Message message);
        //
        // Summary:
        //     Sends an event to device hub
        //
        // Returns:
        //     The message containing the event
        Task SendEventAsync(Message message);
        //
        // Summary:
        //     Sends a batch of events to device hub The output target for sending the given
        //     message A list of one or more messages to send
        //
        // Returns:
        //     The task containing the event
        Task SendEventBatchAsync(string outputName, IEnumerable<Message> messages);
        //
        // Summary:
        //     Sends a batch of events to device hub
        //
        // Returns:
        //     The task containing the event
        Task SendEventBatchAsync(IEnumerable<Message> messages);
        //
        // Summary:
        //     Registers a new delegate for the connection status changed callback. If a delegate
        //     is already associated, it will be replaced with the new delegate. The name of
        //     the method to associate with the delegate.
        void SetConnectionStatusChangesHandler(ConnectionStatusChangesHandler statusChangesHandler);
        //
        // Summary:
        //     Set a callback that will be called whenever the client receives a state update
        //     (desired or reported) from the service. This has the side-effect of subscribing
        //     to the PATCH topic on the service.
        //
        // Parameters:
        //   callback:
        //     Callback to call after the state update has been received and applied
        //
        //   userContext:
        //     Context object that will be passed into callback
        Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext);
        //
        // Summary:
        //     Registers a new delgate for the particular input. If a delegate is already associated
        //     with the input, it will be replaced with the new delegate. The name of the input
        //     to associate with the delegate. The delegate to be used when a message is sent
        //     to the particular inputName. generic parameter to be interpreted by the client
        //     code.
        //
        // Returns:
        //     The task containing the event
        Task SetInputMessageHandlerAsync(string inputName, MessageHandler messageHandler, object userContext);
        //
        // Summary:
        //     Registers a new default delegate which applies to all endpoints. If a delegate
        //     is already associated with the input, it will be called, else the default delegate
        //     will be called. If a default delegate was set previously, it will be overwritten.
        //     The delegate to be called when a message is sent to any input. generic parameter
        //     to be interpreted by the client code.
        //
        // Returns:
        //     The task containing the event
        Task SetMessageHandlerAsync(MessageHandler messageHandler, object userContext);
        //
        // Summary:
        //     Registers a new delegate that is called for a method that doesn't have a delegate
        //     registered for its name. If a default delegate is already registered it will
        //     replace with the new delegate.
        //
        // Parameters:
        //   methodHandler:
        //     The delegate to be used when a method is called by the cloud service and there
        //     is no delegate registered for that method name.
        //
        //   userContext:
        //     Generic parameter to be interpreted by the client code.
        Task SetMethodDefaultHandlerAsync(MethodCallback methodHandler, object userContext);
        //
        // Summary:
        //     Registers a new delegate for the named method. If a delegate is already associated
        //     with the named method, it will be replaced with the new delegate. The name of
        //     the method to associate with the delegate. The delegate to be used when a method
        //     with the given name is called by the cloud service. generic parameter to be interpreted
        //     by the client code.
        Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext);
        //
        // Summary:
        //     Sets the retry policy used in the operation retries.
        //
        // Parameters:
        //   retryPolicy:
        //     The retry policy. The default is new ExponentialBackoff(int.MaxValue, TimeSpan.FromMilliseconds(100),
        //     TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(100));
        void SetRetryPolicy(IRetryPolicy retryPolicy);
        //
        // Summary:
        //     Push reported property changes up to the service.
        //
        // Parameters:
        //   reportedProperties:
        //     Reported properties to push
        Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties);
    }
}