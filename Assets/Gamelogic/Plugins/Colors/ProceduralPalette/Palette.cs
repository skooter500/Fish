using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// Class that represents a list of colors.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class Palette
	{
		[SerializeField] public Color[] colors;

		/// <summary>
		/// Constructs a new palette from the given list of colors.
		/// </summary>
		/// <param name="newColors"></param>
		public Palette(IEnumerable<Color> newColors)
		{
			colors = newColors.ToArray();
		}
	}
}