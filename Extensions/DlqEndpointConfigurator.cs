using MassTransit;
using RabbitMQ.Client;

namespace TCFiapConsumerDLQ.Extensions
{
    public static class DlqEndpointConfigurator
    {
        public static void ConfigureDlqEndpoint<TConsumer>(
            IRabbitMqBusFactoryConfigurator cfg,
            IBusRegistrationContext context,
            string queueName,
            string exchangeName,
            string routingKey) where TConsumer : class, IConsumer
        {
            cfg.ReceiveEndpoint(queueName, e =>
            {
                e.ConfigureConsumer<TConsumer>(context);
                e.Bind(exchangeName, s =>
                {
                    s.RoutingKey = routingKey;
                    s.ExchangeType = ExchangeType.Direct;
                });
            });
        }
    }
}