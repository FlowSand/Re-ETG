// Decompiled with JetBrains decompiler
// Type: BossStatueController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class BossStatueController : BaseBehavior<FullSerializerSerializer>
    {
      private const float c_maxHeightToBeGrounded = 1.5f;
      public tk2dBaseSprite shadowSprite;
      public tk2dSpriteAnimator landVfx;
      public tk2dSpriteAnimator attackVfx;
      public Transform shootPoint;
      public List<BossStatueController.LevelData> levelData;
      public List<GameObject> transformVfx;
      public List<Transform> transformPoints;
      public float transformDelay = 0.5f;
      public float transformMidDelay = 1f;
      public string kaliTransformAnim;
      public Transform kaliExplosionTransform;
      public GameObject kaliExplosionVfx;
      public Transform kaliFireworksTransform;
      public GameObject kaliFireworkdsVfx;
      public float kaliPostTransformDelay = 1f;
      private BossStatuesController m_statuesController;
      private BulletScriptSource m_bulletScriptSource;
      private BossStatueController.StatueState m_state;
      private int m_level;
      private Vector2? m_target;
      private float m_landTimer;
      private bool m_isAttacking;
      private float m_height;
      private float m_initialVelocity;
      private float m_gravity;
      private float m_totalAirTime;
      private Vector2 m_launchGroundPosition;
      private float m_airTimer;
      private float m_maxJumpHeight;
      private Vector2 m_shadowLocalPos;
      private Vector2 m_landVfxOffset;
      private Vector2 m_attackVfxOffset;
      private SpriteAnimatorKiller m_landVfxKiller;
      private SpriteAnimatorKiller m_attackVfxKiller;
      private GameObject m_currentEyeVfx;

      public BossStatueController.LevelData CurrentLevel => this.levelData[this.m_level];

      public Vector2? Target
      {
        get => this.m_target;
        set => this.m_target = value;
      }

      public float DistancetoTarget
      {
        get
        {
          return !this.m_target.HasValue ? 0.0f : Vector2.Distance(this.m_target.Value, this.specRigidbody.UnitCenter - new Vector2(0.0f, this.m_height));
        }
      }

      public Vector2 Position => this.specRigidbody.UnitCenter;

      public Vector2 GroundPosition => this.specRigidbody.UnitCenter - new Vector2(0.0f, this.m_height);

      public bool IsKali => this.m_level >= this.levelData.Count - 1;

      public bool IsGrounded { get; set; }

      public bool IsStomping { get; set; }

      public bool IsTransforming { get; set; }

      public bool ReadyToJump => (double) this.m_landTimer <= 0.0;

      public float HangTime { get; set; }

      public List<BulletScriptSelector> QueuedBulletScript { get; set; }

      public bool SuppressShootVfx { get; set; }

      public BossStatueController.StatueState State
      {
        get => this.m_state;
        set
        {
          this.EndState(this.m_state);
          this.m_state = value;
          this.BeginState(this.m_state);
        }
      }

      protected override void Awake()
      {
        base.Awake();
        this.aiActor.BehaviorOverridesVelocity = true;
        this.IsGrounded = true;
        this.QueuedBulletScript = new List<BulletScriptSelector>();
        this.specRigidbody.HitboxPixelCollider.CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox, CollisionLayer.PlayerBlocker);
      }

      public void Start()
      {
        this.m_statuesController = this.transform.parent.GetComponent<BossStatuesController>();
        this.m_maxJumpHeight = (float) (-0.5 * ((double) this.m_statuesController.AttackHopSpeed * (double) this.m_statuesController.AttackHopSpeed)) / this.m_statuesController.AttackGravity;
        this.encounterTrackable = this.m_statuesController.encounterTrackable;
        this.m_shadowLocalPos = (Vector2) this.shadowSprite.transform.localPosition;
        this.m_landVfxOffset = this.specRigidbody.UnitCenter - this.landVfx.transform.position.XY();
        this.m_attackVfxOffset = this.specRigidbody.UnitCenter - this.attackVfx.transform.position.XY();
        this.m_landVfxKiller = this.landVfx.GetComponent<SpriteAnimatorKiller>();
        this.m_attackVfxKiller = this.attackVfx.GetComponent<SpriteAnimatorKiller>();
        this.landVfx.transform.parent = SpawnManager.Instance.VFX;
        this.attackVfx.transform.parent = SpawnManager.Instance.VFX;
        this.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.WallMovementResctrictor);
        this.bulletBank.CollidesWithEnemies = false;
        this.gameActor.PreventAutoAimVelocity = true;
      }

      public void Update()
      {
        if (!this.m_target.HasValue)
          return;
        if ((bool) (UnityEngine.Object) this.bulletBank)
        {
          PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint(this.m_target.Value);
          if ((bool) (UnityEngine.Object) playerClosestToPoint)
            this.bulletBank.FixedPlayerPosition = new Vector2?(playerClosestToPoint.specRigidbody.GetUnitCenter(ColliderType.HitBox));
        }
        this.specRigidbody.PixelColliders[0].Enabled = (double) this.m_height < 1.5;
        if (this.IsGrounded)
        {
          if ((double) this.m_landTimer > 0.0)
          {
            this.m_landTimer = Mathf.Max(0.0f, this.m_landTimer - BraveTime.DeltaTime);
            this.aiActor.BehaviorVelocity = Vector2.zero;
            return;
          }
          if (this.m_state == BossStatueController.StatueState.StandStill)
            return;
          if (this.m_state == BossStatueController.StatueState.WaitForAttack)
          {
            if (this.QueuedBulletScript.Count == 0)
              return;
            if (this.m_state == BossStatueController.StatueState.WaitForAttack)
              this.m_state = BossStatueController.StatueState.HopToTarget;
          }
          if (this.QueuedBulletScript.Count > 0)
          {
            this.m_initialVelocity = this.m_statuesController.AttackHopSpeed;
            this.m_gravity = this.m_statuesController.AttackGravity;
            this.m_totalAirTime = this.m_statuesController.attackHopTime;
            this.m_isAttacking = true;
          }
          else
          {
            this.m_initialVelocity = this.m_statuesController.MoveHopSpeed;
            this.m_gravity = this.m_statuesController.MoveGravity;
            this.m_totalAirTime = this.m_statuesController.moveHopTime;
          }
          this.IsGrounded = false;
          this.m_airTimer = 0.0f;
          this.m_launchGroundPosition = this.GroundPosition;
          int num = (int) AkSoundEngine.PostEvent("Play_ENM_statue_jump_01", this.gameObject);
        }
        this.m_airTimer += BraveTime.DeltaTime;
        float num1 = this.m_airTimer;
        Vector2 vector2 = Vector2.MoveTowards(this.GroundPosition, this.m_target.Value, this.m_statuesController.CurrentMoveSpeed * BraveTime.DeltaTime);
        if (this.IsStomping)
        {
          float t = this.m_airTimer / (this.m_totalAirTime / 2f);
          vector2 = (double) t > 1.0 ? this.GroundPosition : Vector2.Lerp(this.m_launchGroundPosition, this.Target.Value, t);
          num1 = (double) this.m_airTimer >= (double) this.m_totalAirTime / 2.0 ? ((double) this.m_airTimer >= (double) this.m_totalAirTime / 2.0 + (double) this.HangTime ? this.m_airTimer - this.HangTime : this.m_totalAirTime / 2f) : this.m_airTimer;
        }
        this.m_height = (float) ((double) this.m_initialVelocity * (double) num1 + 0.5 * (double) this.m_gravity * (double) num1 * (double) num1);
        if ((double) this.m_height <= 0.0 && !this.IsGrounded)
        {
          this.m_height = 0.0f;
          this.landVfx.gameObject.SetActive(true);
          this.m_landVfxKiller.Restart();
          this.landVfx.transform.position = (Vector3) (vector2 - this.m_landVfxOffset);
          this.landVfx.sprite.UpdateZDepth();
          this.m_landTimer = this.m_statuesController.groundedTime;
          this.IsGrounded = true;
          if (this.m_isAttacking)
          {
            if (!this.SuppressShootVfx && this.QueuedBulletScript[0] != null && !this.QueuedBulletScript[0].IsNull)
            {
              this.attackVfx.gameObject.SetActive(true);
              this.m_attackVfxKiller.Restart();
              this.attackVfx.transform.position = (Vector3) (vector2 - this.m_attackVfxOffset);
              this.attackVfx.sprite.UpdateZDepth();
            }
            if (this.QueuedBulletScript[0] != null && !this.QueuedBulletScript[0].IsNull)
            {
              this.ShootBulletScript(this.QueuedBulletScript[0]);
              this.spriteAnimator.Play(this.CurrentLevel.fireAnim);
            }
            this.QueuedBulletScript.RemoveAt(0);
            this.m_isAttacking = false;
          }
        }
        this.shadowSprite.spriteAnimator.SetFrame(Mathf.RoundToInt((float) (this.shadowSprite.spriteAnimator.DefaultClip.frames.Length - 1) * Mathf.Clamp01(this.m_height / this.m_maxJumpHeight)));
        this.shadowSprite.transform.localPosition = (Vector3) (this.m_shadowLocalPos - new Vector2(0.0f, this.m_height));
        this.aiActor.BehaviorVelocity = (new Vector2(vector2.x, vector2.y + this.m_height) - this.specRigidbody.UnitCenter) / BraveTime.DeltaTime;
        this.sprite.HeightOffGround = this.m_height;
        this.sprite.UpdateZDepth();
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.landVfx)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.landVfx.gameObject);
        if ((bool) (UnityEngine.Object) this.attackVfx)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.attackVfx.gameObject);
        base.OnDestroy();
      }

      public void LevelUp() => this.StartCoroutine(this.LevelUpCR());

      [DebuggerHidden]
      private IEnumerator LevelUpCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossStatueController.\u003CLevelUpCR\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public void ClearQueuedAttacks()
      {
        int num = !this.m_isAttacking ? 0 : 1;
        while (this.QueuedBulletScript.Count > num)
          this.QueuedBulletScript.RemoveAt(this.QueuedBulletScript.Count - 1);
      }

      public void FakeFireVFX()
      {
        AIBulletBank.Entry bullet = this.bulletBank.GetBullet();
        for (int index = 0; index < this.bulletBank.transforms.Count; ++index)
        {
          Transform transform = this.bulletBank.transforms[index];
          bullet.MuzzleFlashEffects.SpawnAtLocalPosition(Vector3.zero, transform.localEulerAngles.z, transform);
        }
        if (!bullet.PlayAudio)
          return;
        if (!string.IsNullOrEmpty(bullet.AudioSwitch))
        {
          int num1 = (int) AkSoundEngine.SetSwitch("WPN_Guns", bullet.AudioSwitch, this.bulletBank.SoundChild);
          int num2 = (int) AkSoundEngine.PostEvent(bullet.AudioEvent, this.bulletBank.SoundChild);
        }
        else
        {
          int num = (int) AkSoundEngine.PostEvent(bullet.AudioEvent, this.gameObject);
        }
      }

      public void ForceStopBulletScript()
      {
        if (!(bool) (UnityEngine.Object) this.m_bulletScriptSource)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bulletScriptSource);
        this.m_bulletScriptSource = (BulletScriptSource) null;
      }

      private void WallMovementResctrictor(
        SpeculativeRigidbody specRigidbody,
        IntVector2 prevPixelOffset,
        IntVector2 pixelOffset,
        ref bool validLocation)
      {
        if (!validLocation)
          return;
        Func<IntVector2, bool> func = (Func<IntVector2, bool>) (pixel =>
        {
          Vector2 unitMidpoint = PhysicsEngine.PixelToUnitMidpoint(pixel);
          int x = (int) unitMidpoint.x;
          int y = (int) unitMidpoint.y;
          return !GameManager.Instance.Dungeon.data.CheckInBounds(x, y) || GameManager.Instance.Dungeon.data.isWall(x, y) || GameManager.Instance.Dungeon.data[x, y].isExitCell;
        });
        PixelCollider primaryPixelCollider = specRigidbody.PrimaryPixelCollider;
        if (primaryPixelCollider == null)
          return;
        if (func(primaryPixelCollider.LowerLeft + pixelOffset))
        {
          validLocation = false;
        }
        else
        {
          if (!func(primaryPixelCollider.UpperRight + pixelOffset))
            return;
          validLocation = false;
        }
      }

      private void BeginState(BossStatueController.StatueState state)
      {
      }

      private void EndState(BossStatueController.StatueState state)
      {
      }

      private void ShootBulletScript(BulletScriptSelector bulletScript)
      {
        if (!(bool) (UnityEngine.Object) this.m_bulletScriptSource)
          this.m_bulletScriptSource = this.shootPoint.gameObject.GetOrAddComponent<BulletScriptSource>();
        this.m_bulletScriptSource.BulletManager = this.bulletBank;
        this.m_bulletScriptSource.BulletScript = bulletScript;
        this.m_bulletScriptSource.Initialize();
      }

      public enum StatueState
      {
        HopToTarget,
        WaitForAttack,
        StandStill,
      }

      [Serializable]
      public class LevelData
      {
        public string idleSprite;
        public string idleAnim;
        public string fireAnim;
        public string deathAnim;
        public GameObject EyeTrailVFX;
      }
    }

}
