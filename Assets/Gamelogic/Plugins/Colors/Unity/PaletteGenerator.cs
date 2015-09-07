using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Colors
{
	/*
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Algorithms for generating colors
	/// </summary>
	[Version(1)]
	[Serializable]
	public enum ColorGenerationAlgorithm
	{
		Uniform,
		RandomWalk,
		Gradient,
		RandomOffset,
		RandomMix,
		RandomHue,
		RandomSaturation,
		RandomLuminance,
		RandomSaturationLuminance,
		Harmony,
		Tints,
		Shades
	}
	
	/*
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for a color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class ColorPaletteProperties
	{
		/** The number of colors to generate */
		[NonNegative] public int colorCount = 10;
	}

	/*
		@version1_0
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the uniform color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class UniformProperties : ColorPaletteProperties
	{
		public ColorSpace colorSpace = ColorSpace.RGB;
	}

	/*
		@version1_0
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the random walk color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class RandomWalkProperties : ColorPaletteProperties
	{
		public Color startColor;

		public MinMaxFloat offset;
		public bool fixLuminence;
	}

	/*		
		@version1_0
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the gradient generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class RandomGradientProperties : ColorPaletteProperties
	{
		public Gradient gradient;
		public GradientSelectionMode selectionMode;

		[Range(0, 1)] public float jitter;
	}

	/*
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the random offset color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class RandomOffsetProperties : ColorPaletteProperties
	{
		public Color baseColor;
		public MinMaxFloat offset;
	}

	/*		
		@version1_0
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the random mix color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class RandomMixProperties : ColorPaletteProperties
	{
		public Color color1;
		public Color color2;
		public Color color3;
		[Range(0, 1)] public float greyControl;
		public ColorSpace mixMode;
	}

	/*
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the random hue color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class RandomHueProperties : ColorPaletteProperties
	{
		[Range(0, 1)] public float saturation;
		[Range(0, 1)] public float luminance;
	}

	/*
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the random saturation color generation algorithm. 
	/// </summary>
	[Version(1)]
	[Serializable]
	public class RandomSaturation : ColorPaletteProperties
	{
		[Range(0, 1)] public float hue;

		[Range(0, 1)] public float luminance;
	}

	/*
		@version1_0
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the random luminance color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class RandomLuminance : ColorPaletteProperties
	{
		[Range(0, 1)] public float hue;
		[Range(0, 1)] public float saturation;
	}

	/*
		@version1_0
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the random saturation-luminance color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class RandomSaturationLuminance : ColorPaletteProperties
	{
		[Range(0, 1)] public float hue;
	}

	/*
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the tints color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class Tints : ColorPaletteProperties
	{
		public Color color;
	}

	/*
		@version1_0
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the shadescolor generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class Shades : ColorPaletteProperties
	{
		public Color color;
	}

	/*
		@version1_0
		@ingroup EditorSupport
	*/
	/// <summary>
	/// Properties for the harmony color generation algorithm.
	/// </summary>
	[Version(1)]
	[Serializable]
	public class HarmonyProperties : ColorPaletteProperties
	{
		public float offsetAngle0;
		public float offsetAngle1;
		public float offsetAngle2;
		public float rangeAngle0;
		public float rangeAngle1;
		public float rangeAngle2;
		public float saturation;
		public float saturationRange;
		public float luminance;
		public float luminanceRange;
	}

	/*
		@ingroup EditorSupport
	*/
	/// <summary>
	/// A monobehaviour that can be used to configure a color palette. It has various methods, each generating a 
	/// different type of palette based on the configured settings.
	/// </summary>
	[Version(1)]
	[ExecuteInEditMode]
	public class PaletteGenerator : GLMonoBehaviour
	{
		/**
			The algorithm used to generate colors.
			Call Generate() after you chnaged this property 
			to get a new set of colors.
		*/
		public ColorGenerationAlgorithm algorithm = ColorGenerationAlgorithm.Uniform;
		
		public UniformProperties uniform;
		public RandomWalkProperties randomWalk;
		public RandomGradientProperties gradient;
		public RandomOffsetProperties offset;
		public RandomMixProperties randomMix;
		public RandomHueProperties randomHue;
		public RandomSaturation randomSaturation;
		public RandomLuminance randomLuminance;
		public RandomSaturationLuminance randomSaturationLuminance;
		public HarmonyProperties harmonyProperties;
		public Tints tints;
		public Shades shades;
		
		/// <summary>
		/// A palette that contains the last set of colors generated.
		/// </summary>
		public Palette palette;

		private void GenerateUniform()
		{
			List<Color> colors;

			switch (uniform.colorSpace)
			{
				case ColorSpace.RGB:
					colors = ProceduralPalette.GenerateUniform(uniform.colorCount);
					break;
				case ColorSpace.HSL:
					colors = ProceduralPalette.GenerateUniformHSL(uniform.colorCount);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			palette = new Palette(colors);
		}

		private void GenerateRandomWalk()
		{
			var colors = ProceduralPalette.GenerateRandomWalk(
				randomWalk.colorCount,
				randomWalk.startColor,
				randomWalk.offset.min,
				randomWalk.offset.max,
				randomWalk.fixLuminence);

			palette = new Palette(colors);
		}

		private void GenerateRandomGradient()
		{
			List<Color> colors;

			switch (gradient.selectionMode)
			{
				case GradientSelectionMode.Even:
					colors = ProceduralPalette.GenerateColorsEvenGradient(gradient.colorCount, gradient.gradient, 0);
					break;
				case GradientSelectionMode.Random:
					colors = ProceduralPalette.GenerateColorsRandomGradient(gradient.colorCount, gradient.gradient);
					break;
				case GradientSelectionMode.GoldenRatio:
					colors = ProceduralPalette.GenerateColorsGoldenRatioGradient(gradient.colorCount, gradient.gradient);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}


			palette = new Palette(colors);
		}

		private void GenerateRandomOffset()
		{
			var colors = ProceduralPalette.GenerateRandomOffset(offset.colorCount, offset.baseColor, offset.offset.min,
				offset.offset.max);

			palette = new Palette(colors);
		}

		private void GenerateRandomMix()
		{
			var colors = ProceduralPalette.GenerateRandomMix(
				randomMix.colorCount,
				randomMix.color1,
				randomMix.color2,
				randomMix.color3,
				randomMix.greyControl,
				randomMix.mixMode);

			palette = new Palette(colors);
		}

		private void GenerateRandomHue()
		{
			var colors = ProceduralPalette.GenerateRandomHue(randomHue.colorCount, randomHue.saturation, randomHue.luminance);

			palette = new Palette(colors);
		}

		private void GenerateRandomSaturation()
		{
			var colors = ProceduralPalette.GenerateRandomSaturation(randomSaturation.colorCount, randomSaturation.hue,
				randomSaturation.luminance);

			palette = new Palette(colors);
		}

		private void GenerateRandomLuminance()
		{
			var colors = ProceduralPalette.GenerateRandomLuminance(randomLuminance.colorCount, randomLuminance.hue,
				randomLuminance.saturation);

			palette = new Palette(colors);
		}

		private void GenerateRandomSaturationLuminance()
		{
			var colors = ProceduralPalette.GenerateRandomSaturationLuminance(randomSaturationLuminance.colorCount,
				randomSaturationLuminance.hue);

			palette = new Palette(colors);
		}

		private void GenerateHarmony()
		{
			var colors = ProceduralPalette.GenerateHarmony(
				harmonyProperties.colorCount,
				harmonyProperties.offsetAngle0,
				harmonyProperties.offsetAngle1,
				harmonyProperties.offsetAngle2,
				harmonyProperties.rangeAngle0,
				harmonyProperties.rangeAngle1,
				harmonyProperties.rangeAngle2,
				harmonyProperties.saturation,
				harmonyProperties.saturationRange,
				harmonyProperties.luminance,
				harmonyProperties.luminanceRange);

			palette = new Palette(colors);
		}

		private void GenerateShades()
		{
			var hsl = new ColorHSL(shades.color);
			var colors = hsl.GetShades(shades.colorCount).Select(c => c.Color);
			palette = new Palette(colors);
		}

		private void GenerateTints()
		{
			var hsl = new ColorHSL(tints.color);
			var colors = hsl.GetTints(tints.colorCount).Select(c => c.Color);
			palette = new Palette(colors);
		}

		/// <summary>
		/// Regenerates the palette with the current settings.
		/// </summary>
		public void Awake()
		{
			if (uniform == null)
			{
				uniform = new UniformProperties();
			}

			Generate();
		}

		/// <summary>
		/// Generates colors with the algorithm defined by the algorithm field, 
		/// and the associtated properties.
		/// 
		/// Each time this method is called, a different set of colors is created.
		/// </summary>
		public void Generate()
		{
			switch (algorithm)
			{
				case ColorGenerationAlgorithm.Uniform:
					GenerateUniform();
					break;
				case ColorGenerationAlgorithm.RandomWalk:
					GenerateRandomWalk();
					break;
				case ColorGenerationAlgorithm.Gradient:
					GenerateRandomGradient();
					break;
				case ColorGenerationAlgorithm.RandomOffset:
					GenerateRandomOffset();
					break;
				case ColorGenerationAlgorithm.RandomMix:
					GenerateRandomMix();
					break;
				case ColorGenerationAlgorithm.RandomHue:
					GenerateRandomHue();
					break;
				case ColorGenerationAlgorithm.RandomSaturation:
					GenerateRandomSaturation();
					break;
				case ColorGenerationAlgorithm.RandomLuminance:
					GenerateRandomLuminance();
					break;
				case ColorGenerationAlgorithm.RandomSaturationLuminance:
					GenerateRandomSaturationLuminance();
					break;
				case ColorGenerationAlgorithm.Harmony:
					GenerateHarmony();
					break;
				case ColorGenerationAlgorithm.Tints:
					GenerateTints();
					break;
				case ColorGenerationAlgorithm.Shades:
					GenerateShades();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
