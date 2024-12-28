using UnityEditor;
using UnityEngine;

namespace com.game.statsystem.editor
{
    [CustomPropertyDrawer(typeof(PlayerStatModification))]
    public class PlayerStatModificationPropertyDrawer : PropertyDrawer
    {
        private const float MOD_TYPE_ICON_WIDTH = 30f;
        private const float HORIZONTAL_SPACING = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool isArrayElement = property.propertyPath.Contains("Array");
            int height = isArrayElement ? 2 : 3;

            return StatManipulatorEditorHelpers.CalculateHeight(isArrayElement, height);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty incrementalValueProp = property.FindPropertyRelative("m_incrementalValue");
            SerializedProperty percentageValueProp = property.FindPropertyRelative("m_percentageValue");
            SerializedProperty modTypeProp = property.FindPropertyRelative("ModificationType");

            property.serializedObject.Update();

            float incrementalValue = incrementalValueProp.floatValue;
            float percentageValue = percentageValueProp.floatValue;
            StatModificationType modType = (StatModificationType)modTypeProp.enumValueIndex;

            GUIContent actualLabel = EditorGUI.BeginProperty(position, label, property);

            Rect actualPosition = StatManipulatorEditorHelpers.BeginManipulator(position, property, $"Player Stat Modification ({actualLabel})",
                out PlayerStatType statType);

            actualPosition.height = EditorGUIUtility.singleLineHeight;

            float width = actualPosition.width - HORIZONTAL_SPACING;
            float enumWidth = MOD_TYPE_ICON_WIDTH;
            float fieldWidth = width - MOD_TYPE_ICON_WIDTH;

            actualPosition.width = fieldWidth;

            if (modType == StatModificationType.Incremental) 
                incrementalValue = EditorGUI.FloatField(actualPosition, "Value", incrementalValue);

            else if (modType == StatModificationType.Percentage) 
                percentageValue = EditorGUI.FloatField(actualPosition, "Percentage", percentageValue);

            else 
                EditorGUI.LabelField(actualPosition, "Modification type not defined.");

            actualPosition.x += fieldWidth + HORIZONTAL_SPACING;
            actualPosition.width = enumWidth;

            modType = (StatModificationType)EditorGUI.EnumPopup(actualPosition, modType);

            if (StatManipulatorEditorHelpers.EndManipulator(property))
            {
                UnityEngine.Object target = property.serializedObject.targetObject;

                Undo.RecordObject(target, "Player Stat Modification Object (Editor)");

                StatManipulatorEditorHelpers.ApplyManipulatorChanges(property, statType);
                incrementalValueProp.floatValue = incrementalValue;
                percentageValueProp.floatValue = percentageValue;
                modTypeProp.enumValueIndex = (int)modType;

                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            EditorGUI.EndProperty();
        }
    }
}
