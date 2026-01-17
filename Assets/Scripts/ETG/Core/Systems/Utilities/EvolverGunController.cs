// Decompiled with JetBrains decompiler
// Type: EvolverGunController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class EvolverGunController : MonoBehaviour, IGunInheritable
    {
      [PickupIdentifier]
      public int Form01ID;
      [PickupIdentifier]
      public int Form02ID;
      [PickupIdentifier]
      public int Form03ID;
      [PickupIdentifier]
      public int Form04ID;
      [PickupIdentifier]
      public int Form05ID;
      [PickupIdentifier]
      public int Form06ID;
      public int TypesPerForm = 5;
      private Gun m_gun;
      private bool m_initialized;
      private PlayerController m_player;
      private int m_currentForm;
      private HashSet<string> m_enemiesKilled = new HashSet<string>();
      private int m_savedEnemiesKilled;
      private bool m_synergyActive;
      private bool m_wasDeserialized;

      private void Awake() => this.m_gun = this.GetComponent<Gun>();

      private void KilledEnemyContext(PlayerController sourcePlayer, HealthHaver killedEnemy)
      {
        if (!(bool) (UnityEngine.Object) killedEnemy)
          return;
        AIActor component = killedEnemy.GetComponent<AIActor>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        this.m_enemiesKilled.Add(component.EnemyGuid);
        this.UpdateTier(sourcePlayer);
      }

      private void UpdateTier(PlayerController sourcePlayer)
      {
        int num1 = this.m_enemiesKilled.Count + this.m_savedEnemiesKilled;
        int num2 = this.TypesPerForm;
        if ((bool) (UnityEngine.Object) sourcePlayer && sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.NATURAL_SELECTION))
          num2 = Mathf.Max(1, this.TypesPerForm - 2);
        if ((bool) (UnityEngine.Object) sourcePlayer && sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.POWERHOUSE_OF_THE_CELL))
          num1 += num2;
        int num3 = Mathf.Min(Mathf.FloorToInt((float) num1 / (float) num2), 5);
        if (num3 == this.m_currentForm)
          return;
        this.m_currentForm = num3;
        this.TransformToForm(this.m_currentForm);
      }

      private void Update()
      {
        if (this.m_initialized && !(bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
          this.Disengage();
        else if (!this.m_initialized && (bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
          this.Engage();
        if ((bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
        {
          PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
          if (this.m_synergyActive && !currentOwner.HasActiveBonusSynergy(CustomSynergyType.POWERHOUSE_OF_THE_CELL))
          {
            this.m_synergyActive = false;
            this.UpdateTier(currentOwner);
          }
          else if (!this.m_synergyActive && currentOwner.HasActiveBonusSynergy(CustomSynergyType.POWERHOUSE_OF_THE_CELL))
          {
            this.m_synergyActive = true;
            this.UpdateTier(currentOwner);
          }
        }
        if (!this.m_wasDeserialized || !(bool) (UnityEngine.Object) this.m_gun || !(bool) (UnityEngine.Object) this.m_gun.CurrentOwner || !((UnityEngine.Object) this.m_gun.CurrentOwner.CurrentGun == (UnityEngine.Object) this.m_gun))
          return;
        this.m_wasDeserialized = false;
        this.UpdateTier(this.m_gun.CurrentOwner as PlayerController);
      }

      private void OnDestroy()
      {
        this.m_enemiesKilled.Clear();
        this.m_savedEnemiesKilled = 0;
        this.Disengage();
      }

      private void Engage()
      {
        this.m_initialized = true;
        this.m_player = this.m_gun.CurrentOwner as PlayerController;
        this.m_player.OnKilledEnemyContext += new Action<PlayerController, HealthHaver>(this.KilledEnemyContext);
      }

      private void Disengage()
      {
        if ((bool) (UnityEngine.Object) this.m_player)
          this.m_player.OnKilledEnemyContext -= new Action<PlayerController, HealthHaver>(this.KilledEnemyContext);
        this.m_player = (PlayerController) null;
        this.m_initialized = false;
      }

      private void TransformToForm(int targetForm)
      {
        switch (targetForm)
        {
          case 0:
            this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.Form01ID) as Gun);
            break;
          case 1:
            this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.Form02ID) as Gun);
            break;
          case 2:
            this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.Form03ID) as Gun);
            break;
          case 3:
            this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.Form04ID) as Gun);
            break;
          case 4:
            this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.Form05ID) as Gun);
            break;
          case 5:
            this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.Form06ID) as Gun);
            break;
        }
        this.m_gun.GainAmmo(this.m_gun.AdjustedMaxAmmo);
      }

      public void InheritData(Gun sourceGun)
      {
        EvolverGunController component = sourceGun.GetComponent<EvolverGunController>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        this.m_savedEnemiesKilled = component.m_savedEnemiesKilled;
        this.m_enemiesKilled = component.m_enemiesKilled;
      }

      public void MidGameSerialize(List<object> data, int dataIndex)
      {
        data.Add((object) (this.m_savedEnemiesKilled + this.m_enemiesKilled.Count));
      }

      public void MidGameDeserialize(List<object> data, ref int dataIndex)
      {
        this.m_savedEnemiesKilled = (int) data[dataIndex];
        ++dataIndex;
        this.m_wasDeserialized = true;
      }
    }

}
