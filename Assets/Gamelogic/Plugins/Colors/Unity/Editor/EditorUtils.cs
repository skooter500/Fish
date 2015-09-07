using Gamelogic.Colors;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Colors.Editor
{
	/// <summary>
	/// General utilities used for editor code.
	/// </summary>
	[Version(1, 1)]
	public static class EditorUtils 
	{
		public static void DrawColors(SerializedProperty colorsProp, Rect position)
		{
			int colorCount = colorsProp.arraySize;

			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();
			
			int columns = (int) (position.width/16);
			

			float x = position.x;
			float width = 16;
			float height = 16;
			float y = position.y;

			if (columns > 0)
			{
				var indentLevel = EditorGUI.indentLevel;

				EditorGUI.indentLevel = 0;

				for (int i = 0; i < Mathf.Min(100, colorCount); i++)
				{

					if (i != 0 && i%columns == 0)
					{
						x = position.x;
						y += height;
						//EditorGUILayout.EndHorizontal();
						//EditorGUILayout.BeginHorizontal();
					}

					var colorProp = colorsProp.GetArrayElementAtIndex(i);
					var color = colorProp.colorValue;

					//var rect = EditorGUILayout.GetControlRect(false, 16, GUILayout.Width(16));

					//EditorGUI.DrawRect(rect, color);
					EditorGUIUtility.DrawColorSwatch(new Rect(x, y, width-2, height-2), color);
					x += width;
				}

				EditorGUI.indentLevel = indentLevel;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		public static void DrawNodeCurve(Rect start, Rect end)
		{
			var endPos = new Vector3(end.x, end.y + end.height / 2, 0);

			DrawNodeCurve(start, endPos);
		}

		public static void DrawNodeCurve(Rect start, Vector3 endPos)
		{
			var startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
			var startTan = startPos + Vector3.right * 50;
			var endTan = endPos + Vector3.left * 50;
			var shadowCol = new Color(0, 0, 0, 0.06f);

			for (int i = 0; i < 3; i++)
			{// Draw a shadow
				Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
			}

			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
			
			var oldColor = Handles.color;
			
			Handles.color = new Color(.3f, 0.1f, 0.1f);

			Handles.DrawSolidDisc(
				(startPos + endPos) / 2,
				Vector3.forward,
				5);
			
			Handles.color = Color.black;

			Handles.DrawWireDisc(
				(startPos + endPos) / 2,
				Vector3.forward,
				5);

			Handles.color = oldColor;
		}
	}
}

namespace Gamelogic.Colors.Editor.Internal
{
	[InitializeOnLoad]
	public static class GLEditorExtensions
	{
		private static Texture2D colorsIcon;
		private static Texture2D cellIcon;

		private static Texture2D blackSquare;

		private static Texture2D ColorsIcon
		{
			get
			{
				if (colorsIcon == null)
				{
					colorsIcon = (Texture2D) Resources.Load("ColorsEditor/colors");
				}
				return colorsIcon;
			}
		}

		public static Texture2D BlackSquare
		{
			get
			{
				if (blackSquare == null)
				{
					blackSquare = (Texture2D)Resources.Load("black");
				}
				return blackSquare;
			}
		}

		// constructor
		static GLEditorExtensions()
		{
			EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon;
			EditorApplication.projectWindowItemOnGUI += DrawProjectViewIcon;
		}

		private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
		{
			var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

			if (gameObject == null)
				return;

			var rect = new Rect(selectionRect.x + selectionRect.width - 18f, selectionRect.y, 16f, 16f);

			var view = gameObject.GetComponent(typeof (PaletteGenerator));
			
			if (view != null)
			{
				GUI.DrawTexture(rect, ColorsIcon);
			}
		}

		private static void DrawProjectViewIcon(string gUID, Rect selectionRect)
		{
			var path = AssetDatabase.GUIDToAssetPath(gUID);
			if (!path.EndsWith(".colorgraph.asset")) return;

			var rect = new Rect(selectionRect.x + selectionRect.width - 18f, selectionRect.y, 16f, 16f);
			
			GUI.DrawTexture(rect, ColorsIcon);
		}

		/// <summary>
		/// Brings up a save file dialog that allows the user to specify a location to 
		/// save a new colorgraph, makes a new colorgraph, and saves it to the specified 
		/// location.
		/// </summary>
		[MenuItem("Gamelogic/New Color Graph")]
		public static void MakeNewColorGraph()
		{
			var graph = ScriptableObject.CreateInstance<Graph>();

			var path = EditorUtility.SaveFilePanel(
				"Create new Color Graph",
				"Assets",
				"ColorGraph" + ".colorgraph.asset",
				"asset");

			if (path != "")
			{
				path = "Assets" + path.Substring(Application.dataPath.Length);
				Debug.Log(path);
				AssetDatabase.CreateAsset(graph, path);
				AssetDatabase.SaveAssets();
			}
		}
	}
}