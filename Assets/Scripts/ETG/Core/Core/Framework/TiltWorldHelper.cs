// Decompiled with JetBrains decompiler
// Type: TiltWorldHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class TiltWorldHelper : BraveBehaviour
    {
      public float HeightOffGround;
      public bool DoForceLayer;
      public string ForceLayer = "Unoccluded";

      private void Update()
      {
        this.transform.position = this.transform.position.WithZ(this.transform.position.y - this.HeightOffGround);
        this.transform.rotation = Quaternion.identity;
        if (!this.DoForceLayer)
          return;
        this.gameObject.layer = LayerMask.NameToLayer(this.ForceLayer);
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
