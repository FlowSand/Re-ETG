// Decompiled with JetBrains decompiler
// Type: HammerOfDawnController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class HammerOfDawnController : BraveBehaviour
{
  public List<tk2dSprite> BeamSections;
  public tk2dSprite BurstSprite;
  [CheckAnimation(null)]
  public string SectionStartAnimation;
  [CheckAnimation(null)]
  public string SectionAnimation;
  [CheckAnimation(null)]
  public string SectionEndAnimation;
  [CheckAnimation(null)]
  public string CapAnimation;
  [CheckAnimation(null)]
  public string CapEndAnimation;
  public GameObject InitialImpactVFX;
  public float TrackingSpeed = 5f;
  public float InitialDamage = 50f;
  public float DamagePerSecond = 20f;
  public float OverlapRadius = 2.5f;
  public float DamageRadius = 2.5f;
  public float RotationSpeed = 60f;
  public GoopDefinition FireGoop;
  public float FireGoopRadius = 1.5f;
  private PlayerController m_owner;
  private Projectile m_projectile;
  private float m_currentTrackingSpeed;
  private DeadlyDeadlyGoopManager m_manager;
  private float InputScale = 1f;
  private static Dictionary<Projectile, HammerOfDawnController> m_projectileHammerMap = new Dictionary<Projectile, HammerOfDawnController>();
  public static List<HammerOfDawnController> m_extantHammers = new List<HammerOfDawnController>();
  private float m_lifeElapsed;
  private Vector2 m_currentAimPoint;
  private float m_particleCounter;
  private bool m_hasDisposed;

  private float ModifiedDamageRadius => this.DamageRadius * this.InputScale;

  public static bool HasExtantHammer(Projectile p)
  {
    if ((bool) (Object) p && HammerOfDawnController.m_projectileHammerMap.ContainsKey(p) && !(bool) (Object) HammerOfDawnController.m_projectileHammerMap[p])
      HammerOfDawnController.m_projectileHammerMap.Remove(p);
    return (bool) (Object) p && HammerOfDawnController.m_projectileHammerMap.ContainsKey(p) && (bool) (Object) HammerOfDawnController.m_projectileHammerMap[p];
  }

  public static void ClearPerLevelData() => HammerOfDawnController.m_projectileHammerMap.Clear();

  public void AssignOwner(PlayerController p, Projectile beam)
  {
    this.m_owner = p;
    this.m_projectile = beam;
    if ((Object) beam != (Object) null && HammerOfDawnController.m_projectileHammerMap.ContainsKey(beam))
    {
      HammerOfDawnController projectileHammer = HammerOfDawnController.m_projectileHammerMap[beam];
      int num = (int) AkSoundEngine.PostEvent("Play_WPN_dawnhammer_charge_01", this.gameObject);
      if ((bool) (Object) projectileHammer)
        projectileHammer.Dispose();
      if (HammerOfDawnController.m_projectileHammerMap.ContainsKey(beam))
        HammerOfDawnController.m_projectileHammerMap.Remove(beam);
    }
    Color? nullable = new Color?();
    if ((bool) (Object) beam)
    {
      HammerOfDawnController.m_projectileHammerMap.Add(beam, this);
      if ((bool) (Object) beam.sprite)
      {
        nullable = new Color?(beam.sprite.renderer.sharedMaterial.GetColor("_OverrideColor"));
        if ((double) nullable.Value.a > 0.10000000149011612)
          nullable = new Color?(nullable.Value.WithAlpha(1f));
      }
    }
    if ((bool) (Object) p)
      this.InputScale = p.BulletScaleModifier;
    if ((double) this.InputScale > 1.0 || nullable.HasValue)
    {
      tk2dBaseSprite[] componentsInChildren = this.GetComponentsInChildren<tk2dBaseSprite>();
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].scale = new Vector3(this.InputScale, 1f, 1f);
        if (nullable.HasValue)
        {
          componentsInChildren[index].usesOverrideMaterial = true;
          componentsInChildren[index].renderer.material.SetColor("_OverrideColor", nullable.Value);
        }
      }
    }
    HammerOfDawnController.m_extantHammers.Add(this);
  }

  private void Start()
  {
    for (int index = 0; index < this.BeamSections.Count; ++index)
    {
      tk2dSpriteAnimator spriteAnimator = this.BeamSections[index].spriteAnimator;
      if ((bool) (Object) spriteAnimator)
      {
        spriteAnimator.alwaysUpdateOffscreen = true;
        spriteAnimator.PlayForDuration(this.SectionStartAnimation, -1f, this.SectionAnimation);
        int num1 = (int) AkSoundEngine.PostEvent("Play_WPN_dawnhammer_loop_01", this.gameObject);
        int num2 = (int) AkSoundEngine.PostEvent("Play_State_Volume_Lower_01", this.gameObject);
      }
    }
    this.spriteAnimator.alwaysUpdateOffscreen = true;
    this.BurstSprite.UpdateZDepth();
    this.sprite.renderer.enabled = false;
    this.m_currentAimPoint = this.transform.position.XY();
    Exploder.DoRadialDamage(this.InitialDamage, this.transform.position, this.ModifiedDamageRadius, false, true);
    Exploder.DoRadialMajorBreakableDamage(this.InitialDamage, this.transform.position, this.ModifiedDamageRadius);
  }

  private void Update()
  {
    if (this.m_hasDisposed)
      return;
    if ((bool) (Object) this.m_owner && (bool) (Object) this.m_projectile)
    {
      this.m_lifeElapsed += BraveTime.DeltaTime;
      BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.m_owner.PlayerIDX);
      if (instanceForPlayer.IsKeyboardAndMouse())
        this.m_currentAimPoint = this.m_owner.unadjustedAimPoint.XY();
      else
        this.m_currentAimPoint += instanceForPlayer.ActiveActions.Aim.Vector.normalized * this.m_currentTrackingSpeed * BraveTime.DeltaTime;
      Vector2 currentAimPoint = this.m_currentAimPoint;
      Vector2 vector2 = this.transform.position.XY();
      if (HammerOfDawnController.m_extantHammers.Count > 1)
      {
        int count = HammerOfDawnController.m_extantHammers.Count;
        int num1 = Mathf.Clamp(HammerOfDawnController.m_extantHammers.IndexOf(this), 0, HammerOfDawnController.m_extantHammers.Count);
        float num2 = 360f / (float) count;
        float num3 = (float) ((double) UnityEngine.Time.time * (double) this.RotationSpeed % 360.0);
        currentAimPoint += (Quaternion.Euler(0.0f, 0.0f, num3 + num2 * (float) num1) * (Vector3) Vector2.up * this.OverlapRadius).XY();
      }
      this.m_currentTrackingSpeed = Mathf.Lerp(0.0f, this.TrackingSpeed, Mathf.Clamp01(this.m_lifeElapsed / 3f));
      this.transform.position = (vector2 + (currentAimPoint - vector2).normalized * this.m_currentTrackingSpeed * BraveTime.DeltaTime).ToVector3ZisY();
      this.transform.position = BraveMathCollege.ClampToBounds(this.transform.position.XY(), GameManager.Instance.MainCameraController.MinVisiblePoint + new Vector2(-15f, -15f), GameManager.Instance.MainCameraController.MaxVisiblePoint + new Vector2(15f, 15f)).ToVector3ZisY();
      Exploder.DoRadialDamage(this.DamagePerSecond * BraveTime.DeltaTime, vector2.ToVector3ZisY(), this.ModifiedDamageRadius, false, true);
      Exploder.DoRadialMajorBreakableDamage(this.DamagePerSecond * BraveTime.DeltaTime, vector2.ToVector3ZisY(), this.ModifiedDamageRadius);
      if ((bool) (Object) this.m_owner)
        this.ApplyBeamTickToEnemiesInRadius();
      if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.MEDIUM || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH)
      {
        this.m_particleCounter += BraveTime.DeltaTime * (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH ? 50f : 125f);
        if ((double) this.m_particleCounter > 1.0)
        {
          GlobalSparksDoer.DoRadialParticleBurst(Mathf.FloorToInt(this.m_particleCounter), (Vector3) this.sprite.WorldBottomLeft, (Vector3) this.sprite.WorldTopRight, 30f, 2f, 1f, systemType: GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
          this.m_particleCounter %= 1f;
        }
      }
      if ((Object) this.m_manager == (Object) null)
        this.m_manager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.FireGoop);
      this.m_manager.AddGoopCircle(vector2, this.FireGoopRadius * this.InputScale, suppressSplashes: true);
    }
    else
      this.Dispose();
    this.sprite.UpdateZDepth();
    for (int index = 0; index < this.BeamSections.Count; ++index)
      this.BeamSections[index].UpdateZDepth();
    this.BurstSprite.UpdateZDepth();
  }

  private void ApplyBeamTickToEnemiesInRadius()
  {
    Vector2 vector = this.transform.position.XY();
    float num = this.ModifiedDamageRadius * this.ModifiedDamageRadius;
    RoomHandler absoluteRoom = vector.GetAbsoluteRoom();
    if (absoluteRoom == null)
      return;
    List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    if (activeEnemies == null)
      return;
    for (int index = 0; index < activeEnemies.Count; ++index)
    {
      AIActor aiActor = activeEnemies[index];
      if ((bool) (Object) aiActor && (bool) (Object) aiActor.specRigidbody && (double) (aiActor.CenterPosition - vector).sqrMagnitude < (double) num && (bool) (Object) aiActor.healthHaver)
        this.m_owner.DoPostProcessBeamTick((BeamController) null, aiActor.specRigidbody, 1f);
    }
  }

  private void LateUpdate()
  {
    if (this.m_hasDisposed || this.BurstSprite.renderer.enabled)
      return;
    this.sprite.renderer.enabled = true;
    this.spriteAnimator.Play(this.CapAnimation);
  }

  private void Dispose()
  {
    if (this.m_hasDisposed)
      return;
    this.sprite.renderer.enabled = true;
    this.m_hasDisposed = true;
    HammerOfDawnController.m_extantHammers.Remove(this);
    if (HammerOfDawnController.m_projectileHammerMap.ContainsKey(this.m_projectile))
      HammerOfDawnController.m_projectileHammerMap.Remove(this.m_projectile);
    this.m_owner = (PlayerController) null;
    this.m_projectile = (Projectile) null;
    ParticleSystem componentInChildren = this.GetComponentInChildren<ParticleSystem>();
    if ((bool) (Object) componentInChildren)
      BraveUtility.EnableEmission(componentInChildren, false);
    for (int index = 0; index < this.BeamSections.Count; ++index)
      this.BeamSections[index].spriteAnimator.Play(this.SectionEndAnimation);
    this.spriteAnimator.PlayAndDestroyObject(this.CapEndAnimation);
    Object.Destroy((Object) this.gameObject, 1f);
    int num1 = (int) AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", this.gameObject);
    int num2 = (int) AkSoundEngine.PostEvent("Stop_State_Volume_Lower_01", this.gameObject);
  }
}
