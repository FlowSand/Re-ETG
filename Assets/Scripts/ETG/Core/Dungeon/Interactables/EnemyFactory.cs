// Decompiled with JetBrains decompiler
// Type: EnemyFactory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class EnemyFactory : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
      [BetterList]
      public List<EnemyFactoryWaveDefinition> waves;
      public float delayBetweenWaves = 1f;
      public GameObject rewardChestPrefab;
      protected int m_currentWave;
      protected RoomHandler m_room;
      protected int m_spawnPointIterator;
      protected bool m_finished;

      public void ConfigureOnPlacement(RoomHandler room)
      {
        room.OnEnemiesCleared += new System.Action(this.OnWaveCleared);
        this.m_room = room;
      }

      private void Start() => this.SpawnWave();

      protected List<EnemyFactorySpawnPoint> AcquireSpawnPoints()
      {
        return this.m_room.GetComponentsInRoom<EnemyFactorySpawnPoint>();
      }

      [DebuggerHidden]
      private IEnumerator SpawnWaveCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EnemyFactory.<SpawnWaveCR>c__Iterator0()
        {
          $this = this
        };
      }

      public void SpawnWave() => this.StartCoroutine(this.SpawnWaveCR());

      protected void ProvideReward()
      {
        if (!((UnityEngine.Object) this.rewardChestPrefab != (UnityEngine.Object) null))
          return;
        Chest component = UnityEngine.Object.Instantiate<GameObject>(this.rewardChestPrefab, this.transform.position, Quaternion.identity).GetComponent<Chest>();
        component.ConfigureOnPlacement(this.m_room);
        this.m_room.RegisterInteractable((IPlayerInteractable) component);
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(component.specRigidbody);
      }

      public void OnWaveCleared()
      {
        if (this.m_currentWave < this.waves.Count - 1)
        {
          ++this.m_currentWave;
          this.SpawnWave();
        }
        else
        {
          if (this.m_finished)
            return;
          this.m_finished = true;
          this.m_room.HandleRoomAction(RoomEventTriggerAction.UNSEAL_ROOM);
          this.ProvideReward();
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
