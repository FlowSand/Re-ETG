// Decompiled with JetBrains decompiler
// Type: DemoMoveControls
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Add-Remove Controls/Move Child Control")]
public class DemoMoveControls : MonoBehaviour
  {
    public dfScrollPanel from;
    public dfScrollPanel to;

    public void OnClick()
    {
      this.from.SuspendLayout();
      this.to.SuspendLayout();
      while (this.from.Controls.Count > 0)
      {
        dfControl control = this.from.Controls[0];
        this.from.RemoveControl(control);
        control.ZOrder = -1;
        this.to.AddControl(control);
      }
      this.from.ResumeLayout();
      this.to.ResumeLayout();
      this.from.ScrollPosition = Vector2.zero;
    }
  }

