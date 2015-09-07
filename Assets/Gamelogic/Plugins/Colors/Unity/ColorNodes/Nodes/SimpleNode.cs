using System;
using System.Collections.Generic;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A simple node is a node that produces a single output for all its inputs.
	/// </summary>
	/// <typeparam name="TInput"></typeparam>
	/// <typeparam name="TOutput"></typeparam>
	[Version(1, 1)]
	[Abstract]
	public class SimpleNode<TInput, TOutput> : Node<TInput, TOutput>
	{
		/// <summary>
		/// Calculates the single output for this node.
		/// </summary>
		/// <param name="input">The input of this node.</param>
		/// <returns>The output calculated based on the input.</returns>
		[Abstract]
		public virtual TOutput ExecuteSingle(IEnumerable<TInput> input)
		{
			throw new NotImplementedException("Node of type " + GetType() + "should override this method");
		}

		public sealed override List<TOutput> Execute(IEnumerable<TInput> input)
		{
			return new List<TOutput> {ExecuteSingle(input)};
		}
	}
}