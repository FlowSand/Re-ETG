// Decompiled with JetBrains decompiler
// Type: HoveringGunSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class HoveringGunSynergyProcessor : MonoBehaviour
    {
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      [PickupIdentifier]
      public int TargetGunID;
      public bool UsesMultipleGuns;
      [PickupIdentifier]
      public int[] TargetGunIDs;
      public HoveringGunController.HoverPosition PositionType;
      public HoveringGunController.AimType AimType;
      public HoveringGunController.FireType FireType;
      public float FireCooldown = 1f;
      public float FireDuration = 2f;
      public bool OnlyOnEmptyReload;
      public string ShootAudioEvent;
      public string OnEveryShotAudioEvent;
      public string FinishedShootingAudioEvent;
      public HoveringGunSynergyProcessor.TriggerStyle Trigger;
      public int NumToTrigger = 1;
      public float TriggerDuration = -1f;
      public bool ConsumesTargetGunAmmo;
      public float ChanceToConsumeTargetGunAmmo = 0.5f;
      private bool m_actionsLinked;
      private PlayerController m_cachedLinkedPlayer;
      private Gun m_gun;
      private PassiveItem m_item;
      private List<HoveringGunController> m_hovers = new List<HoveringGunController>();
      private List<bool> m_initialized = new List<bool>();

      public void Awake()
      {
        this.m_gun = this.GetComponent<Gun>();
        this.m_item = this.GetComponent<PassiveItem>();
      }

      private bool IsInitialized(int index)
      {
        return this.m_initialized.Count > index && this.m_initialized[index];
      }

      public void Update()
      {
        if (this.Trigger == HoveringGunSynergyProcessor.TriggerStyle.CONSTANT)
        {
          if ((bool) (UnityEngine.Object) this.m_gun)
          {
            if ((bool) (UnityEngine.Object) this.m_gun && this.m_gun.isActiveAndEnabled && (bool) (UnityEngine.Object) this.m_gun.CurrentOwner && this.m_gun.OwnerHasSynergy(this.RequiredSynergy))
            {
              for (int index = 0; index < this.NumToTrigger; ++index)
              {
                if (!this.IsInitialized(index))
                  this.Enable(index);
              }
            }
            else
              this.DisableAll();
          }
          else
          {
            if (!(bool) (UnityEngine.Object) this.m_item)
              return;
            if ((bool) (UnityEngine.Object) this.m_item && (bool) (UnityEngine.Object) this.m_item.Owner && this.m_item.Owner.HasActiveBonusSynergy(this.RequiredSynergy))
            {
              for (int index = 0; index < this.NumToTrigger; ++index)
              {
                if (!this.IsInitialized(index))
                  this.Enable(index);
              }
            }
            else
              this.DisableAll();
          }
        }
        else if (this.Trigger == HoveringGunSynergyProcessor.TriggerStyle.ON_DAMAGE)
        {
          if (!this.m_actionsLinked && (bool) (UnityEngine.Object) this.m_gun && (bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
          {
            PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
            this.m_cachedLinkedPlayer = currentOwner;
            currentOwner.OnReceivedDamage += new Action<PlayerController>(this.HandleOwnerDamaged);
            this.m_actionsLinked = true;
          }
          else
          {
            if (!this.m_actionsLinked || !(bool) (UnityEngine.Object) this.m_gun || (bool) (UnityEngine.Object) this.m_gun.CurrentOwner || !(bool) (UnityEngine.Object) this.m_cachedLinkedPlayer)
              return;
            this.m_cachedLinkedPlayer.OnReceivedDamage -= new Action<PlayerController>(this.HandleOwnerDamaged);
            this.m_cachedLinkedPlayer = (PlayerController) null;
            this.m_actionsLinked = false;
          }
        }
        else
        {
          if (this.Trigger != HoveringGunSynergyProcessor.TriggerStyle.ON_ACTIVE_ITEM)
            return;
          if (!this.m_actionsLinked && (bool) (UnityEngine.Object) this.m_gun && (bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
          {
            PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
            this.m_cachedLinkedPlayer = currentOwner;
            currentOwner.OnUsedPlayerItem += new Action<PlayerController, PlayerItem>(this.HandleOwnerItemUsed);
            this.m_actionsLinked = true;
          }
          else
          {
            if (!this.m_actionsLinked || !(bool) (UnityEngine.Object) this.m_gun || (bool) (UnityEngine.Object) this.m_gun.CurrentOwner || !(bool) (UnityEngine.Object) this.m_cachedLinkedPlayer)
              return;
            this.m_cachedLinkedPlayer.OnUsedPlayerItem -= new Action<PlayerController, PlayerItem>(this.HandleOwnerItemUsed);
            this.m_cachedLinkedPlayer = (PlayerController) null;
            this.m_actionsLinked = false;
          }
        }
      }

      private void HandleOwnerItemUsed(PlayerController sourcePlayer, PlayerItem sourceItem)
      {
        if (!sourcePlayer.HasActiveBonusSynergy(this.RequiredSynergy) || !(bool) (UnityEngine.Object) this.GetOwner())
          return;
        for (int index1 = 0; index1 < this.NumToTrigger; ++index1)
        {
          int index2 = 0;
          while (this.IsInitialized(index2))
            ++index2;
          this.Enable(index2);
          this.StartCoroutine(this.ActiveItemDisable(index2, sourcePlayer));
        }
      }

      private void HandleOwnerDamaged(PlayerController sourcePlayer)
      {
        if (!sourcePlayer.HasActiveBonusSynergy(this.RequiredSynergy))
          return;
        for (int index1 = 0; index1 < this.NumToTrigger; ++index1)
        {
          int index2 = 0;
          while (this.IsInitialized(index2))
            ++index2;
          this.Enable(index2);
          this.StartCoroutine(this.TimedDisable(index2, this.TriggerDuration));
        }
      }

      [DebuggerHidden]
      private IEnumerator ActiveItemDisable(int index, PlayerController player)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HoveringGunSynergyProcessor.<ActiveItemDisable>c__Iterator0()
        {
          player = player,
          index = index,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator TimedDisable(int index, float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HoveringGunSynergyProcessor.<TimedDisable>c__Iterator1()
        {
          duration = duration,
          index = index,
          $this = this
        };
      }

      private void OnDisable() => this.DisableAll();

      private PlayerController GetOwner()
      {
        if ((bool) (UnityEngine.Object) this.m_gun)
          return this.m_gun.CurrentOwner as PlayerController;
        return (bool) (UnityEngine.Object) this.m_item ? this.m_item.Owner : (PlayerController) null;
      }

      private void Enable(int index)
      {
        if (this.m_initialized.Count > index && this.m_initialized[index])
          return;
        PlayerController owner = this.GetOwner();
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, owner.CenterPosition.ToVector3ZisY(), Quaternion.identity);
        gameObject.transform.parent = owner.transform;
        while (this.m_hovers.Count < index + 1)
        {
          this.m_hovers.Add((HoveringGunController) null);
          this.m_initialized.Add(false);
        }
        this.m_hovers[index] = gameObject.GetComponent<HoveringGunController>();
        this.m_hovers[index].ShootAudioEvent = this.ShootAudioEvent;
        this.m_hovers[index].OnEveryShotAudioEvent = this.OnEveryShotAudioEvent;
        this.m_hovers[index].FinishedShootingAudioEvent = this.FinishedShootingAudioEvent;
        this.m_hovers[index].ConsumesTargetGunAmmo = this.ConsumesTargetGunAmmo;
        this.m_hovers[index].ChanceToConsumeTargetGunAmmo = this.ChanceToConsumeTargetGunAmmo;
        this.m_hovers[index].Position = this.PositionType;
        this.m_hovers[index].Aim = this.AimType;
        this.m_hovers[index].Trigger = this.FireType;
        this.m_hovers[index].CooldownTime = this.FireCooldown;
        this.m_hovers[index].ShootDuration = this.FireDuration;
        this.m_hovers[index].OnlyOnEmptyReload = this.OnlyOnEmptyReload;
        Gun targetGun = (Gun) null;
        int targetGunId = this.TargetGunID;
        if (this.UsesMultipleGuns)
          targetGunId = this.TargetGunIDs[index];
        for (int index1 = 0; index1 < owner.inventory.AllGuns.Count; ++index1)
        {
          if (owner.inventory.AllGuns[index1].PickupObjectId == targetGunId)
            targetGun = owner.inventory.AllGuns[index1];
        }
        if (!(bool) (UnityEngine.Object) targetGun)
          targetGun = PickupObjectDatabase.Instance.InternalGetById(targetGunId) as Gun;
        this.m_hovers[index].Initialize(targetGun, owner);
        this.m_initialized[index] = true;
      }

      private void Disable(int index)
      {
        if (!(bool) (UnityEngine.Object) this.m_hovers[index])
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_hovers[index].gameObject);
      }

      private void DisableAll()
      {
        for (int index = 0; index < this.m_hovers.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_hovers[index])
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_hovers[index].gameObject);
        }
        this.m_hovers.Clear();
        this.m_initialized.Clear();
      }

      public void OnDestroy()
      {
        if (!this.m_actionsLinked || !(bool) (UnityEngine.Object) this.m_cachedLinkedPlayer)
          return;
        this.m_cachedLinkedPlayer.OnReceivedDamage -= new Action<PlayerController>(this.HandleOwnerDamaged);
        this.m_cachedLinkedPlayer = (PlayerController) null;
        this.m_actionsLinked = false;
      }

      public enum TriggerStyle
      {
        CONSTANT,
        ON_DAMAGE,
        ON_ACTIVE_ITEM,
      }
    }

}
