// Decompiled with JetBrains decompiler
// Type: AutoArrangeDemoItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Containers/Auto-Arrange Item")]
public class AutoArrangeDemoItem : MonoBehaviour
{
  private dfButton control;
  private dfAnimatedVector2 size;
  private bool isExpanded;

  private void Start()
  {
    this.control = this.GetComponent<dfButton>();
    this.size = new dfAnimatedVector2(this.control.Size, this.control.Size, 0.33f);
    this.control.Text = "#" + (object) (this.control.ZOrder + 1);
  }

  private void Update() => this.control.Size = this.size.Value.RoundToInt();

  private void OnClick() => this.Toggle();

  public void Expand()
  {
    this.size.StartValue = this.size.EndValue;
    this.size.EndValue = new Vector2(128f, 96f);
    this.isExpanded = true;
  }

  public void Collapse()
  {
    this.size.StartValue = this.size.EndValue;
    this.size.EndValue = new Vector2(48f, 48f);
    this.isExpanded = false;
  }

  public void Toggle()
  {
    if (this.isExpanded)
      this.Collapse();
    else
      this.Expand();
  }
}
