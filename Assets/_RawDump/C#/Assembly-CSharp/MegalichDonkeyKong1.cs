// Decompiled with JetBrains decompiler
// Type: MegalichDonkeyKong1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/Megalich/DonkeyKong1")]
public class MegalichDonkeyKong1 : Script
{
  private const int NumWaves = 3;
  private const int NumLargeWaves = 5;
  private const int NumLargeBulletsPerWave = 5;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new MegalichDonkeyKong1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void ShootSmallBullets(float dir, bool isOffset)
  {
    for (int index1 = 0; index1 < 5; ++index1)
    {
      int num1 = 5;
      float num2 = 0.0f;
      if (isOffset)
      {
        --num1;
        num2 += 0.5f;
      }
      for (int index2 = 0; index2 < num1; ++index2)
        this.Fire(new Offset(dir * -19.5f, Mathf.Lerp(0.0f, -10f, (float) (((double) index2 + (double) num2) / 4.0)) - 0.25f, transform: string.Empty), new Brave.BulletScript.Direction((double) dir <= 0.0 ? 180f : 0.0f), new Brave.BulletScript.Speed(14f), (Bullet) new DelayedBullet("frogger", 7 * index1));
    }
  }
}
