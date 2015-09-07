using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A class that can serve as the base class for nodes that simply transform each input
	/// independent of the others.
	/// </summary>
	/// <typeparam name="TInput">The input type of this node.</typeparam>
	/// <typeparam name="TOutput">The output type of this node.</typeparam>
	[Version(1, 1)]
	[Abstract]
	public class TransformNode<TInput, TOutput> : Node<TInput, TOutput>
	{
		/// <summary>
		/// Transforms the given input item. 
		/// </summary>
		/// <param name="input">The input to transform.</param>
		/// <returns>The output of the transformation.</returns>
		[Abstract]
		virtual public TOutput Transform(TInput input)
		{
			throw new NotImplementedException("Node of type " + GetType() + "should override this method");
		}

		public sealed override List<TOutput> Execute(IEnumerable<TInput> input)
		{
			return input.Select<TInput, TOutput>(Transform).ToList();
		}
	}

	/// <summary>
	/// A class that can serve as the base class for nodes that simply transform each input
	/// independent of the others, but also use a random number.
	/// </summary>
	/// <typeparam name="TInput"></typeparam>
	/// <typeparam name="TOutput"></typeparam>
	[Abstract]
	public class TransformRandomNode<TInput, TOutput> : Node<TInput, TOutput>
	{
		#region Public Fields
		/// <summary>
		/// If multi is true, each node is transformed count number of times, each with a 
		/// different random numner. The inputs are processed with the same sent of count
		/// random numbers. If there are 3 inputs, and count is 2, the total output will be
		/// 6.
		/// </summary>
		public bool multi;

		/// <summary>
		/// If multi is true, the number of random numbers to generate and use in the transformation of all nodes.
		/// </summary>
		public int count;
		#endregion

		#region Private Fields
		private List<float> randomValues;
		#endregion

		#region Public Methods
		/// <summary>
		/// Randomly transforms the input.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="randomValue"></param>
		[Abstract]
		public virtual TOutput Transform(TInput input, float randomValue)
		{
			throw new NotImplementedException();
		}

		public sealed override List<TOutput> Execute(IEnumerable<TInput> input)
		{
			var colors = new List<TOutput>();

			if (multi)
			{
				foreach (var color in input)
				{
					for (int i = 0; i < count; i++)
					{
						colors.Add(Transform(color, randomValues[i]));
					}
				}
			}
			else
			{
				var inputList = input.ToList();

				colors.AddRange(inputList.Select((t, i) => Transform(t, randomValues[i])));
			}

			return colors;
		}

		public sealed override void Recompute()
		{
			randomValues = new List<float>();

			for (int i = 0; i < (multi ? count : InputItemCount); i++)
			{
				randomValues.Add(Random.value);
			}
		}
		#endregion
	}

	/// <summary>
	/// A TransformNode that tranforms colors into other colors.
	/// </summary>
	[Abstract]
	public class ColorTransformNode : TransformNode<Color, Color>{}

	/// <summary>
	/// A TransformRandomNode that transforms colors into other colors.
	/// </summary>
	[Abstract]
	public class ColorTransformRandomNode : TransformRandomNode<Color, Color>{}
}