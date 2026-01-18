// Decompiled with JetBrains decompiler
// Type: StickyFrictionManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class StickyFrictionManager : TimeInvariantMonoBehaviour
  {
    public bool FrictionEnabled = true;
    [Header("Damage")]
    public StickyFrictionManager.DamageFriction enemyDamage;
    public StickyFrictionManager.DamageFriction playerDamage;
    public StickyFrictionManager.DamageFriction swordDamage;
    [Header("Death")]
    public float enemyDeathFriction = 0.075f;
    [Header("Explosions")]
    public float explosionFriction = 0.1f;
    private const float FRICTION_REDUCTION_TIME = 0.5f;
    private const float FRICTION_REDUCTION_FALLOFF = 0.5f;
    private static StickyFrictionManager m_instance;
    private List<StickyFrictionModifier> m_fricts = new List<StickyFrictionModifier>();
    private float m_currentMinFriction;

    public static StickyFrictionManager Instance
    {
      get
      {
        if ((UnityEngine.Object) StickyFrictionManager.m_instance == (UnityEngine.Object) null)
        {
          StickyFrictionManager.m_instance = (StickyFrictionManager) UnityEngine.Object.FindObjectOfType(typeof (StickyFrictionManager));
          if ((UnityEngine.Object) StickyFrictionManager.m_instance == (UnityEngine.Object) null)
            StickyFrictionManager.m_instance = new GameObject("_TimRogers").AddComponent<StickyFrictionManager>();
        }
        return StickyFrictionManager.m_instance;
      }
    }

    protected override void InvariantUpdate(float realDeltaTime)
    {
      GameManager.Instance.MainCameraController.CurrentStickyFriction = 1f;
      if (GameManager.Instance.IsPaused || !this.FrictionEnabled || this.m_fricts == null || this.m_fricts.Count <= 0)
        return;
      float multiplier = 1f;
      for (int index = this.m_fricts.Count - 1; index >= 0; --index)
      {
        this.m_fricts[index].elapsed += realDeltaTime;
        if ((double) this.m_fricts[index].elapsed >= 0.0 && (double) this.m_fricts[index].elapsed < (double) this.m_fricts[index].length)
          multiplier *= this.m_fricts[index].GetCurrentMagnitude();
        else
          this.m_fricts.RemoveAt(index);
      }
      BraveTime.SetTimeScaleMultiplier(multiplier, this.gameObject);
    }

    protected override void OnDestroy()
    {
      BraveTime.ClearMultiplier(this.gameObject);
      base.OnDestroy();
    }

    public void RegisterSwordDamageStickyFriction(float damage)
    {
      this.RegisterDamageStickyFriction(damage, this.swordDamage);
    }

    public void RegisterPlayerDamageStickyFriction(float damage)
    {
      this.RegisterDamageStickyFriction(damage, this.playerDamage);
    }

    public void RegisterOtherDamageStickyFriction(float damage)
    {
      this.RegisterDamageStickyFriction(damage, this.enemyDamage);
    }

    private void InternalRegisterFrict(StickyFrictionModifier newFrict, bool ignoreFrictReduction = false)
    {
      newFrict.magnitude = Mathf.Clamp01(Mathf.Max(newFrict.magnitude, this.m_currentMinFriction));
      this.m_fricts.Add(newFrict);
      if (ignoreFrictReduction)
        return;
      this.StartCoroutine(this.HandleAdditionalFrictReduction(newFrict));
    }

    [DebuggerHidden]
    private IEnumerator HandleAdditionalFrictReduction(StickyFrictionModifier newFrict)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new StickyFrictionManager__HandleAdditionalFrictReductionc__Iterator0()
      {
        _this = this
      };
    }

    public void RegisterExplosionStickyFriction()
    {
      this.InternalRegisterFrict(new StickyFrictionModifier(this.explosionFriction, 0.0f, false));
    }

    public void RegisterDeathStickyFriction()
    {
      this.InternalRegisterFrict(new StickyFrictionModifier(this.enemyDeathFriction, 0.0f, false));
    }

    public void RegisterCustomStickyFriction(
      float length,
      float magnitude,
      bool falloff,
      bool ignoreFrictReduction = false)
    {
      if (this.m_fricts.Count > 0)
        this.m_fricts.Clear();
      this.InternalRegisterFrict(new StickyFrictionModifier(length, magnitude, falloff), ignoreFrictReduction);
    }

    private void RegisterDamageStickyFriction(
      float damage,
      StickyFrictionManager.DamageFriction frictionData)
    {
      if (!frictionData.enabled)
        return;
      float t = Mathf.InverseLerp(frictionData.minDamage, frictionData.maxDamage, damage);
      this.InternalRegisterFrict(new StickyFrictionModifier(Mathf.Lerp(frictionData.minFriction, frictionData.maxFriction, t), 0.0f, false));
    }

    [Serializable]
    public class DamageFriction
    {
      public bool enabled;
      public float minFriction;
      public float maxFriction;
      public float minDamage;
      public float maxDamage;
    }
  }

