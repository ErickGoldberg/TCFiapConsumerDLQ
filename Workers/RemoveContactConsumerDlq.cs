using MassTransit;
using TechChallenge.SDK.Infrastructure.Message;
using TechChallenge.SDK.Infrastructure.Persistence;

namespace TCFiapConsumerDLQ.Workers
{
    public class RemoveContactConsumerDlq : IConsumer<RemoveContactMessage>
    {
        private readonly ILogger<RemoveContactConsumerDlq> _logger;
        private readonly IContactRepository _contactRepository;

        public RemoveContactConsumerDlq(ILogger<RemoveContactConsumerDlq> logger, IContactRepository contactRepository)
        {
            _logger = logger;
            _contactRepository = contactRepository;
        }

        public async Task Consume(ConsumeContext<RemoveContactMessage> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Recebida solicitação para deletar o contato com ID: {message.ContactId}");

            var contact = await _contactRepository.GetByIdAsync(message.ContactId);
            if (contact == null)
            {
                _logger.LogWarning($"Contato {message.ContactId} não encontrado!");
                return;
            }

            await _contactRepository.DeleteAsync(contact);

            await Task.Delay(500);
            _logger.LogInformation($"Contato {message.ContactId} removido com sucesso!");
        }
    }
}
