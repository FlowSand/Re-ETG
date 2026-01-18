using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable
namespace Pathfinding
{
    public class Path
    {
        public LinkedList<IntVector2> Positions;
        public LinkedList<IntVector2> PreSmoothedPositions = new LinkedList<IntVector2>();
        public IntVector2 Clearance = IntVector2.One;

        public Path()
        {
            this.Positions = new LinkedList<IntVector2>();
            this.WillReachFinalGoal = true;
        }

        public Path(LinkedList<IntVector2> positions, IntVector2 clearance)
        {
            this.Positions = positions;
            this.Clearance = clearance;
            this.WillReachFinalGoal = true;
        }

        public int Count => this.Positions != null ? this.Positions.Count : 0;

        public IntVector2 First => this.Positions.First.Value;

        public bool WillReachFinalGoal { get; set; }

        public float InaccurateLength
        {
            get
            {
                if (this.Positions.Count == 0)
                    return 0.0f;
                float inaccurateLength = 0.0f;
                LinkedListNode<IntVector2> linkedListNode = this.Positions.First;
                for (LinkedListNode<IntVector2> next = linkedListNode.Next; linkedListNode != null && next != null; next = next.Next)
                {
                    inaccurateLength += (float) IntVector2.ManhattanDistance(linkedListNode.Value, next.Value);
                    linkedListNode = next;
                }
                return inaccurateLength;
            }
        }

        public Vector2 GetFirstCenterVector2()
        {
            return Pathfinder.GetClearanceOffset(this.Positions.First.Value, this.Clearance);
        }

        public Vector2 GetSecondCenterVector2()
        {
            return Pathfinder.GetClearanceOffset(this.Positions.First.Next.Value, this.Clearance);
        }

        public void RemoveFirst() => this.Positions.RemoveFirst();

        public void Smooth(
            Vector2 startPos,
            Vector2 extents,
            CellTypes passableCellTypes,
            bool canPassOccupied,
            IntVector2 clearance)
        {
            Pathfinder.Instance.Smooth(this, startPos, extents, passableCellTypes, canPassOccupied, clearance);
        }
    }
}
