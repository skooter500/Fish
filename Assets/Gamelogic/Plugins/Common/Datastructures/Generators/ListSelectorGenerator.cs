using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamelogic
{
	/// <summary>
	/// Generates items from a list using an index generator.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Version(1, 4)]
	public class ListSelectorGenerator<T> : IGenerator<T>
	{
		private readonly IIntGenerator indexGenerator;
		private readonly List<T> list;

		public ListSelectorGenerator(IEnumerable<T> list, IIntGenerator indexGenerator)
		{
			list.ThrowIfNull("list");

			this.list = list.ToList();

			if (!this.list.Any())
			{
				throw new ArgumentException("cannot be empty", "list");
			}

			this.indexGenerator = indexGenerator;
		}

		public T Next()
		{
			int index = indexGenerator.Next();

			return list[index];
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}
