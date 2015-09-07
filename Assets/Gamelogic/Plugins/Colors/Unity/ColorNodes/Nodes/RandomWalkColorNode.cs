using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that performs a random walk on the first color in its inputs.
	/// </summary>
	[Version(1, 1)]
	public class RandomWalkColorNode:Node<Color, Color>
	{
		/// <summary>
		/// How many colors to grnerate.
		/// </summary>
		public int colorCount;

		/// <summary>
		/// The range of the random offset between successive colors in the walk.
		/// </summary>
		public MinMaxFloat offset;

		/// <summary>
		/// Whether the luminance should be the same for all colors.
		/// </summary>
		public bool fixLuminence;

		private List<Color> colors;

		public override List<Color> Execute(IEnumerable<Color> input)
		{
			colors = new List<Color>();
		
			foreach (var color in input)
			{
				colors.AddRange( ProceduralPalette.GenerateRandomWalk(
					colorCount,
					color,
					offset.min,
					offset.max,
					fixLuminence));
			}

			return colors;
		}

		public override void Recompute()
		{
			//
		}
	}
}