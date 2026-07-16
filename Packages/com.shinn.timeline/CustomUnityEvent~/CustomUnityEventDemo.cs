using UnityEngine;

namespace Shinn.Timeline
{
    /// <summary>Listener targets for the CustomUnityEvent sample scene.</summary>
    public class CustomUnityEventDemo : MonoBehaviour
    {
        public void LogMessage(string message)
        {
            Debug.Log("[CustomUnityEventDemo] " + message);
        }

        public void SetCount(int count)
        {
            Debug.Log("[CustomUnityEventDemo] Count = " + count);
        }
    }
}
