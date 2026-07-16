using UnityEngine;
using UnityEngine.Playables;
using RenderHeads.Media.AVProVideo;

namespace Shinn.Timeline
{
    public class CustomAvproMediaClip : PlayableBehaviour
    {
        public MediaPlayer mediaPlayer { get; set; }
        public MediaPathType pathType { get; set; }
        public string path { get; set; }
        public bool loop { get; set; } = false;
        public float playbackRate { get; set; } = 1f;
        public bool preload { get; set; } = false;

        public bool IsStarted => started;

        private bool started = false;
        private bool opened = false;
        private bool pausedByTimeline = false;

        public override void OnGraphStart(Playable playable)
        {
            if (!Application.isPlaying || !preload || opened)
                return;

            if (mediaPlayer == null || string.IsNullOrEmpty(path))
                return;

            // Open paused so playback can start instantly at the clip.
            mediaPlayer.OpenMedia(pathType, path, autoPlay: false);
            opened = true;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
                return;

            if (mediaPlayer == null || started)
                return;

            started = true;
            pausedByTimeline = false;

            if (!opened)
            {
                mediaPlayer.OpenMedia(pathType, path, autoPlay: false);
                opened = true;
            }
            else
            {
                // Preloaded, or replaying after a timeline loop.
                mediaPlayer.Control.Seek(0.0);
            }

            mediaPlayer.Control.SetLooping(loop);
            mediaPlayer.Control.SetPlaybackRate(playbackRate);
            mediaPlayer.Control.Play();
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying || mediaPlayer == null)
                return;

            // Resume after the timeline was paused mid-clip.
            if (pausedByTimeline)
            {
                pausedByTimeline = false;
                mediaPlayer.Control.Play();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
                return;

            if (mediaPlayer == null || !started)
                return;

            // Clip end is detected by CustomAvproMediaMixer (this callback
            // does not fire when the director holds at the timeline end).
            // While the graph is playing, a pause here means the clip ended.
            if (playable.GetGraph().IsPlaying())
            {
                StopPlayback();
            }
            else
            {
                // The director was paused mid-clip: pause the video with it.
                mediaPlayer.Control.Pause();
                pausedByTimeline = true;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            // Director stopped or the graph was rebuilt: release the video.
            if (!Application.isPlaying || mediaPlayer == null)
                return;

            if (started || pausedByTimeline)
                StopPlayback();
        }

        /// <summary>Pause the video at current position and allows replay (timeline loop).</summary>
        public void StopPlayback()
        {
            if (mediaPlayer != null && mediaPlayer.Control != null)
            {
                // Pause keeps the last frame displayed, unlike Stop() which clears it
                mediaPlayer.Control.Pause();
            }
            
            started = false;
            pausedByTimeline = false;
        }
    }
}
