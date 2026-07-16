using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Shinn.Timeline
{
    [TrackColor(227 / 255f, 59 / 255f, 111 / 255f)]
    [TrackClipType(typeof(CustomAvproMediaPlayable))]
    public class CustomAvproTracker : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<CustomAvproMediaMixer>.Create(graph, inputCount);
        }
    }
}
