using MassTransit;
using TCFiapConsumerDLQ.Workers;
using TechChallenge.SDK;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.RegisterSdkModule(hostContext.Configuration);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<RemoveContactConsumerDlq>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("delete-contact-dlq-queue", e =>
                {
                    e.ConfigureConsumer<RemoveContactConsumerDlq>(context);

                    e.Bind("delete-contact-dlx-exchange", s =>
                    {
                        s.RoutingKey = "delete-contact-dlx"; 
                        s.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
                    });
                });
            });
        });

        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .Build();

await host.RunAsync();