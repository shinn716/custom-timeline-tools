using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Shinn.Timeline
{
    [TrackColor(170 / 255f, 85 / 255f, 255 / 255f)]
    [TrackClipType(typeof(CustomUnityEventPlayable))]
    public class CustomUnityEventTracker : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<CustomUnityEventClip>.Create(graph, inputCount);
        }
    }
}
