// Decompiled with JetBrains decompiler
// Type: CuccoController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class CuccoController : CompanionController
  {
    public int HitsRequired = 5;
    public float HitDecayTime = 5f;
    public int NumToSpawn = 20;
    public float SpawnDuration = 5f;
    public float InternalCooldown;
    public GameObject MicroCuccoPrefab;
    private float m_elapsed;
    private int m_numRecentHits;
    private float m_internalCooldown;

    private void Start()
    {
      this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.HandleDamaged);
    }

    public override void Update()
    {
      base.Update();
      this.m_elapsed += BraveTime.DeltaTime;
      this.m_internalCooldown = Mathf.Max(0.0f, this.m_internalCooldown - BraveTime.DeltaTime);
      if ((double) this.m_elapsed <= (double) this.HitDecayTime)
        return;
      if (this.m_numRecentHits > 0)
        --this.m_numRecentHits;
      this.m_elapsed -= this.HitDecayTime;
    }

    private void HandleDamaged(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      this.healthHaver.FullHeal();
      if ((double) this.m_internalCooldown > 0.0)
        return;
      int num = (int) AkSoundEngine.PostEvent("Play_PET_chicken_cluck_01", this.gameObject);
      ++this.m_numRecentHits;
      if (PassiveItem.IsFlagSetAtAll(typeof (BattleStandardItem)) || (bool) (Object) this.m_owner && (bool) (Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.IsLuteCompanionBuff)
        ++this.m_numRecentHits;
      if (this.m_numRecentHits < this.HitsRequired)
        return;
      this.StartCoroutine(this.HandleAggro());
    }

    [DebuggerHidden]
    private IEnumerator HandleAggro()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CuccoController__HandleAggroc__Iterator0()
      {
        _this = this
      };
    }
  }

