using _03.MessageQueues.Messaging;

namespace _03.MessageQueues.FileService;

public class FileService : IFileService
{
	private readonly IMessageSender messageSender;

	public FileService(IMessageSender messageSender)
	{
		this.messageSender = messageSender;
	}

	public async Task OnFileCreated(string filePath)
	{
		var bytes = await File.ReadAllBytesAsync(filePath);

		try
		{
			await messageSender.Send(new Message() { FilePath = filePath, Data = bytes });
			File.Delete(filePath);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}
}
