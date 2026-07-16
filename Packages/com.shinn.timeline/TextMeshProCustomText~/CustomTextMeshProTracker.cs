using Shinn.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(85 / 255f, 177 / 255f, 85 / 255f)]
[TrackClipType(typeof(CustomTextMeshProControlPlayable))]
public class CustomTextMeshProTracker : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CustomTextMeshProClip>.Create(graph, inputCount);
    }
}