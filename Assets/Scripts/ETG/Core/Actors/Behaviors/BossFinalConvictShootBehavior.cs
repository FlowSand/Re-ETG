// Decompiled with JetBrains decompiler
// Type: BossFinalConvictShootBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalConvict/ShootBehavior")]
public class BossFinalConvictShootBehavior : BasicAttackBehavior
  {
    public GameObject shootPoint;
    public BulletScriptSelector bulletScript;
    public float maxMoveSpeed = 5f;
    public float moveAcceleration = 10f;
    [InspectorCategory("Visuals")]
    public string anim;
    private BulletScriptSource m_bulletSource;
    private float m_verticalVelocity;

    public override void Start() => base.Start();

    public override void Upkeep() => base.Upkeep();

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady() || !(bool) (Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      this.m_aiActor.ClearPath();
      this.m_aiActor.BehaviorOverridesVelocity = true;
      this.m_aiActor.BehaviorVelocity = Vector2.zero;
      this.m_verticalVelocity = 0.0f;
      if (!string.IsNullOrEmpty(this.anim))
        this.m_aiAnimator.PlayUntilCancelled(this.anim);
      this.Fire();
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (!(bool) (Object) this.m_aiActor.TargetRigidbody || this.m_bulletSource.IsEnded || this.IsTargetUnreachable())
        return ContinuousBehaviorResult.Finished;
      Vector2 vector2 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
      float max = (double) Mathf.Abs(vector2.y) <= 5.0 ? this.maxMoveSpeed : 1.5f * this.maxMoveSpeed;
      this.m_verticalVelocity = Mathf.Clamp(this.m_verticalVelocity + Mathf.Sign(vector2.y) * this.moveAcceleration * this.m_deltaTime, -max, max);
      this.m_aiActor.BehaviorVelocity = new Vector2(0.0f, this.m_verticalVelocity);
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_aiActor.BehaviorOverridesVelocity = false;
      if (!string.IsNullOrEmpty(this.anim))
        this.m_aiAnimator.EndAnimationIf(this.anim);
      if ((bool) (Object) this.m_bulletSource)
        this.m_bulletSource.ForceStop();
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override bool IsOverridable() => false;

    public override bool IsReady() => base.IsReady() && !this.IsTargetUnreachable();

    private void Fire()
    {
      if (!(bool) (Object) this.m_bulletSource)
        this.m_bulletSource = this.shootPoint.GetOrAddComponent<BulletScriptSource>();
      this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
      this.m_bulletSource.BulletScript = this.bulletScript;
      this.m_bulletSource.Initialize();
    }

    private bool IsTargetUnreachable(float maxDist = 3.40282347E+38f)
    {
      if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
        return true;
      Vector2 vector2 = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
      int mask = CollisionMask.LayerToMask(CollisionLayer.LowObstacle, CollisionLayer.HighObstacle);
      CollisionData result;
      bool flag = PhysicsEngine.Instance.RigidbodyCastWithIgnores(this.m_aiActor.specRigidbody, PhysicsEngine.UnitToPixel(new Vector2(0.0f, Mathf.Min(vector2.y, maxDist))), out result, true, true, new int?(mask), false, this.m_aiActor.specRigidbody);
      CollisionData.Pool.Free(ref result);
      return flag;
    }
  }

