using System;

namespace Gamelogic
{
	/// <summary>
	/// Class for representing a bounded range.
	/// </summary>
	[Version(1, 2)]
	[Serializable]
	public class MinMaxFloat
	{
		public float min = 0;
		public float max = 1;
	}
}