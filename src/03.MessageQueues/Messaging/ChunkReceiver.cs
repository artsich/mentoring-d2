using Azure.Messaging.ServiceBus;
using System.Collections.Concurrent;

namespace Messaging;

public class ChunkReceiver : IChunkReceiver
{
	private readonly ServiceBusReceiver receiver;

	private Dictionary<Guid, ConcurrentQueue<Chunk>> ChunksMap = new();

	public ChunkReceiver(ServiceBusReceiver receiver)
	{
		this.receiver = receiver;
	}

	async Task<Chunk> IChunkReceiver.Receive()
	{
		Chunk? chunk = null;

		while (chunk == null)
		{
			chunk = await ReceiveChunk();
		}

		return chunk;
	}

	private async Task<Chunk?> ReceiveChunk()
	{
		var receivedMessage = await receiver.ReceiveMessageAsync();

		if (receivedMessage is null)
			return null;

		try
		{
			var result = receivedMessage.Body.ToObjectFromJson<Chunk>();
			await receiver.CompleteMessageAsync(receivedMessage);
			return result;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);

			await receiver.DeadLetterMessageAsync(receivedMessage);

			return null;
		}
	}
}
