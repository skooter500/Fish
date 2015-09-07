using UnityEngine;

namespace Gamelogic
{
	/// <summary>
	/// Provides some utility functions for Colors.
	/// </summary>
	[Version(1)]
	public static class ColorExtensions
	{
		private const float LightOffset = 0.0625f;
		private const float DarkerFactor = 0.9f;

		/// <summary>
		/// Returns a color lighter than the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color Lighter(this Color color)
		{
			return new Color(
				color.r + LightOffset,
				color.g + LightOffset,
				color.b + LightOffset,
				color.a);
		}

		/// <summary>
		/// Returns a color darker than the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color Darker(this Color color)
		{
			return new Color(
				color.r - LightOffset,
				color.g - LightOffset,
				color.b - LightOffset,
				color.a);
		}

		/// <summary>
		/// Returns the brightness of the color, 
		/// defined as the average off the three color channels.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static float Brightness(this Color color)
		{
			return (color.r + color.g + color.b)/3;
		}

		/// <summary>
		/// Returns a new color with the RGB values scaled so that the color has the given
		/// brightness.
		/// </summary>
		/// <remarks>
		/// If the color is too dark, a grey is returned with the right brighness. The alpha
		/// is left uncanged.
		/// </remarks>
		/// <param name="color"></param>
		/// <param name="brightness"></param>
		public static Color WithBrightness(this Color color, float brightness)
		{
			if (color.IsApproximatelyBlack())
			{
				return new Color(brightness, brightness, brightness, color.a);
			}
			
			float factor = brightness/color.Brightness();

			float r = color.r*factor;
			float g = color.g*factor;
			float b = color.b*factor;

			float a = color.a;

			return new Color(r, g, b, a);
		}

		/// <summary>
		/// Returns whether the color is black or almost black.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static bool IsApproximatelyBlack(this Color color)
		{
			return color.r + color.g + color.b <= Mathf.Epsilon;
		}

		/// <summary>
		/// Returns whether the color is white or almost white.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static bool IsApproximatelyWhite(this Color color)
		{
			return color.r + color.g + color.b >= 1 - Mathf.Epsilon;
		}
		
		/// <summary>
		/// Returns an opaque version of the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color Opaque(this Color color)
		{
			return new Color(color.r, color.g, color.b);
		}
		
		/// <summary>
		/// Returns a new color that is this color inverted.
		/// </summary>
		/// <param name="color">The color to invert.</param>
		/// <returns></returns>
		public static Color Invert(this Color color)
		{
			return new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
		}
	}
}
