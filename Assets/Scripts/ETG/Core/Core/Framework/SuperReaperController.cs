using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class SuperReaperController : BraveBehaviour
  {
    private static SuperReaperController m_instance;
    public static bool PreventShooting;
    public BulletScriptSelector BulletScript;
    public Transform ShootPoint;
    public float ShootTimer = 3f;
    public float MinSpeed = 3f;
    public float MaxSpeed = 10f;
    public float MinSpeedDistance = 10f;
    public float MaxSpeedDistance = 50f;
    [NonSerialized]
    public Vector2 knockbackComponent;
    private PlayerController m_currentTargetPlayer;
    private BulletScriptSource m_bulletSource;
    private float m_shootTimer;
    private int c_particlesPerSecond = 50;

    public static SuperReaperController Instance => SuperReaperController.m_instance;

    private void Start()
    {
      SuperReaperController.m_instance = this;
      this.m_shootTimer = this.ShootTimer;
      this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerEntered);
      this.aiAnimator.PlayUntilCancelled("idle");
      this.aiAnimator.PlayUntilFinished("intro");
      this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
      for (int index = 0; index < EncounterDatabase.Instance.Entries.Count; ++index)
      {
        if (EncounterDatabase.Instance.Entries[index].journalData.PrimaryDisplayName == "#SREAPER_ENCNAME")
          GameStatsManager.Instance.HandleEncounteredObjectRaw(EncounterDatabase.Instance.Entries[index].myGuid);
      }
      this.m_currentTargetPlayer = GameManager.Instance.GetRandomActivePlayer();
      if (!(bool) (UnityEngine.Object) this.encounterTrackable)
        return;
      GameStatsManager.Instance.HandleEncounteredObject(this.encounterTrackable);
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      SuperReaperController.m_instance = (SuperReaperController) null;
    }

    private void HandleTriggerEntered(
      SpeculativeRigidbody targetRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody,
      CollisionData collisionData)
    {
      Projectile projectile = targetRigidbody.projectile;
      if (!(bool) (UnityEngine.Object) projectile)
        return;
      projectile.HandleKnockback(this.specRigidbody, targetRigidbody.GetComponent<PlayerController>());
    }

    private void HandleAnimationEvent(
      tk2dSpriteAnimator arg1,
      tk2dSpriteAnimationClip arg2,
      int arg3)
    {
      if (!(arg2.GetFrame(arg3).eventInfo == "fire"))
        return;
      this.SpawnProjectiles();
    }

    private void Update()
    {
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || BossKillCam.BossDeathCamRunning || GameManager.Instance.PreventPausing)
        return;
      if (TimeTubeCreditsController.IsTimeTubing)
      {
        this.gameObject.SetActive(false);
      }
      else
      {
        this.HandleMotion();
        if (!SuperReaperController.PreventShooting)
          this.HandleAttacks();
        this.UpdateBlackPhantomParticles();
      }
    }

    private void HandleAttacks()
    {
      if (this.aiAnimator.IsPlaying("intro"))
        return;
      CellData cellData = GameManager.Instance.Dungeon.data[this.ShootPoint.position.IntXY(VectorConversions.Floor)];
      if (cellData == null || cellData.type == CellType.WALL)
        return;
      this.m_shootTimer -= BraveTime.DeltaTime;
      if ((double) this.m_shootTimer > 0.0)
        return;
      this.aiAnimator.PlayUntilFinished("attack");
      this.m_shootTimer = this.ShootTimer;
    }

    private void SpawnProjectiles()
    {
      if (GameManager.Instance.PreventPausing || BossKillCam.BossDeathCamRunning || SuperReaperController.PreventShooting)
        return;
      CellData cellData = GameManager.Instance.Dungeon.data[this.ShootPoint.position.IntXY(VectorConversions.Floor)];
      if (cellData == null || cellData.type == CellType.WALL)
        return;
      if (!(bool) (UnityEngine.Object) this.m_bulletSource)
        this.m_bulletSource = this.ShootPoint.gameObject.GetOrAddComponent<BulletScriptSource>();
      this.m_bulletSource.BulletManager = this.bulletBank;
      this.m_bulletSource.BulletScript = this.BulletScript;
      this.m_bulletSource.Initialize();
    }

    private void UpdateBlackPhantomParticles()
    {
      if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW || this.aiAnimator.IsPlaying("intro"))
        return;
      Vector3 vector3ZisY1 = this.specRigidbody.UnitBottomLeft.ToVector3ZisY();
      Vector3 vector3ZisY2 = this.specRigidbody.UnitTopRight.ToVector3ZisY();
      GlobalSparksDoer.DoRandomParticleBurst(Mathf.CeilToInt(Mathf.Max(1f, (float) this.c_particlesPerSecond * (float) (((double) vector3ZisY2.y - (double) vector3ZisY1.y) * ((double) vector3ZisY2.x - (double) vector3ZisY1.x)) * BraveTime.DeltaTime)), vector3ZisY1, vector3ZisY2, Vector3.up, 120f, 0.5f, startLifetime: new float?(UnityEngine.Random.Range(1f, 1.65f)), systemType: GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
    }

    private void HandleMotion()
    {
      this.specRigidbody.Velocity = Vector2.zero;
      if (this.aiAnimator.IsPlaying("intro") || (UnityEngine.Object) this.m_currentTargetPlayer == (UnityEngine.Object) null)
        return;
      if (this.m_currentTargetPlayer.healthHaver.IsDead || this.m_currentTargetPlayer.IsGhost)
        this.m_currentTargetPlayer = GameManager.Instance.GetRandomActivePlayer();
      Vector2 vector2 = this.m_currentTargetPlayer.CenterPosition - this.specRigidbody.UnitCenter;
      float num = Mathf.Lerp(this.MinSpeed, this.MaxSpeed, (float) (((double) vector2.magnitude - (double) this.MinSpeedDistance) / ((double) this.MaxSpeedDistance - (double) this.MinSpeedDistance)));
      this.specRigidbody.Velocity = vector2.normalized * num;
      this.specRigidbody.Velocity += this.knockbackComponent;
    }
  }

