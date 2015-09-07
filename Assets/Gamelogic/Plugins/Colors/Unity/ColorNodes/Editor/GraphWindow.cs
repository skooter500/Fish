using System;
using System.Linq;
using Gamelogic.Editor;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gamelogic.Colors.Editor
{
	/// <summary>
	/// A window for editing graphs.
	/// </summary>
	[Version(1, 1)]
	public class GraphWindow : EditorWindow
	{
		public Dictionary<int, SerializedObject> serilializedObjects = new Dictionary<int, SerializedObject>();	
	
		private Graph graph;
		private bool addLink;
		private Node inputWindow;

		private Vector2 mainScrollPosition = Vector2.zero;
		private Vector2 toolbarScrollPosition = Vector2.zero;
	
		public static void ShowEditor(Graph graph)
		{
			var editor = GetWindow<GraphWindow>("Graph");

			editor.graph = graph;
			editor.UpdateSerializablObjects();
			editor.addLink = false;
		}

		public void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			DrawToolbar();
		
			GLEditorGUI.VerticalLine();

			mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

			GUILayout.Label("", GUILayout.Width(2000), GUILayout.Height(1000));
			AddLink();
			DrawWindows();
			DrawCurves();
			HandleNodeDeleteButtons();
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndHorizontal();
		}

		private void HandleNodeDeleteButtons()
		{
			var e = Event.current;

			if (e.type != EventType.mouseDown) return;

			var mousePos = e.mousePosition;

			foreach (var window in graph.Nodes)
			{
				foreach (var input in window.Inputs)
				{
					var x = (input.rect.x + input.rect.width + window.rect.x)/2;
					var y = (input.rect.y + input.rect.height/2 + window.rect.y + window.rect.height/2)/2;

					var pos = new Vector2(x, y);
				
					//Debug.Log(pos + " " + (mousePos - pos).magnitude);

					if ((mousePos - pos).magnitude <= 10)
					{
						Debug.Log("Clicked");

						window.RemoveNodeInput(input);
					
						e.Use();
						ApplySerilizedProperties();
						Repaint();
						return;
					}
				}
			}
		}

		private void DrawCurves()
		{
			if (graph != null && graph.Nodes != null)
			{
				foreach (var window in graph.Nodes)
				{
					foreach (var input in window.Inputs)
					{
						EditorUtils.DrawNodeCurve(input.rect, window.rect);
					}
				}
			}
		}

		private void AddLink()
		{
			if (!addLink) return;
		
			var e = Event.current;
			var mousePos = e.mousePosition;

			EditorUtils.DrawNodeCurve(inputWindow.rect, mousePos);
			Repaint();

			if (e.type != EventType.mouseDown) return;

			var outputWindow = graph.Nodes.FirstOrDefault(t => t.rect.Contains(mousePos));

			if (outputWindow != null)
			{
				outputWindow.AddNodeInput(inputWindow);
				serilializedObjects[outputWindow.id].ApplyModifiedProperties();
			}

			e.Use();

			addLink = false;
			inputWindow = null;
		}

		private void DrawToolbar()
		{
			toolbarScrollPosition = EditorGUILayout.BeginScrollView(toolbarScrollPosition, GUILayout.Width(120));
			EditorGUILayout.BeginVertical();
		
			if (GraphToolbarButton("Clear"))
			{
				graph.Clear();
				UpdateSerializablObjects();
			}

			if (GraphToolbarButton("Recompute"))
			{
				graph.Recompute();
				ApplySerilizedProperties();
			}

			EditorGUILayout.Separator();

			if (GraphToolbarButton("Color"))
			{
				AddNode<ConstantColorNode>();
			}
			if (GraphToolbarButton("RandomColor"))
			{
				AddNode<RandomColorNode>();
			}

			EditorGUILayout.Separator();

			if (GraphToolbarButton("Mix"))
			{
				AddNode<MixNode2>();
			}
			if (GraphToolbarButton("Random Mix 3"))
			{
				AddNode<RandomMix3Node>();
			}

			EditorGUILayout.Separator();

			if (GraphToolbarButton("Set Hue"))
			{
				AddNode<SetHue>();
			}
			if (GraphToolbarButton("Offset Hue"))
			{
				AddNode<OffsetHue>();
			}

			if (GraphToolbarButton("Set Luminance"))
			{
				AddNode<SetLuminance>();
			}
			if (GraphToolbarButton("Offset Luminance"))
			{
				AddNode<OffsetLuminance>();
			}

			if (GraphToolbarButton("Set Saturation"))
			{
				AddNode<SetSaturation>();
			}
			if (GraphToolbarButton("Offset Saturation"))
			{
				AddNode<OffsetSaturation>();
			}

			EditorGUILayout.Separator();

			if (GraphToolbarButton("Random Offset"))
			{
				AddNode<RandomOffsetNode>();
			}

			if (GraphToolbarButton("RandomWalk"))
			{
				AddNode<RandomWalkColorNode>();
			}

			EditorGUILayout.Separator();

			if (GraphToolbarButton("Invert Color"))
			{
				AddNode<InvertColorNode>();
			}
			if (GraphToolbarButton("Invert Luminosity"))
			{
				AddNode<InvertLuminosityNode>();
			}
			if (GraphToolbarButton("Invert Saturation"))
			{
				AddNode<InvertSaturationNode>();
			}
		
			EditorGUILayout.Separator();

			if (GraphToolbarButton("Gradient"))
			{
				AddNode<CreateGradientNode>();
			}
			if (GraphToolbarButton("Sample Gradient"))
			{
				AddNode<SampleGradientNode>();
			}

			if (GraphToolbarButton("Gradient HSL"))
			{
				AddNode<CreateGradientHSLNode>();
			}
			if (GraphToolbarButton("Sample Gradient HSL"))
			{
				AddNode<SampleGradientHSLNode>();
			}
		
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}

		private bool GraphToolbarButton(string label)
		{
			return GUILayout.Button(label, EditorStyles.toolbarButton);
		}

		private void DrawWindows()
		{
			BeginWindows();

			if (graph != null && graph.Nodes != null)
			{
				int i = 0;

				foreach (var nodeWindow in graph.Nodes)
				{
					var nodeWindowCopy = nodeWindow;
					nodeWindow.rect = GUILayout.Window(i, nodeWindow.rect, n => DrawNode(n, nodeWindowCopy), nodeWindow.name);
					i++;
				}
			}

			EndWindows();
		}

		private void ApplySerilizedProperties()
		{
			foreach (var serilializedObject in serilializedObjects.Values)
			{
				serilializedObject.ApplyModifiedProperties();
			}
		}

		public void OnEanble()
		{
			UpdateSerializablObjects();
		}

		public void UpdateSerializablObjects()
		{
			serilializedObjects.Clear();

			foreach (var node in graph.Nodes)
			{
				serilializedObjects[node.id] = new SerializedObject(node);
			}
		}

		public void AddNode<T>()
			where T:Node
		{
			graph.AddNode<T>(mainScrollPosition);

			UpdateSerializablObjects();
		}

		public void RemoveNode(Node window)
		{
			graph.RemoveNode(window);
			UpdateSerializablObjects();
		}

		public void DrawNode(int id, Node window)
		{
			//foreach (var input in window.Inputs)
			//{
			//	EditorUtils.DrawNodeCurve(input.rect, window.rect);
			//}

			float oldWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 100;

			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

			if (GUILayout.Button("Remove", EditorStyles.toolbarButton))
			{
				RemoveNode(window);
				return;
			}

			var serializedObject = serilializedObjects[window.id];
			serializedObject.Update();

			var outputProperty = serializedObject.FindProperty("output");

			if (outputProperty != null)
			{
				if (GUILayout.Button("Add Link", EditorStyles.toolbarButton))
				{
					addLink = true;
					inputWindow = window;
				}
			}

			GUILayout.FlexibleSpace();

			EditorGUILayout.EndHorizontal();

			var property = serilializedObjects[window.id].GetIterator();
			bool isFirst = true;
		
			while (property.NextVisible(isFirst))
			{
				isFirst = false;

				if (property.name != "m_Script" && property.name != "output")
				{
					EditorGUILayout.PropertyField(property, true);
				}
			
			}

			//EditorGUILayout.InspectorTitlebar(true, outputProperty.serializedObject.targetObject);

			if (outputProperty != null)
			{
				if (outputProperty.arraySize > 0)
				{
					var element = outputProperty.GetArrayElementAtIndex(0);

					if (element.propertyType == SerializedPropertyType.Color)
					{
						try
						{
							int height = Mathf.CeilToInt(outputProperty.arraySize/10f)*16;
						
							var rect = EditorGUILayout.GetControlRect(false, height, GUILayout.ExpandHeight(true));


							EditorUtils.DrawColors(outputProperty, rect);
						}
						catch (Exception)
						{
							//
						}
					
					}
					else
					{
						EditorGUILayout.PropertyField(outputProperty, true);
					}
				}
			}

			EditorGUIUtility.labelWidth = oldWidth;
			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
			}

			//GUI.DragWindow();
			GUI.DragWindow();
		}

		public void OnDisable()
		{
			ApplySerilizedProperties();
			graph.Save();
		}
	}
}