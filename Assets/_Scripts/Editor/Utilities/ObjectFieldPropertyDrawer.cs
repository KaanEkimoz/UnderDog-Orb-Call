using UnityEditor;
using UnityEngine;

namespace com.game.utilities.editor
{
    [CustomPropertyDrawer(typeof(ObjectFieldAttribute), true)]
    public class ObjectFieldPropertyDrawer : PropertyDrawer
    {
        public const float OBJECT_FIELD_HEIGHT = 70f;

        private ObjectFieldAttribute p_attribute => attribute as ObjectFieldAttribute;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return OBJECT_FIELD_HEIGHT;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Object value = property.objectReferenceValue;

            GUIContent actualLabel = EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            position.height = OBJECT_FIELD_HEIGHT;

            value = EditorGUI.ObjectField(position, actualLabel, value, p_attribute.Type, false);

            if (EditorGUI.EndChangeCheck())
            {
                Object target = property.serializedObject.targetObject;

                Undo.RecordObject(target, "Object Field (Editor)");

                property.objectReferenceValue = value;

                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            EditorGUI.EndProperty();
        }
    }
}
