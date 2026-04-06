using UnityEditor.UI;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MatchmakingPlayerButton))]
[CanEditMultipleObjects]
public class MatchmakingPlayerButtonEditor : ButtonEditor
{
    SerializedProperty _textProperty;



    protected override void OnEnable()
    {
        base.OnEnable();

        _textProperty = serializedObject.FindProperty("_text");
    }



    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Matchmaking Player", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_textProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
