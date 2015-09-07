//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Editor.Internal
{
	/**
		@version1_2
	*/
	public class GLEditor<T> : UnityEditor.Editor
	{
		public T Target
		{
			get { return (T) (object) target; }
		}

		public T[] Targets
		{
			get { return targets.Cast<T>().ToArray(); }
		}

		public void Splitter()
		{
			GLEditorGUI.Splitter();
		}

		public static int AddCombo(string[] options, int selectedIndex)
		{
			return EditorGUILayout.Popup(selectedIndex, options);
		}

		public bool HasProperty(string propertyName)
		{
			var property = serializedObject.FindProperty(propertyName);

			return property != null;
		}

		public GLSerializedProperty FindProperty(string propertyName)
		{
			var property = new GLSerializedProperty
			{
				SerializedProperty = serializedObject.FindProperty(propertyName),
				CustomTooltip = ""
			};

			if (property.SerializedProperty == null)
			{
				Debug.LogError("Could not find property " + propertyName + " in class " + typeof (T).Name);

				return property;
			}

			Type type = typeof (T);

			FieldInfo field = type.GetField(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (field == null)
			{
				Debug.LogError("Could not find field " + propertyName + " in class " + typeof (T).Name);

				return property;
			}

			/*
			var descriptions = field.GetCustomAttributes(typeof (DescriptionAttribute), true);

			if (descriptions.Any())
			{
				property.CustomTooltip = (descriptions.First() as DescriptionAttribute).Description;
			}
			*/
			return property;
		}

		protected void AddField(SerializedProperty prop)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(prop, true);
			EditorGUILayout.EndHorizontal();
		}

		protected void AddField(GLSerializedProperty prop)
		{
			if (prop == null) return;
			if (prop.SerializedProperty == null) return;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(prop.SerializedProperty,
				new GUIContent(prop.SerializedProperty.name.SplitCamelCase(), prop.CustomTooltip), true);
			EditorGUILayout.EndHorizontal();
		}

		protected void AddLabel(string title, string text)
		{
			EditorGUILayout.LabelField(title, text);
		}
	}
}


