using Dungeonator;
using FullInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/RepositionBehavior")]
public class GatlingGullRepositionBehavior : BasicAttackBehavior
  {
    public float LostSightTime = 5f;
    public float MinDistanceToPlayer = 4f;
    public float LeapSpeedMultiplier = 1f;
    private bool m_passthrough;
    private GatlingGullLeapBehavior m_leapBehavior;
    private float m_lostSightTimer;
    private RoomHandler m_room;
    private List<IntVector2> m_leapPositions = new List<IntVector2>();

    public override void Start()
    {
      base.Start();
      this.m_leapBehavior = (GatlingGullLeapBehavior) ((AttackBehaviorGroup) this.m_aiActor.behaviorSpeculator.AttackBehaviors.Find((Predicate<AttackBehaviorBase>) (b => b is AttackBehaviorGroup))).AttackBehaviors.Find((Predicate<AttackBehaviorGroup.AttackGroupItem>) (b => b.Behavior is GatlingGullLeapBehavior)).Behavior;
      this.m_room = GameManager.Instance.Dungeon.GetRoomFromPosition(this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
      List<GatlingGullLeapPoint> componentsInRoom = this.m_room.GetComponentsInRoom<GatlingGullLeapPoint>();
      for (int index = 0; index < componentsInRoom.Count; ++index)
      {
        if (componentsInRoom[index].ForReposition)
          this.m_leapPositions.Add(componentsInRoom[index].PlacedPosition - this.m_room.area.basePosition);
      }
    }

    public override void Upkeep()
    {
      base.Upkeep();
      if (this.m_passthrough)
      {
        this.m_leapBehavior.Upkeep();
      }
      else
      {
        Vector2 viewport = BraveUtility.WorldPointToViewport((Vector3) this.m_aiActor.specRigidbody.UnitCenter, ViewportType.Gameplay);
        if ((double) viewport.x < 0.0 || (double) viewport.x > 1.0 || (double) viewport.y < -0.15000000596046448 || (double) viewport.y > 1.0 || !this.m_aiActor.HasLineOfSightToTarget)
          this.m_lostSightTimer += this.m_deltaTime;
        else
          this.m_lostSightTimer = 0.0f;
      }
    }

    public override BehaviorResult Update()
    {
      int num1 = (int) base.Update();
      if (this.m_leapPositions.Count == 0 || !this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody || (double) this.m_lostSightTimer < (double) this.LostSightTime)
        return BehaviorResult.Continue;
      Vector2 vector2_1 = Vector2.zero;
      float num2 = float.MaxValue;
      for (int index = 0; index < this.m_leapPositions.Count; ++index)
      {
        Vector2 vector2_2 = (this.m_room.area.basePosition + this.m_leapPositions[index]).ToVector2() + new Vector2(1f, 0.5f);
        float num3 = Vector2.Distance(vector2_2, this.m_aiActor.TargetRigidbody.UnitCenter);
        if ((double) num3 < (double) num2 && (double) num3 > (double) this.MinDistanceToPlayer && this.m_aiActor.HasLineOfSightToTargetFromPosition(vector2_2))
        {
          vector2_1 = vector2_2;
          num2 = num3;
        }
      }
      if (vector2_1 != Vector2.zero)
      {
        this.m_leapBehavior.OverridePosition = new Vector2?(vector2_1);
        this.m_leapBehavior.SpeedMultiplier = this.LeapSpeedMultiplier;
        BehaviorResult behaviorResult = this.m_leapBehavior.Update();
        if (behaviorResult == BehaviorResult.RunContinuous)
        {
          this.m_passthrough = true;
        }
        else
        {
          this.m_leapBehavior.OverridePosition = new Vector2?();
          this.m_leapBehavior.SpeedMultiplier = 1f;
        }
        return behaviorResult;
      }
      Debug.Log((object) "no jumps found!?");
      return BehaviorResult.Continue;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      return this.m_passthrough ? this.m_leapBehavior.ContinuousUpdate() : ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if (this.m_passthrough)
        this.m_leapBehavior.EndContinuousUpdate();
      this.m_passthrough = false;
      this.m_leapBehavior.OverridePosition = new Vector2?();
      this.m_leapBehavior.SpeedMultiplier = 1f;
      this.UpdateCooldowns();
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

    public override bool IsOverridable()
    {
      return !this.m_passthrough || this.m_leapBehavior.IsOverridable();
    }
  }

