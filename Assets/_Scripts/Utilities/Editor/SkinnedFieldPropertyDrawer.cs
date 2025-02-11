using UnityEditor;
using UnityEngine;

namespace com.game.utilities
{
    [CustomPropertyDrawer(typeof(SkinnedField), true)]
    public class SkinnedFieldPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty skinValueProp = property.FindPropertyRelative("SkinValue");

            return EditorGUI.GetPropertyHeight(skinValueProp, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SkinnedField boxedValue = property.boxedValue as SkinnedField;
            SerializedProperty skinValueProp = property.FindPropertyRelative("SkinValue");
            SerializedProperty realValueProp = property.FindPropertyRelative("RealValue");

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            label.tooltip = realValueProp.boxedValue.ToString();

            EditorGUI.PropertyField(position, skinValueProp, label, true);
            realValueProp.boxedValue = boxedValue.Fetch();

            EditorGUI.EndProperty();
        }
    }
}
