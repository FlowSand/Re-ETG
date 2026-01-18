// Decompiled with JetBrains decompiler
// Type: GatlingGullLeapBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/LeapBehaviour")]
public class GatlingGullLeapBehavior : BasicAttackBehavior
  {
    public float AirSpeed = 1f;
    public float MinAirtime = 0.8f;
    public float DamageRadius = 3f;
    public float Damage = 1f;
    public float Force = 1f;
    public ScreenShakeSettings HitScreenShake;
    public ScreenShakeSettings MissScreenShake;
    public GameObject ImpactDustUp;
    public float SmugTime = 1f;
    [HideInInspector]
    [NonSerialized]
    public bool ShouldSmug = true;
    [HideInInspector]
    [NonSerialized]
    public Vector2? OverridePosition;
    [HideInInspector]
    [NonSerialized]
    public float SpeedMultiplier = 1f;
    private Vector3 m_startPosition;
    private Vector3 m_targetLandPosition;
    private tk2dSprite m_sprite;
    private SpeculativeRigidbody m_specRigidbody;
    private Vector3 m_offset;
    private float m_timer;
    private float m_totalAirTime;
    private tk2dSpriteAnimator m_animator;
    private tk2dSpriteAnimator m_shadowAnimator;
    private GatlingGullLeapBehavior.LeapState m_state;

    public override void Start()
    {
      base.Start();
      this.m_sprite = this.m_gameObject.GetComponent<tk2dSprite>();
      this.m_specRigidbody = this.m_gameObject.GetComponent<SpeculativeRigidbody>();
      this.m_specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleMajorBreakableDestruction);
    }

    public override void Upkeep() => base.Upkeep();

    protected void HandleMajorBreakableDestruction(CollisionData rigidbodyCollision)
    {
      MajorBreakable majorBreakable = rigidbodyCollision.OtherRigidbody.GetComponent<MajorBreakable>();
      if ((UnityEngine.Object) majorBreakable == (UnityEngine.Object) null)
        majorBreakable = rigidbodyCollision.OtherRigidbody.GetComponentInParent<MajorBreakable>();
      if (!rigidbodyCollision.Overlap || !((UnityEngine.Object) majorBreakable != (UnityEngine.Object) null))
        return;
      majorBreakable.Break(Vector2.zero);
    }

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      if (!(bool) (UnityEngine.Object) this.m_animator)
        this.m_animator = this.m_aiActor.spriteAnimator;
      if (!(bool) (UnityEngine.Object) this.m_shadowAnimator)
        this.m_shadowAnimator = this.m_aiActor.ShadowObject.GetComponent<tk2dSpriteAnimator>();
      if (!(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      this.m_startPosition = this.m_gameObject.transform.position;
      this.m_aiActor.ClearPath();
      this.m_state = GatlingGullLeapBehavior.LeapState.Jump;
      this.m_aiAnimator.enabled = false;
      tk2dSpriteAnimationClip clipByName = this.m_animator.GetClipByName("jump");
      this.m_animator.Play(clipByName, 0.0f, clipByName.fps * this.SpeedMultiplier);
      this.m_updateEveryFrame = true;
      this.m_offset = (this.m_aiActor.specRigidbody.UnitCenter.ToVector3ZUp() - this.m_aiActor.transform.position).WithZ(0.0f);
      this.m_animator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
      this.UpdateTargetPosition();
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num1 = (int) base.ContinuousUpdate();
      this.m_timer -= this.m_deltaTime * this.SpeedMultiplier;
      if (this.m_state == GatlingGullLeapBehavior.LeapState.Jump)
      {
        if (!this.m_animator.IsPlaying("jump"))
        {
          this.UpdateTargetPosition();
          this.m_totalAirTime = Mathf.Max(this.MinAirtime / this.SpeedMultiplier, (this.m_targetLandPosition - this.m_startPosition).magnitude / this.AirSpeed);
          this.m_timer = this.m_totalAirTime;
          this.m_state = GatlingGullLeapBehavior.LeapState.TrackFromAbove;
          this.m_specRigidbody.enabled = false;
          this.m_sprite.renderer.enabled = false;
          SpriteOutlineManager.ToggleOutlineRenderers((tk2dBaseSprite) this.m_sprite, false);
        }
      }
      else if (this.m_state == GatlingGullLeapBehavior.LeapState.TrackFromAbove)
      {
        this.UpdateTargetPosition();
        this.m_gameObject.transform.position = Vector3.Lerp(this.m_targetLandPosition, this.m_startPosition, Mathf.Clamp01(this.m_timer / this.m_totalAirTime));
        if ((double) this.m_timer <= 0.0)
        {
          this.m_state = GatlingGullLeapBehavior.LeapState.ShadowFall;
          int num2 = (int) AkSoundEngine.PostEvent("Play_ANM_gull_descend_01", this.m_gameObject);
          tk2dSpriteAnimationClip clipByName = this.m_shadowAnimator.GetClipByName("shadow_out");
          this.m_shadowAnimator.Play(clipByName, 0.0f, clipByName.fps * this.SpeedMultiplier);
        }
      }
      else if (this.m_state == GatlingGullLeapBehavior.LeapState.ShadowFall)
      {
        if (!this.m_shadowAnimator.IsPlaying("shadow_out"))
        {
          this.m_state = GatlingGullLeapBehavior.LeapState.Fall;
          this.m_gameObject.transform.position = this.m_targetLandPosition;
          this.m_specRigidbody.enabled = true;
          this.m_specRigidbody.Reinitialize();
          this.m_sprite.renderer.enabled = true;
          SpriteOutlineManager.ToggleOutlineRenderers((tk2dBaseSprite) this.m_sprite, true);
          tk2dSpriteAnimationClip clipByName = this.m_animator.GetClipByName("land");
          this.m_animator.Play(clipByName, 0.0f, clipByName.fps * this.SpeedMultiplier);
        }
      }
      else if (this.m_state == GatlingGullLeapBehavior.LeapState.Fall)
      {
        if (!this.m_animator.IsPlaying("land"))
        {
          this.m_state = GatlingGullLeapBehavior.LeapState.Smug;
          this.m_aiAnimator.enabled = true;
          if (this.ShouldSmug)
          {
            this.m_aiAnimator.PlayForDuration("smug", this.SmugTime, true);
            this.m_timer = this.SmugTime;
          }
        }
      }
      else if (this.m_state == GatlingGullLeapBehavior.LeapState.Smug && ((double) this.m_timer <= 0.0 || !this.ShouldSmug))
      {
        this.ShouldSmug = false;
        return ContinuousBehaviorResult.Finished;
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_updateEveryFrame = false;
      this.m_animator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
      this.UpdateCooldowns();
    }

    public override bool IsOverridable() => false;

    private void HandleAnimationEvent(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frameNo)
    {
      tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNo);
      if (frame.eventInfo == "start_shadow_animation")
      {
        this.m_shadowAnimator.Play("shadow_in");
      }
      else
      {
        if (!(frame.eventInfo == "land_impact"))
          return;
        if ((bool) (UnityEngine.Object) this.ImpactDustUp)
        {
          tk2dSprite component1 = SpawnManager.SpawnVFX(this.ImpactDustUp).GetComponent<tk2dSprite>();
          tk2dSprite component2 = this.m_aiActor.ShadowObject.GetComponent<tk2dSprite>();
          component1.transform.position = this.m_targetLandPosition + this.m_offset;
          component2.AttachRenderer((tk2dBaseSprite) component1);
          component1.HeightOffGround = 0.01f;
        }
        bool flag = false;
        SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
        if ((bool) (UnityEngine.Object) targetRigidbody)
        {
          Vector2 direction = this.m_aiActor.TargetRigidbody.UnitCenter - this.m_aiActor.specRigidbody.UnitCenter;
          if ((double) direction.magnitude < (double) this.DamageRadius)
          {
            if (Mathf.Approximately(direction.magnitude, 0.0f))
              direction = UnityEngine.Random.insideUnitCircle;
            if ((bool) (UnityEngine.Object) targetRigidbody.healthHaver)
              targetRigidbody.healthHaver.ApplyDamage(this.Damage, direction.normalized, this.m_aiActor.GetActorName());
            if ((bool) (UnityEngine.Object) targetRigidbody.knockbackDoer)
              targetRigidbody.knockbackDoer.ApplyKnockback(direction, this.Force);
            targetRigidbody.RegisterGhostCollisionException(this.m_aiActor.specRigidbody);
            flag = true;
            this.ShouldSmug = true;
            GameManager.Instance.MainCameraController.DoScreenShake(this.HitScreenShake, new Vector2?(this.m_aiActor.specRigidbody.UnitCenter));
          }
        }
        if (flag)
          return;
        GameManager.Instance.MainCameraController.DoScreenShake(this.MissScreenShake, new Vector2?(this.m_aiActor.specRigidbody.UnitCenter));
      }
    }

    private void UpdateTargetPosition()
    {
      if (!(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        return;
      Vector2 target = !this.OverridePosition.HasValue ? this.m_aiActor.TargetRigidbody.UnitCenter : this.OverridePosition.Value;
      Vector2 extents = this.m_aiActor.specRigidbody.UnitDimensions / 2f;
      Dungeon dungeon = GameManager.Instance.Dungeon;
      RoomHandler roomFromPosition = dungeon.data.GetRoomFromPosition(target.ToIntVector2(VectorConversions.Floor));
      if (roomFromPosition != null)
      {
        Vector2 min = roomFromPosition.area.basePosition.ToVector2() + extents + Vector2.one * PhysicsEngine.Instance.HalfPixelUnitWidth;
        Vector2 max = (roomFromPosition.area.basePosition + roomFromPosition.area.dimensions).ToVector2() - extents - Vector2.one * PhysicsEngine.Instance.HalfPixelUnitWidth;
        target = Vector2Extensions.Clamp(target, min, max);
      }
      Vector2 vector1 = target + new Vector2(-extents.x, extents.y);
      Vector2 vector2 = target + new Vector2(extents.x, extents.y);
      Vector2 vector3 = target + new Vector2(-extents.x, -extents.y);
      Vector2 vector4 = target + new Vector2(extents.x, -extents.y);
      CellData cellData1 = dungeon.data[vector1.ToIntVector2(VectorConversions.Floor)];
      CellData cellData2 = dungeon.data[vector2.ToIntVector2(VectorConversions.Floor)];
      CellData cellData3 = dungeon.data[vector3.ToIntVector2(VectorConversions.Floor)];
      CellData cellData4 = dungeon.data[vector4.ToIntVector2(VectorConversions.Floor)];
      bool flag1 = cellData1.type != CellType.FLOOR;
      bool flag2 = cellData2.type != CellType.FLOOR;
      bool flag3 = cellData3.type != CellType.FLOOR || cellData3.IsTopWall();
      bool flag4 = cellData4.type != CellType.FLOOR || cellData4.IsTopWall();
      int num = 0;
      if (flag1)
        ++num;
      if (flag2)
        ++num;
      if (flag3)
        ++num;
      if (flag4)
        ++num;
      switch (num)
      {
        case 1:
          if (flag1)
            this.AdjustTarget(ref target, extents, IntVector2.Down, IntVector2.Right);
          if (flag2)
            this.AdjustTarget(ref target, extents, IntVector2.Down, IntVector2.Left);
          if (flag3)
            this.AdjustTarget(ref target, extents, IntVector2.Up, IntVector2.Right);
          if (flag4)
          {
            this.AdjustTarget(ref target, extents, IntVector2.Up, IntVector2.Left);
            break;
          }
          break;
        case 2:
          if (flag3 && flag4)
            this.AdjustTarget(ref target, extents, IntVector2.Up);
          if (flag1 && flag2)
            this.AdjustTarget(ref target, extents, IntVector2.Down);
          if (flag2 && flag4)
            this.AdjustTarget(ref target, extents, IntVector2.Left);
          if (flag1 && flag3)
          {
            this.AdjustTarget(ref target, extents, IntVector2.Right);
            break;
          }
          break;
        case 3:
          if (!flag4)
            this.AdjustTarget(ref target, extents, IntVector2.Down, IntVector2.Right);
          if (!flag3)
            this.AdjustTarget(ref target, extents, IntVector2.Down, IntVector2.Left);
          if (!flag2)
            this.AdjustTarget(ref target, extents, IntVector2.Up, IntVector2.Right);
          if (!flag1)
          {
            this.AdjustTarget(ref target, extents, IntVector2.Up, IntVector2.Left);
            break;
          }
          break;
        case 4:
          if (dungeon.data[vector3.ToIntVector2(VectorConversions.Floor) + new IntVector2(0, 2)].type == CellType.FLOOR)
          {
            this.AdjustTarget(ref target, extents, IntVector2.Up, IntVector2.Up);
            break;
          }
          if (dungeon.data[vector1.ToIntVector2(VectorConversions.Floor) + new IntVector2(2, 0)].type == CellType.FLOOR)
          {
            this.AdjustTarget(ref target, extents, IntVector2.Right, IntVector2.Right);
            break;
          }
          if (dungeon.data[vector2.ToIntVector2(VectorConversions.Floor) + new IntVector2(0, -2)].type == CellType.FLOOR)
          {
            this.AdjustTarget(ref target, extents, IntVector2.Down, IntVector2.Down);
            break;
          }
          if (dungeon.data[vector4.ToIntVector2(VectorConversions.Floor) + new IntVector2(-2, 0)].type == CellType.FLOOR)
          {
            this.AdjustTarget(ref target, extents, IntVector2.Left, IntVector2.Left);
            break;
          }
          break;
      }
      this.m_targetLandPosition = target.ToVector3ZUp() - this.m_offset;
      this.m_targetLandPosition.z = this.m_targetLandPosition.y;
    }

    private void AdjustTarget(ref Vector2 target, Vector2 extents, params IntVector2[] dir)
    {
      for (int index = 0; index < dir.Length; ++index)
      {
        if (dir[index] == IntVector2.Up)
          target.y = (float) ((int) ((double) target.y - (double) extents.y) + 1) + extents.y + PhysicsEngine.Instance.PixelUnitWidth;
        if (dir[index] == IntVector2.Down)
          target.y = (float) (int) ((double) target.y + (double) extents.y) - extents.y - PhysicsEngine.Instance.PixelUnitWidth;
        if (dir[index] == IntVector2.Left)
          target.x = (float) (int) ((double) target.x + (double) extents.x) - extents.x - PhysicsEngine.Instance.PixelUnitWidth;
        if (dir[index] == IntVector2.Right)
          target.x = (float) ((int) ((double) target.x - (double) extents.x) + 1) + extents.x + PhysicsEngine.Instance.PixelUnitWidth;
      }
    }

    public enum LeapState
    {
      Jump,
      TrackFromAbove,
      ShadowFall,
      Fall,
      Smug,
    }
  }

