using MassTransit;
using TechChallenge.SDK.Domain.Models;
using TechChallenge.SDK.Domain.ValueObjects;
using TechChallenge.SDK.Infrastructure.Message;
using TechChallenge.SDK.Infrastructure.Persistence;

namespace TCFiapConsumerDLQ.Workers
{
    public class UpdateContactConsumerDlq(ILogger<UpdateContactConsumerDlq> logger, IContactRepository contactRepository)
        : BaseDlqConsumer<UpdateContactMessage>(logger, contactRepository)
    {
        protected override async Task ProcessMessage(ConsumeContext<UpdateContactMessage> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Recebida solicitação para atualizar o contato com ID: {message.Id}");

            var contact = await _contactRepository.GetByIdAsync(message.Id);
            if (contact == null)
            {
                _logger.LogWarning($"Contato {message.Id} não encontrado!");
                return;
            }

            var contactUpdated = MapContact(message);
            await _contactRepository.UpdateAsync(contactUpdated);
            _logger.LogInformation($"Contato {message.Id} atualizado com sucesso!");
        }

        public Contact MapContact(UpdateContactMessage contactUpdated)
        {
            var contact = new Contact
            {
                Name = new Name(contactUpdated.FirstName, contactUpdated.LastName),
                Email = new Email(contactUpdated.Email),
                Phone = new Phone(contactUpdated.DDD, contactUpdated.Phone)
            };

            return contact;
        }
    }
}
