using Dungeonator;
using FullInspector;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/MergoBehavior")]
public class GiantPowderSkullMergoBehavior : BasicAttackBehavior
  {
    public BulletScriptSelector shootBulletScript;
    public float darknessFadeTime = 1f;
    public float darknessAmount = 0.3f;
    public float playerLightAmount = 0.5f;
    public float fireTime = 8f;
    public float fireMainMidTime = 0.8f;
    public float fireMainDist = 16f;
    public float fireMainDistVariance = 3f;
    [InspectorCategory("Visuals")]
    public string teleportOutAnim;
    [InspectorCategory("Visuals")]
    public string teleportInAnim;
    [InspectorCategory("Visuals")]
    public ParticleSystem roomParticleSystem;
    private GiantPowderSkullMergoBehavior.State m_state;
    private tk2dBaseSprite m_shadowSprite;
    private ParticleSystem m_mainParticleSystem;
    private ParticleSystem m_trailParticleSystem;
    private Vector2 m_roomMin;
    private Vector2 m_roomMax;
    private float m_timer;
    private float m_mainShotTimer;
    private BulletScriptSource m_shootBulletSource;

    public override void Start()
    {
      base.Start();
      PowderSkullParticleController componentInChildren = this.m_aiActor.GetComponentInChildren<PowderSkullParticleController>();
      this.m_mainParticleSystem = componentInChildren.GetComponent<ParticleSystem>();
      this.m_trailParticleSystem = componentInChildren.RotationChild.GetComponentInChildren<ParticleSystem>();
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_timer);
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      this.m_shadowSprite = (tk2dBaseSprite) this.m_aiActor.ShadowObject.GetComponent<tk2dSprite>();
      this.m_state = GiantPowderSkullMergoBehavior.State.Fading;
      this.m_aiActor.healthHaver.minimumHealth = 1f;
      this.m_timer = this.darknessFadeTime;
      this.m_aiActor.ParentRoom.BecomeTerrifyingDarkRoom(this.darknessFadeTime, this.darknessAmount, this.playerLightAmount);
      BraveUtility.EnableEmission(this.m_mainParticleSystem, false);
      BraveUtility.EnableEmission(this.m_trailParticleSystem, false);
      this.m_aiActor.ClearPath();
      this.m_aiActor.knockbackDoer.SetImmobile(true, "CrosshairBehavior");
      this.m_roomMin = this.m_aiActor.ParentRoom.area.UnitBottomLeft;
      this.m_roomMax = this.m_aiActor.ParentRoom.area.UnitTopRight;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      this.UpdateRoomParticles();
      if (this.m_state == GiantPowderSkullMergoBehavior.State.Fading)
      {
        if ((double) this.m_timer <= 0.0)
        {
          this.m_state = GiantPowderSkullMergoBehavior.State.OutAnim;
          this.m_aiAnimator.PlayUntilCancelled(this.teleportOutAnim);
          this.m_aiActor.specRigidbody.enabled = false;
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.m_state == GiantPowderSkullMergoBehavior.State.OutAnim)
      {
        if (!this.m_aiAnimator.IsPlaying(this.teleportOutAnim))
        {
          this.m_state = GiantPowderSkullMergoBehavior.State.Firing;
          this.m_timer = this.fireTime;
          this.m_mainShotTimer = this.fireMainMidTime;
          this.m_shadowSprite.renderer.enabled = false;
          this.m_aiActor.ToggleRenderers(false);
          this.roomParticleSystem.GetComponent<Renderer>().enabled = true;
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.m_state == GiantPowderSkullMergoBehavior.State.Firing)
      {
        if ((double) this.m_timer <= 0.0)
        {
          this.m_aiActor.TeleportSomewhere(new IntVector2?(new IntVector2(5, 5)));
          this.m_state = GiantPowderSkullMergoBehavior.State.Unfading;
          this.m_timer = this.darknessFadeTime;
          this.m_aiActor.ParentRoom.EndTerrifyingDarkRoom(goalIntensity: this.darknessAmount, lightIntensity: this.playerLightAmount);
          this.m_aiActor.ToggleRenderers(true);
          this.m_aiAnimator.PlayUntilFinished(this.teleportInAnim);
          this.m_shadowSprite.renderer.enabled = true;
          this.m_aiActor.ToggleRenderers(true);
          return ContinuousBehaviorResult.Continue;
        }
        this.m_mainShotTimer -= this.m_deltaTime;
        if ((double) this.m_mainShotTimer < 0.0)
        {
          this.ShootBulletScript();
          this.m_mainShotTimer += this.fireMainMidTime;
        }
      }
      else if (this.m_state == GiantPowderSkullMergoBehavior.State.Unfading)
      {
        if (!this.m_aiAnimator.IsPlaying(this.teleportInAnim) && !this.m_aiActor.specRigidbody.enabled)
        {
          this.m_aiActor.specRigidbody.enabled = true;
          BraveUtility.EnableEmission(this.m_mainParticleSystem, true);
          BraveUtility.EnableEmission(this.m_trailParticleSystem, true);
        }
        if ((double) this.m_timer <= 0.0 && !this.m_aiAnimator.IsPlaying(this.teleportInAnim))
          return ContinuousBehaviorResult.Finished;
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
      for (int index = allProjectiles.Count - 1; index >= 0; --index)
      {
        if (allProjectiles[index].Owner is AIActor && allProjectiles[index].name.Contains("cannon", true))
          allProjectiles[index].DieInAir();
      }
      this.m_aiActor.healthHaver.minimumHealth = 0.0f;
      this.m_aiAnimator.EndAnimationIf(this.teleportInAnim);
      this.m_aiAnimator.EndAnimationIf(this.teleportOutAnim);
      this.m_shadowSprite.renderer.enabled = true;
      this.m_aiActor.ToggleRenderers(true);
      this.m_aiActor.specRigidbody.enabled = true;
      BraveUtility.EnableEmission(this.m_mainParticleSystem, true);
      BraveUtility.EnableEmission(this.m_trailParticleSystem, true);
      this.roomParticleSystem.GetComponent<Renderer>().enabled = false;
      this.m_aiActor.ParentRoom.EndTerrifyingDarkRoom(goalIntensity: this.darknessAmount, lightIntensity: this.playerLightAmount);
      this.m_state = GiantPowderSkullMergoBehavior.State.Idle;
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override bool IsOverridable() => false;

    public override void OnActorPreDeath()
    {
      if (this.m_state == GiantPowderSkullMergoBehavior.State.Fading || this.m_state == GiantPowderSkullMergoBehavior.State.Firing)
        this.m_aiActor.ParentRoom.EndTerrifyingDarkRoom(goalIntensity: this.darknessAmount, lightIntensity: this.playerLightAmount);
      base.OnActorPreDeath();
    }

    private void ShootBulletScript()
    {
      if (!(bool) (Object) this.m_shootBulletSource)
        this.m_shootBulletSource = new GameObject("Mergo shoot point").AddComponent<BulletScriptSource>();
      this.m_shootBulletSource.transform.position = (Vector3) this.RandomShootPoint();
      this.m_shootBulletSource.BulletManager = this.m_aiActor.bulletBank;
      this.m_shootBulletSource.BulletScript = this.shootBulletScript;
      this.m_shootBulletSource.Initialize();
    }

    private Vector2 RandomShootPoint()
    {
      Vector2 center = this.m_aiActor.ParentRoom.area.Center;
      if ((bool) (Object) this.m_aiActor.TargetRigidbody)
        this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
      float magnitude = this.fireMainDist + Random.Range(-this.fireMainDistVariance, this.fireMainDistVariance);
      List<Vector2> list = new List<Vector2>();
      DungeonData data = GameManager.Instance.Dungeon.data;
      for (int index = 0; index < 36; ++index)
      {
        Vector2 vector2 = center + BraveMathCollege.DegreesToVector((float) (index * 10), magnitude);
        if (!data.isWall((int) vector2.x, (int) vector2.y) && !data.isTopWall((int) vector2.x, (int) vector2.y))
          list.Add(vector2);
      }
      return BraveUtility.RandomElement<Vector2>(list);
    }

    private void UpdateRoomParticles()
    {
      if (this.m_state == GiantPowderSkullMergoBehavior.State.Idle || this.m_state == GiantPowderSkullMergoBehavior.State.Unfading)
        return;
      if (this.m_state == GiantPowderSkullMergoBehavior.State.Fading && (double) this.m_timer > (double) this.darknessFadeTime / 2.0)
      {
        float num1 = (float) ((1.0 - (double) this.m_timer / (double) this.darknessFadeTime) * 2.0);
        int num2 = Mathf.RoundToInt(200f * this.m_deltaTime);
        for (int index = 0; index < num2; ++index)
        {
          Vector3 vector3 = this.roomParticleSystem.transform.position + (Vector3) BraveMathCollege.DegreesToVector((float) Random.Range(0, 360), Random.Range((float) ((double) num1 * 15.0 - 2.0), num1 * 15f));
          vector3.z = vector3.y;
          float startLifetime = this.roomParticleSystem.startLifetime;
          this.roomParticleSystem.Emit(new ParticleSystem.EmitParams()
          {
            position = vector3,
            velocity = (Vector3) BraveMathCollege.DegreesToVector((float) Random.Range(0, 360), this.roomParticleSystem.startSpeed),
            startLifetime = startLifetime,
            startSize = this.roomParticleSystem.startSize,
            rotation = this.roomParticleSystem.startRotation,
            startColor = (Color32) this.roomParticleSystem.startColor
          }, 1);
        }
      }
      else
      {
        int num = Mathf.RoundToInt(840f * this.m_deltaTime);
        for (int index = 0; index < num; ++index)
        {
          Vector3 vector3 = (Vector3) BraveUtility.RandomVector2(this.m_roomMin, this.m_roomMax, new Vector2(0.5f, 0.5f));
          vector3.z = vector3.y;
          float startLifetime = this.roomParticleSystem.startLifetime;
          this.roomParticleSystem.Emit(new ParticleSystem.EmitParams()
          {
            position = vector3,
            velocity = (Vector3) BraveMathCollege.DegreesToVector((float) Random.Range(0, 360), this.roomParticleSystem.startSpeed),
            startLifetime = startLifetime,
            startSize = this.roomParticleSystem.startSize,
            rotation = this.roomParticleSystem.startRotation,
            startColor = (Color32) this.roomParticleSystem.startColor
          }, 1);
        }
      }
    }

    public enum State
    {
      Idle,
      Fading,
      OutAnim,
      Firing,
      Unfading,
      InAnim,
    }
  }

