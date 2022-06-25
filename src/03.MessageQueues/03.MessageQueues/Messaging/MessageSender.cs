using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

namespace _03.MessageQueues.Messaging
{
	public class ChunksInfo
	{
		public int ChunksCount { get; set; }

		public int Number { get; set; }

		public byte[] Data { get; set; } = Array.Empty<byte>();

		public bool SingleChunk() => ChunksCount == 1;
	}

	public class MessageSender : IMessageSender
	{
		private const string MessageIsTooBig = "Batch message is to big to send.";

		private readonly int maxMsgSize;
		private readonly ServiceBusSender sender;

		public MessageSender(ServiceBusSender sender, int maxMsgSize = 50000)
		{
			this.sender = sender;
			this.maxMsgSize = maxMsgSize;
		}

		public async Task Send(Message @event)
		{
			if (@event.Data == null)
				throw new ArgumentNullException(nameof(@event.Data));

			var data = BinaryData
				.FromObjectAsJson(@event)
				.ToMemory();

			await SendBatch(data);
		}

		private async Task SendBatch(ReadOnlyMemory<byte> data)
		{
			using var batch = await sender.CreateMessageBatchAsync();

			int chunks = data.Length / maxMsgSize;

			if (chunks == 0 || data.Length % maxMsgSize != 0)
			{
				chunks++;
			}
			
			int offset = 0;
			for (int i = 0; i < chunks; i++)
			{
				// todo: Try to simplify it!
				int size = offset + maxMsgSize;
				if (i == chunks - 1)
				{
					size = data.Length;
				}

				var rowData = data[offset..size].ToArray();

				var message = new ServiceBusMessage(
					BinaryData.FromObjectAsJson(new ChunksInfo()
					{
						ChunksCount = chunks,
						Data = rowData,
						Number = i,
					}));

				if (!batch.TryAddMessage(message))
				{
					throw new Exception(MessageIsTooBig);
				}

				offset += maxMsgSize;
			}

			await sender.SendMessagesAsync(batch);
		}
	}
}
