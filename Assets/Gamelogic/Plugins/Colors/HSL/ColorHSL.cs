using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// Class for representing colors in the HSL colorspace.
	/// </summary>
	[Version(1)]
	[Serializable]
	public struct ColorHSL
	{
		#region Constants
		/// <summary>
		/// ColorHSL that represents white.
		/// </summary>
		public static readonly ColorHSL White = new ColorHSL(0, 0, 1);

		/// <summary>
		/// ColorHSL that represents black.
		/// </summary>
		public static readonly ColorHSL Black = new ColorHSL(0, 0, 0);

		private const float Tolerance = 1f/256;
		#endregion

		#region Private members

		private readonly float hue;
		private readonly float saturation;
		private readonly float luminance;

		#endregion

		#region Properties
		/// <summary>
		/// Gets the Hue value
		/// </summary>
		public float H
		{
			get
			{
				return hue;
			}
		}

		/// <summary>
		/// Gets the Saturation value.
		/// </summary>
		public float S
		{
			get
			{
				return saturation;
			}
		}

		/// <summary>
		/// Gets or sets the Luminance value
		/// </summary>
		public float L
		{
			get
			{
				return luminance;
			}
		}

		/// <summary>
		/// Convert from the current ColorHSL to an RGB Color.
		/// </summary>
		public Color Color
		{
			get
			{
				float r = luminance;
				float g = luminance;
				float b = luminance;

				if (!(saturation > 0)) return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b));

				float max = luminance;
				float dif = luminance * saturation;
				float min = luminance - dif;

				float hh = hue * 360f;

				if (hh < 60f)
				{
					r = max;
					g = hh * dif / 60f + min;
					b = min;
				}
				else if (hh < 120f)
				{
					r = -(hh - 120f) * dif / 60f + min;
					g = max;
					b = min;
				}
				else if (hh < 180f)
				{
					r = min;
					g = max;
					b = (hh - 120f) * dif / 60f + min;
				}
				else if (hh < 240f)
				{
					r = min;
					g = -(hh - 240f) * dif / 60f + min;
					b = max;
				}
				else if (hh < 300f)
				{
					r = (hh - 240f) * dif / 60f + min;
					g = min;
					b = max;
				}
				else if (hh <= 360f)
				{
					r = max;
					g = min;
					b = -(hh - 360f) * dif / 60 + min;
				}
				else
				{
					r = 0;
					g = 0;
					b = 0;
				}

				return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b));
			}
		}
		#endregion

		#region Constrcutors
		/// <summary>
		/// Constructs a new ColorHSL that represents the given color.
		/// </summary>
		/// <param name="color"></param>
		public ColorHSL(Color color)
		{
			float r = color.r;
			float g = color.g;
			float b = color.b;

			float max = Mathf.Max(r, Mathf.Max(g, b));

			if (max <= 0)
			{
				hue = 0;
				saturation = 0;
				luminance = 0;
				
				return;
			}

			float min = Mathf.Min(r, Mathf.Min(g, b));
			float dif = max - min;

			if (max > min)
			{
				if (g >= r && g >= b)
				{
					hue = (b - r) / dif * 60f + 120f;
				}
				else if (b >= g && b >= r)
				{
					hue = (r - g) / dif * 60f + 240f;
				}
				else if (b > g)
				{
					hue = (g - b) / dif * 60f + 360f;
				}
				else
				{
					hue = (g - b) / dif * 60f;
				}
				if (hue < 0)
				{
					hue = hue + 360f;
				}
			}
			else
			{
				hue = 0;
			}

			hue *= 1f / 360f;
			saturation = (dif / max) * 1f;
			luminance = max;

			hue = Mathf.Clamp01(hue);
			saturation = Mathf.Clamp01(saturation);
			luminance = Mathf.Clamp01(luminance);
		}

		/// <summary>
		/// Constructor with ColorHSL
		/// </summary>
		/// <param name="hue">Varies from magenta - red - yellow - green - cyan - blue - magenta, 
		/// described as an value between 0.0f and 1.0f.</param>
		/// <param name="saturation">Varies from 0.0f and 1.0f and describes how "grey" the colour is, with 0 being completely unsaturated (grey, 
		/// white or black) and 1 being completely saturated</param>
		/// <param name="luminance">Varies from 0.0f and 1.0f and ranges from black at 0.0f, through 
		/// the standard colour itself at 0.5 to white at 1.0f</param>
		public ColorHSL(float hue, float saturation, float luminance)
		{
			int hueInt = Mathf.FloorToInt(hue);
			this.hue = hue - hueInt; //-3.3 -(-4) = 0.7 // 3.3 - 3 = 0.3

			if (hue < 0) this.hue = 1 - this.hue;

			this.saturation = Mathf.Clamp01(saturation);
			this.luminance = Mathf.Clamp01(luminance);
		}
		#endregion

		#region Static Methods
		/// <summary>
		/// Linearly interpolates between the given colors in HSL color space.
		/// </summary>
		/// <param name="color1"></param>
		/// <param name="color2"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static Color Lerp(Color color1, Color color2, float t)
		{
			return Lerp(color1.ToHSL(), color2.ToHSL(), t).Color;
		}

		/// <summary>
		/// Linearly interpolates between the given colors in HSL color space.
		/// </summary>
		/// <param name="color1"></param>
		/// <param name="color2"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static ColorHSL Lerp(ColorHSL color1, ColorHSL color2, float t)
		{
			if (Math.Abs(color1.S) < Tolerance || Math.Abs(color1.L) < Tolerance)
			{
				var newH = color2.H;
				var newS = Mathf.Lerp(color1.S, color2.S, t);
				var newL = Mathf.Lerp(color1.L, color2.L, t);

				var newColor = new ColorHSL(newH, newS, newL);

				return newColor;
			}
			//else
			if (Math.Abs(color2.S) < Tolerance || Math.Abs(color2.L) < Tolerance)
			{
				var newH = color1.H;
				var newS = Mathf.Lerp(color1.S, color2.S, t);
				var newL = Mathf.Lerp(color1.L, color2.L, t);

				var newColor = new ColorHSL(newH, newS, newL);

				return newColor;
			}
			//else
			{
				var newH = GLMathf.Wlerp01(color1.H, color2.H, t);
				var newS = Mathf.Lerp(color1.S, color2.S, t);
				var newL = Mathf.Lerp(color1.L, color2.L, t);

				var newColor = new ColorHSL(newH, newS, newL);

				return newColor;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets a copy of this ColorHSL with the given hue.
		/// </summary>
		/// <param name="newHue"></param>
		/// <returns></returns>
		public ColorHSL WithHue(float newHue)
		{
			return new ColorHSL(newHue, S, L);
		}

		/// <summary>
		/// Gets a copy of this ColorHSL with the given luminance.
		/// </summary>
		/// <param name="newLuminance"></param>
		/// <returns></returns>
		public ColorHSL WithLuminance(float newLuminance)
		{
			return new ColorHSL(H, S, newLuminance);
		}

		/// <summary>
		/// Gets a copy of this ColorHSL with the given saturation.
		/// </summary>
		/// <param name="newSaturation"></param>
		/// <returns></returns>
		public ColorHSL WithSaturation(float newSaturation)
		{
			return new ColorHSL(H, newSaturation, L);
		}

		/// <summary>
		/// Gets a copy of this ColorHSL with the given hue offset.
		/// </summary>
		/// <param name="hueOffset"></param>
		/// <returns></returns>
		public ColorHSL WithHueOffset(float hueOffset)
		{
			return new ColorHSL(H+hueOffset, S, L);
		}

		/// <summary>
		/// Gets a copy of this ColorHSL with the given luminance offset.
		/// </summary>
		/// <param name="luminanceOffset"></param>
		/// <returns></returns>
		public ColorHSL WithLuminanceOffset(float luminanceOffset)
		{
			return new ColorHSL(H, S, L+luminanceOffset);
		}

		/// <summary>
		/// Gets a copy of this ColorHSL with the given saturation offset.
		/// </summary>
		/// <param name="saturationOffset"></param>
		/// <returns></returns>
		public ColorHSL WithSaturationOffset(float saturationOffset)
		{
			return new ColorHSL(H, S+saturationOffset, L);
		}

		/// <summary>
		/// Gets a copy of this ColorHSL but inverted.
		/// </summary>
		/// <returns></returns>
		public ColorHSL Invert()
		{
			return new ColorHSL(hue + 0.5f, 0, 1 - luminance);
		}

		/// <summary>
		/// Gets a copy of this ColorHSL with the luminance inverted.
		/// </summary>
		/// <returns></returns>
		public ColorHSL InvertLuminosity()
		{
			return new ColorHSL(hue, saturation, 1 - luminance);
		}

		/// <summary>
		/// Gets a copy of this ColorHSL with the saturation inverted.
		/// </summary>
		/// <returns></returns>
		public ColorHSL InvertSaturation()
		{
			return new ColorHSL(hue, 1 - saturation, luminance);
		}

		

		public IEnumerable<ColorHSL> GetTints(int colorCount)
		{
			var gradient = GradientHSL.GetLerp(this, White);

			return gradient.SampleEvenly(colorCount, 0);
		}

		public IEnumerable<ColorHSL> GetShades(int colorCount)
		{
			var gradient = GradientHSL.GetLerp(this, Black);

			return gradient.SampleEvenly(colorCount, 0);
		}

		/// <summary>
		/// Gives a string representation of this color.
		/// </summary>
		/// <returns></returns>
		override public string ToString()
		{
			return string.Format("[h: {0}, s: {1}, l: {2}]", hue, saturation, luminance);
		}
		#endregion
	}
}