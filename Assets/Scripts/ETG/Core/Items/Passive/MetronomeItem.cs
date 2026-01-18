// Decompiled with JetBrains decompiler
// Type: MetronomeItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class MetronomeItem : PassiveItem
  {
    public float damageBoostPerKill = 0.05f;
    public float damageBoostPerKillSynergy = 0.04f;
    public float damageMultiplierCap = 3f;
    public float synergyMultiplierCap = 5f;
    public tk2dSprite eighthNoteSprite;
    public tk2dSprite doubleEighthNoteSprite;
    public Gradient colorGradient;
    public Gradient synergyColorGradient;
    [NonSerialized]
    private Gun m_cachedGunReference;
    [NonSerialized]
    private int m_sequentialKills;
    [NonSerialized]
    private PlayerController m_player;

    private float ModifiedBoost
    {
      get
      {
        return (bool) (UnityEngine.Object) this.Owner && this.Owner.HasActiveBonusSynergy(CustomSynergyType.KEEPING_THE_BEAT) ? this.damageBoostPerKillSynergy : this.damageBoostPerKill;
      }
    }

    private float ModifiedCap
    {
      get
      {
        return (bool) (UnityEngine.Object) this.Owner && this.Owner.HasActiveBonusSynergy(CustomSynergyType.KEEPING_THE_BEAT) ? this.synergyMultiplierCap : this.damageMultiplierCap;
      }
    }

    private Gradient ModifiedGradient
    {
      get
      {
        return (bool) (UnityEngine.Object) this.Owner && this.Owner.HasActiveBonusSynergy(CustomSynergyType.KEEPING_THE_BEAT) ? this.synergyColorGradient : this.colorGradient;
      }
    }

    public override void MidGameSerialize(List<object> data)
    {
      base.MidGameSerialize(data);
      if (!((UnityEngine.Object) this.m_cachedGunReference != (UnityEngine.Object) null))
        return;
      data.Add((object) this.m_cachedGunReference.PickupObjectId);
      data.Add((object) this.m_sequentialKills);
    }

    public override void MidGameDeserialize(List<object> data)
    {
      base.MidGameDeserialize(data);
      if (!(bool) (UnityEngine.Object) this.m_player || this.m_player.inventory == null || data.Count != 2)
        return;
      this.m_sequentialKills = (int) data[1];
      int num = (int) data[0];
      for (int index = 0; index < this.m_player.inventory.AllGuns.Count; ++index)
      {
        if ((bool) (UnityEngine.Object) this.m_player.inventory.AllGuns[index] && this.m_player.inventory.AllGuns[index].PickupObjectId == num)
          this.m_cachedGunReference = this.m_player.inventory.AllGuns[index];
      }
    }

    public float GetCurrentMultiplier()
    {
      return Mathf.Clamp((float) (1.0 + (double) this.m_sequentialKills * (double) this.ModifiedBoost), 0.0f, this.ModifiedCap);
    }

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      base.Pickup(player);
      this.m_player = player;
      player.OnKilledEnemy += new Action<PlayerController>(this.OnKilledEnemy);
      player.GunChanged += new Action<Gun, Gun, bool>(this.OnGunChanged);
      player.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnReceivedDamage);
      this.m_cachedGunReference = player.CurrentGun;
    }

    private void OnGunChanged(Gun old, Gun current, bool newGun)
    {
      bool flag1 = false;
      if ((bool) (UnityEngine.Object) this.m_player && this.m_player.CharacterUsesRandomGuns)
        flag1 = true;
      bool flag2 = false;
      if ((bool) (UnityEngine.Object) this.m_player && this.m_player.inventory != null && this.m_player.inventory.GunChangeForgiveness)
        flag2 = true;
      if ((UnityEngine.Object) old != (UnityEngine.Object) current && !newGun && !flag1 && !flag2)
        this.DoMetronomeBroken(current);
      this.m_cachedGunReference = current;
    }

    private void DoMetronomeUp()
    {
      ++this.m_sequentialKills;
      this.m_player.stats.RecalculateStats(this.m_player);
      int num1 = (int) AkSoundEngine.SetRTPCValue("Pitch_Metronome", (float) this.m_sequentialKills);
      int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_metronome_jingle_01", this.m_player.gameObject);
      float currentMultiplier = this.GetCurrentMultiplier();
      Color tintColor = this.ModifiedGradient.Evaluate(Mathf.InverseLerp(1f, this.ModifiedCap, currentMultiplier));
      if ((double) currentMultiplier >= 2.0)
        this.m_player.BloopItemAboveHead((tk2dBaseSprite) this.doubleEighthNoteSprite, string.Empty, tintColor);
      else
        this.m_player.BloopItemAboveHead((tk2dBaseSprite) this.eighthNoteSprite, string.Empty, tintColor);
    }

    private void DoMetronomeBroken(Gun current)
    {
      float currentMultiplier = this.GetCurrentMultiplier();
      if ((double) currentMultiplier > 1.0)
      {
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", this.m_player.gameObject);
        Color color = this.ModifiedGradient.Evaluate(Mathf.InverseLerp(1f, this.ModifiedCap, currentMultiplier));
        this.m_player.PlayEffectOnActor((double) currentMultiplier < 2.0 ? this.eighthNoteSprite.gameObject : this.doubleEighthNoteSprite.gameObject, Vector3.up * 1.5f).GetComponent<tk2dBaseSprite>().color = color;
      }
      int num1 = (int) AkSoundEngine.SetRTPCValue("Pitch_Metronome", 0.0f);
      this.m_sequentialKills = 0;
      this.m_cachedGunReference = current;
      this.m_player.stats.RecalculateStats(this.m_player);
    }

    private void OnReceivedDamage(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      this.DoMetronomeBroken(this.m_cachedGunReference);
    }

    private void OnKilledEnemy(PlayerController source)
    {
      if ((UnityEngine.Object) source.CurrentGun != (UnityEngine.Object) this.m_cachedGunReference)
        this.DoMetronomeBroken(source.CurrentGun);
      this.DoMetronomeUp();
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      player.OnKilledEnemy -= new Action<PlayerController>(this.OnKilledEnemy);
      player.GunChanged -= new Action<Gun, Gun, bool>(this.OnGunChanged);
      player.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnReceivedDamage);
      debrisObject.GetComponent<MetronomeItem>().m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if ((bool) (UnityEngine.Object) this.m_owner)
      {
        this.m_owner.OnKilledEnemy -= new Action<PlayerController>(this.OnKilledEnemy);
        this.m_owner.GunChanged -= new Action<Gun, Gun, bool>(this.OnGunChanged);
        this.m_owner.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnReceivedDamage);
      }
      base.OnDestroy();
    }
  }

