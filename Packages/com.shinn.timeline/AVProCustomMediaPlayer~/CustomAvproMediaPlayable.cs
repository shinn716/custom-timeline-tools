using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using RenderHeads.Media.AVProVideo;

namespace Shinn.Timeline
{
    public class CustomAvproMediaPlayable : PlayableAsset
    {
        public ExposedReference<MediaPlayer> mediaPlayer;

        [SerializeField, FormerlySerializedAs("fileLocation")]
        [Tooltip("How Path is interpreted (streaming assets, absolute path/URL, ...).")]
        MediaPathType pathType = MediaPathType.RelativeToStreamingAssetsFolder;

        [SerializeField]
        [Tooltip("Media file path or URL, interpreted per Path Type.")]
        string path;

        [SerializeField]
        [Tooltip("Loop the video while the clip is active. Playback still stops when the clip ends.")]
        bool loop = false;

        [SerializeField, Range(0.25f, 4f)]
        [Tooltip("Playback speed multiplier.")]
        float playbackRate = 1;

        [SerializeField]
        [Tooltip("Open the media (paused) when the timeline starts so playback begins instantly at the clip. Only enable when this MediaPlayer is used by a single clip, otherwise clips will overwrite each other's media.")]
        bool preload = false;

        public string Path => path;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CustomAvproMediaClip>.Create(graph);
            var selectClip = playable.GetBehaviour();

            selectClip.mediaPlayer = mediaPlayer.Resolve(graph.GetResolver());
            selectClip.pathType = pathType;
            selectClip.playbackRate = playbackRate;
            selectClip.path = path;
            selectClip.loop = loop;
            selectClip.preload = preload;
            return playable;
        }
    }
}
