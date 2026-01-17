// Decompiled with JetBrains decompiler
// Type: DemoRemoveAllControls
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Add-Remove Controls/Remove Child Controls")]
public class DemoRemoveAllControls : MonoBehaviour
{
  public dfControl target;

  public void Start()
  {
    if (!((Object) this.target == (Object) null))
      return;
    this.target = this.GetComponent<dfControl>();
  }

  public void OnClick()
  {
    while (this.target.Controls.Count > 0)
    {
      dfControl control = this.target.Controls[0];
      this.target.RemoveControl(control);
      Object.DestroyImmediate((Object) control.gameObject);
    }
  }
}
