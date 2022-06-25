using _03.MessageQueues.FileService;
using Azure.Messaging.ServiceBus;

namespace _03.MessageQueues.Messaging
{
	public class ChunkMessageReceiver : IMessageReceiver
	{
		private readonly ServiceBusReceiver receiver;

		public ChunkMessageReceiver(ServiceBusReceiver receiver)
		{
			this.receiver = receiver;
		}

		public async Task<Message?> Receive()
		{
		// $)
		Start:
			var received = await ReceiveFromChunks();

			if (received == null)
			{
		goto Start;
			}

			return received;
		}

		private async Task<Message?> ReceiveFromChunks()
		{
			var chunkMsg = await receiver.ReceiveMessageAsync();

			if (chunkMsg is null)
				return null;

			var chunk = ParseChunk(chunkMsg);
			if (chunk.SingleChunk())
			{
				try
				{
					var singleResult = ToMsg(chunk.Data);
					await receiver.CompleteMessageAsync(chunkMsg);
					return singleResult;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error occured while processing message: {chunkMsg.MessageId}");
					Console.WriteLine(ex);
					await receiver.DeadLetterMessageAsync(chunkMsg);

					return null;
				}
			}


			var otherChunks = new List<ServiceBusReceivedMessage>();
			for(int i = 0; i < chunk.ChunksCount - 1; ++i)
			{
				var msg = await receiver.ReceiveMessageAsync();
				otherChunks.Add(msg);
			}

			var chunksMsg = otherChunks;// await receiver.ReceiveMessagesAsync(chunk.ChunksCount);
			var rowMessageData = UnpackReceivedMsgs(chunksMsg, chunk);
			var resultMsg = ToMsg(rowMessageData);

			await CompleteMsgs(chunksMsg.Prepend(chunkMsg));

			return resultMsg;
		}

		private async Task CompleteMsgs(IEnumerable<ServiceBusReceivedMessage> msgs)
		{
			foreach(var msg in msgs)
			{
				await receiver.CompleteMessageAsync(msg);
			}
		}

		private byte[] UnpackReceivedMsgs(IReadOnlyList<ServiceBusReceivedMessage> msgs, ChunksInfo firstChank)
		{
			var msgRowData = msgs
				.Select(x => ParseChunk(x))
				.Prepend(firstChank)
				.SelectMany(x => x.Data)
				.ToArray();

			return msgRowData;
		}
		private static Message ToMsg(byte[] data) => 
			BinaryData.FromBytes(data).ToObjectFromJson<Message>();

		private static ChunksInfo ParseChunk(ServiceBusReceivedMessage msg)
		{
			try
			{
				return msg.Body.ToObjectFromJson<ChunksInfo>();
			}
			catch (Exception)
			{
				Console.WriteLine($"Can't parse msg by id: {msg.MessageId}");
				throw;
			}
		}
	}
}
