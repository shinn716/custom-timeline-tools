using Shinn.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

// Shows what each clip does directly on the Timeline so users don't have to
// open the inspector clip by clip.

[CustomTimelineEditor(typeof(CustomTextPlayable))]
public class CustomTextClipEditor : ClipEditor
{
    public override void OnClipChanged(TimelineClip clip)
    {
        var asset = clip.asset as CustomTextPlayable;
        if (asset == null || string.IsNullOrEmpty(asset.txtContent))
            return;

        var line = asset.txtContent.Split('\n')[0].TrimEnd('\r');
        clip.displayName = line.Length > 24 ? line.Substring(0, 24) + "…" : line;
    }
}
