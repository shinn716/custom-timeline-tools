using Shinn.Timeline;
using UnityEditor;

[CustomEditor(typeof(CustomTextPlayable)), CanEditMultipleObjects]
public class CustomTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTxt"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("txtContent"));

        var useComponentParameter = serializedObject.FindProperty("useComponentParameter");
        EditorGUILayout.PropertyField(useComponentParameter);

        // Color and size only apply when not using the component's own settings.
        if (!useComponentParameter.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("txtColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fontSize"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
