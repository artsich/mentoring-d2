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
		WaitUntillFileWillBeFree(filePath);
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

	private static void WaitUntillFileWillBeFree(string filePath)
	{
		while (true)
		{
			try
			{
				var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
				fileStream.Close();
				return;
			}
			catch (IOException ioException)
			{
			}
		}
	}
}
