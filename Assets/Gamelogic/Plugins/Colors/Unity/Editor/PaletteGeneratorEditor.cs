using System;
using Gamelogic.Editor;

using Gamelogic.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Colors.Editor
{
	/// <summary>
	/// The editor used for PaletteGenerators.
	/// </summary>
	[Version(1)]
	[CustomEditor(typeof (PaletteGenerator))]
	public class PaletteGeneratorEditor : GLEditor<PaletteGenerator>
	{
		private GLSerializedProperty algorithmProp;
		private GLSerializedProperty currentProeprtyProp;
		private GLSerializedProperty uniformProp;
		private GLSerializedProperty randomWalkProp;
		private GLSerializedProperty gradientProp;
		private GLSerializedProperty offsetProp;
		private GLSerializedProperty randomMixProp;
		private GLSerializedProperty randomHueProp;
		private GLSerializedProperty randomSaturationProp;
		private GLSerializedProperty randomLuminanceProp;
		private GLSerializedProperty randomSaturationLuminanceProp;
		private GLSerializedProperty harmonyPropertiesProp;
		private GLSerializedProperty tintsProp;
		private GLSerializedProperty shadesProp;

		private GLSerializedProperty colorsProp;

		public void OnEnable()
		{
			algorithmProp = FindProperty("algorithm");

			uniformProp = FindProperty("uniform");
			randomWalkProp = FindProperty("randomWalk");
			gradientProp = FindProperty("gradient");
			offsetProp = FindProperty("offset");
			randomMixProp = FindProperty("randomMix");
			randomHueProp = FindProperty("randomHue");
			randomSaturationProp = FindProperty("randomSaturation");
			randomLuminanceProp = FindProperty("randomLuminance");
			randomSaturationLuminanceProp = FindProperty("randomSaturationLuminance");
			harmonyPropertiesProp = FindProperty("harmonyProperties");
			tintsProp = FindProperty("tints");
			shadesProp = FindProperty("shades");
			colorsProp = FindProperty("palette").FindPropertyRelative("colors");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUILayout.Button("Regenerate");

			AddField(algorithmProp);

			var algorithm = (ColorGenerationAlgorithm) algorithmProp.enumValueIndex;

			switch (algorithm)
			{
				case ColorGenerationAlgorithm.Uniform:
					AddField(uniformProp);
					break;
				case ColorGenerationAlgorithm.RandomWalk:
					AddField(randomWalkProp);
					break;
				case ColorGenerationAlgorithm.Gradient:
					AddField(gradientProp);
					break;
				case ColorGenerationAlgorithm.RandomOffset:
					AddField(offsetProp);
					break;
				case ColorGenerationAlgorithm.RandomMix:
					AddField(randomMixProp);
					break;
				case ColorGenerationAlgorithm.RandomHue:
					AddField(randomHueProp);
					break;
				case ColorGenerationAlgorithm.RandomSaturation:
					AddField(randomSaturationProp);
					break;
				case ColorGenerationAlgorithm.RandomLuminance:
					AddField(randomLuminanceProp);
					break;
				case ColorGenerationAlgorithm.RandomSaturationLuminance:
					AddField(randomSaturationLuminanceProp);
					break;
				case ColorGenerationAlgorithm.Harmony:
					AddField(harmonyPropertiesProp);
					break;
				case ColorGenerationAlgorithm.Tints:
					AddField(tintsProp);
					break;
				case ColorGenerationAlgorithm.Shades:
					AddField(shadesProp);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				
				foreach (var t in targets)
				{
					((PaletteGenerator) t).Generate();
				}

				serializedObject.ApplyModifiedProperties();
			}

			EditorUtils.DrawColors(colorsProp.SerializedProperty, EditorGUILayout.GetControlRect(false, 32));

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}