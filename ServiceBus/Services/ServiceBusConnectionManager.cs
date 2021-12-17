using ToDoApi.ServiceBus.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;

namespace ToDoApi.ServiceBus.Services
{
    public class ServiceBusConnectionManager : IServiceBusConnectionManager
    {
        private readonly ILogger<ServiceBusConnectionManager> _logger;
        private readonly ServiceBusConnectionStringBuilder _serviceBusConnectionStringBuilder;
        private ITopicClient _topicClient;

        public ServiceBusConnectionManager(ILogger<ServiceBusConnectionManager> logger,
                               ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceBusConnectionStringBuilder = serviceBusConnectionStringBuilder ??
                throw new ArgumentNullException(nameof(serviceBusConnectionStringBuilder));
            _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
        }

        public ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder => _serviceBusConnectionStringBuilder;

        public ITopicClient CreateTopicClient()
        {
            if (_topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
            }
            return _topicClient;
        }
    }
}