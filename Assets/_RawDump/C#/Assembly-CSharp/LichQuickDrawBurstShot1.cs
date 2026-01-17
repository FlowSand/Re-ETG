// Decompiled with JetBrains decompiler
// Type: LichQuickDrawBurstShot1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/Lich/QuickDrawBurstShot1")]
public class LichQuickDrawBurstShot1 : Script
{
  protected override IEnumerator Top()
  {
    float aimDirection = this.GetAimDirection((float) Random.Range(0, 3), 12f);
    for (int index = -2; index <= 2; ++index)
      this.Fire(new Brave.BulletScript.Direction(aimDirection + (float) (index * 10)), new Brave.BulletScript.Speed(12f), new Bullet("quickHoming"));
    return (IEnumerator) null;
  }
}
