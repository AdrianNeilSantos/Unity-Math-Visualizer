using UnityEngine;
using UnityEditor;


public class CommonEditor : EditorWindow
{

    public virtual void DrawBlockGUI(string label, SerializedProperty property){
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(label, GUILayout.Width(50));
        EditorGUILayout.PropertyField(property, GUIContent.none);
        EditorGUILayout.EndHorizontal();
    }


}