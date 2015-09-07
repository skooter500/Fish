using System.Collections.Generic;

namespace Gamelogic
{
	/// <summary>
	/// A buffer is a generator that buffers a fixed number of elements at a time from another
	/// generator before returning them.
	/// </summary>
	/// <typeparam name="T">The type of items this generator will generate.</typeparam>
	[Version(1, 2)]
	public class Buffer<T> : IGenerator<T>
	{
		private readonly Queue<T> buffer;
		private readonly IGenerator<T> baseGenerator; 

		public Buffer(IGenerator<T> baseGenerator, int bufferCount)
		{
			buffer = new Queue<T>();

			for (int i = 0; i < bufferCount; i++)
			{
				buffer.Enqueue(baseGenerator.Next());
			}

			this.baseGenerator = baseGenerator;
		}

		public IEnumerable<T> PeekAll()
		{
			return buffer;
		}

		public T Next()
		{
			T itemToPop = buffer.Dequeue();
			buffer.Enqueue(baseGenerator.Next());
			return itemToPop;

		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}