using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gamelogic.Colors
{
	/// <summary>
	/// Contains functions for generating palettes of Colors using various alggorithms.
	/// </summary>
	[Version(1)]
	public static class ProceduralPalette
	{
		/**
			The reciprocal of the golden ratio.
		*/
		private const float GoldenRatioConjugate = 0.618033988749895f;

		/**
			Generates a list of colors. For each color, RGB values are chosen randomly.

			@param colorCount The number of colors to generate.
		*/
		public static List<Color> GenerateUniform(int colorCount)
		{
			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{
				var newColor = new Color(Random.value, Random.value, Random.value);

				colors.Add(newColor);
			}

			return colors;
		}

		/**
			Generates a list of colors. For each color, HSL values are chosen randomly.

			@param colorCount The number of colors to generate.
		*/
		public static List<Color> GenerateUniformHSL(int colorCount)
		{
			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{
				var newColor = new ColorHSL(Random.value, Random.value, Random.value);

				colors.Add(newColor.Color);
			}

			return colors;
		}


		/**
			Generates a random triadic color harmony. You can specify two angle offsets (from a 
			randomly selected reference), and an angle range around each of those.

			You can also specify base level saturation and luminance, and a range for each.	
		*/
		public static List<Color> GenerateHarmony(
			int colorCount,
			float offsetAngle1,
			float offsetAngle2,
			float rangeAngle0,
			float rangeAngle1,
			float rangeAngle2,
			float saturation, float saturationRange,
			float luminance, float luminanceRange)
		{
			var referenceAngle = Random.value * 360;

			return GenerateHarmony(colorCount, referenceAngle, offsetAngle1, offsetAngle2, rangeAngle0, rangeAngle1, rangeAngle2, saturation, saturationRange, luminance, luminanceRange);
		}

		public static List<Color> GenerateHarmony(int colorCount, float referenceAngle, float offsetAngle1, float offsetAngle2, float rangeAngle0,
			float rangeAngle1, float rangeAngle2, float saturation, float saturationRange, float luminance, float luminanceRange)
		{
			var colors = new List<Color>();
			for (int i = 0; i < colorCount; i++)
			{
				var randomAngle = Random.value*(rangeAngle0 + rangeAngle1 + rangeAngle2);

				if (randomAngle > rangeAngle0)
				{
					if (randomAngle < rangeAngle0 + rangeAngle1)
					{
						randomAngle += offsetAngle1;
					}
					else
					{
						randomAngle += offsetAngle2;
					}
				}

				var newSaturation = saturation + (Random.value - 0.5f)*saturationRange;
				var newLuminance = luminance + +(Random.value - 0.5f)*luminanceRange;

				var hslColor = new ColorHSL(((referenceAngle + randomAngle)/360.0f)%1.0f, newSaturation, newLuminance);

				colors.Add(hslColor.Color);
			}

			return colors;
		}

		/**
			Generates a sequence of colors where the next color is a random offset from the previous color.

			@param colorCount The number of colors to generate
			@param startColor The first color in the sequence
			@param 
		*/
		public static List<Color> GenerateRandomWalk(
			int colorCount, 
			Color startColor, 
			float minOffset, 
			float maxOffset, 
			bool fixLightness)
		{
			var colors = new List<Color>();

			var newColor = startColor;
			
			for (int i = 0; i < colorCount; i++)
			{
				var amplitude = minOffset + (maxOffset - minOffset) * Random.value;
				var offset = amplitude * Random.onUnitSphere;
			
				newColor = new Color(
					Mathf.Clamp01(newColor.r + offset.x),
					Mathf.Clamp01(newColor.g + offset.y), 
					Mathf.Clamp01(newColor.b + offset.z));

				colors.Add(newColor);
			}

			if (fixLightness)
			{
				var brightness = startColor.Brightness();

				return colors.Select(color => color.WithBrightness(brightness)).ToList();
			}

			return colors;
		}

		/**
			Generates a list of colours each of which is a random mixture of the three given colors. 
		*/
		public static List<Color> GenerateRandomMix(
			int colorCount, 
			Color color1, 
			Color color2, 
			Color color3, 
			float greyControl,
			ColorSpace mixMode)
		{
			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{
				var newColor = RandomMix(color1, color2, color3, greyControl, mixMode);
				colors.Add(newColor);
			}

			return colors;
		}

		/**
			Generates a list of colours that are randoly offset from a given color.
		*/
		public static List<Color> GenerateRandomOffset(int colorCount, Color startColor, float min, float max)
		{
			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{
				var amplitude = min + (max - min) * Random.value;
				var offset = amplitude * Random.onUnitSphere;

				var newColor = new Color(
					Mathf.Clamp01(startColor.r + offset.x),
					Mathf.Clamp01(startColor.g + offset.y),
					Mathf.Clamp01(startColor.b + offset.z));

				colors.Add(newColor);
			}

			return colors;
		}

		/**
			Generates a list of colours with random hue, given saturation and given luminance
		*/
		public static List<Color> GenerateRandomHue(int colorCount, float saturation, float luminance)
		{
			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{
				var hslColor = new ColorHSL(Random.value, saturation, luminance);

				colors.Add(hslColor.Color);
			}

			return colors;
		}
		/**
			Generates a list of colours with given hue, random saturation and given luminance
		*/
		public static List<Color> GenerateRandomSaturation(int colorCount, float hue, float luminance)
		{
			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{
				var hslColor = new ColorHSL(hue, Random.value, luminance);

				colors.Add(hslColor.Color);
			}

			return colors;
		}

		/**
			Generates a list of colours with given hue, given saturation and random luminance
		*/
		public static List<Color> GenerateRandomLuminance(int colorCount, float hue, float saturation)
		{
			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{
				var hslColor = new ColorHSL(hue, saturation, Random.value);

				colors.Add(hslColor.Color);
			}

			return colors;
		}

		/**
			Generates a list of colours with given hue, random saturation and random luminance
		*/
		public static List<Color> GenerateRandomSaturationLuminance(int colorCount, float hue)
		{
			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{
				var hslColor = new ColorHSL(hue, Random.value, Random.value);

				colors.Add(hslColor.Color);
			}

			return colors;
		}

		/// <summary>
		/// Gives a list of colors where that maximises distance on 
		/// the gradient between consecutaive colors.
		/// </summary>
		/// <param name="colorCount"></param>
		/// <param name="gradient"></param>
		/// <returns></returns>
		public static List<Color> GenerateColorsGoldenRatioGradient(int colorCount, Gradient gradient)
		{
			var colors = new List<Color>();
			var t = Random.value;


			for (int i = 0; i < colorCount; i++)
			{
				var newColor = gradient.Evaluate(t);

				colors.Add(newColor);

				t += GoldenRatioConjugate;
				t %= 1.0f;

			}

			return colors;
		}

		/// <summary>
		/// Gives a list of colors where that maximises distance on 
		/// the gradient between consecutaive colors.
		/// </summary>
		/// <param name="colorCount"></param>
		/// <param name="gradient"></param>
		/// <returns></returns>
		public static List<ColorHSL> GenerateColorsGoldenRatioGradient(int colorCount, GradientHSL gradient)
		{
			var colors = new List<ColorHSL>();
			var t = Random.value;


			for (int i = 0; i < colorCount; i++)
			{
				var newColor = gradient.Evaluate(t);

				colors.Add(newColor);

				t += GoldenRatioConjugate;
				t %= 1.0f;

			}

			return colors;
		}

		/**
			Generates a color with random hue in the given range, given saturation and given luminance. 
		*/
		public static List<Color> GenerateRandomHueInRange(int colorCount, float startHue, float endHue, float saturation, float luminance)
		{
			var colors = new List<Color>();
			var hueRange = endHue - startHue;

			if(hueRange < 0)
			{
				hueRange += 1.0f;
			}

			for (int i = 0; i < colorCount; i++)
			{
				var newHue = hueRange * Random.value + startHue;

				if(newHue > 1.0)
				{
					newHue -= 1.0f;
				}

				var hslColor = new ColorHSL(newHue, saturation, luminance);

				colors.Add(hslColor.Color);
			}

			return colors;
		}

		/**
			Gives a color that is a random mix of the given colors.
		*/
		public static Color RandomMix(Color color1, Color color2, Color color3, float greyControl, ColorSpace mixMode)
		{
			return Mix(
				color1,
				color2,
				color3,
				greyControl,
				Random.Range(0, 3),
				Random.value,
				Random.value,
				Random.value, 
				mixMode);
		}

		/**
			Gives a color that is a random mix of the given colors.
		*/
		public static Color Mix(
			Color color1, 
			Color color2, 
			Color color3, 
			float greyControl,
			int selectorIndex,
			float redMixFactor,
			float greenMixFactor,
			float blueMixFactor,
			ColorSpace mixMode
			)
		{
			//var randomIndex = Random.Range(0, 3);

			var mixRatio1 = (selectorIndex == 0) ? redMixFactor * greyControl : redMixFactor;
			var mixRatio2 = (selectorIndex == 1) ? greenMixFactor * greyControl : greenMixFactor;
			var mixRatio3 = (selectorIndex == 2) ? blueMixFactor * greyControl : blueMixFactor;

			var sum = mixRatio1 + mixRatio2 + mixRatio3;

			mixRatio1 /= sum;
			mixRatio2 /= sum;
			mixRatio3 /= sum;

			switch (mixMode)
			{
				case ColorSpace.RGB:
					return new Color(
						mixRatio1 * color1.r + mixRatio2 * color2.r + mixRatio3 * color3.r,
						mixRatio1 * color1.g + mixRatio2 * color2.g + mixRatio3 * color3.g,
						mixRatio1 * color1.b + mixRatio2 * color2.b + mixRatio3 * color3.b);
				//case ColorSpace.HSL:
				default:
				{
					var sum12 = mixRatio1 + mixRatio2;

					var m2 = mixRatio2/sum12;

					var col = ColorHSL.Lerp(color1, color2, m2);

					return ColorHSL.Lerp(col, color3, mixRatio3);
				}
			}
		}
		
		public static int SaturateToByte(float floatValue)
		{
			var intValue = (int)floatValue;

			if (intValue > 255)
			{
				intValue = 255;
			}
			else if( intValue < 0)
			{
				intValue = 0;
			}

			return intValue;
		}

		/// <summary>
		/// Generates a list of colours randomly sampled from a gradient.
		/// </summary>
		/// <param name="colorCount"></param>
		/// <param name="gradient"></param>
		/// <returns></returns>
		public static List<Color> GenerateColorsRandomGradient(int colorCount, Gradient gradient)
		{
			var colors = new List<Color>();

			for (int i = 0; i < colorCount; i++)
			{
				var color = gradient.Evaluate(Random.value);
				colors.Add(color);
			}

			return colors;
		}

		/// <summary>
		/// Generates a list of colours randomly sampled from a gradient.
		/// </summary>
		/// <param name="colorCount"></param>
		/// <param name="gradient"></param>
		/// <returns></returns>
		public static List<ColorHSL> GenerateColorsRandomGradient(int colorCount, GradientHSL gradient)
		{
			var colors = new List<ColorHSL>();

			for (int i = 0; i < colorCount; i++)
			{
				var color = gradient.Evaluate(Random.value);
				colors.Add(color);
			}

			return colors;
		}

		/// <summary>
		/// Generate colors evely spaced out on a given gradient.
		/// </summary>
		/// <param name="colorCount"></param>
		/// <param name="gradient"></param>
		/// <param name="jitter"></param>
		/// <returns></returns>
		public static List<Color> GenerateColorsEvenGradient(
			int colorCount, 
			Gradient gradient, 
			float jitter)
		{
			return gradient.SampleEvenly(colorCount, jitter).ToList();
		}

		/// <summary>
		/// Generate colors evely spaced out on a given gradient.
		/// </summary>
		/// <param name="colorCount"></param>
		/// <param name="gradient"></param>
		/// <param name="jitter"></param>
		/// <returns></returns>
		public static List<ColorHSL> GenerateColorsEvenGradient(
			int colorCount,
			GradientHSL gradient,
			float jitter)
		{
			return gradient.SampleEvenly(colorCount, jitter).ToList();
		}
	}
}