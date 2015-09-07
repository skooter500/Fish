using UnityEditor;
using UnityEngine;

namespace Gamelogic.Colors.Editor
{
	[Version(1, 1)]
	[CustomPropertyDrawer(typeof(ReadonlyColorList), true)]
	public class ReadonlyColorListPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!property.isArray)
			{
				//Debug.LogError("must be an array");
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			int colorCount = property.arraySize;
			if (colorCount == 0) return;

			if (property.GetArrayElementAtIndex(0).propertyType == SerializedPropertyType.Color)
			{
				float width = position.width/5;
				float x = position.x;
				float y = position.y;
				float height = 16;

				var indentLevel = EditorGUI.indentLevel;

				EditorGUI.indentLevel = 0;

				//Only draw 100 colors
				for (int i = 0; i < Mathf.Min(100, colorCount); i++)
				{
					var colorProp = property.GetArrayElementAtIndex(i);
					var color = colorProp.colorValue;
					var rect = new Rect(x, y, width, height);
					EditorGUI.DrawRect(rect, color);

					x += width;

					if ((i + 1)%5 == 0)
					{
						x = position.x;
						y += height;
					}
				}

				EditorGUI.indentLevel = indentLevel;
			}
			else
			{
				EditorGUI.PropertyField(position, property, label);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!property.isArray)
			{
				//Debug.LogError("must be an array");
				Debug.Log(property.propertyType);
				return base.GetPropertyHeight(property, label);
			}

			int colorCount = property.arraySize;
			if (colorCount == 0) return 0;

			if (property.GetArrayElementAtIndex(0).propertyType == SerializedPropertyType.Color)
			{
				return 16*Mathf.Ceil(colorCount/5f);
			}
			else
			{
				return base.GetPropertyHeight(property, label);
			}
		}
	}
}