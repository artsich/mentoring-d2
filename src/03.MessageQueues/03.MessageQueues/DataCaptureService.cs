using _03.MessageQueues.FileService;

namespace _03.MessageQueues;

public class DataCaptureService
{
	private readonly FileSystemWatcher watcher;
	private readonly IFileService fileService;

	public DataCaptureService(ListenOptions options, IFileService fileService)
	{
		watcher = new FileSystemWatcher(options.Folder);
		watcher.Created += OnFileCreated;
		watcher.Filter = options.Filter;
		watcher.IncludeSubdirectories = true;
		this.fileService = fileService;
	}

	public void Run()
	{
		watcher.EnableRaisingEvents = true;
	}

	private async void OnFileCreated(object sender, FileSystemEventArgs e)
	{
		await fileService.OnFileCreated(e.FullPath);
	}
}
