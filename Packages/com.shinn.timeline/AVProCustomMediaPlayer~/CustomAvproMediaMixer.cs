using UnityEngine;
using UnityEngine.Playables;

namespace Shinn.Timeline
{
    /// <summary>
    /// Track mixer that stops each clip's video when the clip is no longer
    /// active. Clip-side OnBehaviourPause is unreliable for end detection
    /// (it never fires when the director holds at the timeline end), so the
    /// mixer polls input weights every frame instead.
    ///
    /// Enhanced stop detection:
    /// - Tracks last active state per clip to catch transitions
    /// - Uses >= duration instead of &lt; to handle floating-point edge cases
    /// </summary>
    public class CustomAvproMediaMixer : PlayableBehaviour
    {
        private System.Collections.Generic.Dictionary<int, bool> _clipWasActive = new System.Collections.Generic.Dictionary<int, bool>();

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
                return;

            int count = playable.GetInputCount();
            for (int i = 0; i < count; i++)
            {
                var input = (ScriptPlayable<CustomAvproMediaClip>)playable.GetInput(i);
                var clip = input.GetBehaviour();
                if (clip == null || !clip.IsStarted)
                    continue;

                int clipId = input.GetHashCode();
                float clipTime = (float)input.GetTime();
                float clipDuration = (float)input.GetDuration();
                float inputWeight = playable.GetInputWeight(i);

                // Primary check: weight > 0 AND time within clip bounds
                bool isActive = inputWeight > 0f && clipTime < clipDuration;
                bool shouldBeActive = isActive;

                // Track state transitions to avoid repeated StopPlayback calls
                bool wasActive = _clipWasActive.TryGetValue(clipId, out var val) ? val : true;

                // Stop if: was active but now inactive, OR time has reached/exceeded duration
                bool shouldStop = (!shouldBeActive && wasActive) || clipTime >= clipDuration;

                if (shouldStop)
                {
                    clip.StopPlayback();
                    _clipWasActive[clipId] = false;
                }
                else if (shouldBeActive)
                {
                    _clipWasActive[clipId] = true;
                }

                // Cleanup stale entries (optional, prevents dictionary growth)
                if (!shouldBeActive && !clip.IsStarted)
                {
                    _clipWasActive.Remove(clipId);
                }
            }
        }
    }
}
