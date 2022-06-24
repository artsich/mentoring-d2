using _03.MessageQueues.Messaging;

namespace _03.MessageQueues.Processing;

public class MainProcessingService : IProcessingService
{
	private readonly IMessageReceiver messageReceiver;
	private readonly string folderToSave;

	public MainProcessingService(IMessageReceiver messageReceiver, string folderToSave)
	{
		this.messageReceiver = messageReceiver;
		this.folderToSave = folderToSave;
	}

	public async Task Process()
	{
		var message = await messageReceiver.Receive();

		if (message == null)
		{
			Console.WriteLine("Wrong message received.");
			return;
		}

		Console.WriteLine($"File is Received: " + message.FilePath);

		if (message.Data is null || message.Data.Length == 0)
		{
			Console.WriteLine($"File: {message.FilePath} is empty :( ");
			return;
		}

		var pathToSave = Path.Combine(folderToSave, Path.GetFileName(message.FilePath));

		await File.WriteAllBytesAsync(pathToSave, message.Data);
	}
}