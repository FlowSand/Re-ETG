// Decompiled with JetBrains decompiler
// Type: BossFinalRobotTerminatorShotgun1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalRobot/TerminatorShotgun1")]
public class BossFinalRobotTerminatorShotgun1 : Script
  {
    protected override IEnumerator Top()
    {
      switch (Random.Range(0, 4))
      {
        case 0:
          for (int index = -2; index <= 2; ++index)
            this.Fire(new Brave.BulletScript.Direction((float) (index * 6), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(5f), (Bullet) null);
          break;
        case 1:
          for (int index = -2; index <= 2; ++index)
            this.Fire(new Brave.BulletScript.Direction((float) (index * 6), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) null);
          break;
        case 2:
          float aimDirection = this.GetAimDirection(1f, 9f);
          for (int index = -2; index <= 2; ++index)
            this.Fire(new Brave.BulletScript.Direction(aimDirection + (float) (index * 6)), new Brave.BulletScript.Speed((float) (10.0 - (double) Mathf.Abs(index) * 0.5)), (Bullet) null);
          break;
        case 3:
          for (int index = -2; index <= 2; ++index)
            this.Fire(new Brave.BulletScript.Direction((float) (index * 6), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) null);
          break;
      }
      return (IEnumerator) null;
    }
  }

