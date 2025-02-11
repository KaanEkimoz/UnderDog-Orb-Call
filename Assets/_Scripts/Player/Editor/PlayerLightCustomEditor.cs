using UnityEditor;
using UnityEngine;

namespace com.game.player.editor
{
    [CustomEditor(typeof(PlayerLight))]
    public class PlayerLightCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical("box");



            EditorGUILayout.EndVertical();
        }
    }
}
