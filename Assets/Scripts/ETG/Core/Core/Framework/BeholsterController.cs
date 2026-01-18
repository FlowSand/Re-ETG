// Decompiled with JetBrains decompiler
// Type: BeholsterController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BeholsterController : BraveBehaviour
  {
    [Header("Eye Sprites")]
    public tk2dSprite eyeSprite;
    public Transform pupilTransform;
    public tk2dSprite pupilSprite;
    [Header("Beam Data")]
    public Transform beamTransform;
    public VFXPool chargeUpVfx;
    public VFXPool chargeDownVfx;
    public ProjectileModule beamModule;
    [Header("Beam Firing Point")]
    public Vector2 firingEllipseCenter;
    public float firingEllipseA;
    public float firingEllipseB;
    public float GlitchWorldHealthModifier = 1f;
    private BeholsterTentacleController[] m_tentacles;
    private bool m_laserActive;
    private bool m_firingLaser;
    private float m_laserAngle;
    private BasicBeamController m_laserBeam;

    public bool LaserActive => this.m_laserActive;

    public bool FiringLaser => this.m_firingLaser;

    public float LaserAngle
    {
      get => this.m_laserAngle;
      set
      {
        this.m_laserAngle = value;
        if (!this.m_firingLaser)
          return;
        this.aiAnimator.FacingDirection = value;
      }
    }

    public BasicBeamController LaserBeam => this.m_laserBeam;

    public Vector2 LaserFiringCenter => this.transform.position.XY() + this.firingEllipseCenter;

    public void Start()
    {
      if (this.aiActor.ParentRoom != null && this.aiActor.ParentRoom.area.PrototypeRoomName == "DoubleBeholsterRoom01")
      {
        GameManager.Instance.Dungeon.IsGlitchDungeon = true;
        this.healthHaver.SetHealthMaximum(this.healthHaver.GetMaxHealth() * this.GlitchWorldHealthModifier);
        this.healthHaver.ForceSetCurrentHealth(this.healthHaver.GetMaxHealth());
        this.sprite.usesOverrideMaterial = true;
        this.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
      }
      this.m_tentacles = this.GetComponentsInChildren<BeholsterTentacleController>();
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.2f);
      if ((bool) (Object) this.eyeSprite)
      {
        this.healthHaver.RegisterBodySprite((tk2dBaseSprite) this.eyeSprite);
        this.eyeSprite.usesOverrideMaterial = false;
      }
      if ((bool) (Object) this.pupilSprite)
        this.healthHaver.RegisterBodySprite((tk2dBaseSprite) this.pupilSprite);
      this.aiAnimator.FacingDirection = -90f;
      this.aiAnimator.Update();
      this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
    }

    public void Update()
    {
      float facingDirection = this.aiAnimator.FacingDirection;
      if (this.spriteAnimator.CurrentClip != null && this.spriteAnimator.CurrentClip.name.Contains("idle"))
      {
        if ((double) facingDirection > 155.0 || (double) facingDirection < 25.0)
        {
          if ((double) facingDirection <= -60.0 && (double) facingDirection >= -120.0)
          {
            float num = Mathf.InverseLerp(-120f, -60f, facingDirection);
            this.pupilSprite.transform.localPosition = new Vector3(PhysicsEngine.PixelToUnit((int) ((double) num * 11.0) - 5), (double) Mathf.Abs(num - 0.5f) <= 0.35 ? 0.0f : PhysicsEngine.PixelToUnit(1), this.pupilSprite.transform.localPosition.z);
          }
          else if ((double) Mathf.Abs(facingDirection) >= 90.0)
          {
            float num1 = (double) facingDirection <= 0.0 ? facingDirection : facingDirection - 360f;
            if ((double) num1 < -180.0)
            {
              this.pupilSprite.transform.localPosition = new Vector3(0.0f, 0.0f, this.pupilSprite.transform.localPosition.z);
            }
            else
            {
              float num2 = Mathf.InverseLerp(-180f, -120f, num1);
              this.pupilSprite.transform.localPosition = new Vector3(PhysicsEngine.PixelToUnit((int) ((double) num2 * 21.0)), -PhysicsEngine.PixelToUnit(Mathf.Min((int) ((double) num2 * 26.0), 7)), this.pupilSprite.transform.localPosition.z);
            }
          }
          else if ((double) facingDirection > 0.0)
          {
            this.pupilSprite.transform.localPosition = new Vector3(0.0f, 0.0f, this.pupilSprite.transform.localPosition.z);
          }
          else
          {
            float num = Mathf.InverseLerp(0.0f, -60f, facingDirection);
            this.pupilSprite.transform.localPosition = new Vector3(-PhysicsEngine.PixelToUnit((int) ((double) num * 21.0)), -PhysicsEngine.PixelToUnit(Mathf.Min((int) ((double) num * 26.0), 7)), this.pupilSprite.transform.localPosition.z);
          }
        }
      }
      else
        this.pupilSprite.transform.localPosition = new Vector3(0.0f, 0.0f, this.pupilSprite.transform.localPosition.z);
      if (!this.m_firingLaser)
        return;
      this.aiAnimator.PlayUntilCancelled("eyelaser", true);
    }

    public void LateUpdate()
    {
      int spriteIdByName = this.sprite.GetSpriteIdByName(this.GetEyeSprite(this.sprite.CurrentSprite.name));
      if (spriteIdByName > 0)
      {
        this.eyeSprite.usesOverrideMaterial = false;
        this.eyeSprite.renderer.enabled = true;
        this.eyeSprite.SetSprite(spriteIdByName);
      }
      else
        this.eyeSprite.renderer.enabled = false;
    }

    protected override void OnDestroy() => base.OnDestroy();

    public void StartFiringTentacles(BeholsterTentacleController[] tentacles = null)
    {
      if (tentacles == null)
        tentacles = this.m_tentacles;
      List<BeholsterTentacleController> tentacleControllerList = new List<BeholsterTentacleController>();
      for (int index = 0; index < tentacles.Length; ++index)
      {
        if (tentacles[index].IsReady)
          tentacleControllerList.Add(tentacles[index]);
      }
      if (tentacleControllerList.Count <= 0)
        return;
      tentacleControllerList[Random.Range(0, tentacleControllerList.Count)].StartFiring();
    }

    public void SingleFireTentacle(BeholsterTentacleController[] tentacles = null, float? angleOffset = null)
    {
      if (tentacles == null)
        tentacles = this.m_tentacles;
      List<BeholsterTentacleController> tentacleControllerList = new List<BeholsterTentacleController>();
      for (int index = 0; index < tentacles.Length; ++index)
      {
        if (tentacles[index].IsReady)
          tentacleControllerList.Add(tentacles[index]);
      }
      if (tentacleControllerList.Count <= 0)
        return;
      tentacleControllerList[Random.Range(0, tentacleControllerList.Count)].SingleFire(angleOffset);
    }

    public void StopFiringTentacles(BeholsterTentacleController[] tentacles = null)
    {
      if (tentacles == null)
        tentacles = this.m_tentacles;
      for (int index = 0; index < tentacles.Length; ++index)
        tentacles[index].CeaseAttack();
    }

    public void OnDamaged(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      if ((double) resultValue > 0.0)
        return;
      if (this.m_firingLaser)
      {
        this.chargeUpVfx.DestroyAll();
        this.chargeDownVfx.DestroyAll();
        this.StopFiringLaser();
        if ((Object) this.m_laserBeam != (Object) null)
        {
          this.m_laserBeam.DestroyBeam();
          this.m_laserBeam = (BasicBeamController) null;
        }
      }
      foreach (Component tentacle in this.m_tentacles)
      {
        foreach (Renderer componentsInChild in tentacle.GetComponentsInChildren<Renderer>())
          componentsInChild.enabled = false;
      }
    }

    public void PrechargeFiringLaser()
    {
      int num = (int) AkSoundEngine.PostEvent("Play_ENM_beholster_charging_01", this.gameObject);
      this.m_laserActive = true;
      this.aiAnimator.LockFacingDirection = true;
      this.aiAnimator.FacingDirection = (double) this.aiAnimator.FacingDirection <= 0.0 || (double) this.aiAnimator.FacingDirection >= 180.0 ? -90f : 90f;
      this.aiAnimator.PlayUntilCancelled("charge", true);
    }

    public void ChargeFiringLaser(float time)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_ENM_deathray_charge_01", this.gameObject);
      this.m_laserActive = true;
      bool flag = (double) this.aiAnimator.FacingDirection > 0.0 && (double) this.aiAnimator.FacingDirection < 180.0;
      if (flag)
        this.chargeUpVfx.SpawnAtLocalPosition(Vector3.zero, 0.0f, this.beamTransform, new Vector2?(Vector2.zero), new Vector2?(Vector2.zero), true);
      else
        this.chargeDownVfx.SpawnAtLocalPosition(Vector3.zero, 0.0f, this.beamTransform, new Vector2?(Vector2.zero), new Vector2?(Vector2.zero), true);
      foreach (SpriteAnimatorChanger componentsInChild in this.beamTransform.GetComponentsInChildren<SpriteAnimatorChanger>())
        componentsInChild.time = time / 2f;
      tk2dSprite[] componentsInChildren = this.beamTransform.GetComponentsInChildren<tk2dSprite>();
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].HeightOffGround += !flag ? 1f : -1f;
        componentsInChildren[index].UpdateZDepth();
      }
    }

    public void StartFiringLaser(float laserAngle)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_ENM_deathray_shot_01", this.gameObject);
      this.m_laserActive = true;
      this.m_firingLaser = true;
      this.LaserAngle = laserAngle;
      this.aiAnimator.LockFacingDirection = true;
      this.aiAnimator.PlayUntilCancelled("eyelaser", true);
      this.StartCoroutine(this.FireBeam(this.beamModule));
    }

    public void StopFiringLaser()
    {
      if (!this.m_firingLaser)
        return;
      int num = (int) AkSoundEngine.PostEvent("Stop_ENM_deathray_loop_01", this.gameObject);
      this.m_laserActive = false;
      this.m_firingLaser = false;
      this.aiAnimator.LockFacingDirection = false;
      this.aiAnimator.EndAnimationIf("eyelaser");
    }

    [DebuggerHidden]
    protected IEnumerator FireBeam(ProjectileModule mod)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BeholsterController__FireBeamc__Iterator0()
      {
        mod = mod,
        _this = this
      };
    }

    private string GetEyeSprite(string sprite)
    {
      int n = 2;
      if (sprite.Contains("appear") || sprite.Contains("die"))
        n = 1;
      else if (sprite.Contains("eyelaser") || sprite.Contains("idle"))
        n = 2;
      else if (sprite.Contains("charge"))
        n = 3;
      return sprite.Insert(BraveUtility.GetNthIndexOf(sprite, '_', n), "_eye");
    }
  }

