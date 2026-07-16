using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Shinn.Timeline
{
    public class CustomTextPlayable : PlayableAsset
    {
        public ExposedReference<Text> targetTxt;

        [TextArea]
        public string txtContent;

        [Space]
        [Tooltip("Keep the Text component's own color and font size.")]
        public bool useComponentParameter = true;
        public Color txtColor = Color.black;
        [FormerlySerializedAs("txtSzie")]
        public int fontSize = 14;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CustomTextClip>.Create(graph);
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
