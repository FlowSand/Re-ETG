// Decompiled with JetBrains decompiler
// Type: FleshCubeImpact1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System;
using System.Collections;
using UnityEngine;

#nullable disable
public class FleshCubeImpact1 : Script
{
  private const int NumBullets = 11;

  protected override IEnumerator Top()
  {
    this.FireLine(0.0f);
    this.FireLine(90f);
    this.FireLine(180f);
    this.FireLine(270f);
    return (IEnumerator) null;
  }

  private void FireLine(float startingAngle)
  {
    float num1 = 9f;
    for (int index = 0; index < 11; ++index)
    {
      float num2 = Mathf.Atan((float) (((double) index * (double) num1 - 45.0) / 45.0)) * 57.29578f;
      float f = Mathf.Cos(num2 * ((float) Math.PI / 180f));
      float num3 = (double) Mathf.Abs(f) >= 0.0001 ? 1f / f : 1f;
      this.Fire(new Brave.BulletScript.Direction(num2 + startingAngle), new Brave.BulletScript.Speed(num3 * 9f), (Bullet) null);
    }
  }
}
