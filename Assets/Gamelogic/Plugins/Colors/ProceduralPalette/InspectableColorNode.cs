using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A class that can be used to access a graph and it's nodes and their outputs from 
	/// code.
	/// </summary>
	[Version(1, 1)]
	[Serializable]
	public class InspectableColorNode
	{
		/// <summary>
		/// The graph that should be accessed.
		/// </summary>
		public Graph graph;

		/// <summary>
		/// The node of the graph that this InspectableColorNode accesses.
		/// </summary>
		[HideInInspector]
		[SerializeField]
		public Node selectedNode;

		/// <summary>
		/// The output colors of the selected node.
		/// </summary>
		public IEnumerable<Color> Colors
		{
			get
			{
				if (selectedNode != null)
				{
					var outputNode = selectedNode as Node<Color>;

					if (outputNode != null)
					{
						return outputNode.Output;
					}
				}

				return null;
			}
		}
	}
}