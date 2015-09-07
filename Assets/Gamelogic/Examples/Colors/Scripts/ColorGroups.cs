using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Colors.Examples
{
	/**
		An example script that shows one way you can configure a scene to use colorrs from your palette.

		Objects in the same color group gets the same color.

		Objects in the uniqueObjects list each gets a different color.

		This object just chnages the material colors of renderers, and the colors of UI Images. You
		should be able to configure it easily for your own purposes.
	*/

	[Serializable]
	public class ColorGroup
	{
		public List<GameObject> objects;
	}

	public class ColorGroups : Singleton<ColorGroups>
	{
		public List<ColorGroup> colorGroups;
		public List<GameObject> uniqueObjects;

		public void Start()
		{
			SampleColors();
		}

		public void SampleColors()
		{
			var paletteGenerator = FindObjectOfType<PaletteGenerator>();
			paletteGenerator.Generate();

			int i = 0;

			foreach (var colorGroup in colorGroups)
			{
				var color = paletteGenerator.palette.colors[i];

				foreach (var obj in colorGroup.objects)
				{
					foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
					{
						renderer.material.color = color;
					}

					foreach (var image in obj.GetComponentsInChildren<Image>())
					{
						image.color = color;
					}
				}

				i++;

				i %= paletteGenerator.palette.colors.Length;
			}

			foreach (var obj in uniqueObjects)
			{
				var color = paletteGenerator.palette.colors[i];

				foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
				{
					renderer.material.color = color;
				}

				foreach (var image in obj.GetComponentsInChildren<Image>())
				{
					image.color = color;
				}

				i++;

				i %= paletteGenerator.palette.colors.Length;
			}
		}
	}
}