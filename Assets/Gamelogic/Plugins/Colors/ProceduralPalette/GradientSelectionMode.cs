using System;

namespace Gamelogic.Colors
{
	/*
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Which method to use to select colors from a gradient.
	/// </summary>
	[Version(1)]
	[Serializable]
	public enum GradientSelectionMode
	{
		/// <summary>
		/// Evenly across the gradient.
		/// </summary>
		Even,

		/// <summary>
		/// Randomly from the gradient.
		/// </summary>
		Random,

		/// <summary>
		/// Using the cyclic golden ratio method.
		/// </summary>
		GoldenRatio
	}
}