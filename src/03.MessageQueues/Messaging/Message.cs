namespace Messaging
{
	public class Message
	{
		private string filePath = string.Empty;

		public string FilePath 
		{ 
			get => filePath; 
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value));
				}

				filePath = value;
			}
		}

		public byte[] Data { get; set; } = Array.Empty<byte>();

		public string GetFileName() => Path.GetFileName(FilePath);
	}
}
