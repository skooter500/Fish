using System.Collections.Generic;
using System.Linq;

namespace Gamelogic
{
	/// <summary>
	/// Generates elements chosen randomly (with uniform distribution).
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Version(1, 4)]
	public class RandomElementGenerator<T> : IGenerator<T>
	{
		private readonly ListSelectorGenerator<T> generator; 
		public RandomElementGenerator(IEnumerable<T> list) 
		{
			list.ThrowIfNull("list");

			generator = new ListSelectorGenerator<T>(list, new UniformIntGenerator(list.Count()));
		}

		public T Next()
		{
			return generator.Next();
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}
