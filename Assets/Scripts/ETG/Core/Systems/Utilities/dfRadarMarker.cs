using System;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Radar/Radar Marker")]
public class dfRadarMarker : MonoBehaviour
    {
        public dfRadarMain radar;
        public string markerType;
        public string outOfRangeType;
        [NonSerialized]
        internal dfControl marker;
        [NonSerialized]
        internal dfControl outOfRangeMarker;

        public void OnEnable()
        {
            if (string.IsNullOrEmpty(this.markerType))
                return;
            if ((UnityEngine.Object) this.radar == (UnityEngine.Object) null)
            {
                this.radar = UnityEngine.Object.FindObjectOfType(typeof (dfRadarMain)) as dfRadarMain;
                if ((UnityEngine.Object) this.radar == (UnityEngine.Object) null)
                {
                    Debug.LogWarning((object) "No radar found");
                    return;
                }
            }
            this.radar.AddMarker(this);
        }

        public void OnDisable()
        {
            if (!((UnityEngine.Object) this.radar != (UnityEngine.Object) null))
                return;
            this.radar.RemoveMarker(this);
        }
    }

