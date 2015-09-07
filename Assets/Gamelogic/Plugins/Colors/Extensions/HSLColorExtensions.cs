using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// Extensions for Colors related to the HSL color model.
	/// </summary>
	[Version(1)]
	public static class HSLColorExtensions
	{
		/// <summary>
		/// Gets the saturuation of the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static float GetSaturation(this Color color)
		{
			return new ColorHSL(color).S;
		}

		/// <summary>
		/// Gets teh hue of the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static float GetHue(this Color color)
		{
			return new ColorHSL(color).H;
		}

		/// <summary>
		/// Get the luminance of the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static float GetLuminance(this Color color)
		{
			return new ColorHSL(color).L;
		}

		/// <summary>
		/// Converts the given color to a ColorHSL.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		[Version(1, 1)]
		public static ColorHSL ToHSL(this Color color)
		{
			return new ColorHSL(color);
		}
	}
}