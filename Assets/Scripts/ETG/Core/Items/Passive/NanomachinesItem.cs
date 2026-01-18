// Decompiled with JetBrains decompiler
// Type: NanomachinesItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class NanomachinesItem : PassiveItem
  {
    public int initialArmorBoost = 2;
    public float DamagePerArmor = 2f;
    protected float m_receivedDamageCounter;
    [Header("Nanomachines, Son")]
    public float RageSynergyDuration = 10f;
    public Color RageFlatColor = Color.red;
    public float RageDamageMultiplier = 2f;
    public GameObject RageOverheadVFX;
    private float m_rageElapsed;
    private GameObject rageInstanceVFX;

    public override void MidGameSerialize(List<object> data)
    {
      base.MidGameSerialize(data);
      data.Add((object) this.m_receivedDamageCounter);
    }

    public override void MidGameDeserialize(List<object> data)
    {
      base.MidGameDeserialize(data);
      if (data.Count != 1)
        return;
      this.m_receivedDamageCounter = (float) data[0];
    }

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      if (!this.m_pickedUpThisRun)
        player.healthHaver.Armor += (float) this.initialArmorBoost;
      player.OnReceivedDamage += new Action<PlayerController>(this.PlayerReceivedDamage);
      base.Pickup(player);
    }

    private void PlayerReceivedDamage(PlayerController obj)
    {
      this.m_receivedDamageCounter += 0.5f;
      float num = 0.0f;
      if (this.Owner.HasActiveBonusSynergy(CustomSynergyType.NANOMACHINES_SON))
        num = 0.5f;
      if ((double) this.m_receivedDamageCounter < (double) this.DamagePerArmor - (double) num)
        return;
      this.m_receivedDamageCounter = 0.0f;
      ++this.m_owner.healthHaver.Armor;
      this.HandleRageEffect();
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<NanomachinesItem>().m_pickedUpThisRun = true;
      player.OnReceivedDamage -= new Action<PlayerController>(this.PlayerReceivedDamage);
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (UnityEngine.Object) this.m_owner)
        return;
      this.m_owner.OnReceivedDamage -= new Action<PlayerController>(this.PlayerReceivedDamage);
    }

    private void HandleRageEffect()
    {
      if (!this.Owner.HasActiveBonusSynergy(CustomSynergyType.NANOMACHINES_SON))
        return;
      if ((double) this.m_rageElapsed > 0.0)
      {
        this.m_rageElapsed = this.RageSynergyDuration;
        if (!(bool) (UnityEngine.Object) this.RageOverheadVFX || !((UnityEngine.Object) this.rageInstanceVFX == (UnityEngine.Object) null))
          return;
        this.rageInstanceVFX = this.Owner.PlayEffectOnActor(this.RageOverheadVFX, new Vector3(0.0f, 1.375f, 0.0f), alreadyMiddleCenter: true);
      }
      else
        this.Owner.StartCoroutine(this.HandleRageCooldown());
    }

    [DebuggerHidden]
    private IEnumerator HandleRageCooldown()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new NanomachinesItem__HandleRageCooldownc__Iterator0()
      {
        _this = this
      };
    }
  }

