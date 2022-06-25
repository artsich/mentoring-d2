namespace _03.MessageQueues.Messaging
{
	public interface IMessageReceiver
	{
		Task<Message> Receive();
	}
}
