namespace Messaging;

public class Chunk
{
	public string Name { get; set; } = string.Empty;

	public int ChunksCount { get; set; }

	public int Number { get; set; }

	public int Offset { get; set; }

	public int FullFileSize { get; set; }

	public byte[] Data { get; set; } = Array.Empty<byte>();

	public bool SingleChunk() => ChunksCount == 1;
}
