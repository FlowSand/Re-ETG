// Decompiled with JetBrains decompiler
// Type: StatHolder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class StatHolder : MonoBehaviour
{
  public bool RequiresPlayerItemActive;
  public StatModifier[] modifiers;

  private void Start()
  {
    if (!this.RequiresPlayerItemActive)
      return;
    PlayerItem component = this.GetComponent<PlayerItem>();
    if (!(bool) (UnityEngine.Object) component)
      return;
    component.OnActivationStatusChanged += (Action<PlayerItem>) (a =>
    {
      if (!(bool) (UnityEngine.Object) a.LastOwner)
        return;
      a.LastOwner.stats.RecalculateStats(a.LastOwner);
    });
  }
}
