using System.Collections.Generic;
using System.Linq;
using Gamelogic.Diagnostics;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that randomly offsets the colors of its inputs.
	/// </summary>
	[Version(1, 1)]
	public class RandomOffsetNode : Node<Color, Color>
	{
		/// <summary>
		/// If true, the node will generate count number of random values, and generate 
		/// a set of colors for each input color using the same set of random values for
		/// eachg color. The number of outputs will be the number of inputs times the number count.
		/// 
		/// If false, a random value will be generated for each color, and one color will be generated for each 
		/// input color using that random value. The number of outputs will be the same as the number of inputs.
		/// </summary>
		public bool multi;

		/// <summary>
		/// If multi is true, the number of random numbers to generate.
		/// </summary>
		public int count;
		public float range;

		private List<float> redRandomOffsets;
		private List<float> greenRandomOffsets;
		private List<float> blueRandomOffsets;

		public override List<Color> Execute(IEnumerable<Color> input)
		{
			var colors = new List<Color>();

			if (multi)
			{
				foreach (var color in input)
				{
					for (int i = 0; i < count; i++)
					{
						var rOffset = redRandomOffsets[i];
						var gOffset = redRandomOffsets[i];
						var bOffset = redRandomOffsets[i];

						var newColor = new Color(
							color.r + rOffset,
							color.g + gOffset,
							color.b + bOffset);

						colors.Add(newColor);
					}
				}
			}
			else
			{
				var inputList = input.ToList();

				for (int i = 0; i < inputList.Count; i++)
				{
					var color = inputList[i];

					var rOffset = redRandomOffsets[i];
					var gOffset = redRandomOffsets[i];
					var bOffset = redRandomOffsets[i];

					var newColor = new Color(
						color.r + rOffset,
						color.g + gOffset,
						color.b + bOffset);

					colors.Add(newColor);
				}
			}

			return colors;
		}

		public override void Recompute()
		{
			redRandomOffsets = new List<float>();
			greenRandomOffsets = new List<float>();
			blueRandomOffsets = new List<float>();

			for (int i = 0; i < (multi ? count : InputItemCount); i++)
			{
				redRandomOffsets.Add(GLRandom.RandomOffset(0, range));
				greenRandomOffsets.Add(GLRandom.RandomOffset(0, range));
				blueRandomOffsets.Add(GLRandom.RandomOffset(0, range));
			}
		}
	}
}