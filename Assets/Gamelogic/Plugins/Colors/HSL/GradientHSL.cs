using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gamelogic.Colors
{
	/// <summary>
	/// Represents a key-value pair for a GradientHSL
	/// </summary>
	[Version(1)]
	[Serializable]
	public sealed class GradientHSLColorKey
	{
		public float time;
		public ColorHSL color;

		public GradientHSLColorKey(ColorHSL color, float time)
		{
			this.color = color;
			this.time = time;
		}
	}
	
	/// <summary>
	/// Represents a gradient of HSL colors.
	/// </summary>
	[Version(1)]
	[Serializable]
	public sealed class GradientHSL
	{
		[SerializeField] private ResponseCurveHSL curve;

		/// <summary>
		/// Constrcust a new HSL gradient.
		/// </summary>
		public GradientHSL()
		{
			curve = new ResponseCurveHSL(
				new List<float> {0, 1},
				new List<ColorHSL> {ColorHSL.White, ColorHSL.Black});
		}

		public void SetKeys(IEnumerable<GradientHSLColorKey> keys)
		{
			var gradientHSLKeys = keys as IList<GradientHSLColorKey> ?? keys.ToList();
			
			curve = new ResponseCurveHSL(
				gradientHSLKeys.Select(key => key.time),
				gradientHSLKeys.Select(key => key.color));
		}

		/// <summary>
		/// Gives the HSL color in the gradient at the given time.
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public ColorHSL Evaluate(float time)
		{
			return curve[time];
		}

		/// <summary>
		/// Returns a list of HSL colors sampled evenly across the gradient.
		/// </summary>
		/// <param name="colorCount">The number of colors to return.</param>
		/// <param name="jitter">The amount of jitter. This value should be between 0 and 1, and represents the extent
		/// of a random offset of the sampled colors. For good results the jitter should be less than
		/// half the distance between samples, that is, less than 1/(2*(colorCount - 1)).</param>
		/// <returns></returns>
		public IEnumerable<ColorHSL> SampleEvenly(int colorCount, float jitter)
		{
			float tDelta = 1/(float) (colorCount - 1);
			float t = Random.Range(0, jitter);

			var colors = new List<ColorHSL>();

			for (int i = 0; i < colorCount; i++)
			{

				var color = Evaluate(t);
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
		public static GradientHSL GetLerp(ColorHSL color1, ColorHSL color2)
		{
			var gradient = new GradientHSL {curve = ResponseCurveHSL.GetLerp(0, 1, color1, color2)};

			return gradient;
		}
	}
}