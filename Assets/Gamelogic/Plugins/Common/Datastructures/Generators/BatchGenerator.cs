using System.Collections.Generic;
using System.Linq;

namespace Gamelogic
{
	/// <summary>
	/// Generates batches of items. The same batch is returned each time.
	/// Bath generators are more useful when used in conjunction with another 
	/// generator that pocesses the batches, such as ShuffledBatchGenerator.
	/// </summary>
	/// <typeparam name="T">The type of items this generator will generate.</typeparam>
	[Version(1, 2)]
	public class BatchGenerator<T>:IGenerator<IEnumerable<T>>
	{
		public List<T> batchTemplate;

		public BatchGenerator()
		{
			batchTemplate = new List<T>();
		}

		public BatchGenerator(IEnumerable<T> batchTemplate)
		{
			this.batchTemplate = batchTemplate.ToList();
		}

		/// <summary>
		/// Adds a new item to the batch template.
		/// </summary>
		/// <param name="batchElement"></param>
		public void Add(T batchElement)
		{
			batchTemplate.Add(batchElement);
		}

		/// <summary>
		/// Removes an item from the batch template.
		/// </summary>
		/// <param name="batchElement"></param>
		public void Remove(T batchElement)
		{
			batchTemplate.Remove(batchElement);
		}

		public IEnumerable<T> Next()
		{
			return batchTemplate;
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}