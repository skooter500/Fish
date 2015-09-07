namespace Gamelogic
{
	/// <summary>
	/// A generator that generates the same element each time.
	/// This is useful in situations where a generator is expected,
	/// but a constant is desired (for example, when constructing 
	/// compound generators).
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Version(1, 4)]
	public class RepeatGenerator<T> : IGenerator<T>
	{
		/// <summary>
		/// The lement that this generator will repeat.
		/// </summary>
		private readonly T item;

		/// <summary>
		/// Constructs a new generator that repeats the given element.
		/// </summary>
		/// <param name="item">The element to repeat.</param>
		public RepeatGenerator(T item)
		{
			this.item = item;
		}

		public T Next()
		{
			return item;
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}