// Decompiled with JetBrains decompiler
// Type: PathingTrapController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class PathingTrapController : TrapController
  {
    public float damage;
    public bool ignoreInvulnerabilityFrames;
    public float knockbackStrength;
    public bool hitsEnemies;
    public float enemyDamage;
    public float enemyKnockbackStrength;
    [TogglesProperty("bloodyAnimation", "Bloody Animation")]
    public bool usesBloodyAnimation;
    [HideInInspector]
    public string bloodyAnimation;
    public bool usesDirectionalAnimations;
    [ShowInInspectorIf("usesDirectionalAnimations", true)]
    public string northAnimation;
    [ShowInInspectorIf("usesDirectionalAnimations", true)]
    public string eastAnimation;
    [ShowInInspectorIf("usesDirectionalAnimations", true)]
    public string southAnimation;
    [ShowInInspectorIf("usesDirectionalAnimations", true)]
    public string westAnimation;
    public bool usesDirectionalShadowAnimations;
    [ShowInInspectorIf("usesDirectionalShadowAnimations", true)]
    public tk2dSpriteAnimator shadowAnimator;
    [ShowInInspectorIf("usesDirectionalShadowAnimations", true)]
    public string northShadowAnimation;
    [ShowInInspectorIf("usesDirectionalShadowAnimations", true)]
    public string eastShadowAnimation;
    [ShowInInspectorIf("usesDirectionalShadowAnimations", true)]
    public string southShadowAnimation;
    [ShowInInspectorIf("usesDirectionalShadowAnimations", true)]
    public string westShadowAnimation;
    public bool pauseAnimationOnRest = true;
    [Header("Sawblade Options")]
    public Transform Sparks_A;
    public Transform Sparks_B;
    private Vector3 m_sparksAStartPos;
    private Vector3 m_sparksBStartPos;
    private bool m_IsSoundPlaying;
    private RoomHandler m_parentRoom;
    protected Vector2 m_cachedSparkVelocity;
    private bool m_isBloodied;
    private bool m_isAnimating;
    private PathMover m_pathMover;
    private tk2dSpriteAnimationClip m_startingAnimation;
    private tk2dSpriteAnimationClip m_startingShadowAnimation;

    public override void Start()
    {
      base.Start();
      this.m_pathMover = this.GetComponent<PathMover>();
      this.m_parentRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
      if ((bool) (UnityEngine.Object) this.specRigidbody)
      {
        this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision);
        this.specRigidbody.OnPathTargetReached += new System.Action(this.OnPathTargetReached);
        List<CollisionData> overlappingCollisions = new List<CollisionData>();
        if (PhysicsEngine.Instance.OverlapCast(this.specRigidbody, overlappingCollisions, false))
        {
          for (int index = 0; index < overlappingCollisions.Count; ++index)
          {
            SpeculativeRigidbody otherRigidbody = overlappingCollisions[index].OtherRigidbody;
            if ((bool) (UnityEngine.Object) otherRigidbody && (bool) (UnityEngine.Object) otherRigidbody.minorBreakable)
              otherRigidbody.minorBreakable.Break();
          }
        }
      }
      this.m_startingAnimation = this.spriteAnimator.CurrentClip;
      if ((bool) (UnityEngine.Object) this.shadowAnimator)
        this.m_startingShadowAnimation = this.spriteAnimator.CurrentClip;
      if (GameManager.Instance.PlayerIsNearRoom(this.m_parentRoom))
      {
        int num = (int) AkSoundEngine.PostEvent("Play_ENV_trap_active", this.gameObject);
        this.m_IsSoundPlaying = true;
      }
      this.m_isAnimating = true;
      if ((UnityEngine.Object) this.Sparks_A != (UnityEngine.Object) null)
        this.m_sparksAStartPos = this.Sparks_A.localPosition;
      if (!((UnityEngine.Object) this.Sparks_B != (UnityEngine.Object) null))
        return;
      this.m_sparksBStartPos = this.Sparks_B.localPosition;
    }

    protected void UpdateSparks()
    {
      float t1 = 5f;
      float t2 = 5f;
      if ((UnityEngine.Object) this.m_pathMover != (UnityEngine.Object) null)
      {
        t2 = Vector2.Distance(this.specRigidbody.Position.UnitPosition, this.m_pathMover.GetCurrentTargetPosition());
        t1 = Vector2.Distance(this.specRigidbody.Position.UnitPosition, this.m_pathMover.GetPreviousTargetPosition());
      }
      if (this.m_pathMover.Path.wrapMode != SerializedPath.SerializedPathWrapMode.Loop)
      {
        if (this.m_pathMover.CurrentIndex == 0 || this.m_pathMover.CurrentIndex == this.m_pathMover.Path.nodes.Count - 1)
          t2 = 1f;
        if (this.m_pathMover.PreviousIndex == 0 || this.m_pathMover.PreviousIndex == this.m_pathMover.Path.nodes.Count - 1)
          t1 = 1f;
      }
      if (this.specRigidbody.Velocity == Vector2.zero)
        return;
      if ((UnityEngine.Object) this.Sparks_A != (UnityEngine.Object) null)
      {
        Vector2 vector2 = !(this.specRigidbody.Velocity == Vector2.zero) ? this.specRigidbody.Velocity : this.m_cachedSparkVelocity;
        if (this.specRigidbody.Velocity != Vector2.zero)
          this.m_cachedSparkVelocity = vector2;
        if ((double) Mathf.Abs(vector2.x) > (double) Mathf.Abs(vector2.y))
        {
          if (!this.Sparks_A.gameObject.activeSelf)
            this.Sparks_A.gameObject.SetActive(true);
          this.Sparks_A.localPosition = (double) vector2.x <= 0.0 ? this.m_sparksBStartPos : this.m_sparksAStartPos;
          this.Sparks_A.localPosition = Vector3.Lerp((double) this.m_pathMover.GetPreviousSourcePosition().y <= (double) this.specRigidbody.Position.UnitPosition.y ? new Vector3(0.0f, this.m_sparksAStartPos.x, 0.0f) : new Vector3(0.0f, this.m_sparksBStartPos.x, 0.0f), this.Sparks_A.localPosition, t1);
        }
        else
        {
          if (!this.Sparks_A.gameObject.activeSelf)
            this.Sparks_A.gameObject.SetActive(true);
          this.Sparks_A.localPosition = (double) vector2.y <= 0.0 ? new Vector3(0.0f, this.m_sparksBStartPos.x, 0.0f) : new Vector3(0.0f, this.m_sparksAStartPos.x, 0.0f);
          this.Sparks_A.localPosition = Vector3.Lerp((double) this.m_pathMover.GetPreviousSourcePosition().x >= (double) this.specRigidbody.Position.UnitPosition.x ? this.m_sparksBStartPos : this.m_sparksAStartPos, this.Sparks_A.localPosition, t1);
        }
      }
      if (!((UnityEngine.Object) this.Sparks_B != (UnityEngine.Object) null))
        return;
      Vector2 vector2_1 = !(this.specRigidbody.Velocity == Vector2.zero) ? this.specRigidbody.Velocity : this.m_cachedSparkVelocity;
      if (this.specRigidbody.Velocity != Vector2.zero)
        this.m_cachedSparkVelocity = vector2_1;
      if ((double) Mathf.Abs(vector2_1.x) > (double) Mathf.Abs(vector2_1.y))
      {
        if (!this.Sparks_B.gameObject.activeSelf)
          this.Sparks_B.gameObject.SetActive(true);
        this.Sparks_B.localPosition = (double) vector2_1.x <= 0.0 ? this.m_sparksAStartPos : this.m_sparksBStartPos;
        this.Sparks_B.localPosition = Vector3.Lerp((double) this.m_pathMover.GetNextTargetPosition().y <= (double) this.specRigidbody.Position.UnitPosition.y ? new Vector3(0.0f, this.m_sparksAStartPos.x, 0.0f) : new Vector3(0.0f, this.m_sparksBStartPos.x, 0.0f), this.Sparks_B.localPosition, t2);
      }
      else
      {
        if (!this.Sparks_B.gameObject.activeSelf)
          this.Sparks_B.gameObject.SetActive(true);
        this.Sparks_B.localPosition = (double) vector2_1.y <= 0.0 ? new Vector3(0.0f, this.m_sparksAStartPos.x, 0.0f) : new Vector3(0.0f, this.m_sparksBStartPos.x, 0.0f);
        this.Sparks_B.localPosition = Vector3.Lerp((double) this.m_pathMover.GetNextTargetPosition().x <= (double) this.specRigidbody.Position.UnitPosition.x ? this.m_sparksAStartPos : this.m_sparksBStartPos, this.Sparks_B.localPosition, t2);
      }
    }

    public virtual void Update()
    {
      if (this.m_IsSoundPlaying)
      {
        if (!GameManager.Instance.PlayerIsNearRoom(this.m_parentRoom) || this.pauseAnimationOnRest && this.specRigidbody.Velocity == Vector2.zero)
        {
          this.m_IsSoundPlaying = false;
          int num = (int) AkSoundEngine.PostEvent("Stop_ENV_trap_active", this.gameObject);
        }
      }
      else if (GameManager.Instance.PlayerIsNearRoom(this.m_parentRoom) && (!this.pauseAnimationOnRest || !(this.specRigidbody.Velocity == Vector2.zero)))
      {
        this.m_IsSoundPlaying = true;
        int num = (int) AkSoundEngine.PostEvent("Play_ENV_trap_active", this.gameObject);
      }
      this.UpdateSparks();
      if (this.specRigidbody.Velocity == Vector2.zero)
      {
        if (!this.pauseAnimationOnRest || !this.m_isAnimating)
          return;
        this.spriteAnimator.Stop();
        if ((bool) (UnityEngine.Object) this.shadowAnimator)
          this.shadowAnimator.Stop();
        this.m_isAnimating = false;
      }
      else
      {
        this.m_isAnimating = true;
        if ((UnityEngine.Object) this.spriteAnimator != (UnityEngine.Object) null)
          this.spriteAnimator.Sprite.UpdateZDepth();
        if (this.usesDirectionalAnimations)
        {
          IntVector2 intMajorAxis = BraveUtility.GetIntMajorAxis(this.specRigidbody.Velocity);
          if (intMajorAxis == IntVector2.North)
            this.spriteAnimator.Play(this.northAnimation);
          else if (intMajorAxis == IntVector2.East)
            this.spriteAnimator.Play(this.eastAnimation);
          else if (intMajorAxis == IntVector2.South)
            this.spriteAnimator.Play(this.southAnimation);
          else if (intMajorAxis == IntVector2.West)
            this.spriteAnimator.Play(this.westAnimation);
          if (!this.usesDirectionalShadowAnimations)
            return;
          if (intMajorAxis == IntVector2.North)
            this.shadowAnimator.Play(this.northShadowAnimation);
          else if (intMajorAxis == IntVector2.East)
            this.shadowAnimator.Play(this.eastShadowAnimation);
          else if (intMajorAxis == IntVector2.South)
          {
            this.shadowAnimator.Play(this.southShadowAnimation);
          }
          else
          {
            if (!(intMajorAxis == IntVector2.West))
              return;
            this.shadowAnimator.Play(this.westShadowAnimation);
          }
        }
        else
        {
          if (this.m_startingAnimation != null)
            this.spriteAnimator.Play(this.m_startingAnimation);
          if (!(bool) (UnityEngine.Object) this.shadowAnimator || this.m_startingShadowAnimation == null)
            return;
          this.shadowAnimator.Play(this.m_startingShadowAnimation);
        }
      }
    }

    protected override void OnDestroy() => base.OnDestroy();

    public override GameObject InstantiateObject(
      RoomHandler targetRoom,
      IntVector2 loc,
      bool deferConfiguration = false)
    {
      this.m_markCellOccupied = false;
      return base.InstantiateObject(targetRoom, loc, deferConfiguration);
    }

    private void OnTriggerCollision(
      SpeculativeRigidbody rigidbody,
      SpeculativeRigidbody source,
      CollisionData collisionData)
    {
      if (rigidbody.gameActor is PlayerController)
      {
        if ((rigidbody.gameActor as PlayerController).IsEthereal)
          return;
        this.Damage(rigidbody, this.damage, this.knockbackStrength);
      }
      else if (this.hitsEnemies && (bool) (UnityEngine.Object) rigidbody.aiActor)
      {
        this.Damage(rigidbody, this.enemyDamage, this.enemyKnockbackStrength);
      }
      else
      {
        Chest component = rigidbody.GetComponent<Chest>();
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || component.IsBroken || component.TemporarilyUnopenable)
          return;
        component.majorBreakable.Break(source.Velocity);
      }
    }

    private void OnPathTargetReached()
    {
      if (!this.m_IsSoundPlaying)
        return;
      int num = (int) AkSoundEngine.PostEvent("Play_ENV_trap_turn", this.gameObject);
    }

    protected virtual void Damage(
      SpeculativeRigidbody rigidbody,
      float damage,
      float knockbackStrength)
    {
      if ((double) damage <= 0.0)
        return;
      if ((double) knockbackStrength > 0.0 && (bool) (UnityEngine.Object) rigidbody.knockbackDoer)
        rigidbody.knockbackDoer.ApplySourcedKnockback(rigidbody.UnitCenter - this.specRigidbody.UnitCenter, knockbackStrength, this.gameObject);
      if (!rigidbody.healthHaver.IsVulnerable && !this.ignoreInvulnerabilityFrames)
        return;
      HealthHaver healthHaver = rigidbody.healthHaver;
      float num1 = damage;
      Vector2 zero = Vector2.zero;
      string enemiesString = StringTableManager.GetEnemiesString("#TRAP");
      bool invulnerabilityFrames = this.ignoreInvulnerabilityFrames;
      double damage1 = (double) num1;
      Vector2 direction = zero;
      string sourceName = enemiesString;
      int num2 = invulnerabilityFrames ? 1 : 0;
      healthHaver.ApplyDamage((float) damage1, direction, sourceName, ignoreInvulnerabilityFrames: num2 != 0);
      if (!this.m_isBloodied && this.usesBloodyAnimation && !string.IsNullOrEmpty(this.bloodyAnimation))
        this.spriteAnimator.Play(this.bloodyAnimation);
      this.m_isBloodied = true;
    }
  }

