// Decompiled with JetBrains decompiler
// Type: dfRadarMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
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

}
