// Decompiled with JetBrains decompiler
// Type: CompanionSynergyProcessor
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
    public class CompanionSynergyProcessor : MonoBehaviour
    {
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public bool RequiresNoSynergy;
      public bool PersistsOnDisable;
      [EnemyIdentifier]
      public string CompanionGuid;
      [NonSerialized]
      public bool PreventRespawnOnFloorLoad;
      private Gun m_gun;
      private PassiveItem m_item;
      private PlayerItem m_activeItem;
      [NonSerialized]
      public PlayerController ManuallyAssignedPlayer;
      private GameObject m_extantCompanion;
      private bool m_active;
      private PlayerController m_cachedPlayer;

      public GameObject ExtantCompanion => this.m_extantCompanion;

      private void CreateCompanion(PlayerController owner)
      {
        if (this.PreventRespawnOnFloorLoad)
          return;
        this.m_extantCompanion = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid(this.CompanionGuid).gameObject, owner.transform.position, Quaternion.identity);
        CompanionController orAddComponent = this.m_extantCompanion.GetOrAddComponent<CompanionController>();
        orAddComponent.Initialize(owner);
        if (!(bool) (UnityEngine.Object) orAddComponent.specRigidbody)
          return;
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody);
      }

      private void DestroyCompanion()
      {
        if (!(bool) (UnityEngine.Object) this.m_extantCompanion)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantCompanion);
        this.m_extantCompanion = (GameObject) null;
      }

      private void Awake()
      {
        this.m_gun = this.GetComponent<Gun>();
        this.m_item = this.GetComponent<PassiveItem>();
        this.m_activeItem = this.GetComponent<PlayerItem>();
      }

      public void Update()
      {
        PlayerController player = this.ManuallyAssignedPlayer;
        if (!(bool) (UnityEngine.Object) player && (bool) (UnityEngine.Object) this.m_item)
          player = this.m_item.Owner;
        if (!(bool) (UnityEngine.Object) player && (bool) (UnityEngine.Object) this.m_activeItem && this.m_activeItem.PickedUp && (bool) (UnityEngine.Object) this.m_activeItem.LastOwner)
          player = this.m_activeItem.LastOwner;
        if (!(bool) (UnityEngine.Object) player && (bool) (UnityEngine.Object) this.m_gun && this.m_gun.CurrentOwner is PlayerController)
          player = this.m_gun.CurrentOwner as PlayerController;
        if ((bool) (UnityEngine.Object) player && (this.RequiresNoSynergy || player.HasActiveBonusSynergy(this.RequiredSynergy)) && !this.m_active)
        {
          this.m_active = true;
          this.m_cachedPlayer = player;
          this.ActivateSynergy(player);
        }
        else
        {
          if ((bool) (UnityEngine.Object) player && (this.RequiresNoSynergy || player.HasActiveBonusSynergy(this.RequiredSynergy) || !this.m_active))
            return;
          this.DeactivateSynergy();
          this.m_active = false;
        }
      }

      private void OnDisable()
      {
        if (this.m_active && !this.PersistsOnDisable)
        {
          this.DeactivateSynergy();
          this.m_active = false;
        }
        else
        {
          if (!this.m_active || !(bool) (UnityEngine.Object) this.m_cachedPlayer)
            return;
          this.m_cachedPlayer.StartCoroutine(this.HandleDisabledChecks());
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleDisabledChecks()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CompanionSynergyProcessor.<HandleDisabledChecks>c__Iterator0()
        {
          _this = this
        };
      }

      private void OnDestroy()
      {
        if (!this.m_active)
          return;
        this.DeactivateSynergy();
        this.m_active = false;
      }

      public void ActivateSynergy(PlayerController player)
      {
        player.OnNewFloorLoaded += new Action<PlayerController>(this.HandleNewFloor);
        this.CreateCompanion(player);
      }

      private void HandleNewFloor(PlayerController obj)
      {
        this.DestroyCompanion();
        if (this.PreventRespawnOnFloorLoad)
          return;
        this.CreateCompanion(obj);
      }

      public void DeactivateSynergy()
      {
        if ((UnityEngine.Object) this.m_cachedPlayer != (UnityEngine.Object) null)
        {
          this.m_cachedPlayer.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloor);
          this.m_cachedPlayer = (PlayerController) null;
        }
        this.DestroyCompanion();
      }
    }

}
