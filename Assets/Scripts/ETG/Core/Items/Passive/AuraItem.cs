// Decompiled with JetBrains decompiler
// Type: AuraItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class AuraItem : PassiveItem
  {
    public float AuraRadius = 5f;
    public CoreDamageTypes damageTypes;
    public float DamagePerSecond = 5f;
    public bool DamageFallsOffInRadius;
    public GameObject AuraVFX;
    public NumericSynergyMultiplier[] damageMultiplierSynergies;
    public NumericSynergyMultiplier[] rangeMultiplierSynergies;
    private GameObject m_extantAuraVFX;
    private Action<AIActor, float> AuraAction;
    private bool didDamageEnemies;

    private float ModifiedAuraRadius => this.AuraRadius * this.GetRangeMultiplier();

    private float ModifiedDamagePerSecond => this.DamagePerSecond * this.GetDamageMultiplier();

    protected override void Update()
    {
      base.Update();
      if (!this.m_pickedUp || !(bool) (UnityEngine.Object) this.m_owner || this.m_owner.IsStealthed || GameManager.Instance.IsLoadingLevel)
        return;
      this.DoAura();
    }

    public override DebrisObject Drop(PlayerController player)
    {
      if ((UnityEngine.Object) this.m_extantAuraVFX != (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantAuraVFX);
        this.m_extantAuraVFX = (GameObject) null;
      }
      return base.Drop(player);
    }

    protected float GetDamageMultiplier()
    {
      float damageMultiplier = 1f;
      if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
      {
        for (int index = 0; index < this.damageMultiplierSynergies.Length; ++index)
        {
          if (this.m_owner.HasActiveBonusSynergy(this.damageMultiplierSynergies[index].RequiredSynergy))
            damageMultiplier *= this.damageMultiplierSynergies[index].SynergyMultiplier;
        }
      }
      return damageMultiplier;
    }

    protected float GetRangeMultiplier()
    {
      float rangeMultiplier = 1f;
      if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
      {
        for (int index = 0; index < this.rangeMultiplierSynergies.Length; ++index)
        {
          if (this.m_owner.HasActiveBonusSynergy(this.rangeMultiplierSynergies[index].RequiredSynergy))
            rangeMultiplier *= this.rangeMultiplierSynergies[index].SynergyMultiplier;
        }
      }
      return rangeMultiplier;
    }

    protected virtual void DoAura()
    {
      if (!((UnityEngine.Object) this.m_extantAuraVFX == (UnityEngine.Object) null))
        ;
      this.didDamageEnemies = false;
      if (this.AuraAction == null)
        this.AuraAction = (Action<AIActor, float>) ((actor, dist) =>
        {
          float num = this.ModifiedDamagePerSecond * BraveTime.DeltaTime;
          if (this.DamageFallsOffInRadius)
          {
            float t = dist / this.ModifiedAuraRadius;
            num = Mathf.Lerp(num, 0.0f, t);
          }
          if ((double) num > 0.0)
            this.didDamageEnemies = true;
          actor.healthHaver.ApplyDamage(num, Vector2.zero, "Aura", this.damageTypes);
        });
      if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null && this.m_owner.CurrentRoom != null)
        this.m_owner.CurrentRoom.ApplyActionToNearbyEnemies(this.m_owner.CenterPosition, this.ModifiedAuraRadius, this.AuraAction);
      if (!this.didDamageEnemies)
        return;
      this.m_owner.DidUnstealthyAction();
    }

    protected override void OnDestroy()
    {
      if ((UnityEngine.Object) this.m_extantAuraVFX != (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantAuraVFX);
        this.m_extantAuraVFX = (GameObject) null;
      }
      base.OnDestroy();
    }
  }

