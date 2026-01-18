using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace Dungeonator
{
    [Serializable]
    public class PrototypeEventTriggerArea
    {
        [SerializeField]
        public List<Vector2> triggerCells;

        public PrototypeEventTriggerArea() => this.triggerCells = new List<Vector2>();

        public PrototypeEventTriggerArea(IEnumerable<Vector2> cells)
        {
            this.triggerCells = new List<Vector2>(cells);
        }

        public PrototypeEventTriggerArea(IEnumerable<IntVector2> cells)
        {
            this.triggerCells = new List<Vector2>();
            foreach (IntVector2 cell in cells)
                this.triggerCells.Add(cell.ToVector2());
        }

        public PrototypeEventTriggerArea CreateMirror(IntVector2 roomDimensions)
        {
            PrototypeEventTriggerArea mirror = new PrototypeEventTriggerArea();
            for (int index = 0; index < this.triggerCells.Count; ++index)
            {
                Vector2 triggerCell = this.triggerCells[index];
                triggerCell.x = (float) roomDimensions.x - (triggerCell.x + 1f);
                mirror.triggerCells.Add(triggerCell);
            }
            return mirror;
        }
    }
}
