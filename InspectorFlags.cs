using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DomeMasterSystem {
	[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Enum)]
	public sealed class InspectorFlagsAttribute : PropertyAttribute {}

	#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(InspectorFlagsAttribute))]
	public sealed class InspectorFlagsAttributeDrawer : PropertyDrawer {
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			property.intValue = EditorGUI.MaskField (position, label, property.intValue, property.enumNames);
		}
	}
	#endif
}
