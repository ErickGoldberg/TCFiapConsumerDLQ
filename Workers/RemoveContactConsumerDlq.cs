using MassTransit;
using TechChallenge.SDK.Infrastructure.Message;
using TechChallenge.SDK.Infrastructure.Persistence;

namespace TCFiapConsumerDLQ.Workers
{
    public class RemoveContactConsumerDlq : BaseDlqConsumer<RemoveContactMessage>
    {
        public RemoveContactConsumerDlq(ILogger<RemoveContactConsumerDlq> logger, IContactRepository contactRepository)
            : base(logger, contactRepository)
        {
        }

        protected override async Task ProcessMessage(ConsumeContext<RemoveContactMessage> context)
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
