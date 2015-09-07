using System;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Colors.Examples
{
	
	/// <summary>
	/// A small utility script that supports regenerating colors using the space key, and
	/// taking screen shots using the F9 key.
	/// </summary>
	public class ColorExamplesNodes : GLMonoBehaviour
	{
		public Graph graph;

		public GameObject[] walls;
		public InspectableColorNode wallColorNode;

		public GameObject[] props;
		public InspectableColorNode propColorNode;

		public void Start()
		{
			ResetColors();
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				ResetColors();
			} 

			if (Input.GetKeyDown(KeyCode.F9))
			{
				Application.CaptureScreenshot("screen" + DateTime.Now.Ticks + ".png");
			}
		}

		private void ResetColors()
		{
			graph.Recompute();

			var wallColor = wallColorNode.Colors.First();

			foreach (var wall in walls)
			{
				wall.GetComponent<Renderer>().material.color = wallColor;
			}

			var propColors =
				propColorNode.Colors.ToRandomElementGenerator();

			foreach (var prop in props)
			{
				prop.GetComponent<Renderer>().material.color = propColors.Next();
			}
		}
	}
}