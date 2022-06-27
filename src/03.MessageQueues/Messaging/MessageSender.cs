using Azure.Messaging.ServiceBus;

namespace Messaging;

public class MessageSender : IMessageSender
{
	private readonly ServiceBusSender sender;
	private readonly int maxMsgSize;

	public MessageSender(ServiceBusSender sender, int maxMsgSize = 180_000)
	{
		this.sender = sender;
		this.maxMsgSize = maxMsgSize;
	}

	public async Task Send(Message fileMessage)
	{
		var data = fileMessage.Data;

		int chunksCount = data.Length / maxMsgSize;

		if (chunksCount == 0 || data.Length % maxMsgSize != 0)
			chunksCount++;

		var splitSize = maxMsgSize;
		var offset = 0;
		for(int i = 0; i < chunksCount; i++)
		{
			if (i == chunksCount - 1)
			{
				splitSize = data.Length % maxMsgSize;
			}

			var chunkInfo = new Chunk()
			{
				Name = fileMessage.GetFileName(),
				Number = i,
				Offset = offset,
				Count = chunksCount,
				FullFileSize = data.Length,
				Data = data[offset..(offset+splitSize)].ToArray(),
			};

			var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(chunkInfo));
			offset += maxMsgSize;

			await sender.SendMessageAsync(message);

			Console.WriteLine($"Chunk by number {chunkInfo.Number} is sent!");
		}
	}
}
