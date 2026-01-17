// Decompiled with JetBrains decompiler
// Type: BuffEnemiesBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class BuffEnemiesBehavior : BasicAttackBehavior
    {
      public float SearchInterval = 1f;
      public float EnemiesToBuff = 1f;
      public bool UsesBuffEffect = true;
      public AIActorBuffEffect buffEffect;
      public bool JamEnemies;
      public GameObject SmallJamEffect;
      public GameObject LargeJamEffect;
      [InspectorCategory("Visuals")]
      public string BuffAnimation;
      [InspectorCategory("Visuals")]
      public string BuffVfx;
      private float m_searchTimer;
      private List<AIActor> m_buffedEnemies = new List<AIActor>();
      private static List<AIActor> s_activeEnemies = new List<AIActor>();

      public override void Start()
      {
        base.Start();
        this.m_aiActor.IsBuffEnemy = true;
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_searchTimer);
      }

      public override BehaviorResult Update()
      {
        int num = (int) base.Update();
        BehaviorResult behaviorResult = base.Update();
        if (behaviorResult != BehaviorResult.Continue)
          return behaviorResult;
        if (!this.IsReady() || (double) this.m_searchTimer > 0.0)
          return BehaviorResult.Continue;
        this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref BuffEnemiesBehavior.s_activeEnemies);
        BuffEnemiesBehavior.s_activeEnemies.Remove(this.m_aiActor);
        for (int index = BuffEnemiesBehavior.s_activeEnemies.Count - 1; index >= 0; --index)
        {
          if (!this.IsGoodBuffTarget(BuffEnemiesBehavior.s_activeEnemies[index]))
            BuffEnemiesBehavior.s_activeEnemies.RemoveAt(index);
        }
        if (BuffEnemiesBehavior.s_activeEnemies.Count == 0)
          return BehaviorResult.Continue;
        while ((double) this.m_buffedEnemies.Count < (double) this.EnemiesToBuff && BuffEnemiesBehavior.s_activeEnemies.Count > 0)
        {
          int index = Random.Range(0, BuffEnemiesBehavior.s_activeEnemies.Count);
          this.m_buffedEnemies.Add(BuffEnemiesBehavior.s_activeEnemies[index]);
          BuffEnemiesBehavior.s_activeEnemies.RemoveAt(index);
        }
        for (int index = 0; index < this.m_buffedEnemies.Count; ++index)
          this.BuffEnemy(this.m_buffedEnemies[index]);
        this.m_searchTimer = this.SearchInterval;
        if (!string.IsNullOrEmpty(this.BuffAnimation))
          this.m_aiAnimator.PlayUntilCancelled(this.BuffAnimation, true);
        if (!string.IsNullOrEmpty(this.BuffVfx))
          this.m_aiAnimator.PlayVfx(this.BuffVfx);
        if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.knockbackDoer)
          this.m_aiActor.knockbackDoer.SetImmobile(true, nameof (BuffEnemiesBehavior));
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        for (int index = 0; index < this.m_buffedEnemies.Count; ++index)
        {
          AIActor buffedEnemy = this.m_buffedEnemies[index];
          if (!(bool) (Object) buffedEnemy || buffedEnemy.healthHaver.IsDead)
          {
            this.m_buffedEnemies.RemoveAt(index);
            --index;
          }
        }
        if ((double) this.m_searchTimer <= 0.0)
        {
          this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref BuffEnemiesBehavior.s_activeEnemies);
          BuffEnemiesBehavior.s_activeEnemies.Remove(this.m_aiActor);
          for (int index = 0; index < this.m_buffedEnemies.Count; ++index)
            BuffEnemiesBehavior.s_activeEnemies.Remove(this.m_buffedEnemies[index]);
          for (int index = BuffEnemiesBehavior.s_activeEnemies.Count - 1; index >= 0; --index)
          {
            if (!this.IsGoodBuffTarget(BuffEnemiesBehavior.s_activeEnemies[index]))
              BuffEnemiesBehavior.s_activeEnemies.RemoveAt(index);
          }
          if (BuffEnemiesBehavior.s_activeEnemies.Count > 0)
          {
            while ((double) this.m_buffedEnemies.Count < (double) this.EnemiesToBuff && BuffEnemiesBehavior.s_activeEnemies.Count > 0)
            {
              int index = Random.Range(0, BuffEnemiesBehavior.s_activeEnemies.Count);
              AIActor activeEnemy = BuffEnemiesBehavior.s_activeEnemies[index];
              BuffEnemiesBehavior.s_activeEnemies.RemoveAt(index);
              this.m_buffedEnemies.Add(activeEnemy);
              this.BuffEnemy(activeEnemy);
            }
          }
          this.m_searchTimer = this.SearchInterval;
        }
        return this.m_buffedEnemies.Count > 0 ? ContinuousBehaviorResult.Continue : ContinuousBehaviorResult.Finished;
      }

      public override void EndContinuousUpdate()
      {
        for (int index = 0; index < this.m_buffedEnemies.Count; ++index)
          this.UnbuffEnemy(this.m_buffedEnemies[index]);
        this.m_buffedEnemies.Clear();
        if (!string.IsNullOrEmpty(this.BuffAnimation))
          this.m_aiAnimator.EndAnimationIf(this.BuffAnimation);
        if (!string.IsNullOrEmpty(this.BuffVfx))
          this.m_aiAnimator.StopVfx(this.BuffVfx);
        if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.knockbackDoer)
          this.m_aiActor.knockbackDoer.SetImmobile(false, nameof (BuffEnemiesBehavior));
        this.UpdateCooldowns();
      }

      public override void OnActorPreDeath()
      {
        if (this.m_buffedEnemies.Count <= 0)
          return;
        for (int index = 0; index < this.m_buffedEnemies.Count; ++index)
          this.UnbuffEnemy(this.m_buffedEnemies[index]);
      }

      protected virtual void BuffEnemy(AIActor enemy)
      {
        if (!(bool) (Object) enemy)
          return;
        if (this.JamEnemies)
        {
          if ((bool) (Object) enemy.specRigidbody)
          {
            if (enemy.IsSignatureEnemy)
              enemy.PlaySmallExplosionsStyleEffect(this.LargeJamEffect, 8, 0.025f);
            else
              enemy.PlayEffectOnActor(this.SmallJamEffect, Vector3.zero, useHitbox: true);
          }
          enemy.BecomeBlackPhantom();
        }
        if (!this.UsesBuffEffect)
          return;
        enemy.ApplyEffect((GameActorEffect) this.buffEffect);
      }

      protected virtual void UnbuffEnemy(AIActor enemy)
      {
        if (!(bool) (Object) enemy)
          return;
        if (this.JamEnemies)
          enemy.UnbecomeBlackPhantom();
        if (!this.UsesBuffEffect)
          return;
        enemy.RemoveEffect((GameActorEffect) this.buffEffect);
      }

      private bool IsGoodBuffTarget(AIActor enemy)
      {
        return (bool) (Object) enemy && !enemy.IsBuffEnemy && !enemy.IsHarmlessEnemy && (!(bool) (Object) enemy.healthHaver || enemy.healthHaver.IsVulnerable && !enemy.healthHaver.PreventAllDamage) && (!this.JamEnemies || !enemy.IsBlackPhantom);
      }
    }

}
