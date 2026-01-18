// Decompiled with JetBrains decompiler
// Type: DEMO_CoverflowItemInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Coverflow/Item Info")]
public class DEMO_CoverflowItemInfo : MonoBehaviour
  {
    public dfCoverflow scroller;
    public string[] descriptions;
    private dfLabel label;

    public void Start() => this.label = this.GetComponent<dfLabel>();

    private void Update()
    {
      if ((Object) this.scroller == (Object) null || this.descriptions == null || this.descriptions.Length == 0)
        return;
      this.label.Text = this.descriptions[Mathf.Max(0, Mathf.Min(this.descriptions.Length - 1, this.scroller.selectedIndex))];
    }
  }

