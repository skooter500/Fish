//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEditor;
using UnityEngine;

namespace Gamelogic.Editor
{
	/// <summary>
	/// Functions to suplement Unity EditorGUI functions.
	/// </summary>
	[Version(1, 2)]
	public static class GLEditorGUI
	{
		public static readonly GUIStyle SplitterStyle;
		public static readonly GUIStyle LineStyle;

		private static readonly Color SplitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);
		static GLEditorGUI()
		{
			SplitterStyle = new GUIStyle
			{
				normal = {background = EditorGUIUtility.whiteTexture},
				stretchWidth = true,
				margin = new RectOffset(0, 0, 7, 7)
			};

			LineStyle = new GUIStyle
			{
				normal = { background = EditorGUIUtility.whiteTexture },
				stretchWidth = true,
				margin = new RectOffset(0, 0, 0, 0)
			};
		}

		

		// GUILayout Style
		public static void Splitter(Color rgb, float thickness = 1)
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, SplitterStyle, GUILayout.Height(thickness));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = rgb;
				SplitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void Splitter(float thickness, GUIStyle splitterStyle)
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = SplitterColor;
				splitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void VerticalLine()
		{
			VerticalLine(SplitterColor, 2);
		}

		public static void VerticalLine(Color color, float thickness = 1)
		{
			Rect position = GUILayoutUtility.GetRect(
				GUIContent.none,
				LineStyle, 
				GUILayout.Width(thickness),
				GUILayout.ExpandHeight(true));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = color;
				LineStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void VerticalLine(float thickness, GUIStyle splitterStyle)
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Width(thickness));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = SplitterColor;
				splitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void Splitter(float thickness = 1)
		{
			Splitter(thickness, SplitterStyle);
		}

		// GUI Style
		public static void Splitter(Rect position)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = SplitterColor;
				SplitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}
	}
}