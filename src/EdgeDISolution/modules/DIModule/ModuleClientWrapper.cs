using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace DIModule
{

    public class ModuleClientWrapper : IModuleClient
    {
        private readonly ModuleClient moduleClient;

        public ModuleClientWrapper(ModuleClient moduleClient)
        {
            this.moduleClient = moduleClient ?? throw new System.ArgumentNullException(nameof(moduleClient));
        }

        public string ProductInfo { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int DiagnosticSamplingPercentage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public uint OperationTimeoutInMilliseconds { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Task AbandonAsync(Message message) => this.moduleClient.AbandonAsync(message);

        public Task AbandonAsync(string lockToken) => this.moduleClient.AbandonAsync(lockToken);

        public Task CloseAsync() => this.moduleClient.CloseAsync();

        public Task CompleteAsync(Message message) => this.moduleClient.CompleteAsync(message);

        public Task CompleteAsync(string lockToken) => this.moduleClient.CompleteAsync(lockToken);

        public Task<Twin> GetTwinAsync() => this.moduleClient.GetTwinAsync();

        public Task<MethodResponse> InvokeMethodAsync(
            string deviceId, 
            string moduleId, 
            MethodRequest methodRequest, 
            CancellationToken cancellationToken) => this.moduleClient.InvokeMethodAsync(deviceId, moduleId, methodRequest, cancellationToken);
            
        public Task<MethodResponse> InvokeMethodAsync(
            string deviceId, 
            string moduleId, 
            MethodRequest methodRequest) => this.moduleClient.InvokeMethodAsync(deviceId, moduleId, methodRequest);

        public Task<MethodResponse> InvokeMethodAsync(
            string deviceId, 
            MethodRequest methodRequest, 
            CancellationToken cancellationToken) => this.moduleClient.InvokeMethodAsync(deviceId, methodRequest, cancellationToken);
            

        public Task<MethodResponse> InvokeMethodAsync(string deviceId, MethodRequest methodRequest) => this.moduleClient.InvokeMethodAsync(deviceId, methodRequest);
        
        public Task OpenAsync() => this.moduleClient.OpenAsync();
        

        public Task SendEventAsync(string outputName, Message message) => this.moduleClient.SendEventAsync(outputName, message);
        

        public Task SendEventAsync(Message message) => this.moduleClient.SendEventAsync(message);

        public Task SendEventBatchAsync(string outputName, IEnumerable<Message> messages) => this.moduleClient.SendEventBatchAsync(outputName, messages);
        
        public Task SendEventBatchAsync(IEnumerable<Message> messages) => this.moduleClient.SendEventBatchAsync(messages);
        

        public void SetConnectionStatusChangesHandler(ConnectionStatusChangesHandler statusChangesHandler) => this.moduleClient.SetConnectionStatusChangesHandler(statusChangesHandler);
        
        public Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext) => this.moduleClient.SetDesiredPropertyUpdateCallbackAsync(callback, userContext);
        public Task SetInputMessageHandlerAsync(string inputName, MessageHandler messageHandler, object userContext) => this.moduleClient.SetInputMessageHandlerAsync(inputName, messageHandler, userContext);

        public Task SetMessageHandlerAsync(MessageHandler messageHandler, object userContext) => this.moduleClient.SetMessageHandlerAsync(messageHandler, userContext);

        public Task SetMethodDefaultHandlerAsync(MethodCallback methodHandler, object userContext) => this.moduleClient.SetMethodDefaultHandlerAsync(methodHandler, userContext);

        public Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext) => this.moduleClient.SetMethodHandlerAsync(methodName, methodHandler, userContext);

        public void SetRetryPolicy(IRetryPolicy retryPolicy) => this.moduleClient.SetRetryPolicy(retryPolicy);
        

        public Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties) => this.moduleClient.UpdateReportedPropertiesAsync(reportedProperties);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.moduleClient.Dispose();
                }

                disposedValue = true;
            }
        }

      
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}