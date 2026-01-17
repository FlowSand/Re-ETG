// Decompiled with JetBrains decompiler
// Type: BeholsterRocketBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName("Bosses/Beholster/RocketBehavior2")]
    public class BeholsterRocketBehavior : BasicAttackBehavior
    {
      public bool LineOfSight = true;
      public float WindUpTime = 1f;
      public GameObject TargetVFX;
      public float[] FiringAngles;
      public float FireCooldown;
      public BeholsterTentacleController[] Tentacles;
      private BeholsterController m_beholster;
      private float m_windupTimer;
      private GameObject m_spawnedTargetVfx;
      private float m_fireTimer;
      private int m_fireIndex;

      public override void Start()
      {
        base.Start();
        this.m_beholster = this.m_aiActor.GetComponent<BeholsterController>();
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_windupTimer);
        this.DecrementTimer(ref this.m_fireTimer);
      }

      public override BehaviorResult Update()
      {
        BehaviorResult behaviorResult = base.Update();
        if (behaviorResult != BehaviorResult.Continue)
          return behaviorResult;
        if (!this.IsReady())
          return BehaviorResult.Continue;
        bool flag = this.LineOfSight && !this.m_aiActor.HasLineOfSightToTarget;
        if ((Object) this.m_aiActor.TargetRigidbody == (Object) null || flag)
        {
          this.m_beholster.StopFiringTentacles(this.Tentacles);
          return BehaviorResult.Continue;
        }
        if ((double) this.WindUpTime > 0.0)
        {
          this.m_windupTimer = this.WindUpTime;
          this.m_aiActor.ClearPath();
          if ((bool) (Object) this.TargetVFX)
          {
            this.m_spawnedTargetVfx = Object.Instantiate<GameObject>(this.TargetVFX, (Vector3) this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox), Quaternion.identity);
            this.m_spawnedTargetVfx.transform.parent = this.m_aiActor.TargetRigidbody.transform;
            tk2dBaseSprite component = this.m_spawnedTargetVfx.GetComponent<tk2dBaseSprite>();
            tk2dBaseSprite sprite = this.m_aiActor.TargetRigidbody.sprite;
            if ((bool) (Object) component && (bool) (Object) sprite)
            {
              sprite.AttachRenderer(component);
              component.HeightOffGround = 5f;
              component.UpdateZDepth();
            }
          }
        }
        this.m_fireIndex = 0;
        this.m_fireTimer = 0.0f;
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        if ((double) this.m_windupTimer > 0.0 || (double) this.m_fireTimer > 0.0)
          return ContinuousBehaviorResult.Continue;
        this.m_beholster.SingleFireTentacle(this.Tentacles, new float?(this.FiringAngles[this.m_fireIndex % this.FiringAngles.Length]));
        this.m_fireTimer = this.FireCooldown;
        ++this.m_fireIndex;
        if ((bool) (Object) this.m_spawnedTargetVfx)
        {
          Object.Destroy((Object) this.m_spawnedTargetVfx);
          this.m_spawnedTargetVfx = (GameObject) null;
        }
        if (this.m_fireIndex < this.FiringAngles.Length * (!this.m_aiActor.IsBlackPhantom ? 1 : 2))
          return ContinuousBehaviorResult.Continue;
        this.UpdateCooldowns();
        return ContinuousBehaviorResult.Finished;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        if (!(bool) (Object) this.m_spawnedTargetVfx)
          return;
        Object.Destroy((Object) this.m_spawnedTargetVfx);
        this.m_spawnedTargetVfx = (GameObject) null;
      }

      public override void Destroy()
      {
        base.Destroy();
        if (!(bool) (Object) this.m_spawnedTargetVfx)
          return;
        Object.Destroy((Object) this.m_spawnedTargetVfx);
        this.m_spawnedTargetVfx = (GameObject) null;
      }

      public override bool IsReady()
      {
        if (!base.IsReady())
          return false;
        for (int index = 0; index < this.Tentacles.Length; ++index)
        {
          if (this.Tentacles[index].IsReady)
            return true;
        }
        return false;
      }
    }

}
