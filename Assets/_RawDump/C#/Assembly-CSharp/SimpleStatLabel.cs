// Decompiled with JetBrains decompiler
// Type: SimpleStatLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SimpleStatLabel : MonoBehaviour
{
  public TrackedStats stat;
  protected dfLabel m_label;

  private void Start() => this.m_label = this.GetComponent<dfLabel>();

  private void Update()
  {
    if (!(bool) (Object) this.m_label || !this.m_label.IsVisible)
      return;
    this.m_label.Text = IntToStringSansGarbage.GetStringForInt(Mathf.FloorToInt(GameStatsManager.Instance.GetPlayerStatValue(this.stat)));
  }
}
