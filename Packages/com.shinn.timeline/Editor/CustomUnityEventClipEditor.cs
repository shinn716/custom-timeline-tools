using Shinn.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

// Shows the call directly on the Timeline clip, e.g. "Light.set_intensity".
[CustomTimelineEditor(typeof(CustomUnityEventPlayable))]
public class CustomUnityEventClipEditor : ClipEditor
{
    public override void OnClipChanged(TimelineClip clip)
    {
        var asset = clip.asset as CustomUnityEventPlayable;
        if (asset == null || string.IsNullOrEmpty(asset.MethodName))
            return;

        var typeName = asset.DeclaringTypeName;
        int dot = string.IsNullOrEmpty(typeName) ? -1 : typeName.LastIndexOf('.');
        var shortType = dot >= 0 ? typeName.Substring(dot + 1) : typeName;

        clip.displayName = string.IsNullOrEmpty(shortType)
            ? asset.MethodName
            : shortType + "." + asset.MethodName;
    }
}
