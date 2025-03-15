using MassTransit;
using TechChallenge.SDK.Infrastructure.Message;
using TechChallenge.SDK.Infrastructure.Persistence;

namespace TCFiapConsumerDLQ.Workers
{
    public class RemoveContactConsumerDlq(ILogger<RemoveContactConsumerDlq> logger, IContactRepository contactRepository)
        : IConsumer<RemoveContactMessage>
    {
        public async Task Consume(ConsumeContext<RemoveContactMessage> context)
        {
            try
            {
                var message = context.Message;
                logger.LogInformation($"Recebida solicitação para deletar o contato com ID: {message.ContactId}");

                var contact = await contactRepository.GetByIdAsync(message.ContactId);
                if (contact == null)
                {
                    logger.LogWarning($"Contato {message.ContactId} não encontrado!");
                    return;
                }

                await contactRepository.DeleteAsync(contact);

                await Task.Delay(500);
                logger.LogInformation($"Contato {message.ContactId} removido com sucesso!");
            }
            catch (Exception ex)
            {
                logger.LogError($"Um erro ocorreu ao tentar remover contato. Message: {ex.Message} / StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
