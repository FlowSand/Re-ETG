// Decompiled with JetBrains decompiler
// Type: ShelletonRespawnController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class ShelletonRespawnController : BraveBehaviour
    {
      public tk2dBaseSprite headSprite;
      public float skullHealth = 50f;
      public float skullTime = 8f;
      public float minDistFromPlayer = 4f;
      public string deathAnim;
      public string preRegenAnim;
      public string regenAnim;
      public string regenFromNothingAnim;
      [Header("Shell Sucking")]
      public float radius = 15f;
      public float gravityForce = 50f;
      public float destroyRadius = 0.2f;
      private float m_cachedStartingHealth;
      private float m_radiusSquared;
      private bool m_shouldShellSuck;
      private int m_numRegenerations;
      private int m_cachedHeadDefaultSpriteId;
      private ShelletonRespawnController.State m_state;

      public void Start()
      {
        this.m_cachedStartingHealth = this.healthHaver.GetMaxHealth();
        this.m_radiusSquared = this.radius * this.radius;
        this.healthHaver.minimumHealth = 1f;
        this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
        this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
        this.aiActor.CustomPitDeathHandling += new AIActor.CustomPitHandlingDelegate(this.CustomPitDeathHandling);
        this.m_cachedHeadDefaultSpriteId = this.headSprite.spriteId;
      }

      public void Update()
      {
        if (this.aiActor.IsFalling && this.behaviorSpeculator.enabled)
          this.behaviorSpeculator.InterruptAndDisable();
        if (!this.m_shouldShellSuck)
          return;
        for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
          this.AdjustDebrisVelocity(StaticReferenceManager.AllDebris[index]);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnDamaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory,
        Vector2 damageDirection)
      {
        if (this.m_state != ShelletonRespawnController.State.Normal || (double) resultValue != 1.0)
          return;
        this.StartCoroutine(this.RegenerationCR());
      }

      private void AnimationEventTriggered(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frame)
      {
        if (!(clip.GetFrame(frame).eventInfo == "shell_suck"))
          return;
        this.m_shouldShellSuck = true;
      }

      private void CustomPitDeathHandling(AIActor actor, ref bool suppressDeath)
      {
        if (this.m_state == ShelletonRespawnController.State.SkullRegeneration)
        {
          this.healthHaver.minimumHealth = 0.0f;
          this.healthHaver.IsVulnerable = true;
        }
        else
        {
          suppressDeath = true;
          this.Reposition();
          foreach (UnityEngine.Object componentsInChild in this.GetComponentsInChildren<TileSpriteClipper>(true))
            UnityEngine.Object.Destroy(componentsInChild);
          this.headSprite.SetSprite(this.m_cachedHeadDefaultSpriteId);
          this.aiActor.RecoverFromFall();
          this.StartCoroutine(this.RegenerateFromNothingCR());
        }
      }

      [DebuggerHidden]
      private IEnumerator RegenerationCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShelletonRespawnController.\u003CRegenerationCR\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator RegenerateFromNothingCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShelletonRespawnController.\u003CRegenerateFromNothingCR\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }

      private bool AdjustDebrisVelocity(DebrisObject debris)
      {
        if (debris.IsPickupObject || (UnityEngine.Object) debris.GetComponent<BlackHoleDoer>() != (UnityEngine.Object) null || !debris.name.Contains("shell", true))
          return false;
        float f = Vector2.SqrMagnitude(debris.sprite.WorldCenter - this.specRigidbody.UnitCenter);
        if ((double) f > (double) this.m_radiusSquared)
          return false;
        float currentDistance = Mathf.Sqrt(f);
        if ((double) currentDistance < (double) this.destroyRadius)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) debris.gameObject);
          return true;
        }
        Vector2 accelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, currentDistance, this.gravityForce);
        float num = Mathf.Clamp(BraveTime.DeltaTime, 0.0f, 0.02f);
        if (debris.HasBeenTriggered)
          debris.ApplyVelocity(accelerationForRigidbody * num);
        else if ((double) currentDistance < (double) this.radius / 2.0)
          debris.Trigger((Vector3) (accelerationForRigidbody * num), 0.5f);
        return true;
      }

      private Vector2 GetFrameAccelerationForRigidbody(
        Vector2 unitCenter,
        float currentDistance,
        float g)
      {
        float num1 = Mathf.Clamp01((float) (1.0 - (double) currentDistance / (double) this.radius));
        float num2 = g * num1 * num1;
        return (this.specRigidbody.UnitCenter - unitCenter).normalized * num2;
      }

      private void Reposition()
      {
        Vector2 worldpoint1 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay);
        Vector2 worldpoint2 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay);
        IntVector2 bottomLeft = worldpoint1.ToIntVector2(VectorConversions.Ceil);
        IntVector2 topRight = worldpoint2.ToIntVector2(VectorConversions.Floor) - IntVector2.One;
        PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
        Vector2 playerLowerLeft = bestActivePlayer.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
        Vector2 playerUpperRight = bestActivePlayer.specRigidbody.HitboxPixelCollider.UnitTopRight;
        bool hasOtherPlayer = false;
        Vector2 otherPlayerLowerLeft = Vector2.zero;
        Vector2 otherPlayerUpperRight = Vector2.zero;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        {
          PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(bestActivePlayer);
          if ((bool) (UnityEngine.Object) otherPlayer && (bool) (UnityEngine.Object) otherPlayer.healthHaver && otherPlayer.healthHaver.IsAlive)
          {
            hasOtherPlayer = true;
            otherPlayerLowerLeft = otherPlayer.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
            otherPlayerUpperRight = otherPlayer.specRigidbody.HitboxPixelCollider.UnitTopRight;
          }
        }
        CellValidator cellValidator = (CellValidator) (c =>
        {
          for (int index1 = 0; index1 < this.aiActor.Clearance.x; ++index1)
          {
            for (int index2 = 0; index2 < this.aiActor.Clearance.y; ++index2)
            {
              if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2))
                return false;
            }
          }
          PixelCollider hitboxPixelCollider = this.aiActor.specRigidbody.HitboxPixelCollider;
          Vector2 aMin = new Vector2((float) c.x + (float) (0.5 * ((double) this.aiActor.Clearance.x - (double) hitboxPixelCollider.UnitWidth)), (float) c.y);
          Vector2 aMax = aMin + hitboxPixelCollider.UnitDimensions;
          return (double) BraveMathCollege.AABBDistanceSquared(aMin, aMax, playerLowerLeft, playerUpperRight) >= (double) this.minDistFromPlayer && (!hasOtherPlayer || (double) BraveMathCollege.AABBDistanceSquared(aMin, aMax, otherPlayerLowerLeft, otherPlayerUpperRight) >= (double) this.minDistFromPlayer) && c.x >= bottomLeft.x && c.y >= bottomLeft.y && c.x + this.aiActor.Clearance.x - 1 <= topRight.x && c.y + this.aiActor.Clearance.y - 1 <= topRight.y;
        });
        Vector2 vector2 = this.aiActor.specRigidbody.UnitCenter - this.aiActor.transform.position.XY();
        IntVector2? randomAvailableCell = this.aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(this.aiActor.Clearance), new CellTypes?(this.aiActor.PathableTiles), cellValidator: cellValidator);
        if (randomAvailableCell.HasValue)
        {
          this.aiActor.transform.position = (Vector3) (Pathfinder.GetClearanceOffset(randomAvailableCell.Value, this.aiActor.Clearance) - vector2);
          this.aiActor.specRigidbody.Reinitialize();
        }
        else
          UnityEngine.Debug.LogWarning((object) "TELEPORT FAILED!", (UnityEngine.Object) this.aiActor);
      }

      private enum State
      {
        Normal,
        SkullRegeneration,
      }
    }

}
