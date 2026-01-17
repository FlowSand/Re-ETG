// Decompiled with JetBrains decompiler
// Type: FragileGunItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class FragileGunItem : PassiveItem
    {
      public GameObject GunPiecePrefab;
      private PlayerController m_player;
      private Dictionary<int, int> m_workingDictionary = new Dictionary<int, int>();
      private Dictionary<int, int> m_gunToAmmoDictionary = new Dictionary<int, int>();

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        this.m_player = player;
        base.Pickup(player);
        player.OnReceivedDamage += new Action<PlayerController>(this.HandleTookDamage);
      }

      private void HandleTookDamage(PlayerController obj)
      {
        if (!(bool) (UnityEngine.Object) obj || !(bool) (UnityEngine.Object) obj.CurrentGun || obj.CurrentGun.InfiniteAmmo)
          return;
        this.BreakGun(obj, obj.CurrentGun);
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        this.m_player = (PlayerController) null;
        debrisObject.GetComponent<FragileGunItem>().m_pickedUpThisRun = true;
        player.OnReceivedDamage -= new Action<PlayerController>(this.HandleTookDamage);
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        if (!(bool) (UnityEngine.Object) this.m_player)
          return;
        this.m_player.OnReceivedDamage -= new Action<PlayerController>(this.HandleTookDamage);
      }

      private void BreakGun(PlayerController sourcePlayer, Gun sourceGun)
      {
        int num = 5;
        for (int index = 0; index < num; ++index)
          LootEngine.SpawnItem(this.GunPiecePrefab, (Vector3) sourcePlayer.CenterPosition, UnityEngine.Random.insideUnitCircle.normalized, 10f).GetComponentInChildren<FragileGunItemPiece>().AssignGun(sourceGun);
        this.m_workingDictionary.Add(sourceGun.PickupObjectId, num);
        this.m_gunToAmmoDictionary.Add(sourceGun.PickupObjectId, sourceGun.ammo);
        sourcePlayer.inventory.RemoveGunFromInventory(sourceGun);
      }

      public void AcquirePiece(FragileGunItemPiece piece)
      {
        if (piece.AssignedGunId == -1 || !this.m_workingDictionary.ContainsKey(piece.AssignedGunId))
          return;
        this.m_workingDictionary[piece.AssignedGunId] = this.m_workingDictionary[piece.AssignedGunId] - 1;
        if (this.m_workingDictionary[piece.AssignedGunId] > 0)
          return;
        this.m_workingDictionary.Remove(piece.AssignedGunId);
        PickupObject byId = PickupObjectDatabase.GetById(piece.AssignedGunId);
        if (!(bool) (UnityEngine.Object) byId)
          return;
        Gun player = LootEngine.TryGiveGunToPlayer(byId.gameObject, this.m_owner);
        if (!this.m_gunToAmmoDictionary.ContainsKey(piece.AssignedGunId) || !(bool) (UnityEngine.Object) player)
          return;
        player.ammo = this.m_gunToAmmoDictionary[piece.AssignedGunId];
        this.m_gunToAmmoDictionary.Remove(piece.AssignedGunId);
      }
    }

}
