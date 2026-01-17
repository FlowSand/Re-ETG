// Decompiled with JetBrains decompiler
// Type: TowerBossIrisController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class TowerBossIrisController : BraveBehaviour
    {
      public TowerBossController tower;
      public bool fuseAlive = true;
      public float openDuration = 10f;
      private tk2dSprite m_sprite;

      public bool IsOpen => this.healthHaver.IsVulnerable;

      private void Start()
      {
        this.m_sprite = this.GetComponentInChildren<tk2dSprite>();
        this.m_sprite.IsPerpendicular = false;
        this.healthHaver.persistsOnDeath = true;
        this.healthHaver.IsVulnerable = false;
        this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.Damaged);
        this.healthHaver.OnDeath += new Action<Vector2>(this.Die);
      }

      private void Update()
      {
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void Open()
      {
        this.healthHaver.IsVulnerable = true;
        this.spriteAnimator.Play("tower_boss_leftPanel_open");
        this.StartCoroutine(this.TimedClose());
      }

      [DebuggerHidden]
      private IEnumerator TimedClose()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TowerBossIrisController.\u003CTimedClose\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public void Close()
      {
        this.healthHaver.IsVulnerable = false;
        this.spriteAnimator.Play("tower_boss_rightPanel_open");
      }

      private void Damaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory,
        Vector2 damageDirection)
      {
      }

      private void Die(Vector2 finalDamageDirection)
      {
        this.fuseAlive = false;
        if (this.tower.currentPhase == TowerBossController.TowerBossPhase.PHASE_ONE)
        {
          this.tower.NotifyFuseDestruction(this);
          this.healthHaver.FullHeal();
          this.healthHaver.IsVulnerable = false;
        }
        else
        {
          this.tower.NotifyFuseDestruction(this);
          this.healthHaver.IsVulnerable = false;
        }
      }
    }

}
