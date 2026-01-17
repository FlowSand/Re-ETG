// Decompiled with JetBrains decompiler
// Type: MidGamePlayerData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class MidGamePlayerData
    {
      [fsProperty]
      public PlayableCharacters CharacterIdentity;
      [fsProperty]
      public float CurrentHealth = 1f;
      [fsProperty]
      public float CurrentArmor;
      [fsProperty]
      public int CurrentKeys;
      [fsProperty]
      public int CurrentCurrency;
      [fsProperty]
      public int CurrentBlanks;
      [fsProperty]
      public List<MidGameGunData> guns;
      [fsProperty]
      public List<MidGameActiveItemData> activeItems;
      [fsProperty]
      public List<MidGamePassiveItemData> passiveItems;
      [fsProperty]
      public List<StatModifier> ownerlessStatModifiers;
      [fsProperty]
      public int CostumeID;
      [fsProperty]
      public int MasteryTokensCollected;
      [fsProperty]
      public bool CharacterUsesRandomGuns;
      [fsProperty]
      public ChallengeModeType ChallengeMode;
      [fsProperty]
      public bool HasTakenDamageThisRun;
      [fsProperty]
      public bool HasFiredNonStartingGun;
      [fsProperty]
      public bool HasBloodthirst;
      [fsProperty]
      public bool IsTemporaryEeveeForUnlock;

      public MidGamePlayerData(PlayerController p)
      {
        this.CharacterIdentity = p.characterIdentity;
        this.CostumeID = !p.IsUsingAlternateCostume ? 0 : 1;
        this.MasteryTokensCollected = p.MasteryTokensCollectedThisRun;
        this.CharacterUsesRandomGuns = p.CharacterUsesRandomGuns;
        this.ChallengeMode = ChallengeManager.ChallengeModeType;
        this.HasTakenDamageThisRun = p.HasTakenDamageThisRun;
        this.HasFiredNonStartingGun = p.HasFiredNonStartingGun;
        this.HasBloodthirst = (bool) (Object) p.GetComponent<Bloodthirst>();
        this.IsTemporaryEeveeForUnlock = p.IsTemporaryEeveeForUnlock;
        this.CurrentHealth = p.healthHaver.GetCurrentHealth();
        this.CurrentArmor = p.healthHaver.Armor;
        this.CurrentKeys = p.carriedConsumables.KeyBullets;
        this.CurrentCurrency = p.carriedConsumables.Currency;
        this.CurrentBlanks = p.Blanks;
        this.guns = new List<MidGameGunData>();
        if (p.inventory != null && p.inventory.AllGuns != null)
        {
          for (int index = 0; index < p.inventory.AllGuns.Count; ++index)
          {
            if (!p.inventory.AllGuns[index].PreventSaveSerialization)
              this.guns.Add(new MidGameGunData(p.inventory.AllGuns[index]));
          }
        }
        this.activeItems = new List<MidGameActiveItemData>();
        if (p.activeItems != null)
        {
          for (int index = 0; index < p.activeItems.Count; ++index)
          {
            if (!p.activeItems[index].PreventSaveSerialization)
              this.activeItems.Add(new MidGameActiveItemData(p.activeItems[index]));
          }
        }
        this.passiveItems = new List<MidGamePassiveItemData>();
        if (p.passiveItems != null)
        {
          for (int index = 0; index < p.passiveItems.Count; ++index)
          {
            if (!p.passiveItems[index].PreventSaveSerialization)
              this.passiveItems.Add(new MidGamePassiveItemData(p.passiveItems[index]));
          }
        }
        this.ownerlessStatModifiers = new List<StatModifier>();
        if (p.ownerlessStatModifiers == null)
          return;
        for (int index = 0; index < p.ownerlessStatModifiers.Count; ++index)
        {
          if (!p.ownerlessStatModifiers[index].ignoredForSaveData)
            this.ownerlessStatModifiers.Add(p.ownerlessStatModifiers[index]);
        }
      }
    }

}
