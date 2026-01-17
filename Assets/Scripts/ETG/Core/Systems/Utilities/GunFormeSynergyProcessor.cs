// Decompiled with JetBrains decompiler
// Type: GunFormeSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class GunFormeSynergyProcessor : MonoBehaviour
    {
      public GunFormeData[] Formes;
      private Gun m_gun;
      private int CurrentForme;
      [NonSerialized]
      public bool JustActiveReloaded;

      private void Awake()
      {
        this.m_gun = this.GetComponent<Gun>();
        this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReloadPressed);
      }

      private void Update()
      {
        if ((bool) (UnityEngine.Object) this.m_gun && !(bool) (UnityEngine.Object) this.m_gun.CurrentOwner && this.CurrentForme != 0)
        {
          this.ChangeForme(this.Formes[0]);
          this.CurrentForme = 0;
        }
        this.JustActiveReloaded = false;
      }

      private void HandleReloadPressed(PlayerController ownerPlayer, Gun sourceGun, bool manual)
      {
        if (this.JustActiveReloaded || !manual || sourceGun.IsReloading)
          return;
        int nextValidForme = this.GetNextValidForme(ownerPlayer);
        if (nextValidForme == this.CurrentForme)
          return;
        this.ChangeForme(this.Formes[nextValidForme]);
        this.CurrentForme = nextValidForme;
      }

      private int GetNextValidForme(PlayerController ownerPlayer)
      {
        for (int index = 0; index < this.Formes.Length; ++index)
        {
          int nextValidForme = (index + this.CurrentForme) % this.Formes.Length;
          if (nextValidForme != this.CurrentForme && this.Formes[nextValidForme].IsValid(ownerPlayer))
            return nextValidForme;
        }
        return this.CurrentForme;
      }

      private void ChangeForme(GunFormeData targetForme)
      {
        Gun byId = PickupObjectDatabase.GetById(targetForme.FormeID) as Gun;
        this.m_gun.TransformToTargetGun(byId);
        if (!(bool) (UnityEngine.Object) this.m_gun.encounterTrackable || !(bool) (UnityEngine.Object) byId.encounterTrackable)
          return;
        this.m_gun.encounterTrackable.journalData.PrimaryDisplayName = byId.encounterTrackable.journalData.PrimaryDisplayName;
        this.m_gun.encounterTrackable.journalData.ClearCache();
        PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
        if (!(bool) (UnityEngine.Object) currentOwner)
          return;
        GameUIRoot.Instance.TemporarilyShowGunName(currentOwner.IsPrimaryPlayer);
      }

      public static void AssignTemporaryOverrideGun(
        PlayerController targetPlayer,
        int gunID,
        float duration)
      {
        if (!(bool) (UnityEngine.Object) targetPlayer || targetPlayer.IsGhost)
          return;
        targetPlayer.StartCoroutine(GunFormeSynergyProcessor.HandleTransformationDuration(targetPlayer, gunID, duration));
      }

      [DebuggerHidden]
      public static IEnumerator HandleTransformationDuration(
        PlayerController targetPlayer,
        int gunID,
        float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GunFormeSynergyProcessor.\u003CHandleTransformationDuration\u003Ec__Iterator0()
        {
          targetPlayer = targetPlayer,
          gunID = gunID,
          duration = duration
        };
      }

      protected static void ClearTemporaryOverrideGun(PlayerController targetPlayer, Gun m_extantGun)
      {
        if (!(bool) (UnityEngine.Object) targetPlayer || !(bool) (UnityEngine.Object) m_extantGun)
          return;
        if ((bool) (UnityEngine.Object) targetPlayer)
        {
          targetPlayer.inventory.GunLocked.RemoveOverride("override gun");
          targetPlayer.inventory.DestroyGun(m_extantGun);
          m_extantGun = (Gun) null;
        }
        targetPlayer.inventory.GunChangeForgiveness = false;
      }
    }

}
