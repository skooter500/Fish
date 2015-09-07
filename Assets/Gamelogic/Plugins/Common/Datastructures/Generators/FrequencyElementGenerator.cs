using System.Collections.Generic;

namespace Gamelogic
{
	/// <summary>
	/// Generates items at the same frequencies as they
	/// occur in a set from which this generator is constructed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Version(1, 4)]
	public class FrequencyElementGenerator<T> : IGenerator<T>
	{
		private readonly ListSelectorGenerator<T> elementGenerator;

		public FrequencyElementGenerator(IEnumerable<T> elements)
		{
			elements.ThrowIfNull("elements");

			var counts = new Dictionary<T, float>();

			foreach (var element in elements)
			{
				if (counts.ContainsKey(element))
				{
					counts[element]++;
				}
				else
				{
					counts[element] = 1;
				}
			}

			var indexGenerator = new FrequencyIntGenerator(counts.Values);
			elementGenerator = new ListSelectorGenerator<T>(counts.Keys, indexGenerator);
		}

		public T Next()
		{
			return elementGenerator.Next();
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}
