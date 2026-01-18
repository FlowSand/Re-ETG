// Decompiled with JetBrains decompiler
// Type: KeybulletFleeBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class KeybulletFleeBehavior : MovementBehaviorBase
  {
    private const float c_screenXBuffer = 0.0333333351f;
    private const float c_screenYBuffer = 0.0740740746f;
    public float PathInterval = 0.25f;
    public float TimeOnScreenToFlee = 1.25f;
    public float FleeMoveSpeed = 9.5f;
    public float PreDisappearTime = 1f;
    public string DisappearAnimation;
    public bool ChangeColliderOnDisappear = true;
    public float BlackPhantomMultiplier = 1f;
    public float MinGoalDistFromPlayer = 10f;
    private float m_repathTimer;
    private float m_timer;
    private float m_onScreenTime;
    private float m_awakeTime;
    private IntVector2 m_targetPos;
    private Shader m_cachedShader;
    private tk2dSprite m_shadowSprite;
    private KeybulletFleeBehavior.State m_state;
    private Vector2 m_playerPos;
    private Vector2? m_player2Pos;

    public override void Start()
    {
      base.Start();
      this.m_aiActor.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.HandleDamaged);
      this.m_aiActor.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
      this.m_aiActor.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
      this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEvent);
      this.m_aiActor.DoDustUps = false;
      this.m_aiActor.IsWorthShootingAt = true;
    }

    private void OnAnimationEvent(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frame)
    {
      if (!(clip.GetFrame(frame).eventInfo == "blackPhantomPoof") || !(bool) (UnityEngine.Object) this.m_aiActor || !this.m_aiActor.IsBlackPhantom)
        return;
      this.DoBlackPhantomPoof();
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_repathTimer);
      this.DecrementTimer(ref this.m_timer);
      this.m_awakeTime += this.m_deltaTime;
    }

    public override BehaviorResult Update()
    {
      if ((UnityEngine.Object) this.m_shadowSprite == (UnityEngine.Object) null)
        this.m_shadowSprite = this.m_aiActor.ShadowObject.GetComponent<tk2dSprite>();
      if (this.m_state == KeybulletFleeBehavior.State.Idle)
      {
        PixelCollider hitboxPixelCollider = this.m_aiActor.specRigidbody.HitboxPixelCollider;
        if ((bool) (UnityEngine.Object) GameManager.Instance.MainCameraController && hitboxPixelCollider != null && (BraveUtility.PointIsVisible(hitboxPixelCollider.UnitTopCenter, -0.0740740746f, ViewportType.Gameplay) || BraveUtility.PointIsVisible(hitboxPixelCollider.UnitBottomCenter, -0.0740740746f, ViewportType.Gameplay) || BraveUtility.PointIsVisible(hitboxPixelCollider.UnitCenterLeft, -0.0333333351f, ViewportType.Gameplay) || BraveUtility.PointIsVisible(hitboxPixelCollider.UnitCenterRight, -0.0333333351f, ViewportType.Gameplay)))
        {
          this.m_onScreenTime += this.m_deltaTime;
          this.m_aiActor.ClearPath();
        }
        if ((double) this.m_onScreenTime > (double) this.TimeOnScreenToFlee || (double) this.m_onScreenTime > 0.0 && (double) this.m_awakeTime < 1.5)
        {
          this.Flee();
          return BehaviorResult.SkipRemainingClassBehaviors;
        }
      }
      else if (this.m_state == KeybulletFleeBehavior.State.Fleeing)
      {
        if (this.m_aiActor.PathComplete)
        {
          this.m_timer = this.PreDisappearTime;
          this.m_state = KeybulletFleeBehavior.State.WaitingToDisappear;
          this.m_aiActor.SetResistance(EffectResistanceType.Freeze, 1f);
          this.m_aiActor.behaviorSpeculator.ImmuneToStun = true;
          if ((bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
            this.m_aiActor.knockbackDoer.SetImmobile(true, "My people need me");
          return BehaviorResult.SkipRemainingClassBehaviors;
        }
        if ((double) this.m_repathTimer <= 0.0)
        {
          this.m_aiActor.PathfindToPosition(this.m_targetPos.ToCenterVector2());
          this.m_repathTimer = this.PathInterval;
        }
      }
      else if (this.m_state == KeybulletFleeBehavior.State.WaitingToDisappear)
      {
        if ((double) this.m_timer <= 0.0)
        {
          if (!string.IsNullOrEmpty(this.DisappearAnimation))
          {
            if (this.m_aiActor.IsBlackPhantom)
              this.m_aiAnimator.FpsScale *= this.BlackPhantomMultiplier;
            this.m_aiAnimator.PlayUntilFinished(this.DisappearAnimation, true);
          }
          if (this.ChangeColliderOnDisappear)
          {
            List<PixelCollider> pixelColliders = this.m_aiActor.specRigidbody.PixelColliders;
            for (int index = 0; index < pixelColliders.Count; ++index)
            {
              PixelCollider pixelCollider = pixelColliders[index];
              if (pixelCollider.Enabled && pixelCollider.CollisionLayer == CollisionLayer.EnemyHitBox)
              {
                pixelCollider.Enabled = false;
                break;
              }
            }
            for (int index = pixelColliders.Count - 1; index >= 0; --index)
            {
              PixelCollider pixelCollider = pixelColliders[index];
              if (!pixelCollider.Enabled && pixelCollider.CollisionLayer == CollisionLayer.EnemyHitBox)
              {
                pixelCollider.Enabled = true;
                break;
              }
            }
          }
          if (!this.m_aiActor.IsBlackPhantom)
          {
            this.m_cachedShader = this.m_aiActor.renderer.material.shader;
            this.m_aiActor.sprite.usesOverrideMaterial = true;
            this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            this.m_aiActor.renderer.material.SetFloat("_VertexColor", 1f);
          }
          this.m_aiActor.sprite.HeightOffGround = 1f;
          this.m_aiActor.sprite.UpdateZDepth();
          this.m_state = KeybulletFleeBehavior.State.Disappearing;
        }
      }
      else if (this.m_state == KeybulletFleeBehavior.State.Disappearing)
      {
        if (!this.m_aiActor.IsBlackPhantom)
        {
          float alpha = Mathf.Clamp01(Mathf.Lerp(1.5f, 0.0f, this.m_aiAnimator.CurrentClipProgress));
          this.m_aiActor.sprite.color = this.m_aiActor.sprite.color.WithAlpha(alpha);
          if ((bool) (UnityEngine.Object) this.m_shadowSprite)
            this.m_shadowSprite.color = this.m_aiActor.sprite.color.WithAlpha(alpha);
        }
        if (!this.m_aiAnimator.IsPlaying(this.DisappearAnimation))
          this.m_aiActor.EraseFromExistence(true);
      }
      return this.m_state == KeybulletFleeBehavior.State.Idle && (double) this.m_onScreenTime <= 0.0 ? BehaviorResult.Continue : BehaviorResult.SkipRemainingClassBehaviors;
    }

    private void HandleDamaged(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      if (this.m_state != KeybulletFleeBehavior.State.Idle)
        return;
      this.Flee();
    }

    private void OnPreDeath(Vector2 obj)
    {
      this.m_aiActor.sprite.HeightOffGround = 1f;
      this.m_aiActor.sprite.UpdateZDepth();
      if (this.m_state != KeybulletFleeBehavior.State.Disappearing || this.m_aiActor.IsBlackPhantom)
        return;
      this.m_aiActor.sprite.usesOverrideMaterial = false;
      this.m_aiActor.renderer.material.shader = this.m_cachedShader;
    }

    private void OnAnimationCompleted(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
    {
      if (this.m_state != KeybulletFleeBehavior.State.Disappearing)
        return;
      this.m_aiActor.EraseFromExistence(true);
    }

    private void Flee()
    {
      this.m_aiActor.ClearPath();
      this.m_aiActor.DoDustUps = true;
      IntVector2? fleePoint = this.GetFleePoint();
      if (fleePoint.HasValue)
      {
        this.m_targetPos = fleePoint.Value;
        if ((double) this.FleeMoveSpeed > 0.0)
          this.m_aiActor.MovementSpeed = TurboModeController.MaybeModifyEnemyMovementSpeed(this.FleeMoveSpeed);
        this.m_aiActor.PathfindToPosition(this.m_targetPos.ToCenterVector2());
        this.m_repathTimer = this.PathInterval;
      }
      this.m_state = KeybulletFleeBehavior.State.Fleeing;
    }

    private IntVector2? GetFleePoint()
    {
      PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
      this.m_playerPos = bestActivePlayer.specRigidbody.UnitCenter;
      this.m_player2Pos = new Vector2?();
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(bestActivePlayer);
        if ((bool) (UnityEngine.Object) otherPlayer && (bool) (UnityEngine.Object) otherPlayer.healthHaver && otherPlayer.healthHaver.IsAlive)
          this.m_player2Pos = new Vector2?(otherPlayer.specRigidbody.UnitCenter);
      }
      FloodFillUtility.PreprocessContiguousCells(this.m_aiActor.ParentRoom, this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
      IntVector2? nullable = new IntVector2?();
      RoomHandler parentRoom = this.m_aiActor.ParentRoom;
      IntVector2? fleePoint = parentRoom.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: new Pathfinding.CellValidator(this.CellValidator));
      if (!fleePoint.HasValue)
        fleePoint = parentRoom.GetRandomWeightedAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellWeightFinder: new Func<IntVector2, float>(this.CellWeighter));
      return fleePoint;
    }

    private bool CellValidator(IntVector2 c)
    {
      if (!FloodFillUtility.WasFilled(c))
        return false;
      bool flag = false;
      DungeonData data = GameManager.Instance.Dungeon.data;
      for (int index1 = 0; index1 < this.m_aiActor.Clearance.x && !flag; ++index1)
      {
        for (int index2 = 0; index2 < this.m_aiActor.Clearance.y && !flag; ++index2)
        {
          if (data.isWall(c.x + index1 - 1, c.y + index2))
          {
            flag = true;
            break;
          }
          if (data.isWall(c.x + index1 + 1, c.y + index2))
          {
            flag = true;
            break;
          }
          if (data.isWall(c.x + index1, c.y + index2 + 1))
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        return false;
      Vector2 clearanceOffset = Pathfinder.GetClearanceOffset(c, this.m_aiActor.Clearance);
      return (double) Vector2.Distance(clearanceOffset, this.m_playerPos) >= (double) this.MinGoalDistFromPlayer && (!this.m_player2Pos.HasValue || (double) Vector2.Distance(clearanceOffset, this.m_player2Pos.Value) >= (double) this.MinGoalDistFromPlayer);
    }

    private float CellWeighter(IntVector2 c)
    {
      if (!FloodFillUtility.WasFilled(c))
        return 0.0f;
      bool flag1 = false;
      DungeonData data = GameManager.Instance.Dungeon.data;
      for (int index1 = 0; index1 < this.m_aiActor.Clearance.x && !flag1; ++index1)
      {
        for (int index2 = 0; index2 < this.m_aiActor.Clearance.y && !flag1; ++index2)
        {
          if (data.isWall(c.x + index1 - 1, c.y + index2))
          {
            flag1 = true;
            break;
          }
          if (data.isWall(c.x + index1 + 1, c.y + index2))
          {
            flag1 = true;
            break;
          }
          if (data.isWall(c.x + index1, c.y + index2 + 1))
          {
            flag1 = true;
            break;
          }
        }
      }
      bool flag2 = false;
      if (!flag1)
      {
        for (int index3 = 0; index3 < this.m_aiActor.Clearance.x && !flag1; ++index3)
        {
          for (int index4 = 0; index4 < this.m_aiActor.Clearance.y && !flag1; ++index4)
          {
            if (data.isPit(c.x + index3 - 1, c.y + index4))
            {
              flag2 = true;
              break;
            }
            if (data.isPit(c.x + index3 + 1, c.y + index4))
            {
              flag2 = true;
              break;
            }
            if (data.isPit(c.x + index3, c.y + index4 + 1))
            {
              flag2 = true;
              break;
            }
            if (data.isPit(c.x + index3, c.y + index4 - 1))
            {
              flag2 = true;
              break;
            }
          }
        }
      }
      Vector2 clearanceOffset = Pathfinder.GetClearanceOffset(c, this.m_aiActor.Clearance);
      float a = Vector2.Distance(clearanceOffset, this.m_playerPos);
      if (this.m_player2Pos.HasValue)
        a = Mathf.Min(a, Vector2.Distance(clearanceOffset, this.m_player2Pos.Value));
      return a + (!flag1 ? (!flag2 ? 0.0f : 50f) : 100f);
    }

    private void DoBlackPhantomPoof()
    {
      if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW)
        return;
      Vector3 vector3ZisY1 = this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitBottomLeft.ToVector3ZisY();
      Vector3 vector3ZisY2 = this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitTopRight.ToVector3ZisY();
      vector3ZisY1.z = vector3ZisY1.y - 6f;
      vector3ZisY2.z = vector3ZisY2.y - 6f;
      int num = (int) (50.0 * (((double) vector3ZisY2.y - (double) vector3ZisY1.y) * ((double) vector3ZisY2.x - (double) vector3ZisY1.x)));
      GlobalSparksDoer.DoRandomParticleBurst(num, vector3ZisY1, vector3ZisY2, Vector3.up / 2f, 120f, 0.2f, startLifetime: new float?(UnityEngine.Random.Range(1f, 1.65f)), systemType: GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
      GlobalSparksDoer.DoRandomParticleBurst(num, vector3ZisY1, vector3ZisY2, Vector3.up / 2f, 120f, 0.2f, startLifetime: new float?(UnityEngine.Random.Range(1f, 1.65f)), systemType: GlobalSparksDoer.SparksType.DARK_MAGICKS);
      if ((double) UnityEngine.Random.value < 0.5)
        GlobalSparksDoer.DoRandomParticleBurst(1, vector3ZisY1, vector3ZisY2.WithY(vector3ZisY1.y + 0.1f), Vector3.right / 2f, 25f, 0.2f, startLifetime: new float?(UnityEngine.Random.Range(1f, 1.65f)), systemType: GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
      else
        GlobalSparksDoer.DoRandomParticleBurst(1, vector3ZisY1, vector3ZisY2.WithY(vector3ZisY1.y + 0.1f), Vector3.left / 2f, 25f, 0.2f, startLifetime: new float?(UnityEngine.Random.Range(1f, 1.65f)), systemType: GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
    }

    private enum State
    {
      Idle,
      Fleeing,
      WaitingToDisappear,
      Disappearing,
    }
  }

