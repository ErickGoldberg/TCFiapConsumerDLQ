using MassTransit;
using TCFiapConsumerDLQ.Extensions;
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

                DlqEndpointConfigurator.ConfigureDlqEndpoint<RemoveContactConsumerDlq>(
                    cfg, context,
                    "delete-contact-dlq-queue",
                    "delete-contact-dlx-exchange",
                    "delete-contact-dlx");

                DlqEndpointConfigurator.ConfigureDlqEndpoint<UpdateContactConsumerDlq>(
                    cfg, context,
                    "update-contact-dlq-queue",
                    "update-contact-dlx-exchange",
                    "update-contact-dlx");
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