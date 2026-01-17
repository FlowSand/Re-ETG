// Decompiled with JetBrains decompiler
// Type: WindowTilt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Examples/General/Window Tilt")]
    public class WindowTilt : MonoBehaviour
    {
      private dfControl control;

      private void Start()
      {
        this.control = this.GetComponent<dfControl>();
        if (!((Object) this.control == (Object) null))
          return;
        this.enabled = false;
      }

      private void Update()
      {
        this.control.transform.localRotation = Quaternion.Euler(0.0f, (float) (((double) this.control.GetCamera().WorldToViewportPoint(this.control.GetCenter()).x * 2.0 - 1.0) * 20.0), 0.0f);
      }
    }

}
