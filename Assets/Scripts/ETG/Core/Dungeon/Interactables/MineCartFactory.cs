using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class MineCartFactory : DungeonPlaceableBehaviour
    {
        public MineCartController MineCartPrefab;
        [DwarfConfigurable]
        public float TargetPathIndex;
        [DwarfConfigurable]
        public float TargetPathNodeIndex;
        [DwarfConfigurable]
        public float MaxCarts = 5f;
        [DwarfConfigurable]
        public float DelayBetweenCarts = 3f;
        [DwarfConfigurable]
        public float DelayUponDestruction = 1f;
        public bool ForceCartActive;
        [NonSerialized]
        private List<MineCartController> m_spawnedCarts;
        [NonSerialized]
        private float m_delayTimer = 1f;
        private RoomHandler m_room;

        private void Start()
        {
            this.m_room = this.GetAbsoluteParentRoom();
            this.m_spawnedCarts = new List<MineCartController>();
        }

        private void Update()
        {
            if (!GameManager.Instance.IsAnyPlayerInRoom(this.m_room))
                return;
            for (int index = 0; index < this.m_spawnedCarts.Count; ++index)
            {
                if (!(bool) (UnityEngine.Object) this.m_spawnedCarts[index])
                {
                    this.m_spawnedCarts.RemoveAt(index);
                    --index;
                    this.m_delayTimer = Mathf.Max(this.DelayUponDestruction, this.m_delayTimer);
                }
            }
            if ((double) this.m_delayTimer <= 0.0 && (double) this.m_spawnedCarts.Count < (double) this.MaxCarts)
            {
                this.m_delayTimer = this.DelayBetweenCarts;
                this.DoSpawnCart();
            }
            this.m_delayTimer = Mathf.Max(0.0f, this.m_delayTimer - BraveTime.DeltaTime);
        }

        [DebuggerHidden]
        private IEnumerator DelayedApplyVelocity(MineCartController mcc)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MineCartFactory__DelayedApplyVelocityc__Iterator0()
            {
                mcc = mcc
            };
        }

        protected void DoSpawnCart()
        {
            RoomHandler absoluteParentRoom = this.GetAbsoluteParentRoom();
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.MineCartPrefab.gameObject, this.transform.position, Quaternion.identity);
            MineCartController component1 = gameObject.GetComponent<MineCartController>();
            PathMover component2 = gameObject.GetComponent<PathMover>();
            if (this.ForceCartActive)
                component1.ForceActive = true;
            absoluteParentRoom.RegisterInteractable((IPlayerInteractable) component1);
            component2.Path = absoluteParentRoom.area.runtimePrototypeData.paths[Mathf.RoundToInt(this.TargetPathIndex)];
            component2.PathStartNode = Mathf.RoundToInt(this.TargetPathNodeIndex);
            component2.RoomHandler = absoluteParentRoom;
            this.m_spawnedCarts.Add(component1);
            if (component1.occupation != MineCartController.CartOccupationState.EMPTY || (double) this.m_spawnedCarts.Count >= (double) this.MaxCarts)
                return;
            this.StartCoroutine(this.DelayedApplyVelocity(component1));
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

