namespace _03.MessageQueues.Messaging
{
	public interface IMessageSender
	{
		Task Send(Message @event);
	}
}
