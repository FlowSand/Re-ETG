// Decompiled with JetBrains decompiler
// Type: AshManEngageDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class AshManEngageDoer : CustomEngageDoer
    {
      public float FromStatueChance = 0.5f;
      public string BreakablePrefix = "Forge_Ash_Bullet";
      public float MinSpawnDelay = 2f;
      public float MaxSpawnDelay = 6f;
      private bool m_isFinished;
      private bool m_brokeEarly;

      public void Awake()
      {
        if ((double) Random.value > (double) this.FromStatueChance)
        {
          this.m_isFinished = true;
        }
        else
        {
          this.aiActor.HasDonePlayerEnterCheck = true;
          this.aiActor.CollisionDamage = 0.0f;
        }
      }

      public override void StartIntro()
      {
        if (this.m_isFinished)
          return;
        List<MinorBreakable> list = new List<MinorBreakable>();
        RoomHandler parentRoom = this.aiActor.ParentRoom;
        List<MinorBreakable> allMinorBreakables = StaticReferenceManager.AllMinorBreakables;
        DungeonData data = GameManager.Instance.Dungeon.data;
        for (int index = 0; index < allMinorBreakables.Count; ++index)
        {
          MinorBreakable minorBreakable = allMinorBreakables[index];
          if (minorBreakable.name.StartsWith(this.BreakablePrefix) && data.GetAbsoluteRoomFromPosition(minorBreakable.transform.position.IntXY(VectorConversions.Floor)) == parentRoom)
            list.Add(minorBreakable);
        }
        if (list.Count == 0)
        {
          this.m_isFinished = true;
          this.aiActor.invisibleUntilAwaken = false;
          this.aiActor.ToggleRenderers(true);
          this.aiAnimator.PlayDefaultAwakenedState();
          this.aiActor.State = AIActor.ActorState.Normal;
        }
        else
          this.StartCoroutine(this.DoIntro(BraveUtility.RandomElement<MinorBreakable>(list)));
      }

      [DebuggerHidden]
      private IEnumerator DoIntro(MinorBreakable breakable)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AshManEngageDoer__DoIntroc__Iterator0()
        {
          breakable = breakable,
          _this = this
        };
      }

      public override bool IsFinished => this.m_isFinished;

      private void OnBreak() => this.m_brokeEarly = true;
    }

}
