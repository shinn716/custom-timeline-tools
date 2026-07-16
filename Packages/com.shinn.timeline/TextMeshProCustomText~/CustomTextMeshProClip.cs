using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

namespace Shinn.Timeline
{
    public class CustomTextMeshProClip : PlayableBehaviour
    {
        public TextMeshProUGUI targetTxt { get; set; }
        public Color txtColor { get; set; }
        public string txtContent { get; set; }
        public bool useOriginConfig { get; set; }
        public int fontSize { get; set; }
        private bool fadeinFlag = false;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (targetTxt != null)
            {
                targetTxt.text = txtContent;

                if (useOriginConfig)
                    return;

                targetTxt.color = Color.Lerp(targetTxt.color, txtColor, info.effectiveWeight);
                targetTxt.fontSize = (int)Mathf.Lerp(targetTxt.fontSize, fontSize, info.effectiveWeight);
            }
        }

        // On clip end
        //public override void OnBehaviourPause(Playable playable, FrameData info)
        //{
        //    var duration = playable.GetDuration();
        //    var count = playable.GetTime() + info.deltaTime;

        //    if ((info.effectivePlayState == PlayState.Paused && count > duration))       // timeline done: playable.GetGraph().GetRootPlayable(0).IsDone()
        //    {
        //        Debug.Log("Clip done! " + this.txtContent);
        //    }
        //}
    }
}