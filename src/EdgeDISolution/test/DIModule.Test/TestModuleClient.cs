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
        public string ProductInfo { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


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



        public int DiagnosticSamplingPercentage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public uint OperationTimeoutInMilliseconds { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Task AbandonAsync(Message message)
        {
            throw new System.NotImplementedException();
        }

        public Task AbandonAsync(string lockToken)
        {
            throw new System.NotImplementedException();
        }

        public Task CloseAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task CompleteAsync(Message message)
        {
            throw new System.NotImplementedException();
        }

        public Task CompleteAsync(string lockToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Twin> GetTwinAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<MethodResponse> InvokeMethodAsync(string deviceId, string moduleId, MethodRequest methodRequest, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<MethodResponse> InvokeMethodAsync(string deviceId, string moduleId, MethodRequest methodRequest)
        {
            throw new System.NotImplementedException();
        }

        public Task<MethodResponse> InvokeMethodAsync(string deviceId, MethodRequest methodRequest, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<MethodResponse> InvokeMethodAsync(string deviceId, MethodRequest methodRequest)
        {
            throw new System.NotImplementedException();
        }

        public Task OpenAsync() => Task.FromResult(0);

        Dictionary<string, List<Message>> sentEventCollection = new Dictionary<string, List<Message>>();

        public IEnumerable<Message> GetSentEvents(string inputName)
        {
            if (this.sentEventCollection.TryGetValue(inputName, out var list))
                return list;
            return new Message[0];
        }

        public Task SendEventAsync(string outputName, Message message)
        {
            if (!sentEventCollection.TryGetValue(outputName, out var list))
            {
                list = new List<Message>();
                sentEventCollection[outputName] = list;
            }
            list.Add(message);

            return Task.FromResult(0);
        }

        public Task SendEventAsync(Message message) => SendEventAsync(string.Empty, message);

        public Task SendEventBatchAsync(string outputName, IEnumerable<Message> messages)
        {
            if (!sentEventCollection.TryGetValue(outputName, out var list))
            {
                list = new List<Message>();
                sentEventCollection[outputName] = list;
            }
            list.AddRange(messages);

            return Task.FromResult(0);        
        }

        public Task SendEventBatchAsync(IEnumerable<Message> messages) => SendEventBatchAsync(string.Empty, messages);

        public void SetConnectionStatusChangesHandler(ConnectionStatusChangesHandler statusChangesHandler)
        {
            throw new System.NotImplementedException();
        }

        public Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext)
        {
            throw new System.NotImplementedException();
        }

        Dictionary<string, Tuple<MessageHandler, object>> inputMessageHandlers = new Dictionary<string, Tuple<MessageHandler, object>>();
        public Task SetInputMessageHandlerAsync(string inputName, MessageHandler messageHandler, object userContext)
        {
            inputMessageHandlers[inputName] = new Tuple<MessageHandler, object>(messageHandler, userContext);
            return Task.FromResult(0);
        }

        public Task SetMessageHandlerAsync(MessageHandler messageHandler, object userContext)
        {
            throw new System.NotImplementedException();
        }

        public Task SetMethodDefaultHandlerAsync(MethodCallback methodHandler, object userContext)
        {
            throw new System.NotImplementedException();
        }

        public Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext)
        {
            throw new System.NotImplementedException();
        }

        public void SetRetryPolicy(IRetryPolicy retryPolicy)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties)
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