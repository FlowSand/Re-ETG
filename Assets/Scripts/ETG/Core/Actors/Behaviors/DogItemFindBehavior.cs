// Decompiled with JetBrains decompiler
// Type: DogItemFindBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class DogItemFindBehavior : BehaviorBase
    {
      public GenericLootTable ItemFindLootTable;
      public float ChanceToFindItemOnRoomClear = 0.05f;
      public string ItemFindAnimName;
      private float m_findTimer;

      public override void Start()
      {
        base.Start();
        if (!((UnityEngine.Object) this.m_aiActor.CompanionOwner != (UnityEngine.Object) null))
          return;
        this.m_aiActor.CompanionOwner.OnRoomClearEvent += new Action<PlayerController>(this.HandleRoomCleared);
      }

      public override void Destroy()
      {
        if ((UnityEngine.Object) this.m_aiActor.CompanionOwner != (UnityEngine.Object) null)
          this.m_aiActor.CompanionOwner.OnRoomClearEvent -= new Action<PlayerController>(this.HandleRoomCleared);
        base.Destroy();
      }

      [DebuggerHidden]
      private IEnumerator DelayedSpawnItem(Vector2 spawnPoint)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DogItemFindBehavior.<DelayedSpawnItem>c__Iterator0()
        {
          spawnPoint = spawnPoint,
          _this = this
        };
      }

      private void HandleRoomCleared(PlayerController obj)
      {
        if ((double) UnityEngine.Random.value >= (double) this.ChanceToFindItemOnRoomClear)
          return;
        this.m_findTimer = 4.5f;
        if (!string.IsNullOrEmpty(this.ItemFindAnimName))
          this.m_aiAnimator.PlayUntilFinished(this.ItemFindAnimName);
        GameManager.Instance.Dungeon.StartCoroutine(this.DelayedSpawnItem(this.m_aiActor.CenterPosition));
      }

      public override void Upkeep() => base.Upkeep();

      public override BehaviorResult Update()
      {
        if ((double) this.m_findTimer > 0.0)
        {
          this.DecrementTimer(ref this.m_findTimer);
          this.m_aiActor.ClearPath();
        }
        return base.Update();
      }
    }

}
