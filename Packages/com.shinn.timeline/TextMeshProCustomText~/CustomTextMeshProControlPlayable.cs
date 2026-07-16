using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

namespace Shinn.Timeline
{
    public class CustomTextMeshProControlPlayable : PlayableAsset
    {
        public ExposedReference<TextMeshProUGUI> targetTxt;
         
        [TextArea]
        public string txtContent;

        [Space]
        public bool useComponentParameter = true;
        public Color txtColor = Color.black;
        [UnityEngine.Serialization.FormerlySerializedAs("txtSzie")]
        public int fontSize = 14;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CustomTextMeshProClip>.Create(graph);
            var selectClip = playable.GetBehaviour();

            selectClip.targetTxt = targetTxt.Resolve(graph.GetResolver());
            selectClip.txtContent = txtContent;
            selectClip.useOriginConfig = useComponentParameter;
            selectClip.txtColor = txtColor;
            selectClip.fontSize = fontSize;

            return playable;
        }
    }
}