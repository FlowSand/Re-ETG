// Decompiled with JetBrains decompiler
// Type: GatlingGullLastStandBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/LastStandBehavior")]
public class GatlingGullLastStandBehavior : BasicAttackBehavior
  {
    public float HealthThreshold = 5f;
    public float AngleVariance = 20f;
    public string OverrideBulletName;
    private bool m_passthrough;
    private GatlingGullLeapBehavior m_leapBehavior;
    private RoomHandler m_room;
    private readonly Vector2 m_leapPosition = new Vector2(19f, 13f);
    private bool m_isInDeathPosition;
    private readonly string[] m_animNames = new string[8]
    {
      "fire_up",
      "fire_north_east",
      "fire_right",
      "fire_south_east",
      "fire_down",
      "fire_south_west",
      "fire_left",
      "fire_north_west"
    };
    private string m_cachedAnimationName;

    public override void Start()
    {
      base.Start();
      this.m_leapBehavior = (GatlingGullLeapBehavior) ((AttackBehaviorGroup) this.m_aiActor.behaviorSpeculator.AttackBehaviors.Find((Predicate<AttackBehaviorBase>) (b => b is AttackBehaviorGroup))).AttackBehaviors.Find((Predicate<AttackBehaviorGroup.AttackGroupItem>) (b => b.Behavior is GatlingGullLeapBehavior)).Behavior;
      this.m_room = GameManager.Instance.Dungeon.GetRoomFromPosition(this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
      this.m_aiActor.healthHaver.minimumHealth = 1f;
    }

    public override void Upkeep()
    {
      base.Upkeep();
      if (!this.m_passthrough)
        return;
      this.m_leapBehavior.Upkeep();
    }

    public override bool OverrideOtherBehaviors()
    {
      return (double) this.m_aiActor.healthHaver.GetCurrentHealth() <= (double) this.HealthThreshold;
    }

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      if ((double) this.m_aiActor.healthHaver.GetCurrentHealth() > (double) this.HealthThreshold)
        return BehaviorResult.Continue;
      this.m_leapBehavior.OverridePosition = new Vector2?(this.m_room.area.basePosition.ToVector2() + this.m_leapPosition);
      BehaviorResult behaviorResult = this.m_leapBehavior.Update();
      if (behaviorResult == BehaviorResult.RunContinuous)
        this.m_passthrough = true;
      else
        this.m_leapBehavior.OverridePosition = new Vector2?();
      return behaviorResult;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (this.m_passthrough)
      {
        if (this.m_leapBehavior.ContinuousUpdate() == ContinuousBehaviorResult.Finished)
        {
          this.m_passthrough = false;
          this.m_leapBehavior.OverridePosition = new Vector2?();
          this.UpdateCooldowns();
          this.m_aiActor.healthHaver.minimumHealth = 0.0f;
          this.m_isInDeathPosition = true;
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            GameManager.Instance.AllPlayers[index].BossKillingMode = true;
        }
      }
      else if (this.m_isInDeathPosition)
      {
        if (!(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        {
          this.m_aiShooter.ManualGunAngle = false;
        }
        else
        {
          Vector2 inVec = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.CenterPosition;
          int octant = BraveMathCollege.VectorToOctant(inVec);
          this.m_aiShooter.ManualGunAngle = true;
          this.m_aiShooter.GunAngle = Mathf.Atan2(inVec.y, inVec.x) * 57.29578f;
          Vector2 direction = (Vector2) (Quaternion.Euler(0.0f, 0.0f, (float) (octant * -45)) * (Vector3) Vector2.up);
          this.m_aiShooter.volley.projectiles[0].angleVariance = this.AngleVariance;
          this.m_aiShooter.ShootInDirection(direction, this.OverrideBulletName);
          this.m_cachedAnimationName = this.m_animNames[octant];
          this.m_aiAnimator.PlayUntilCancelled(this.m_cachedAnimationName, true);
        }
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void SetDeltaTime(float deltaTime)
    {
      base.SetDeltaTime(deltaTime);
      if (!this.m_passthrough)
        return;
      this.m_leapBehavior.SetDeltaTime(deltaTime);
    }

    public override bool IsReady()
    {
      return this.m_passthrough ? this.m_leapBehavior.IsReady() : base.IsReady();
    }

    public override bool UpdateEveryFrame()
    {
      return this.m_passthrough ? this.m_leapBehavior.UpdateEveryFrame() : base.UpdateEveryFrame();
    }

    public override bool IsOverridable() => false;
  }

