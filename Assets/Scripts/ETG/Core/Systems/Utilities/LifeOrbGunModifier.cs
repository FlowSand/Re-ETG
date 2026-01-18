// Decompiled with JetBrains decompiler
// Type: LifeOrbGunModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class LifeOrbGunModifier : MonoBehaviour
  {
    public float damageFraction = 1f;
    public GameObject OverheadVFX;
    public GameObject OnKilledEnemyVFX;
    public GameObject OnBurstGunVFX;
    public GameObject OnBurstDamageVFX;
    private GameObject m_overheadVFXInstance;
    private Gun m_gun;
    private bool m_connected;
    private PlayerController m_lastOwner;
    private HealthHaver m_lastTargetDamaged;
    private float m_totalDamageDealtToLastTarget;
    private float m_storedSoulDamage;
    private bool m_isDealingBurstDamage;

    private void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReloadPressed);
    }

    private void HandleReloadPressed(PlayerController owner, Gun source, bool reloadSomething)
    {
      if ((double) this.m_storedSoulDamage <= 0.0)
        return;
      if ((bool) (UnityEngine.Object) this.OnBurstGunVFX)
        SpawnManager.SpawnVFX(this.OnBurstGunVFX, owner.CurrentGun.barrelOffset.position, Quaternion.identity);
      this.m_isDealingBurstDamage = true;
      owner.CurrentRoom.ApplyActionToNearbyEnemies(owner.transform.position.XY(), 100f, new Action<AIActor, float>(this.ProcessEnemy));
      this.m_isDealingBurstDamage = false;
      this.ClearSoul(false);
    }

    private void OnDisable() => this.ClearSoul(true);

    private void ClearSoul(bool disabling)
    {
      this.m_storedSoulDamage = 0.0f;
      this.m_gun.idleAnimation = string.Empty;
      if (!disabling)
        this.m_gun.PlayIdleAnimation();
      if (!(bool) (UnityEngine.Object) this.m_overheadVFXInstance)
        return;
      SpawnManager.Despawn(this.m_overheadVFXInstance.gameObject);
      this.m_overheadVFXInstance = (GameObject) null;
    }

    private void ProcessEnemy(AIActor a, float distance)
    {
      if (!(bool) (UnityEngine.Object) a || !a.IsNormalEnemy || !(bool) (UnityEngine.Object) a.healthHaver || a.IsGone)
        return;
      if ((bool) (UnityEngine.Object) this.m_lastOwner)
        a.healthHaver.ApplyDamage(this.m_storedSoulDamage * this.damageFraction, Vector2.zero, this.m_lastOwner.ActorName);
      else
        a.healthHaver.ApplyDamage(this.m_storedSoulDamage * this.damageFraction, Vector2.zero, "projectile");
      if (!(bool) (UnityEngine.Object) this.OnBurstDamageVFX)
        return;
      a.PlayEffectOnActor(this.OnBurstDamageVFX, Vector3.zero);
    }

    private void Update()
    {
      if (!this.m_connected && (bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
      {
        this.m_connected = true;
        this.m_lastOwner = this.m_gun.CurrentOwner as PlayerController;
        this.m_lastOwner.OnDealtDamageContext += new Action<PlayerController, float, bool, HealthHaver>(this.HandlePlayerDealtDamage);
      }
      else
      {
        if (!this.m_connected || (bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
          return;
        this.m_connected = false;
        this.m_lastOwner.OnDealtDamageContext -= new Action<PlayerController, float, bool, HealthHaver>(this.HandlePlayerDealtDamage);
      }
    }

    private void HandlePlayerDealtDamage(
      PlayerController source,
      float damage,
      bool fatal,
      HealthHaver target)
    {
      if ((UnityEngine.Object) source.CurrentGun != (UnityEngine.Object) this.m_gun || this.m_isDealingBurstDamage)
        return;
      if ((UnityEngine.Object) this.m_lastTargetDamaged != (UnityEngine.Object) target)
      {
        this.m_lastTargetDamaged = target;
        this.m_totalDamageDealtToLastTarget = 0.0f;
      }
      this.m_totalDamageDealtToLastTarget += damage;
      if (!fatal)
        return;
      this.m_storedSoulDamage = this.m_totalDamageDealtToLastTarget;
      this.m_lastTargetDamaged = (HealthHaver) null;
      this.m_totalDamageDealtToLastTarget = 0.0f;
      if ((bool) (UnityEngine.Object) this.OverheadVFX && !(bool) (UnityEngine.Object) this.m_overheadVFXInstance)
      {
        this.m_overheadVFXInstance = source.PlayEffectOnActor(this.OverheadVFX, Vector3.up);
        this.m_overheadVFXInstance.transform.localPosition = this.m_overheadVFXInstance.transform.localPosition.Quantize(1f / 16f);
        this.m_gun.idleAnimation = "life_orb_full_idle";
        this.m_gun.PlayIdleAnimation();
      }
      if (!(bool) (UnityEngine.Object) this.OnKilledEnemyVFX || !(bool) (UnityEngine.Object) target || !(bool) (UnityEngine.Object) target.aiActor)
        return;
      target.aiActor.PlayEffectOnActor(this.OnKilledEnemyVFX, new Vector3(0.0f, 0.5f, 0.0f), false);
    }
  }

