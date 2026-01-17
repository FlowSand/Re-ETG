// Decompiled with JetBrains decompiler
// Type: EmptyBottleItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class EmptyBottleItem : PlayerItem
    {
      private EmptyBottleItem.EmptyBottleContents m_contents;
      public string EmptySprite;
      public string ContainsHeartSprite;
      public string ContainsHalfHeartSprite;
      public string ContainsAmmoSprite;
      public string ContainsFairySprite;
      public string ContainsSoulSprite;
      public string ContainsSpreadAmmoSprite;
      public string ContainsBlankSprite;
      public string ContainsKeySprite;
      public GameObject HealVFX;
      public GameObject AmmoVFX;
      public GameObject FairyVFX;
      public float SoulDamage = 30f;
      public GameObject OnBurstDamageVFX;

      public EmptyBottleItem.EmptyBottleContents Contents
      {
        get => this.m_contents;
        set
        {
          this.m_contents = value;
          this.UpdateSprite();
        }
      }

      public override bool CanBeUsed(PlayerController user)
      {
        if (this.m_contents == EmptyBottleItem.EmptyBottleContents.NONE)
        {
          if (!this.CanReallyBeUsed(user))
            return false;
        }
        else if (this.m_contents == EmptyBottleItem.EmptyBottleContents.ENEMY_SOUL)
        {
          if (user.CurrentRoom == null || !user.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
            return false;
        }
        else if (this.m_contents == EmptyBottleItem.EmptyBottleContents.FAIRY && (double) user.healthHaver.GetCurrentHealthPercentage() >= 1.0)
          return false;
        return base.CanBeUsed(user);
      }

      private bool BottleFullCanBeConsumed(PlayerController user)
      {
        switch (this.m_contents)
        {
          case EmptyBottleItem.EmptyBottleContents.NONE:
            if (!this.CanReallyBeUsed(user))
              return false;
            break;
          case EmptyBottleItem.EmptyBottleContents.HALF_HEART:
          case EmptyBottleItem.EmptyBottleContents.FULL_HEART:
            if ((double) user.healthHaver.GetCurrentHealthPercentage() >= 1.0)
              return false;
            break;
          case EmptyBottleItem.EmptyBottleContents.AMMO:
            if ((UnityEngine.Object) user.CurrentGun == (UnityEngine.Object) null || user.CurrentGun.ammo == user.CurrentGun.AdjustedMaxAmmo || !user.CurrentGun.CanGainAmmo || user.CurrentGun.InfiniteAmmo)
              return false;
            break;
          case EmptyBottleItem.EmptyBottleContents.FAIRY:
            if ((double) user.healthHaver.GetCurrentHealthPercentage() >= 1.0)
              return false;
            break;
          case EmptyBottleItem.EmptyBottleContents.ENEMY_SOUL:
            if (user.CurrentRoom == null || !user.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
              return false;
            break;
          case EmptyBottleItem.EmptyBottleContents.SPREAD_AMMO:
            if ((UnityEngine.Object) user.CurrentGun == (UnityEngine.Object) null || user.CurrentGun.ammo == user.CurrentGun.AdjustedMaxAmmo || !user.CurrentGun.CanGainAmmo || user.CurrentGun.InfiniteAmmo)
              return false;
            break;
        }
        return true;
      }

      private bool CanReallyBeUsed(PlayerController user)
      {
        if (!(bool) (UnityEngine.Object) user)
          return false;
        if (user.CurrentRoom != null)
        {
          List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
          if (activeEnemies != null)
          {
            for (int index = 0; index < activeEnemies.Count; ++index)
            {
              AIActor aiActor = activeEnemies[index];
              if ((bool) (UnityEngine.Object) aiActor && (bool) (UnityEngine.Object) aiActor.encounterTrackable && aiActor.encounterTrackable.journalData.PrimaryDisplayName == "#GUNFAIRY_ENCNAME")
                return true;
            }
          }
        }
        List<DebrisObject> allDebris = StaticReferenceManager.AllDebris;
        if (allDebris != null)
        {
          for (int index = 0; index < allDebris.Count; ++index)
          {
            DebrisObject debrisObject = allDebris[index];
            if ((bool) (UnityEngine.Object) debrisObject && debrisObject.IsPickupObject)
            {
              float sqrMagnitude = (user.CenterPosition - debrisObject.transform.position.XY()).sqrMagnitude;
              if ((double) sqrMagnitude <= 25.0)
              {
                HealthPickup component1 = debrisObject.GetComponent<HealthPickup>();
                AmmoPickup component2 = debrisObject.GetComponent<AmmoPickup>();
                KeyBulletPickup component3 = debrisObject.GetComponent<KeyBulletPickup>();
                SilencerItem component4 = debrisObject.GetComponent<SilencerItem>();
                if (((bool) (UnityEngine.Object) component1 && component1.armorAmount == 0 && ((double) component1.healAmount == 0.5 || (double) component1.healAmount == 1.0) || (bool) (UnityEngine.Object) component2 || (bool) (UnityEngine.Object) component3 || (bool) (UnityEngine.Object) component4) && (double) Mathf.Sqrt(sqrMagnitude) < 5.0)
                  return true;
              }
            }
          }
        }
        if ((bool) (UnityEngine.Object) user)
        {
          IPlayerInteractable lastInteractable = user.GetLastInteractable();
          if (lastInteractable is HeartDispenser && (bool) (UnityEngine.Object) (lastInteractable as HeartDispenser) && HeartDispenser.CurrentHalfHeartsStored > 0)
            return true;
        }
        return false;
      }

      protected override void OnPreDrop(PlayerController user)
      {
        if ((bool) (UnityEngine.Object) user)
          user.OnReceivedDamage -= new Action<PlayerController>(this.HandleOwnerTookDamage);
        base.OnPreDrop(user);
      }

      public override void Pickup(PlayerController player)
      {
        base.Pickup(player);
        player.OnReceivedDamage += new Action<PlayerController>(this.HandleOwnerTookDamage);
      }

      private void HandleOwnerTookDamage(PlayerController sourcePlayer)
      {
        if (!(bool) (UnityEngine.Object) sourcePlayer || !sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.EMPTY_VESSELS) || this.Contents != EmptyBottleItem.EmptyBottleContents.NONE)
          return;
        this.Contents = EmptyBottleItem.EmptyBottleContents.ENEMY_SOUL;
      }

      public override void MidGameSerialize(List<object> data)
      {
        base.MidGameSerialize(data);
        data.Add((object) (int) this.Contents);
      }

      public override void MidGameDeserialize(List<object> data)
      {
        base.MidGameDeserialize(data);
        if (data.Count != 1)
          return;
        this.Contents = (EmptyBottleItem.EmptyBottleContents) data[0];
      }

      [DebuggerHidden]
      private IEnumerator HandleSuck(tk2dSprite targetSprite)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EmptyBottleItem__HandleSuckc__Iterator0()
        {
          targetSprite = targetSprite,
          _this = this
        };
      }

      protected override void DoEffect(PlayerController user)
      {
        int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_bottle_cork_01", this.gameObject);
        if (this.Contents == EmptyBottleItem.EmptyBottleContents.NONE)
        {
          tk2dSpriteCollectionData spriteCollection = (tk2dSpriteCollectionData) null;
          int spriteId = -1;
          Vector3 vector3 = Vector3.zero;
          AIActor aiActor = (AIActor) null;
          List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
          if (activeEnemies != null)
          {
            for (int index = 0; index < activeEnemies.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) activeEnemies[index] && (bool) (UnityEngine.Object) activeEnemies[index].encounterTrackable && activeEnemies[index].encounterTrackable.journalData.PrimaryDisplayName == "#GUNFAIRY_ENCNAME")
                aiActor = activeEnemies[index];
            }
          }
          if ((bool) (UnityEngine.Object) aiActor)
          {
            if ((bool) (UnityEngine.Object) aiActor.sprite)
            {
              spriteCollection = aiActor.sprite.Collection;
              spriteId = aiActor.sprite.spriteId;
              vector3 = aiActor.transform.position;
            }
            aiActor.EraseFromExistence();
            this.Contents = EmptyBottleItem.EmptyBottleContents.FAIRY;
          }
          else
          {
            if ((bool) (UnityEngine.Object) user)
            {
              IPlayerInteractable lastInteractable = user.GetLastInteractable();
              if (lastInteractable is HeartDispenser && (bool) (UnityEngine.Object) (lastInteractable as HeartDispenser) && HeartDispenser.CurrentHalfHeartsStored > 0)
              {
                if (HeartDispenser.CurrentHalfHeartsStored > 1)
                {
                  HeartDispenser.CurrentHalfHeartsStored -= 2;
                  this.Contents = EmptyBottleItem.EmptyBottleContents.FULL_HEART;
                  return;
                }
                --HeartDispenser.CurrentHalfHeartsStored;
                this.Contents = EmptyBottleItem.EmptyBottleContents.HALF_HEART;
                return;
              }
            }
            if (StaticReferenceManager.AllDebris != null)
            {
              DebrisObject debrisObject = (DebrisObject) null;
              float num2 = float.MaxValue;
              for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
              {
                DebrisObject allDebri = StaticReferenceManager.AllDebris[index];
                if (allDebri.IsPickupObject)
                {
                  float sqrMagnitude = (user.CenterPosition - allDebri.transform.position.XY()).sqrMagnitude;
                  if ((double) sqrMagnitude <= 25.0)
                  {
                    HealthPickup component1 = allDebri.GetComponent<HealthPickup>();
                    AmmoPickup component2 = allDebri.GetComponent<AmmoPickup>();
                    KeyBulletPickup component3 = allDebri.GetComponent<KeyBulletPickup>();
                    SilencerItem component4 = allDebri.GetComponent<SilencerItem>();
                    if ((bool) (UnityEngine.Object) component1 && component1.armorAmount == 0 && ((double) component1.healAmount == 0.5 || (double) component1.healAmount == 1.0) || (bool) (UnityEngine.Object) component2 || (bool) (UnityEngine.Object) component3 || (bool) (UnityEngine.Object) component4)
                    {
                      float num3 = Mathf.Sqrt(sqrMagnitude);
                      if ((double) num3 < (double) num2 && (double) num3 < 5.0)
                      {
                        num2 = num3;
                        debrisObject = allDebri;
                      }
                    }
                  }
                }
              }
              if ((bool) (UnityEngine.Object) debrisObject)
              {
                HealthPickup component5 = debrisObject.GetComponent<HealthPickup>();
                AmmoPickup component6 = debrisObject.GetComponent<AmmoPickup>();
                KeyBulletPickup component7 = debrisObject.GetComponent<KeyBulletPickup>();
                SilencerItem component8 = debrisObject.GetComponent<SilencerItem>();
                if ((bool) (UnityEngine.Object) component5)
                {
                  if ((bool) (UnityEngine.Object) component5.sprite)
                  {
                    spriteCollection = component5.sprite.Collection;
                    spriteId = component5.sprite.spriteId;
                    vector3 = component5.transform.position;
                  }
                  if (component5.armorAmount == 0 && (double) component5.healAmount == 0.5)
                  {
                    this.Contents = EmptyBottleItem.EmptyBottleContents.HALF_HEART;
                    UnityEngine.Object.Destroy((UnityEngine.Object) component5.gameObject);
                  }
                  else if (component5.armorAmount == 0 && (double) component5.healAmount == 1.0)
                  {
                    this.Contents = EmptyBottleItem.EmptyBottleContents.FULL_HEART;
                    UnityEngine.Object.Destroy((UnityEngine.Object) component5.gameObject);
                  }
                }
                else if ((bool) (UnityEngine.Object) component6)
                {
                  if ((bool) (UnityEngine.Object) component6.sprite)
                  {
                    spriteCollection = component6.sprite.Collection;
                    spriteId = component6.sprite.spriteId;
                    vector3 = component6.transform.position;
                  }
                  this.Contents = component6.mode != AmmoPickup.AmmoPickupMode.SPREAD_AMMO ? EmptyBottleItem.EmptyBottleContents.AMMO : EmptyBottleItem.EmptyBottleContents.SPREAD_AMMO;
                  UnityEngine.Object.Destroy((UnityEngine.Object) component6.gameObject);
                }
                else if ((bool) (UnityEngine.Object) component7)
                {
                  if ((bool) (UnityEngine.Object) component7.sprite)
                  {
                    spriteCollection = component7.sprite.Collection;
                    spriteId = component7.sprite.spriteId;
                    vector3 = component7.transform.position;
                  }
                  this.Contents = EmptyBottleItem.EmptyBottleContents.KEY;
                  UnityEngine.Object.Destroy((UnityEngine.Object) component7.gameObject);
                }
                else if ((bool) (UnityEngine.Object) component8)
                {
                  if ((bool) (UnityEngine.Object) component8.sprite)
                  {
                    spriteCollection = component8.sprite.Collection;
                    spriteId = component8.sprite.spriteId;
                    vector3 = component8.transform.position;
                  }
                  this.Contents = EmptyBottleItem.EmptyBottleContents.BLANK;
                  UnityEngine.Object.Destroy((UnityEngine.Object) component8.gameObject);
                }
              }
            }
          }
          if (!((UnityEngine.Object) spriteCollection != (UnityEngine.Object) null))
            return;
          GameManager.Instance.Dungeon.StartCoroutine(this.HandleSuck(tk2dSprite.AddComponent(new GameObject("sucked sprite")
          {
            transform = {
              position = vector3
            }
          }, spriteCollection, spriteId)));
        }
        else if (this.BottleFullCanBeConsumed(user))
          this.UseContainedItem(user);
        else
          this.ThrowContainedItem(user);
      }

      private void ThrowContainedItem(PlayerController user)
      {
        switch (this.Contents)
        {
          case EmptyBottleItem.EmptyBottleContents.HALF_HEART:
            LootEngine.SpawnHealth(user.CenterPosition, 1, new Vector2?(UnityEngine.Random.insideUnitCircle.normalized));
            break;
          case EmptyBottleItem.EmptyBottleContents.FULL_HEART:
            LootEngine.SpawnHealth(user.CenterPosition, 2, new Vector2?(UnityEngine.Random.insideUnitCircle.normalized));
            break;
          case EmptyBottleItem.EmptyBottleContents.AMMO:
            LootEngine.SpawnItem((GameObject) BraveResources.Load("Ammo_Pickup"), user.CenterPosition.ToVector3ZUp(), UnityEngine.Random.insideUnitCircle.normalized, 4f);
            break;
          case EmptyBottleItem.EmptyBottleContents.SPREAD_AMMO:
            LootEngine.SpawnItem((GameObject) BraveResources.Load("Ammo_Pickup_Spread"), user.CenterPosition.ToVector3ZUp(), UnityEngine.Random.insideUnitCircle.normalized, 4f);
            break;
          case EmptyBottleItem.EmptyBottleContents.BLANK:
            LootEngine.SpawnItem(PickupObjectDatabase.GetById(GlobalItemIds.Blank).gameObject, user.CenterPosition.ToVector3ZUp(), UnityEngine.Random.insideUnitCircle.normalized, 4f);
            break;
          case EmptyBottleItem.EmptyBottleContents.KEY:
            LootEngine.SpawnItem(PickupObjectDatabase.GetById(GlobalItemIds.Key).gameObject, user.CenterPosition.ToVector3ZUp(), UnityEngine.Random.insideUnitCircle.normalized, 4f);
            break;
        }
        this.Contents = EmptyBottleItem.EmptyBottleContents.NONE;
      }

      private void UseContainedItem(PlayerController user)
      {
        switch (this.Contents)
        {
          case EmptyBottleItem.EmptyBottleContents.HALF_HEART:
            user.healthHaver.ApplyHealing(0.5f);
            int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", this.gameObject);
            user.PlayEffectOnActor(this.HealVFX, Vector3.zero);
            break;
          case EmptyBottleItem.EmptyBottleContents.FULL_HEART:
            user.healthHaver.ApplyHealing(1f);
            int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", this.gameObject);
            user.PlayEffectOnActor(this.HealVFX, Vector3.zero);
            break;
          case EmptyBottleItem.EmptyBottleContents.AMMO:
            if ((UnityEngine.Object) user.CurrentGun != (UnityEngine.Object) null && user.CurrentGun.AdjustedMaxAmmo > 0)
            {
              user.CurrentGun.GainAmmo(user.CurrentGun.AdjustedMaxAmmo);
              user.CurrentGun.ForceImmediateReload();
              int num3 = (int) AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", this.gameObject);
              user.PlayEffectOnActor(this.AmmoVFX, Vector3.zero);
              string header = StringTableManager.GetString("#AMMO_SINGLE_GUN_REFILLED_HEADER");
              string description = $"{user.CurrentGun.GetComponent<EncounterTrackable>().journalData.GetPrimaryDisplayName()} {StringTableManager.GetString("#AMMO_SINGLE_GUN_REFILLED_BODY")}";
              tk2dBaseSprite sprite = user.CurrentGun.GetSprite();
              if (!GameUIRoot.Instance.BossHealthBarVisible)
              {
                GameUIRoot.Instance.notificationController.DoCustomNotification(header, description, sprite.Collection, sprite.spriteId);
                break;
              }
              break;
            }
            break;
          case EmptyBottleItem.EmptyBottleContents.FAIRY:
            int num4 = (int) AkSoundEngine.PostEvent("Play_NPC_faerie_heal_01", this.gameObject);
            user.PlayFairyEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Fairy_Fly") as GameObject, Vector3.zero, 4.5f, true);
            user.StartCoroutine(this.HandleHearts(user));
            break;
          case EmptyBottleItem.EmptyBottleContents.ENEMY_SOUL:
            user.CurrentRoom.ApplyActionToNearbyEnemies(user.transform.position.XY(), 100f, new Action<AIActor, float>(this.SoulProcessEnemy));
            break;
          case EmptyBottleItem.EmptyBottleContents.SPREAD_AMMO:
            float num5 = 0.5f;
            float num6 = 0.2f;
            user.CurrentGun.GainAmmo(Mathf.CeilToInt((float) user.CurrentGun.AdjustedMaxAmmo * num5));
            for (int index = 0; index < user.inventory.AllGuns.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) user.inventory.AllGuns[index] && (UnityEngine.Object) user.CurrentGun != (UnityEngine.Object) user.inventory.AllGuns[index])
                user.inventory.AllGuns[index].GainAmmo(Mathf.FloorToInt((float) user.inventory.AllGuns[index].AdjustedMaxAmmo * num6));
            }
            user.CurrentGun.ForceImmediateReload();
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
              PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(user);
              if (!otherPlayer.IsGhost)
              {
                for (int index = 0; index < otherPlayer.inventory.AllGuns.Count; ++index)
                {
                  if ((bool) (UnityEngine.Object) otherPlayer.inventory.AllGuns[index])
                    otherPlayer.inventory.AllGuns[index].GainAmmo(Mathf.FloorToInt((float) otherPlayer.inventory.AllGuns[index].AdjustedMaxAmmo * num6));
                }
                otherPlayer.CurrentGun.ForceImmediateReload();
              }
            }
            int num7 = (int) AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", this.gameObject);
            user.PlayEffectOnActor(this.AmmoVFX, Vector3.zero);
            string header1 = StringTableManager.GetString("#AMMO_SINGLE_GUN_REFILLED_HEADER");
            string description1 = StringTableManager.GetString("#AMMO_SPREAD_REFILLED_BODY");
            tk2dBaseSprite sprite1 = user.CurrentGun.GetSprite();
            if (!GameUIRoot.Instance.BossHealthBarVisible)
            {
              GameUIRoot.Instance.notificationController.DoCustomNotification(header1, description1, sprite1.Collection, sprite1.spriteId);
              break;
            }
            break;
          case EmptyBottleItem.EmptyBottleContents.BLANK:
            ++user.Blanks;
            break;
          case EmptyBottleItem.EmptyBottleContents.KEY:
            ++user.carriedConsumables.KeyBullets;
            break;
        }
        this.Contents = EmptyBottleItem.EmptyBottleContents.NONE;
      }

      private void SoulProcessEnemy(AIActor a, float distance)
      {
        if (!(bool) (UnityEngine.Object) a || !a.IsNormalEnemy || !(bool) (UnityEngine.Object) a.healthHaver || a.IsGone)
          return;
        if ((bool) (UnityEngine.Object) this.LastOwner)
          a.healthHaver.ApplyDamage(this.SoulDamage, Vector2.zero, this.LastOwner.ActorName);
        else
          a.healthHaver.ApplyDamage(this.SoulDamage, Vector2.zero, "projectile");
        if (!(bool) (UnityEngine.Object) this.OnBurstDamageVFX)
          return;
        a.PlayEffectOnActor(this.OnBurstDamageVFX, Vector3.zero);
      }

      [DebuggerHidden]
      private IEnumerator HandleHearts(PlayerController targetPlayer)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EmptyBottleItem__HandleHeartsc__Iterator1()
        {
          targetPlayer = targetPlayer
        };
      }

      private void UpdateSprite()
      {
        switch (this.Contents)
        {
          case EmptyBottleItem.EmptyBottleContents.NONE:
            this.spriteAnimator.Stop();
            this.sprite.SetSprite(this.EmptySprite);
            break;
          case EmptyBottleItem.EmptyBottleContents.HALF_HEART:
            this.spriteAnimator.Stop();
            this.sprite.SetSprite(this.ContainsHalfHeartSprite);
            break;
          case EmptyBottleItem.EmptyBottleContents.FULL_HEART:
            this.spriteAnimator.Stop();
            this.sprite.SetSprite(this.ContainsHeartSprite);
            break;
          case EmptyBottleItem.EmptyBottleContents.AMMO:
            this.spriteAnimator.Stop();
            this.sprite.SetSprite(this.ContainsAmmoSprite);
            break;
          case EmptyBottleItem.EmptyBottleContents.FAIRY:
            this.spriteAnimator.Stop();
            this.sprite.SetSprite(this.ContainsFairySprite);
            break;
          case EmptyBottleItem.EmptyBottleContents.ENEMY_SOUL:
            this.sprite.SetSprite(this.ContainsSoulSprite);
            this.spriteAnimator.Play("empty_bottle_soul");
            break;
          case EmptyBottleItem.EmptyBottleContents.SPREAD_AMMO:
            this.spriteAnimator.Stop();
            this.sprite.SetSprite(this.ContainsSpreadAmmoSprite);
            break;
          case EmptyBottleItem.EmptyBottleContents.BLANK:
            this.spriteAnimator.Stop();
            this.sprite.SetSprite(this.ContainsBlankSprite);
            break;
          case EmptyBottleItem.EmptyBottleContents.KEY:
            this.spriteAnimator.Stop();
            this.sprite.SetSprite(this.ContainsKeySprite);
            break;
        }
      }

      protected override void CopyStateFrom(PlayerItem other)
      {
        base.CopyStateFrom(other);
        EmptyBottleItem emptyBottleItem = other as EmptyBottleItem;
        if (!(bool) (UnityEngine.Object) emptyBottleItem)
          return;
        this.m_contents = emptyBottleItem.m_contents;
        this.sprite.SetSprite(emptyBottleItem.sprite.spriteId);
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.LastOwner)
          this.LastOwner.OnReceivedDamage -= new Action<PlayerController>(this.HandleOwnerTookDamage);
        base.OnDestroy();
      }

      public enum EmptyBottleContents
      {
        NONE,
        HALF_HEART,
        FULL_HEART,
        AMMO,
        FAIRY,
        ENEMY_SOUL,
        SPREAD_AMMO,
        BLANK,
        KEY,
      }
    }

}
