// Decompiled with JetBrains decompiler
// Type: DraGunBigNoseShot2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/BigNoseShot2")]
public class DraGunBigNoseShot2 : Script
  {
    public int NumTraps = 5;
    private static int[] s_xValues;
    private static int[] s_yValues;

    protected override IEnumerator Top()
    {
      if (DraGunBigNoseShot2.s_xValues == null || DraGunBigNoseShot2.s_yValues == null)
      {
        DraGunBigNoseShot2.s_xValues = new int[this.NumTraps];
        DraGunBigNoseShot2.s_yValues = new int[this.NumTraps];
        for (int index = 0; index < this.NumTraps; ++index)
        {
          DraGunBigNoseShot2.s_xValues[index] = index;
          DraGunBigNoseShot2.s_yValues[index] = index;
        }
      }
      Vector2 vector2_1 = this.BulletBank.aiActor.ParentRoom.area.UnitBottomLeft + new Vector2(1f, 20f);
      Vector2 vector2_2 = new Vector2(34f, 11f);
      Vector2 vector2_3 = new Vector2(vector2_2.x / (float) this.NumTraps, vector2_2.y / (float) this.NumTraps);
      BraveUtility.RandomizeArray<int>(DraGunBigNoseShot2.s_xValues);
      BraveUtility.RandomizeArray<int>(DraGunBigNoseShot2.s_yValues);
      for (int index = 0; index < this.NumTraps; ++index)
      {
        int xValue = DraGunBigNoseShot2.s_xValues[index];
        int yValue = DraGunBigNoseShot2.s_yValues[index];
        this.Fire(new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(8f), (Bullet) new DraGunBigNoseShot2.EnemyBullet(vector2_1 + new Vector2(((float) xValue + Random.value) * vector2_3.x, ((float) yValue + Random.value) * vector2_3.y)));
      }
      return (IEnumerator) null;
    }

    public class EnemyBullet : Bullet
    {
      public const int StartShootDelay = 60;
      public const int MinShootTime = 45;
      public const int MaxShootTime = 90;
      public const int LifeTimeMin = 480;
      public const int LifeTimeMax = 600;
      private Vector2 m_goalPos;

      public EnemyBullet(Vector2 goalPos)
        : base("homing")
      {
        this.m_goalPos = goalPos;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DraGunBigNoseShot2.EnemyBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

