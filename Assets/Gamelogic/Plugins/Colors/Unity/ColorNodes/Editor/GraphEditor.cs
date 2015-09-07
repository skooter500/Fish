using UnityEditor;
using UnityEngine;

namespace Gamelogic.Colors.Editor
{
	/// <summary>
	/// Editor for displaying a Graph in the inspector.
	/// </summary>
	[Version(1, 1)]
	[CustomEditor(typeof(Graph))]
	public class GraphEditor : UnityEditor.Editor
	{
		override public void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("Edit"))
			{
				GraphWindow.ShowEditor((Graph)target);
			}
		}
	}
}