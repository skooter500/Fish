using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// Extension methods for Unity's Gradient class.
	/// </summary>
	[Version(1)]
	public static class GradientExtensions
	{
		/// <summary>
		/// Returns a list of colors sampled evenly across the gradient.
		/// </summary>
		/// <param name="gradient">The gradient to sample</param>
		/// <param name="colorCount">The number of colors to return</param>
		/// <param name="jitter">The amount of jitter. This value should be between 0 and 1, and represents the extent
		/// of a random offset of the sampled colors. For good results the jitter should be less than
		/// half the distance between samples, that is, less than 1/(2*(colorCount - 1)).</param>
		/// <returns></returns>
		public static IEnumerable<Color> SampleEvenly(this Gradient gradient, int colorCount, float jitter)
		{
			float tDelta = 1/(float) (colorCount - 1);
			float t = + Random.Range(0, jitter);

			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{

				var color = gradient.Evaluate(t);
				colors.Add(color);
				t += tDelta + Random.Range(-jitter, (i == colorCount - 2) ? 0 : jitter);
			}

			return colors;
		}

		/// <summary>
		/// Returns a gradient that represents a linear interpolation between two colors.
		/// </summary>
		/// <param name="color1"></param>
		/// <param name="color2"></param>
		/// <returns></returns>
		public static Gradient GetLerp(Color color1, Color color2)
		{
			var gradient = new Gradient();

			var leftColorKey = new GradientColorKey(color1, 0);
			var rightColorKey = new GradientColorKey(color2, 1);

			var leftAlphaKey = new GradientAlphaKey(color1.a, 0);
			var rightAlphaKey = new GradientAlphaKey(color1.a, 1);

			gradient.SetKeys(
				new[]{leftColorKey, rightColorKey},
				new[]{leftAlphaKey, rightAlphaKey});

			return gradient;
		}
	}

	/// <summary>
	/// Extension methods for Unity's Gradient class.
	/// </summary>
	[Version(1)]
	public static class GradientHSLExtensions
	{
		/// <summary>
		/// Returns a list of colors sampled evenly across the gradient.
		/// </summary>
		/// <param name="gradient">The gradient to sample</param>
		/// <param name="colorCount">The number of colors to return</param>
		/// <param name="jitter">The amount of jitter. This value should be between 0 and 1, and represents the extent
		///		of a random offset of the sampled colors. For good results the jitter should be less than
		///		half the distance between samples, that is, less than 1/(2*(colorCount - 1)).</param>
		/// <returns></returns>
		public static IEnumerable<ColorHSL> SampleEvenly(this GradientHSL gradient, int colorCount, float jitter)
		{
			float tDelta = 1 / (float)(colorCount - 1);
			float t = +Random.Range(0, jitter);

			var colors = new List<ColorHSL>();

			for (int i = 0; i < colorCount; i++)
			{

				var color = gradient.Evaluate(t);
				colors.Add(color);
				t += tDelta + Random.Range(-jitter, (i == colorCount - 2) ? 0 : jitter);
			}

			return colors;
		}

		/// <summary>
		/// Returns a gradient that represents a linear interpolation between two colors.
		/// </summary>
		/// <param name="color1">The left end of the gradient.</param>
		/// <param name="color2">The right end of the gradient.</param>
		/// <returns>A GradientHSL that represents a linear interpolation between color1 and color2.</returns>
		public static GradientHSL GetLerp(ColorHSL color1, ColorHSL color2)
		{
			var gradient = new GradientHSL();

			var leftColorKey = new GradientHSLColorKey(color1, 0);
			var rightColorKey = new GradientHSLColorKey(color2, 1);

			gradient.SetKeys(
				new[] { leftColorKey, rightColorKey });

			return gradient;
		}
	}
}