namespace Messaging
{
	public interface IChunkReceiver
	{
		Task<Chunk> Receive();
	}
}
