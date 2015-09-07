using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node with no inputs, and that outputs constant colors 
	/// (selected from the nodes interface).
	/// </summary>
	[Version(1,1)]
	public class ConstantColorNode : Node<None, Color>
	{
		/// <summary>
		/// The list of selected colors. These colors will be the output of this node.
		/// </summary>
		public List<Color> colors;

		public override List<Color> Execute(IEnumerable<None> input)
		{
			return colors;
		}

		public override void Recompute()
		{
			//nothing to compute
		}
	}
}