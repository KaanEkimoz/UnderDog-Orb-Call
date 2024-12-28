using UnityEditor;
using UnityEngine;

namespace com.game.statsystem.editor
{
    [CustomPropertyDrawer(typeof(PlayerStatOverride))]
    public class PlayerStatOverridePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool isArrayElement = property.propertyPath.Contains("Array");
            int height = isArrayElement ? 2 : 3;

            return StatManipulatorEditorHelpers.CalculateHeight(isArrayElement, height);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProp = property.FindPropertyRelative("NewValue");

            property.serializedObject.Update();

            float value = valueProp.floatValue;

            GUIContent actualLabel = EditorGUI.BeginProperty(position, label, property);

            Rect actualPosition = StatManipulatorEditorHelpers.BeginManipulator(position, property, $"Player Stat Override ({actualLabel})"
                , out PlayerStatType statType);

            actualPosition.height = EditorGUIUtility.singleLineHeight;

            value = EditorGUI.FloatField(actualPosition, "New Value", value);
            actualPosition.y += EditorGUIUtility.singleLineHeight;

            if (StatManipulatorEditorHelpers.EndManipulator(property))
            {
                UnityEngine.Object target = property.serializedObject.targetObject;

                Undo.RecordObject(target, "Player Stat Override Object (Editor)");

                StatManipulatorEditorHelpers.ApplyManipulatorChanges(property, statType);
                valueProp.floatValue = value;

                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            EditorGUI.EndProperty();
        }
    }
}
