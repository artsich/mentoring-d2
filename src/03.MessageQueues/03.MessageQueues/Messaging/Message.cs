namespace _03.MessageQueues.Messaging
{
	public class Message
	{
		public string FilePath { get; set; } = string.Empty;

		public byte[]? Data { get; set; }
	}
}
