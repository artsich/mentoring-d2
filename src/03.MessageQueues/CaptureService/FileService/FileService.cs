using Messaging;

namespace CaptureService.FileService;

public class FileService : IFileService
{
	private readonly IMessageSender messageSender;

	public FileService(IMessageSender messageSender)
	{
		this.messageSender = messageSender;
	}

	public async Task OnFileCreated(string filePath)
	{
		if (FileIsAvailable(filePath))
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

	private static bool FileIsAvailable(string filePath)
	{
		while (true)
		{
			try
			{
				if (!File.Exists(filePath))
					return false;

				using var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
				fileStream.Close();

				break;
			}
			catch (PathTooLongException)
			{
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
			catch (IOException)
			{
			}
		}

		return true;
	}
}
