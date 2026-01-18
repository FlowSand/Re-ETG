// Decompiled with JetBrains decompiler
// Type: FusebombBots1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Fusebomb/Bots1")]
public class FusebombBots1 : Script
  {
    private const string EnemyGuid = "4538456236f64ea79f483784370bc62f";

    protected override IEnumerator Top()
    {
      int num1 = 4;
      List<AIActor> spawnedActors = new List<AIActor>();
      Vector2 vector2_1 = this.BulletBank.aiActor.ParentRoom.area.UnitBottomLeft + new Vector2(1f, 5.5f);
      Vector2 max = new Vector2((float) (this.BulletBank.aiActor.ParentRoom.area.dimensions.x - 2), 4.75f);
      float num2 = max.x / (float) num1;
      for (int index = 0; index < num1; ++index)
      {
        Vector2 vector2_2 = BraveUtility.RandomVector2(Vector2.zero, max);
        vector2_2.x = (float) ((double) vector2_2.x % (double) num2 + (double) num2 * (double) index);
        Vector2 goalPos = vector2_1 + vector2_2;
        this.Fire(new Brave.BulletScript.Direction((goalPos - this.Position).ToAngle()), (Bullet) new FusebombBots1.BotBullet(goalPos, spawnedActors, 60 + 10 * index));
      }
      return (IEnumerator) null;
    }

    private class BotBullet : Bullet
    {
      private Vector2 m_goalPos;
      private int m_flightTime;

      public BotBullet(Vector2 goalPos, List<AIActor> spawnedActors, int flightTime)
        : base("bot")
      {
        this.m_goalPos = goalPos;
        this.m_flightTime = flightTime;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FusebombBots1.BotBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

