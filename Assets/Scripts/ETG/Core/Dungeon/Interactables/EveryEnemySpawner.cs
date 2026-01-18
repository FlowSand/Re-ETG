using System;
using System.Collections;
using System.Diagnostics;

using Dungeonator;

#nullable disable

public class EveryEnemySpawner : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
        public string[] ignoreList;
        public bool reinforce;
        private RoomHandler m_room;
        private AIActor m_blobulinPrefab;

        public void Start()
        {
            this.m_room.Entered += new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
            this.m_blobulinPrefab = EnemyDatabase.Instance.Entries.Find((Predicate<EnemyDatabaseEntry>) (e => e.path.Contains("/Blobulin.prefab"))).GetPrefab<AIActor>();
        }

        public void ConfigureOnPlacement(RoomHandler room) => this.m_room = room;

        public void PlayerEntered(PlayerController playerController)
        {
            this.StartCoroutine(this.SpawnAllEnemies());
        }

        [DebuggerHidden]
        private IEnumerator SpawnAllEnemies()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new EveryEnemySpawner__SpawnAllEnemiesc__Iterator0()
            {
                _this = this
            };
        }
    }

