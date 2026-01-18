using UnityEngine;

#nullable disable

    public abstract class dfTouchInputSourceComponent : MonoBehaviour
    {
        public int Priority;

        public abstract IDFTouchInputSource Source { get; }
    }

