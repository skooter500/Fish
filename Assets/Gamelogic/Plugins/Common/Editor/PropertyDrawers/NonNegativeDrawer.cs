using UnityEditor;
using UnityEngine;

namespace Gamelogic.Editor
{
	/// <summary>
	/// A property drawer for fields marked with the NonNegative Attribute.
	/// </summary>
	[Version(1, 2)]
	[CustomPropertyDrawer(typeof (NonNegativeAttribute))]
	public class NonNegativeDrawer : PropertyDrawer
	{
		private const int TextHeight = 16;

		public override void OnGUI(Rect position,
			SerializedProperty prop,
			GUIContent label)
		{
			var textFieldPosition = position;
			textFieldPosition.height = TextHeight;

			switch (prop.propertyType)
			{
				case SerializedPropertyType.Integer:
				{
					EditorGUI.BeginChangeCheck();

					int n = EditorGUI.IntField(position, label, prop.intValue);

					if (EditorGUI.EndChangeCheck() && n >= 0)
						prop.intValue = n;
				}
				break;

				case SerializedPropertyType.Float:
				{
					EditorGUI.BeginChangeCheck();

					float x = EditorGUI.FloatField(position, label, prop.floatValue);

					if (EditorGUI.EndChangeCheck() && x >= 0)
						prop.floatValue = x;

				}
				break;

				default:
					EditorGUI.LabelField(position, label.text, "Use NonNegative with float or int");
				break;
			}
		}
	}
}