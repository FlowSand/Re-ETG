// Decompiled with JetBrains decompiler
// Type: GunderfuryController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class GunderfuryController : MonoBehaviour
{
  public static int[] expTiers = new int[6]
  {
    0,
    800,
    2100,
    3750,
    5500,
    7500
  };
  [SerializeField]
  public List<GunderfuryTier> tiers;
  public tk2dSpriteAnimator idleVFX;
  private Gun m_gun;
  private bool m_initialized;
  private PlayerController m_player;
  private int m_currentTier;
  private float m_sparkTimer;

  private void Awake()
  {
    this.m_gun = this.GetComponent<Gun>();
    this.idleVFX.gameObject.SetActive(false);
  }

  public static int GetCurrentTier()
  {
    if (!Application.isPlaying)
      return 0;
    int gunderfuryExperience = GameStatsManager.Instance.CurrentAccumulatedGunderfuryExperience;
    int currentTier = 0;
    for (int index = 0; index < GunderfuryController.expTiers.Length; ++index)
    {
      if (GunderfuryController.expTiers[index] <= gunderfuryExperience && GunderfuryController.expTiers[index] > GunderfuryController.expTiers[currentTier])
        currentTier = index;
    }
    return currentTier;
  }

  public static int GetCurrentLevel()
  {
    if (!Application.isPlaying)
      return 0;
    int currentTier = GunderfuryController.GetCurrentTier();
    int currentLevel = currentTier * 10 + 10;
    if (currentTier < 5)
    {
      float num = (float) (GameStatsManager.Instance.CurrentAccumulatedGunderfuryExperience - GunderfuryController.expTiers[currentTier]) / (float) (GunderfuryController.expTiers[currentTier + 1] - GunderfuryController.expTiers[currentTier]);
      currentLevel += Mathf.FloorToInt(num * 10f);
    }
    return currentLevel;
  }

  private void Update()
  {
    if ((bool) (UnityEngine.Object) this.m_gun.CurrentOwner && !this.m_initialized)
    {
      this.m_player = this.m_gun.CurrentOwner as PlayerController;
      this.m_player.OnKilledEnemyContext += new Action<PlayerController, HealthHaver>(this.HandleKilledEnemy);
      this.m_initialized = true;
    }
    else if (!(bool) (UnityEngine.Object) this.m_gun.CurrentOwner && this.m_initialized)
    {
      this.m_initialized = false;
      if ((bool) (UnityEngine.Object) this.m_player)
        this.m_player.OnKilledEnemyContext -= new Action<PlayerController, HealthHaver>(this.HandleKilledEnemy);
      this.m_player = (PlayerController) null;
    }
    int currentTier = GunderfuryController.GetCurrentTier();
    if (this.m_currentTier != currentTier)
    {
      this.m_currentTier = currentTier;
      this.m_gun.CeaseAttack();
      this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.tiers[currentTier].GunID) as Gun);
      if (string.IsNullOrEmpty(this.tiers[currentTier].IdleVFX))
      {
        this.idleVFX.gameObject.SetActive(false);
      }
      else
      {
        this.idleVFX.gameObject.SetActive(true);
        this.idleVFX.Play(this.tiers[currentTier].IdleVFX);
      }
    }
    if (currentTier >= 5 && GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH)
    {
      this.m_sparkTimer += BraveTime.DeltaTime * 30f;
      int num = Mathf.FloorToInt(this.m_sparkTimer);
      if (num > 0)
      {
        this.m_sparkTimer -= (float) num;
        GlobalSparksDoer.DoRadialParticleBurst(num, this.m_gun.PrimaryHandAttachPoint.position, this.m_gun.barrelOffset.position, 360f, 4f, 4f, startLifetime: new float?(0.5f), startColor: new Color?(Color.white), systemType: GlobalSparksDoer.SparksType.DARK_MAGICKS);
      }
    }
    if (!this.idleVFX.gameObject.activeSelf || !(bool) (UnityEngine.Object) this.m_gun || !(bool) (UnityEngine.Object) this.m_gun.sprite)
      return;
    this.idleVFX.sprite.FlipY = this.m_gun.sprite.FlipY;
    this.idleVFX.renderer.enabled = this.m_gun.renderer.enabled;
  }

  private void OnDestroy()
  {
    if (!(bool) (UnityEngine.Object) this.m_player)
      return;
    this.m_player.OnKilledEnemyContext -= new Action<PlayerController, HealthHaver>(this.HandleKilledEnemy);
  }

  private void HandleKilledEnemy(PlayerController sourcePlayer, HealthHaver killedEnemy)
  {
    if (GameStatsManager.Instance.CurrentAccumulatedGunderfuryExperience > 10000000)
      return;
    if (!(bool) (UnityEngine.Object) killedEnemy || (double) killedEnemy.GetMaxHealth() < 0.0)
      ++GameStatsManager.Instance.CurrentAccumulatedGunderfuryExperience;
    else
      GameStatsManager.Instance.CurrentAccumulatedGunderfuryExperience += Mathf.Max(1, Mathf.CeilToInt(killedEnemy.GetMaxHealth() / 10f));
  }
}
