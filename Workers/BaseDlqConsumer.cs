using MassTransit;
using TechChallenge.SDK.Infrastructure.Persistence;

namespace TCFiapConsumerDLQ.Workers
{
    public abstract class BaseDlqConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        protected readonly ILogger _logger;
        protected readonly IContactRepository _contactRepository;

        protected BaseDlqConsumer(ILogger logger, IContactRepository contactRepository)
        {
            _logger = logger;
            _contactRepository = contactRepository;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            try
            {
                await ProcessMessage(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao processar mensagem do tipo {typeof(TMessage).Name}: {ex.Message} / StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        protected abstract Task ProcessMessage(ConsumeContext<TMessage> context);
    }
}
