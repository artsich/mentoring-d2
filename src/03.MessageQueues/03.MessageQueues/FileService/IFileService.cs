namespace _03.MessageQueues.FileService;

public interface IFileService
{
	Task OnFileCreated(string filePath);
}
