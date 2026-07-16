using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

namespace Shinn.Timeline
{
    public class CustomTextClip : PlayableBehaviour
    {
        public Text targetTxt { get; set; }
        public Color txtColor { get; set; }
        public string txtContent { get; set; }
        public bool useOriginConfig { get; set; }
        public int fontSize { get; set; }

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
    }
}
