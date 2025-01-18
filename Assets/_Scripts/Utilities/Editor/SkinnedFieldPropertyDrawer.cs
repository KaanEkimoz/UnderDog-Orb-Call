//using com.game.utilities;
//using UnityEditor;
//using UnityEngine;

//namespace com.game
//{
//    [CustomPropertyDrawer(typeof(SkinnedField))]
//    public class SkinnedFieldPropertyDrawer : PropertyDrawer
//    {
//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            return EditorGUI.GetPropertyHeight(property, label, true);
//        }

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            SerializedProperty realValueProp = property.FindPropertyRelative("RealValue");
//            SkinnedField fieldObject = property.boxedValue as SkinnedField;

//            GUIContent actualLabel = EditorGUI.BeginProperty(position, label, property);

//            EditorGUI.ObjectField(position, label, fieldObject.,  fieldObject.GetSkinnedType(),
//                fieldObject.AllowSceneObjects);

//            EditorGUI.EndProperty();
//        }
//    }
//}
