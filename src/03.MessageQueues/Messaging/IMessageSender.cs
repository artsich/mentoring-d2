namespace Messaging
{
	public interface IMessageSender
	{
		Task Send(Message @event);
	}
}
