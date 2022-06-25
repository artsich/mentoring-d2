using Azure.Messaging.ServiceBus;

namespace _03.MessageQueues.Messaging
{

	public class Chunk
	{
		public Guid Id { get; set; }

		public int Count { get; set; }

		public int Number { get; set; }

		public byte[] Data { get; set; } = Array.Empty<byte>();

		public bool SingleChunk() => Count == 1;
	}

	public class MessageSender : IMessageSender
	{
		private readonly ServiceBusSender sender;
		private readonly int maxMsgSize;

		public MessageSender(ServiceBusSender sender, int maxMsgSize = 180_000)
		{
			this.sender = sender;
			this.maxMsgSize = maxMsgSize;
		}

		public async Task Send(Message @event)
		{
			var data = BinaryData.FromObjectAsJson(@event).ToMemory();

			int chunks = data.Length / maxMsgSize;

			if (chunks == 0 || data.Length % maxMsgSize != 0)
				chunks++;

			var id = Guid.NewGuid();

			var splitSize = maxMsgSize;
			var offset = 0;
			for(int i = 0; i < chunks; i++)
			{
				if (i == chunks - 1)
				{
					splitSize = data.Length % maxMsgSize;
				}

				var chunkInfo = new Chunk()
				{
					Id = id,
					Count = chunks,
					Number = i,
					Data = data[offset..(offset+splitSize)].ToArray(),
				};

				var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(chunkInfo));
				offset += maxMsgSize;

				await sender.SendMessageAsync(message);
			}
		}
	}
}
