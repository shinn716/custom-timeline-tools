using RenderHeads.Media.AVProVideo;
using Shinn.Timeline;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

[CustomEditor(typeof(CustomAvproMediaPlayable))]
public class CustomAvproMediaPlayableEditor : Editor
{
    private SerializedProperty mediaPlayerProp;
    private SerializedProperty pathTypeProp;
    private SerializedProperty pathProp;
    private SerializedProperty loopProp;
    private SerializedProperty playbackRateProp;
    private SerializedProperty preloadProp;

    private void OnEnable()
    {
        mediaPlayerProp = serializedObject.FindProperty("mediaPlayer");
        pathTypeProp = serializedObject.FindProperty("pathType");
        pathProp = serializedObject.FindProperty("path");
        loopProp = serializedObject.FindProperty("loop");
        playbackRateProp = serializedObject.FindProperty("playbackRate");
        preloadProp = serializedObject.FindProperty("preload");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(mediaPlayerProp);

        var playable = (CustomAvproMediaPlayable)target;
        var director = TimelineEditor.inspectedDirector;
        var resolved = director != null ? playable.mediaPlayer.Resolve(director) : null;
        if (resolved == null)
            EditorGUILayout.HelpBox("Assign a MediaPlayer — nothing will play at runtime without one.", MessageType.Warning);

        EditorGUILayout.PropertyField(pathTypeProp);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(pathProp);
        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            var picked = EditorUtility.OpenFilePanel("Select media file",
                GetBaseFolder((MediaPathType)pathTypeProp.enumValueIndex), "");
            if (!string.IsNullOrEmpty(picked))
                ApplyPickedPath(picked);
        }
        EditorGUILayout.EndHorizontal();

        if (string.IsNullOrEmpty(pathProp.stringValue))
            EditorGUILayout.HelpBox("Set the media file path or URL.", MessageType.Info);

        EditorGUILayout.PropertyField(loopProp);
        EditorGUILayout.PropertyField(playbackRateProp);
        EditorGUILayout.PropertyField(preloadProp);

        serializedObject.ApplyModifiedProperties();
    }

    private static string GetBaseFolder(MediaPathType pathType)
    {
        switch (pathType)
        {
            case MediaPathType.RelativeToStreamingAssetsFolder:
                return Application.streamingAssetsPath;
            case MediaPathType.RelativeToProjectFolder:
                return System.IO.Path.GetDirectoryName(Application.dataPath);
            case MediaPathType.RelativeToDataFolder:
                return Application.dataPath;
            case MediaPathType.RelativeToPersistentDataFolder:
                return Application.persistentDataPath;
            default:
                return string.Empty;
        }
    }

    private void ApplyPickedPath(string absolutePath)
    {
        var pathType = (MediaPathType)pathTypeProp.enumValueIndex;
        var baseFolder = GetBaseFolder(pathType).Replace('\\', '/');
        absolutePath = absolutePath.Replace('\\', '/');

        if (pathType != MediaPathType.AbsolutePathOrURL &&
            !string.IsNullOrEmpty(baseFolder) &&
            absolutePath.StartsWith(baseFolder + "/", System.StringComparison.OrdinalIgnoreCase))
        {
            pathProp.stringValue = absolutePath.Substring(baseFolder.Length + 1);
        }
        else
        {
            // Outside the base folder for the current Path Type: fall back
            // to an absolute path.
            pathProp.stringValue = absolutePath;
            pathTypeProp.enumValueIndex = (int)MediaPathType.AbsolutePathOrURL;
        }
    }
}
