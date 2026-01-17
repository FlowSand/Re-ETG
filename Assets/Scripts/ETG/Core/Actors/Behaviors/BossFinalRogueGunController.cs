// Decompiled with JetBrains decompiler
// Type: BossFinalRogueGunController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public abstract class BossFinalRogueGunController : BaseBehavior<FullSerializerSerializer>
    {
      public BossFinalRogueController ship;
      public BossFinalRogueGunController.FireType fireType = BossFinalRogueGunController.FireType.Triggered;
      [InspectorShowIf("IsTimed")]
      public float initialDelay;
      [InspectorShowIf("IsTimed")]
      public float delay;
      private float m_shotTimer;

      public bool IsTimed() => this.fireType == BossFinalRogueGunController.FireType.Timed;

      public virtual void Start()
      {
        if (this.fireType == BossFinalRogueGunController.FireType.Timed)
          this.m_shotTimer = this.initialDelay;
        this.ship.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
      }

      public virtual void Update()
      {
        if (!this.ship.aiActor.enabled || !this.ship.behaviorSpeculator.enabled || this.fireType != BossFinalRogueGunController.FireType.Timed || !this.IsFinished)
          return;
        this.m_shotTimer -= BraveTime.DeltaTime;
        if ((double) this.m_shotTimer > 0.0)
          return;
        this.Fire();
        this.m_shotTimer = this.delay;
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnPreDeath(Vector2 deathDir) => this.CeaseFire();

      public abstract void Fire();

      public abstract bool IsFinished { get; }

      public abstract void CeaseFire();

      public enum FireType
      {
        Triggered = 10, // 0x0000000A
        Timed = 20, // 0x00000014
      }
    }

}
