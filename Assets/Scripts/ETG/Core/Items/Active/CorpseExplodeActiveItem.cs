// Decompiled with JetBrains decompiler
// Type: CorpseExplodeActiveItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class CorpseExplodeActiveItem : PlayerItem
    {
      public ScreenShakeSettings ScreenShake;
      public ExplosionData CorpseExplosionData;
      public bool UsesCrisisStoneSynergy;
      public GameObject ShieldForCrisisStoneSynergy;
      private AIBulletBank m_bulletBank;

      public override void Pickup(PlayerController player)
      {
        base.Pickup(player);
        this.m_bulletBank = this.GetComponent<AIBulletBank>();
        if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
          PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
        if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
          PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
        else
          PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
      }

      protected override void OnPreDrop(PlayerController player)
      {
        base.OnPreDrop(player);
        if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
          return;
        PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
        if (PassiveItem.ActiveFlagItems[player][this.GetType()] != 0)
          return;
        PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
      }

      protected override void DoEffect(PlayerController user)
      {
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_dead_again_01", this.gameObject);
        bool flag = false;
        for (int index = 0; index < StaticReferenceManager.AllCorpses.Count; ++index)
        {
          GameObject allCorpse = StaticReferenceManager.AllCorpses[index];
          if ((bool) (UnityEngine.Object) allCorpse && (bool) (UnityEngine.Object) allCorpse.GetComponent<tk2dBaseSprite>() && allCorpse.transform.position.GetAbsoluteRoom() == user.CurrentRoom)
          {
            flag = true;
            Vector2 worldCenter = allCorpse.GetComponent<tk2dBaseSprite>().WorldCenter;
            Exploder.Explode((Vector3) worldCenter, this.CorpseExplosionData, Vector2.zero, ignoreQueues: true);
            if (user.HasActiveBonusSynergy(CustomSynergyType.CORPSE_EXPLOSHOOT))
            {
              float nearestDistance = -1f;
              AIActor nearestEnemy = user.CurrentRoom.GetNearestEnemy(worldCenter, out nearestDistance);
              if ((bool) (UnityEngine.Object) nearestEnemy)
                this.FireBullet((Vector3) worldCenter, nearestEnemy.CenterPosition - worldCenter);
            }
            if (user.HasActiveBonusSynergy(CustomSynergyType.CRISIS_ROCK))
              UnityEngine.Object.Instantiate<GameObject>(this.ShieldForCrisisStoneSynergy, (Vector3) worldCenter, Quaternion.identity);
            UnityEngine.Object.Destroy((UnityEngine.Object) allCorpse.gameObject);
          }
        }
        if (flag)
          GameManager.Instance.MainCameraController.DoScreenShake(this.ScreenShake, new Vector2?(user.CenterPosition));
        else
          this.ClearCooldowns();
      }

      protected override void OnDestroy()
      {
        if (this.m_pickedUp && (bool) (UnityEngine.Object) this.LastOwner && PassiveItem.ActiveFlagItems != null && PassiveItem.ActiveFlagItems.ContainsKey(this.LastOwner) && PassiveItem.ActiveFlagItems[this.LastOwner].ContainsKey(this.GetType()))
        {
          PassiveItem.ActiveFlagItems[this.LastOwner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.LastOwner][this.GetType()] - 1);
          if (PassiveItem.ActiveFlagItems[this.LastOwner][this.GetType()] == 0)
            PassiveItem.ActiveFlagItems[this.LastOwner].Remove(this.GetType());
        }
        base.OnDestroy();
      }

      private void FireBullet(Vector3 shootPoint, Vector2 direction)
      {
        Projectile component = this.m_bulletBank.CreateProjectileFromBank((Vector2) shootPoint, BraveMathCollege.Atan2Degrees(direction), "default").GetComponent<Projectile>();
        if (!(bool) (UnityEngine.Object) component || !(bool) (UnityEngine.Object) this.LastOwner)
          return;
        component.collidesWithPlayer = false;
        component.collidesWithEnemies = true;
        component.SetOwnerSafe((GameActor) this.LastOwner, this.LastOwner.ActorName);
        component.SetNewShooter(this.LastOwner.specRigidbody);
        component.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker));
      }
    }

}
