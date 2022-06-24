using Azure.Messaging.ServiceBus;

namespace _03.MessageQueues.Messaging
{
	public class MessageSender : IMessageSender
	{
		private readonly ServiceBusSender sender;

		public MessageSender(ServiceBusSender sender)
		{
			this.sender = sender;
		}

		public async Task Send(Message @event)
		{
			var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(@event));
			await sender.SendMessageAsync(message);
		}
	}
}
