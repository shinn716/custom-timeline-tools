using System;
using System.Linq;
using Shinn.Timeline;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(CustomUnityEventPlayable))]
public class CustomUnityEventPlayableEditor : Editor
{
    private SerializedProperty targetProp;
    private SerializedProperty declaringTypeProp;
    private SerializedProperty methodNameProp;
    private SerializedProperty parameterTypeProp;
    private SerializedProperty parameterTypeNameProp;
    private SerializedProperty intProp;
    private SerializedProperty floatProp;
    private SerializedProperty stringProp;
    private SerializedProperty boolProp;
    private SerializedProperty vector3Prop;
    private SerializedProperty vector2Prop;
    private SerializedProperty vector4Prop;
    private SerializedProperty colorProp;
    private SerializedProperty quaternionProp;
    private SerializedProperty objectProp;
    private SerializedProperty enumProp;

    private void OnEnable()
    {
        targetProp = serializedObject.FindProperty("target");
        declaringTypeProp = serializedObject.FindProperty("declaringTypeName");
        methodNameProp = serializedObject.FindProperty("methodName");
        parameterTypeProp = serializedObject.FindProperty("parameterType");
        parameterTypeNameProp = serializedObject.FindProperty("parameterTypeName");
        intProp = serializedObject.FindProperty("intInput");
        floatProp = serializedObject.FindProperty("floatInput");
        stringProp = serializedObject.FindProperty("stringInput");
        boolProp = serializedObject.FindProperty("boolInput");
        vector3Prop = serializedObject.FindProperty("vector3Input");
        vector2Prop = serializedObject.FindProperty("vector2Input");
        vector4Prop = serializedObject.FindProperty("vector4Input");
        colorProp = serializedObject.FindProperty("colorInput");
        quaternionProp = serializedObject.FindProperty("quaternionInput");
        objectProp = serializedObject.FindProperty("objectInput");
        enumProp = serializedObject.FindProperty("enumInput");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(targetProp);

        var playable = (CustomUnityEventPlayable)target;
        var resolved = ResolveTarget(playable);

        if (resolved == null)
        {
            EditorGUILayout.HelpBox("Drag a target object (GameObject, Light, custom MonoBehaviour, ...) to list its methods.", MessageType.Warning);
        }
        else
        {
            DrawMethodPopup(resolved);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawMethodPopup(Object resolved)
    {
        var candidates = CustomUnityEventPlayable.GetMethodCandidates(resolved);
        if (candidates.Count == 0)
        {
            EditorGUILayout.HelpBox("No invocable public methods found on '" + resolved.name + "'.", MessageType.Warning);
            return;
        }

        string[] options = candidates.Select(c => c.displayPath).ToArray();

        int index = candidates.FindIndex(c =>
            c.declaringTypeName == declaringTypeProp.stringValue &&
            c.methodName == methodNameProp.stringValue &&
            (int)c.parameterType == parameterTypeProp.enumValueIndex &&
            c.parameterTypeName == parameterTypeNameProp.stringValue);

        if (index < 0 && !string.IsNullOrEmpty(methodNameProp.stringValue))
            EditorGUILayout.HelpBox("Saved method '" + methodNameProp.stringValue + "' no longer exists on the target.", MessageType.Warning);

        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUILayout.Popup("Method", Mathf.Max(index, 0), options);
        if (EditorGUI.EndChangeCheck() || string.IsNullOrEmpty(methodNameProp.stringValue))
        {
            var picked = candidates[newIndex];
            declaringTypeProp.stringValue = picked.declaringTypeName;
            methodNameProp.stringValue = picked.methodName;
            parameterTypeProp.enumValueIndex = (int)picked.parameterType;
            parameterTypeNameProp.stringValue = picked.parameterTypeName;
        }

        DrawArgumentField();
    }

    private void DrawArgumentField()
    {
        switch ((CustomUnityEventPlayable.ParameterType)parameterTypeProp.enumValueIndex)
        {
            case CustomUnityEventPlayable.ParameterType.Int:
                EditorGUILayout.PropertyField(intProp, new GUIContent("Int Value"));
                break;
            case CustomUnityEventPlayable.ParameterType.Float:
                EditorGUILayout.PropertyField(floatProp, new GUIContent("Float Value"));
                break;
            case CustomUnityEventPlayable.ParameterType.String:
                EditorGUILayout.PropertyField(stringProp, new GUIContent("String Value"));
                break;
            case CustomUnityEventPlayable.ParameterType.Bool:
                EditorGUILayout.PropertyField(boolProp, new GUIContent("Bool Value"));
                break;
            case CustomUnityEventPlayable.ParameterType.Vector3:
                EditorGUILayout.PropertyField(vector3Prop, new GUIContent("Vector3 Value"));
                break;
            case CustomUnityEventPlayable.ParameterType.Vector2:
                EditorGUILayout.PropertyField(vector2Prop, new GUIContent("Vector2 Value"));
                break;
            case CustomUnityEventPlayable.ParameterType.Vector4:
                EditorGUILayout.PropertyField(vector4Prop, new GUIContent("Vector4 Value"));
                break;
            case CustomUnityEventPlayable.ParameterType.Color:
                EditorGUILayout.PropertyField(colorProp, new GUIContent("Color Value"));
                break;
            case CustomUnityEventPlayable.ParameterType.Quaternion:
            {
                EditorGUI.BeginChangeCheck();
                var euler = EditorGUILayout.Vector3Field("Rotation (Euler)", quaternionProp.quaternionValue.eulerAngles);
                if (EditorGUI.EndChangeCheck())
                    quaternionProp.quaternionValue = Quaternion.Euler(euler);
                break;
            }
            case CustomUnityEventPlayable.ParameterType.ObjectReference:
            {
                var objType = Type.GetType(parameterTypeNameProp.stringValue);
                if (objType == null || !typeof(Object).IsAssignableFrom(objType))
                    objType = typeof(Object);

                EditorGUI.BeginChangeCheck();
                // Scene objects are not allowed: the argument is serialized on
                // the clip asset, which can only reference project assets.
                var picked = EditorGUILayout.ObjectField(new GUIContent("Object Value (asset)"), objectProp.objectReferenceValue, objType, false);
                if (EditorGUI.EndChangeCheck())
                    objectProp.objectReferenceValue = picked;
                break;
            }
            case CustomUnityEventPlayable.ParameterType.Enum:
            {
                var enumType = Type.GetType(parameterTypeNameProp.stringValue);
                if (enumType != null && enumType.IsEnum)
                {
                    var current = (Enum)Enum.ToObject(enumType, enumProp.intValue);
                    EditorGUI.BeginChangeCheck();
                    var picked = EditorGUILayout.EnumPopup("Enum Value", current);
                    if (EditorGUI.EndChangeCheck())
                        enumProp.intValue = Convert.ToInt32(picked);
                }
                else
                {
                    EditorGUILayout.PropertyField(enumProp, new GUIContent("Enum Value (int)"));
                }
                break;
            }
        }
    }

    private static Object ResolveTarget(CustomUnityEventPlayable playable)
    {
        var director = TimelineEditor.inspectedDirector;
        if (director == null)
            return null;

        return playable.target.Resolve(director);
    }
}
