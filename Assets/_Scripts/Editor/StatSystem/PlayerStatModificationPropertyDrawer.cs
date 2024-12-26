using UnityEditor;
using UnityEngine;

namespace com.game.statsystem.editor
{
    [CustomPropertyDrawer(typeof(PlayerStatModification))]
    public class PlayerStatModificationPropertyDrawer : PropertyDrawer
    {
        private const float MOD_TYPE_ICON_MAX_WIDTH = 30f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool isArrayElement = property.propertyPath.Contains("Array");
            float height = isArrayElement ? 2 : 3;

            return EditorGUIUtility.singleLineHeight * height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty statTypeProp = property.FindPropertyRelative("TargetStatType");
            SerializedProperty incrementalValueProp = property.FindPropertyRelative("m_incrementalValue");
            SerializedProperty percentageValueProp = property.FindPropertyRelative("m_percentageValue");
            SerializedProperty modTypeProp = property.FindPropertyRelative("ModificationType");

            property.serializedObject.Update();
            bool isArrayElement = property.propertyPath.Contains("Array");
            PlayerStatType statType = (PlayerStatType)statTypeProp.enumValueIndex;
            float incrementalValue = incrementalValueProp.floatValue;
            float percentageValue = percentageValueProp.floatValue;
            StatModificationType modType = (StatModificationType)modTypeProp.enumValueIndex;

            GUIContent actualLabel = EditorGUI.BeginProperty(position, label, property);

            if (!isArrayElement)
            {
                GUIStyle boxStyle = new GUIStyle("window");
                boxStyle.richText = true;

                GUILayout.BeginVertical($"Player Stat Modification ({actualLabel})", boxStyle);
            }

            EditorGUI.BeginChangeCheck();

            statType = (PlayerStatType)EditorGUILayout.EnumPopup(statType);

            EditorGUILayout.BeginHorizontal();

            if (modType == StatModificationType.Incremental) 
                incrementalValue = EditorGUILayout.FloatField("Value", incrementalValue);

            else if (modType == StatModificationType.Percentage) 
                percentageValue = EditorGUILayout.FloatField("Percentage", percentageValue);

            else 
                EditorGUILayout.LabelField("Modification type not defined.");

            modType = (StatModificationType)EditorGUILayout.EnumPopup(modType,
                GUILayout.MaxWidth(MOD_TYPE_ICON_MAX_WIDTH));

            EditorGUILayout.EndHorizontal();

            if (!isArrayElement) GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                UnityEngine.Object target = property.serializedObject.targetObject;

                Undo.RecordObject(target, "Player Stat Modification Object (Editor)");

                statTypeProp.enumValueIndex = (int)statType;
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
