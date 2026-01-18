// Decompiled with JetBrains decompiler
// Type: KaliberThreeHeads1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Kaliber/ThreeHeads1")]
public class KaliberThreeHeads1 : Script
  {
    private const int NumBullets = 28;

    protected override IEnumerator Top()
    {
      float num1 = this.RandomAngle();
      float num2 = 12.8571424f;
      for (int index = 0; index < 28; ++index)
        this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f), new Bullet("burst"));
      return (IEnumerator) null;
    }
  }

