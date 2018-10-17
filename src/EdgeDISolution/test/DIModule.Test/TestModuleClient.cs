namespace DIModule.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DIModule;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Shared;
    using Newtonsoft.Json;

    public class TestModuleClient : IModuleClient
    {
        public virtual string ProductInfo { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


        // Route message to modules
        public Message RouteMessage(string inputName, string message) => RouteMessage(inputName, new Message(UTF8Encoding.UTF8.GetBytes(message)));

        // Route message to modules
        public Message RouteMessage(string inputName, object payload) => RouteMessage(inputName, new Message(UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload))));
        

        // Route message to modules
        public Message RouteMessage(string inputName, Message message)
        {
            if (this.inputMessageHandlers.TryGetValue(inputName, out var t))
            {
                t.Item1(message, t.Item2);
            }

            return message;
        }



        public virtual int DiagnosticSamplingPercentage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public virtual uint OperationTimeoutInMilliseconds { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public virtual Task AbandonAsync(Message message)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task AbandonAsync(string lockToken)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task CloseAsync() => Task.FromResult(0);

        public virtual Task CompleteAsync(Message message)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task CompleteAsync(string lockToken)
        {
            throw new System.NotImplementedException();
        }

        Twin getTwinResult = new Twin();
        public TestModuleClient SetGetTwinResult(Twin twin) 
        {
            this.getTwinResult = twin;
            return this;

        }

        public TestModuleClient SetGetTwinResult(object desiredProperties) 
        {
            var twin = new Twin();
            twin.Properties.Desired = new TwinCollection(JsonConvert.SerializeObject(desiredProperties));
            this.getTwinResult = twin;
            return this;
        }

        public virtual Task<Twin> GetTwinAsync() => Task.FromResult(getTwinResult);

        public virtual Task<MethodResponse> InvokeMethodAsync(string deviceId, string moduleId, MethodRequest methodRequest, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<MethodResponse> InvokeMethodAsync(string deviceId, string moduleId, MethodRequest methodRequest)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<MethodResponse> InvokeMethodAsync(string deviceId, MethodRequest methodRequest, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<MethodResponse> InvokeMethodAsync(string deviceId, MethodRequest methodRequest)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task OpenAsync() => Task.FromResult(0);

        Dictionary<string, List<Message>> sentEventCollection = new Dictionary<string, List<Message>>();

        public IEnumerable<Message> GetSentEvents(string inputName)
        {
            if (this.sentEventCollection.TryGetValue(inputName, out var list))
                return list;
            return new Message[0];
        }

        public virtual Task SendEventAsync(string outputName, Message message)
        {
            if (!sentEventCollection.TryGetValue(outputName, out var list))
            {
                list = new List<Message>();
                sentEventCollection[outputName] = list;
            }
            list.Add(message);

            return Task.FromResult(0);
        }

        public virtual Task SendEventAsync(Message message) => SendEventAsync(string.Empty, message);

        public virtual Task SendEventBatchAsync(string outputName, IEnumerable<Message> messages)
        {
            if (!sentEventCollection.TryGetValue(outputName, out var list))
            {
                list = new List<Message>();
                sentEventCollection[outputName] = list;
            }
            list.AddRange(messages);

            return Task.FromResult(0);        
        }

        public virtual Task SendEventBatchAsync(IEnumerable<Message> messages) => SendEventBatchAsync(string.Empty, messages);

        public virtual void SetConnectionStatusChangesHandler(ConnectionStatusChangesHandler statusChangesHandler)
        {
            throw new System.NotImplementedException();
        }

        DesiredPropertyUpdateCallback propertyUpdateCallback;
        object propertyUpdateCallbackUserContext;
        public virtual Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext)
        {
            this.propertyUpdateCallback = callback;
            this.propertyUpdateCallbackUserContext = userContext;
            return Task.FromResult(0);
        }

        // Test method to trigger twin desired property change
        public async Task TriggerDesiredPropertyChange(object value)
        {
            var json = JsonConvert.SerializeObject(value);
            await TriggerDesiredPropertyChange(new TwinCollection(json));
        }

        // Test method to trigger twin desired property change
        public async Task TriggerDesiredPropertyChange(TwinCollection value)
        {
            if (this.propertyUpdateCallback != null)
                await this.propertyUpdateCallback(value, this.propertyUpdateCallbackUserContext);
        }

        Dictionary<string, Tuple<MessageHandler, object>> inputMessageHandlers = new Dictionary<string, Tuple<MessageHandler, object>>();
        public virtual Task SetInputMessageHandlerAsync(string inputName, MessageHandler messageHandler, object userContext)
        {
            inputMessageHandlers[inputName] = new Tuple<MessageHandler, object>(messageHandler, userContext);
            return Task.FromResult(0);
        }

        public virtual Task SetMessageHandlerAsync(MessageHandler messageHandler, object userContext)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task SetMethodDefaultHandlerAsync(MethodCallback methodHandler, object userContext)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext)
        {
            throw new System.NotImplementedException();
        }

        public virtual void SetRetryPolicy(IRetryPolicy retryPolicy)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties)
        {
            throw new System.NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TestModuleClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}