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

                cfg.ReceiveEndpoint("delete-contact-queue", e =>
                {
                    e.ConfigureConsumer<RemoveContactConsumerDlq>(context);

                    // DLQ Exchange 
                    e.SetQueueArgument("x-dead-letter-exchange", "delete-contact-dlx-exchange");
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