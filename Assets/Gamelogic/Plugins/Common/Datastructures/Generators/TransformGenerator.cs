using System;

namespace Gamelogic
{
	/// <summary>
	/// A generator that transforms generated elements with a given transformation function.
	/// </summary>
	/// <typeparam name="T">The type of elements to generate</typeparam>
	/// <typeparam name="U">The type of the elements that underlying generator generates</typeparam>
	[Version(1, 4)]
	public class TransformGenerator<T, U> : IGenerator<T>
	{
		private readonly IGenerator<U> generator;
		private readonly Func<U, T> transform;

		/// <summary>
		/// Constructs a new TransformGenerator that generates elements from the given generator, transformed
		/// by the given transform.
		/// </summary>
		/// <param name="generator">The generator to use for generating elements</param>
		/// <param name="transform">The transform to apply to generated elements before they are returned.</param>
		public TransformGenerator(IGenerator<U> generator, Func<U, T> transform)
		{
			this.generator = generator;
			this.transform = transform;
		}

		public T Next()
		{
			return transform(generator.Next());
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}