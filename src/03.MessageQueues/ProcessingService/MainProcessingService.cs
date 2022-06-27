using Messaging;

namespace ProcessingService;

public class MainProcessingService : IProcessingService
{
	private readonly IChunkReceiver chunkReceiver;
	private readonly string folderToSave;

	public MainProcessingService(IChunkReceiver chunkReceiver, string folderToSave)
	{
		this.chunkReceiver = chunkReceiver;
		this.folderToSave = folderToSave;

		Directory.CreateDirectory(folderToSave);
	}

	public async Task Process()
	{
		var chunk = await chunkReceiver.Receive();

		Console.WriteLine($"Chunk by name {chunk.Name}, Count: {chunk.Count}, Number: {chunk.Number}, ");

		if (chunk.Data is null || chunk.Data.Length == 0)
		{
			Console.WriteLine($"Chunk: {chunk.Name} is empty :( ");
			return;
		}

		var pathToSave = Path.Combine(folderToSave, Path.GetFileName(chunk.Name));

		using (var fileWriter = File.OpenWrite(pathToSave))
		{
			fileWriter.Seek(chunk.Offset, SeekOrigin.Begin);
			fileWriter.Write(chunk.Data);
		}

		var fileInfo = new FileInfo(pathToSave);
		if (fileInfo.Length == chunk.FullFileSize)
		{
			Console.WriteLine($"<<<<< File {chunk.Name} loaded! >>>>>");
		}
	}
}