namespace CaptureService.FileService;

public interface IFileService
{
	Task OnFileCreated(string filePath);
}
