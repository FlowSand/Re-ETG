using System.Collections.Generic;

using Pathfinding;
using UnityEngine;

using Dungeonator;

#nullable disable

public class PlacedBlockerConfigurable : BraveBehaviour, IPlaceConfigurable
    {
        public PlacedBlockerConfigurable.ColliderSelection colliderSelection;
        [ShowInInspectorIf("colliderSelection", 0, false)]
        public bool SpecifyPixelCollider;
        [ShowInInspectorIf("SpecifyPixelCollider", false)]
        public int SpecifiedPixelCollider;
        private bool m_initialized;
        private List<OccupiedCells> m_allOccupiedCells;

        public bool ShowSpecifiedPixelCollider()
        {
            return this.colliderSelection == PlacedBlockerConfigurable.ColliderSelection.Single && this.SpecifyPixelCollider;
        }

        public void Start()
        {
        }

        public void ConfigureOnPlacement(RoomHandler room) => this.Initialize(room);

        protected override void OnDestroy()
        {
            if (GameManager.HasInstance && Pathfinder.HasInstance && (bool) (Object) this.specRigidbody && this.m_allOccupiedCells != null)
            {
                for (int index = 0; index < this.m_allOccupiedCells.Count; ++index)
                    this.m_allOccupiedCells[index]?.Clear();
            }
            base.OnDestroy();
        }

        private void Initialize(RoomHandler room)
        {
            if (this.m_initialized)
                return;
            if ((bool) (Object) this.specRigidbody)
            {
                this.specRigidbody.Initialize();
                if (this.colliderSelection == PlacedBlockerConfigurable.ColliderSelection.All)
                {
                    this.m_allOccupiedCells = new List<OccupiedCells>(this.specRigidbody.PixelColliders.Count);
                    for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                        this.m_allOccupiedCells.Add(new OccupiedCells(this.specRigidbody, this.specRigidbody.PixelColliders[index], room));
                }
                else
                {
                    this.m_allOccupiedCells = new List<OccupiedCells>(1);
                    if (this.SpecifyPixelCollider)
                        this.m_allOccupiedCells.Add(new OccupiedCells(this.specRigidbody, this.specRigidbody.PixelColliders[this.SpecifiedPixelCollider], room));
                    else
                        this.m_allOccupiedCells.Add(new OccupiedCells(this.specRigidbody, room));
                }
            }
            this.m_initialized = true;
        }

        public enum ColliderSelection
        {
            Single,
            All,
        }
    }

