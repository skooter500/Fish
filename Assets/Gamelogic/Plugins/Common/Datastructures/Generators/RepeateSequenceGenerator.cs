using System.Collections.Generic;

namespace Gamelogic
{
	/// <summary>
	/// A generator that repeats a given sequence.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Version(1, 4)]
	public class RepeateSequenceGenerator<T> : IGenerator<T>
	{
		private readonly IEnumerator<T> enumerator;
		/// <summary>
		/// Construts a new RepeateSequenceGenerator.
		/// </summary>
		/// <param name="sequence">The sequence that this sequence will repeat.</param>
		public RepeateSequenceGenerator(IEnumerable<T> sequence)
		{
			enumerator = sequence.GetEnumerator();
		}

		public T Next()
		{
			bool hasNext = enumerator.MoveNext();
			var current = enumerator.Current;

			if (!hasNext)
			{
				enumerator.Reset();
			}

			return current;
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}