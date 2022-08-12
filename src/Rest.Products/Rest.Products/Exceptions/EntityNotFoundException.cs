namespace Rest.Products.Exceptions
{
	public class EntityNotFoundException : Exception
	{
		public EntityNotFoundException(object id)
			: base($"Entity by id: {id} not found")
		{
		}
	}
}
