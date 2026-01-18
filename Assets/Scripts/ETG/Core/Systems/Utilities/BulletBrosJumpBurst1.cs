// Decompiled with JetBrains decompiler
// Type: BulletBrosJumpBurst1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Bosses/BulletBros/JumpBurst1")]
public class BulletBrosJumpBurst1 : Script
  {
    private const int NumBullets = 12;

    protected override IEnumerator Top()
    {
      float num1 = this.RandomAngle();
      float num2 = 30f;
      for (int index = 0; index < 12; ++index)
        this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f), new Bullet("jump", true));
      return (IEnumerator) null;
    }
  }

