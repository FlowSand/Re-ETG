using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class MulticompanionItem : PassiveItem
    {
        [EnemyIdentifier]
        public string CompanionGuid;
        public bool HasSynergy;
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        [EnemyIdentifier]
        public string SynergyCompanionGuid;
        public int SynergyMaxNumberOfCompanions = 3;
        public int MaxNumberOfCompanions = 8;
        public bool TriggersOnRoomClear;
        public bool TriggersOnEnemyKill;
        private List<CompanionController> m_companions = new List<CompanionController>();
        private bool m_synergyActive;

        private void CreateNewCompanion(PlayerController owner)
        {
            int num = !this.HasSynergy || !this.m_synergyActive ? this.MaxNumberOfCompanions : this.SynergyMaxNumberOfCompanions;
            if (this.m_companions.Count >= num && num >= 0)
                return;
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(!this.HasSynergy || !this.m_synergyActive ? this.CompanionGuid : this.SynergyCompanionGuid);
            Vector3 position = owner.transform.position;
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                position += new Vector3(1.125f, -5f / 16f, 0.0f);
            CompanionController orAddComponent = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, position, Quaternion.identity).GetOrAddComponent<CompanionController>();
            this.m_companions.Add(orAddComponent);
            orAddComponent.Initialize(owner);
            if (!(bool) (UnityEngine.Object) orAddComponent.specRigidbody)
                return;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody);
        }

        private void DestroyAllCompanions()
        {
            for (int index = this.m_companions.Count - 1; index >= 0; --index)
            {
                if ((bool) (UnityEngine.Object) this.m_companions[index])
                    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_companions[index].gameObject);
                this.m_companions.RemoveAt(index);
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnNewFloorLoaded += new Action<PlayerController>(this.HandleNewFloor);
            if (this.TriggersOnRoomClear)
                player.OnRoomClearEvent += new Action<PlayerController>(this.HandleRoomCleared);
            if (this.TriggersOnEnemyKill)
                player.OnKilledEnemy += new Action<PlayerController>(this.HandleEnemyKilled);
            this.CreateNewCompanion(player);
        }

        private void HandleEnemyKilled(PlayerController p) => this.CreateNewCompanion(p);

        private void HandleRoomCleared(PlayerController p) => this.CreateNewCompanion(p);

        protected override void Update()
        {
            base.Update();
            for (int index = this.m_companions.Count - 1; index >= 0; --index)
            {
                if (!(bool) (UnityEngine.Object) this.m_companions[index])
                    this.m_companions.RemoveAt(index);
                else if ((bool) (UnityEngine.Object) this.m_companions[index].healthHaver && this.m_companions[index].healthHaver.IsDead)
                {
                    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_companions[index].gameObject);
                    this.m_companions.RemoveAt(index);
                }
            }
            if (!(bool) (UnityEngine.Object) this.m_owner || !this.HasSynergy)
                return;
            if (this.m_synergyActive && !this.m_owner.HasActiveBonusSynergy(this.RequiredSynergy))
            {
                this.DestroyAllCompanions();
                this.m_synergyActive = false;
            }
            else
            {
                if (this.m_synergyActive || !this.m_owner.HasActiveBonusSynergy(this.RequiredSynergy))
                    return;
                this.DestroyAllCompanions();
                this.m_synergyActive = true;
            }
        }

        private void HandleNewFloor(PlayerController obj)
        {
            this.DestroyAllCompanions();
            this.CreateNewCompanion(obj);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            this.DestroyAllCompanions();
            player.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloor);
            player.OnRoomClearEvent -= new Action<PlayerController>(this.HandleRoomCleared);
            player.OnKilledEnemy -= new Action<PlayerController>(this.HandleEnemyKilled);
            return base.Drop(player);
        }

        protected override void OnDestroy()
        {
            if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
            {
                this.m_owner.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloor);
                this.m_owner.OnRoomClearEvent -= new Action<PlayerController>(this.HandleRoomCleared);
                this.m_owner.OnKilledEnemy -= new Action<PlayerController>(this.HandleEnemyKilled);
            }
            this.DestroyAllCompanions();
            base.OnDestroy();
        }
    }

