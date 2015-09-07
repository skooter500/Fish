using UnityEngine;

namespace Gamelogic.Colors.Examples
{
/**
	A small utility script that supports regenerating colors using the space key, and
	taking screen shots using the F9 key.
*/

	public class ColorExamplesLegacy : GLMonoBehaviour
	{
		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				var colorGroups = FindObjectOfType<ColorGroups>();

				if (colorGroups != null)
				{
					colorGroups.SampleColors();
				}
			}
		}
	}
}