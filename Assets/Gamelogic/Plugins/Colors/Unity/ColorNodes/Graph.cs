using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamelogic.Colors
{
	/// <summary>
	/// A class the represents a computational graph. Each node in this graph
	/// takes some inputs, and calculates outputs, that can in turn be fed as 
	/// inputs into other nodes.
	/// 
	/// All nodes produces outputs as lists. When a node has multiple inputs, 
	/// these are all combined into a single list for the node to operate on.
	/// </summary>
	[Version(1,1)]
	public sealed class Graph : ScriptableObject
	{
		#region Private Fields
		[HideInInspector]
		[SerializeField]
		private int idCounter;

		[HideInInspector]
		[SerializeField] 
// ReSharper disable once FieldCanBeMadeReadOnly.Local
// Cannot be readonly becuase it is serialized.
		private List<Node> nodes = new List<Node>();
		#endregion

		#region Properties
		/// <summary>
		/// Returns all the nodes in this graph.
		/// </summary>
		public List<Node> Nodes
		{
			get { return nodes; }
		}
		#endregion

		#region Public Methods
#if UNITY_EDITOR

		///  <summary>
		///  Creates and adds a new unlinked node to the graph.
		///  </summary>
		///  <typeparam name="T">The type of the node to add.</typeparam>
		/// <param name="initialPosition">The initial position the node will be displayed
		///     in the visual representation.</param>
		/// <returns>The newly created node.</returns>
		public void AddNode<T>(Vector2 initialPosition) 
			where T:Node
		{
			var node = CreateInstance<T>();

			node.id = idCounter;
			node.name = "(" + idCounter + ") " + node.GetType().Name;
			node.rect = new Rect(initialPosition.x, initialPosition.y, 0, 0);

			nodes.Add(node);
			idCounter++;
		
			AssetDatabase.AddObjectToAsset(node, this);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(node));
		
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Unlinks this node from other nodes, destroys it, and removes it from the graph.
		/// </summary>
		/// <param name="node">The node to remove.</param>
		public void RemoveNode(Node node)
		{
			nodes.Remove(node);

			foreach (var node1 in nodes)
			{
				node1.RemoveNodeInput(node);
			}

			var path = AssetDatabase.GetAssetOrScenePath(node);
			DestroyImmediate(node, true);

			AssetDatabase.ImportAsset(path);
			AssetDatabase.SaveAssets();
			EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Removes all nodes from this graph.
		/// </summary>
		public void Clear()
		{
			foreach (var node in nodes)
			{
				DestroyImmediate(node, true);
			}

			nodes.Clear();

			var path = AssetDatabase.GetAssetOrScenePath(this);

			AssetDatabase.ImportAsset(path);
			AssetDatabase.SaveAssets();
			EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Save the asset database.
		/// </summary>
		public void Save()
		{
			AssetDatabase.SaveAssets();
		}
#endif

		/// <summary>
		/// Calls recompute on all nodes in the graph.
		/// </summary>
		public void Recompute()
		{
			foreach (var node in nodes)
			{
				node.Recompute();
			}

			foreach (var node in nodes)
			{
				node.UpdateStatic();
			}
		}
		#endregion
	}
}