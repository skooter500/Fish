using System;
using System.Collections.Generic;

namespace Gamelogic
{
	/// <summary>
	/// A buffer generaor that will only generate items that will ensure the buffer can pass the predicate.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Version(1, 4)]
	public class BufferFilterGenerator<T>:IGenerator<T>
	{
	
		private readonly LinkedList<T> buffer;
		private readonly IGenerator<T> baseGenerator;
		private readonly Func<IEnumerable<T>, bool> predicate;

		public BufferFilterGenerator(IGenerator<T> baseGenerator, int bufferCount, Func<IEnumerable<T>, bool> predicate)
		{
			this.baseGenerator = baseGenerator;
			this.predicate = predicate;

			buffer = new LinkedList<T>();

			for (int i = 0; i < bufferCount; i++)
			{
				buffer.AddLast(baseGenerator.Next());

				FixBuffer();
			}

		
		}

		private void FixBuffer()
		{
			while (!predicate(buffer))
			{
				buffer.RemoveLast();
				buffer.AddLast(baseGenerator.Next());
			}
		}

		public IEnumerable<T> PeekAll()
		{
			return buffer;
		}

		public T Next()
		{
			T itemToPop = buffer.First.Value;
			buffer.RemoveFirst();

			buffer.AddLast(baseGenerator.Next());

			FixBuffer();
		
			return itemToPop;
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}