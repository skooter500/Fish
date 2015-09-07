using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Colors.Examples
{
	public class GUIExample : GLMonoBehaviour
	{
		public Graph graph;

		public List<Button> buttons;
		public InspectableColorNode buttonNodeColor;

		public void Start()
		{
			Reset();
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Reset();
			}
		}

		public void Reset()
		{
			graph.Recompute();

			var generator = buttonNodeColor.Colors.ToPeriodicGenerator();

			foreach (var button in buttons)
			{
				button.image.color = generator.Next();
			}
		}
	}
}