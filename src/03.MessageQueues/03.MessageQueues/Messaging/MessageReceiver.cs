using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
using System.Collections.Concurrent;

namespace _03.MessageQueues.Messaging
{
	public class MessageReceiver : IMessageReceiver
	{
		private readonly ServiceBusReceiver receiver;

		private Dictionary<Guid, ConcurrentQueue<Chunk>> ChunksMap = new();

		public MessageReceiver(ServiceBusReceiver receiver)
		{
			this.receiver = receiver;
		}

		public async Task<Message> Receive()
		{
			Message? message = null;

			while(message == null)
			{
				message = await ReceiveChunks();
			}

			return message;
		}

		private async Task<Message?> ReceiveChunks()
		{
			var receivedMessage = await receiver.ReceiveMessageAsync();
			if (receivedMessage is null)
				return null;

			try
			{
				Message result = null;

				var chunk = receivedMessage.Body.ToObjectFromJson<Chunk>();

				if (chunk.SingleChunk())
				{
					result = BinaryData.FromBytes(chunk.Data).ToObjectFromJson<Message>();
				}
				else
				{
					if (!ChunksMap.ContainsKey(chunk.Id))
					{
						ChunksMap.Add(chunk.Id, new ConcurrentQueue<Chunk>());
					}

					ChunksMap[chunk.Id].Enqueue(chunk);
					if (ChunksMap[chunk.Id].Count == chunk.Count)
					{
						result = UnwrapChunks(ChunksMap[chunk.Id].ToList());
						ChunksMap.Remove(chunk.Id);
					}

					Console.WriteLine($"Chunk by id received: {chunk.Id}, Count: {chunk.Count}, Number: {chunk.Number}, ");
				}

				await receiver.CompleteMessageAsync(receivedMessage);
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return null;
			}
		}

		private static Message UnwrapChunks(IList<Chunk> chunks)
		{
			var rawData = chunks
				.OrderBy(x => x.Number)
				.SelectMany(x => x.Data)
				.ToArray();

			return BinaryData.FromBytes(rawData).ToObjectFromJson<Message>();
		}
	}
}
