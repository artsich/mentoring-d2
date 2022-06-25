using _03.MessageQueues.Messaging;

namespace _03.MessageQueues.FileService;

public class Msg
{
	public string FileName { get; set; }

	public int FullSize { get; set; }
}

public class Chunk
{
	public int Number { get; set; }

	public byte[] Data { get; set; }
}

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

		var text = await File.ReadAllTextAsync(filePath);

		try
		{
			await messageSender.Send(new Message() { FilePath = filePath, Data = text });
			File.Delete(filePath);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}

	private void WaitUntillFileWillBeFree(string filePath)
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
