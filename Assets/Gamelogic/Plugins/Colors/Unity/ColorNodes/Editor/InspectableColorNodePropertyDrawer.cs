using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Colors.Editor
{
	/// <summary>
	/// An editor for the InspectableColorNode class.
	/// </summary>
	[Version(1, 1)]
	[CustomPropertyDrawer(typeof (InspectableColorNode))]
	public class InspectableColorNodePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var availableWidth = position.width - EditorGUIUtility.labelWidth;

			var graphRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth + availableWidth/2, position.height);
			var nodeRect = new Rect(position.x + EditorGUIUtility.labelWidth + availableWidth / 2, position.y, availableWidth / 2, position.height);
			var graphProperty = property.FindPropertyRelative("graph");
		
			EditorGUI.PropertyField(graphRect, graphProperty, label);

			var graph = graphProperty.objectReferenceValue as Graph;

			if (graph != null)
			{
				var nodes = graph.Nodes.ToList();

				//Debug.Log(graph.Nodes.ListToString());

				var nodeProperty = property.FindPropertyRelative("selectedNode");
				var selectedNode = nodeProperty.objectReferenceValue as Node;
				int selectedNodeIndex = -1;

				if (selectedNode != null)
				{
					selectedNodeIndex = graph.Nodes.FindIndex(
						n => n.id == selectedNode.id
						);
				}
			
				selectedNodeIndex = EditorGUI.Popup(
					nodeRect,
					selectedNodeIndex,
					nodes.Select(n => n.name).ToArray());

				if (selectedNodeIndex != -1)
				{
					//inspectableColorNode.se
					nodeProperty.objectReferenceValue = nodes[selectedNodeIndex];
				}
			
			}

			//if (GUI.changed)
			{
			
				property.serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.EndProperty();  
		}
	}
}