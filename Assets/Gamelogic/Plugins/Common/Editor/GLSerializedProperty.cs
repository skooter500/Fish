using UnityEditor;
using Object = UnityEngine.Object;

namespace Gamelogic.Editor
{
	/// <summary>
	/// Wraps a SerializedProperty, and provides additional functions, such as
	/// tooltips and a more powerful Find method.
	/// </summary>
	[Version(1, 2)]
	public class GLSerializedProperty
	{
		public SerializedProperty SerializedProperty { get; set; }
		public string CustomTooltip { get; set; }

		public SerializedPropertyType propertyType
		{
			get { return SerializedProperty.propertyType; }
		}

		public Object objectReferenceValue
		{
			get { return SerializedProperty.objectReferenceValue; }
			set { SerializedProperty.objectReferenceValue = value; }
		}

		public int enumValueIndex
		{
			get { return SerializedProperty.enumValueIndex; }
			set { SerializedProperty.enumValueIndex = value; }
		}

		public bool boolValue
		{
			get { return SerializedProperty.boolValue; }
			set { SerializedProperty.boolValue = value; }
		}

		public int intValue
		{
			get { return SerializedProperty.intValue; }
			set { SerializedProperty.intValue = value; }
		}

		public string stringValue
		{
			get { return SerializedProperty.stringValue; }
			set { SerializedProperty.stringValue = value; }
		}

		public GLSerializedProperty FindPropertyRelative(string name)
		{
			SerializedProperty property = SerializedProperty.FindPropertyRelative(name);
			return new GLSerializedProperty
			{
				SerializedProperty = property
			};
		}
	}
}
