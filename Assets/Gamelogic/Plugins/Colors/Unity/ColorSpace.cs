using System;

namespace Gamelogic.Colors
{
	/*
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Labels for different color spaces.
	/// </summary>
	[Version(1)]
	[Serializable]
	public enum ColorSpace
	{
		/// <summary>
		/// The RGB color space.
		/// </summary>
		RGB,
		/// <summary>
		/// The HSL color space.
		/// </summary>
		HSL
	}
}