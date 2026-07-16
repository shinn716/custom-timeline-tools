using Shinn.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

// Shows the media file name directly on the Timeline clip.
[CustomTimelineEditor(typeof(CustomAvproMediaPlayable))]
public class CustomAvproClipEditor : ClipEditor
{
    public override void OnClipChanged(TimelineClip clip)
    {
        var asset = clip.asset as CustomAvproMediaPlayable;
        if (asset == null || string.IsNullOrEmpty(asset.Path))
            return;

        clip.displayName = System.IO.Path.GetFileName(asset.Path);
    }
}
