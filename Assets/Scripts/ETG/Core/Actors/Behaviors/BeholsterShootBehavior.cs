// Decompiled with JetBrains decompiler
// Type: BeholsterShootBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName("Bosses/Beholster/ShootBehavior")]
    public class BeholsterShootBehavior : BasicAttackBehavior
    {
      public bool LineOfSight = true;
      public float WindUpTime = 1f;
      public BulletScriptSelector BulletScript;
      public BeholsterTentacleController Tentacle;
      private BeholsterShootBehavior.State m_state;
      private float m_windupTimer;

      public override void Start() => base.Start();

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_windupTimer);
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
          return BehaviorResult.Continue;
        this.m_state = BeholsterShootBehavior.State.Windup;
        this.m_windupTimer = this.WindUpTime;
        this.m_aiActor.ClearPath();
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        if (this.m_state == BeholsterShootBehavior.State.Windup)
        {
          if ((double) this.m_windupTimer <= 0.0)
          {
            if ((bool) (Object) this.m_aiActor.TargetRigidbody)
              this.m_aiActor.bulletBank.FixedPlayerPosition = new Vector2?(this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox));
            this.m_aiAnimator.LockFacingDirection = true;
            this.Tentacle.BulletScriptSource.FreezeTopPosition = true;
            this.Tentacle.ShootBulletScript(this.BulletScript);
            this.m_state = BeholsterShootBehavior.State.Firing;
          }
          return ContinuousBehaviorResult.Continue;
        }
        if (this.m_state != BeholsterShootBehavior.State.Firing || !this.Tentacle.BulletScriptSource.IsEnded)
          return ContinuousBehaviorResult.Continue;
        this.m_state = BeholsterShootBehavior.State.Ready;
        this.m_aiActor.bulletBank.FixedPlayerPosition = new Vector2?();
        this.m_aiAnimator.LockFacingDirection = false;
        this.Tentacle.BulletScriptSource.FreezeTopPosition = false;
        this.UpdateCooldowns();
        return ContinuousBehaviorResult.Finished;
      }

      private enum State
      {
        Ready,
        Windup,
        Firing,
      }
    }

}
