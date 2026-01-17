// Decompiled with JetBrains decompiler
// Type: BulletShotgunManSawedOff1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using UnityEngine;

#nullable disable
[InspectorDropdownName("BulletShotgunMan/SawedOff1")]
public class BulletShotgunManSawedOff1 : Script
{
  protected override IEnumerator Top()
  {
    float aimDirection = this.GetAimDirection(1f, 9f);
    for (int index = -2; index <= 2; ++index)
      this.Fire(new Brave.BulletScript.Direction(aimDirection + (float) (index * 6)), new Brave.BulletScript.Speed((float) (10.0 - (double) Mathf.Abs(index) * 0.5)), (Bullet) null);
    return (IEnumerator) null;
  }
}
