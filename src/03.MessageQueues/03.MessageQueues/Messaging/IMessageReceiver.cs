using Azure.Messaging.ServiceBus;

namespace _03.MessageQueues.Messaging
{
	public interface IMessageReceiver
	{
		Task<Message?> Receive();
	}

	public class MessageReceiver : IMessageReceiver
	{
		private readonly ServiceBusReceiver receiver;

		public MessageReceiver(ServiceBusReceiver receiver)
		{
			this.receiver = receiver;
		}

		public async Task<Message?> Receive()
		{
			var receivedMessage = await receiver.ReceiveMessageAsync();
			try
			{
				var message = receivedMessage.Body.ToObjectFromJson<Message>();

				await receiver.CompleteMessageAsync(receivedMessage);

				return message;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);

				return null;
			}
		}
	}
}
