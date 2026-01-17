// Decompiled with JetBrains decompiler
// Type: ShovelGunModifier
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

namespace ETG.Core.Systems.Utilities
{
    public class ShovelGunModifier : MonoBehaviour, IGunInheritable
    {
      public GameObject HoleVFX;
      public GenericLootTable GoodDigLootTable;
      public GenericLootTable SynergyGoodDigLootTable;
      public GenericLootTable BadDigLootTable;
      public GenericLootTable SynergyBadDigLootTable;
      public int NumberOfGoodDigs = 5;
      public int NumberOfGoodDigsAddedBySynergy = 5;
      public bool WeightedByShotsRemaining = true;
      public bool OnlyOnEmptyReload;
      private Gun m_gun;
      private bool m_alreadyRolledReward;
      private int m_goodDigsUsed;
      private bool m_wasReloading;
      private RoomHandler m_lastRoomDugGood;

      public void Start()
      {
        this.m_gun = this.GetComponent<Gun>();
        this.m_gun.LockedHorizontalOnReload = true;
        if (this.OnlyOnEmptyReload)
          this.m_gun.OnAutoReload += new Action<PlayerController, Gun>(this.HandleReloadPressedSimple);
        else
          this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReloadPressed);
      }

      private void Update()
      {
        if (this.m_wasReloading && (bool) (UnityEngine.Object) this.m_gun && !this.m_gun.IsReloading)
          this.m_wasReloading = false;
        if (!(bool) (UnityEngine.Object) this.m_gun || !((UnityEngine.Object) this.m_gun.CurrentOwner != (UnityEngine.Object) null) || this.m_gun.ClipShotsRemaining <= 0)
          return;
        this.m_alreadyRolledReward = false;
      }

      private void HandleReloadPressedSimple(PlayerController ownerPlayer, Gun sourceGun)
      {
        this.HandleReloadPressed(ownerPlayer, sourceGun, false);
      }

      private void HandleReloadPressed(PlayerController ownerPlayer, Gun sourceGun, bool something)
      {
        if (sourceGun.IsReloading)
        {
          if (this.m_wasReloading)
            return;
          this.m_wasReloading = true;
          ownerPlayer.StartCoroutine(this.HandleDig(sourceGun));
        }
        else
          this.m_wasReloading = false;
      }

      [DebuggerHidden]
      private IEnumerator HandleDig(Gun sourceGun)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShovelGunModifier.\u003CHandleDig\u003Ec__Iterator0()
        {
          sourceGun = sourceGun,
          \u0024this = this
        };
      }

      public void InheritData(Gun sourceGun)
      {
        ShovelGunModifier component = sourceGun.GetComponent<ShovelGunModifier>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        this.m_goodDigsUsed = component.m_goodDigsUsed;
      }

      public void MidGameSerialize(List<object> data, int dataIndex)
      {
        data.Add((object) this.m_goodDigsUsed);
      }

      public void MidGameDeserialize(List<object> data, ref int dataIndex)
      {
        this.m_goodDigsUsed = (int) data[dataIndex];
        ++dataIndex;
      }
    }

}
