using CaptureService.FileService;

namespace CaptureService;

public class DataCaptureService
{
	private readonly FileSystemWatcher watcher;
	private readonly IFileService fileService;

	public DataCaptureService(ListenOptions options, IFileService fileService)
	{
		Directory.CreateDirectory(options.Folder);

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
		try
		{
			Console.WriteLine($"File by path {e.Name} created");
			await fileService.OnFileCreated(e.FullPath);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}
}
