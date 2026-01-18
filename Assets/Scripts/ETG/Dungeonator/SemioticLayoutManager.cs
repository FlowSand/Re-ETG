using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnityEngine;

#nullable disable
namespace Dungeonator
{
    public class SemioticLayoutManager
    {
        private int MAXIMUM_ROOM_DIMENSION = 50;
        private List<RoomHandler> m_allRooms;
        private HashSet<IntVector2> m_exitTestPoints = new HashSet<IntVector2>();
        private IntVector2 m_currentOffset = IntVector2.Zero;
        private HashSet<IntVector2> m_occupiedCells;
        private HashSet<IntVector2> m_temporaryPathfindingWalls = new HashSet<IntVector2>();
        private List<Tuple<IntVector2, IntVector2>> m_rectangleDecomposition;
        private static List<HashSet<IntVector2>> PooledResizedHashsets = new List<HashSet<IntVector2>>();
        private static IntVector2[] SimpleCardinals = new IntVector2[5]
        {
            IntVector2.Up,
            2 * IntVector2.Up,
            IntVector2.Right,
            IntVector2.Down,
            IntVector2.Left
        };
        private static IntVector2[] LayoutCardinals = new IntVector2[12]
        {
            IntVector2.Up,
            IntVector2.Right,
            IntVector2.Down,
            IntVector2.Left,
            2 * IntVector2.Up,
            3 * IntVector2.Up,
            new IntVector2(1, 1),
            new IntVector2(1, 2),
            new IntVector2(-1, 1),
            new IntVector2(-1, 2),
            new IntVector2(1, -1),
            new IntVector2(-1, -1)
        };
        private static IntVector2[] LayoutPathCardinals = new IntVector2[17]
        {
            IntVector2.Up,
            IntVector2.Right,
            IntVector2.Down,
            IntVector2.Left,
            2 * IntVector2.Up,
            3 * IntVector2.Up,
            2 * IntVector2.Right,
            new IntVector2(2, 1),
            new IntVector2(2, 2),
            new IntVector2(1, 3),
            new IntVector2(2, 3),
            new IntVector2(1, 1),
            new IntVector2(1, 2),
            new IntVector2(-1, 1),
            new IntVector2(-1, 2),
            new IntVector2(1, -1),
            new IntVector2(-1, -1)
        };
        private const int SEARCH_DISTANCE_LAYOUT = 3;
        public bool FindNearestValidLocataionForLayout2Success;
        public IntVector2 FindNearestValidLocationForLayout2Result;
        private const int PER_ROOM_HALLWAY_EXTENSION_MAX = 4;
        private const int PER_LAYOUT_HALLWAY_EXTENSION_MAX = 12;
        private int m_FIRST_FAILS;
        private int m_SECOND_FAILS;
        private int m_THIRD_FAILS;
        private int m_FOURTH_FAILS;
        public bool CanPlaceLayoutAtPointSuccess;

        public SemioticLayoutManager()
        {
            this.m_allRooms = new List<RoomHandler>();
            if (SemioticLayoutManager.PooledResizedHashsets.Count > 0)
            {
                int index1 = 0;
                for (int index2 = 0; index2 < SemioticLayoutManager.PooledResizedHashsets.Count; ++index2)
                {
                    if (SemioticLayoutManager.PooledResizedHashsets[index1].Count < SemioticLayoutManager.PooledResizedHashsets[index2].Count)
                        index1 = index2;
                }
                this.m_occupiedCells = SemioticLayoutManager.PooledResizedHashsets[index1];
                SemioticLayoutManager.PooledResizedHashsets.RemoveAt(index1);
            }
            else
                this.m_occupiedCells = new HashSet<IntVector2>();
        }

        public List<RoomHandler> Rooms => this.m_allRooms;

        public HashSet<IntVector2> OccupiedCells => this.m_occupiedCells;

        public IntVector2 Dimensions
        {
            get
            {
                Tuple<IntVector2, IntVector2> maxCellPositions = this.GetMinAndMaxCellPositions();
                return maxCellPositions.Second - maxCellPositions.First;
            }
        }

        public IntVector2 NegativeDimensions
        {
            get => IntVector2.Max(IntVector2.Zero, IntVector2.Zero - this.GetMinimumCellPosition());
        }

        public IntVector2 PositiveDimensions
        {
            get => IntVector2.Max(IntVector2.Zero, this.GetMaximumCellPosition());
        }

        public List<Tuple<IntVector2, IntVector2>> RectangleDecomposition
        {
            get => this.m_rectangleDecomposition;
        }

        public void ComputeRectangleDecomposition()
        {
            if (this.m_rectangleDecomposition == null)
                this.m_rectangleDecomposition = new List<Tuple<IntVector2, IntVector2>>();
            else if (this.m_rectangleDecomposition.Count != 0)
                return;
            HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
            foreach (IntVector2 occupiedCell in this.m_occupiedCells)
            {
                if (!intVector2Set.Contains(occupiedCell))
                {
                    int x1 = 1;
                    int y1 = 1;
                    while (true)
                    {
                        int y2 = occupiedCell.y + y1;
                        if (this.m_occupiedCells.Contains(new IntVector2(occupiedCell.x, y2)))
                            ++y1;
                        else
                            break;
                    }
                    while (true)
                    {
                        int x2 = occupiedCell.x + x1;
                        bool flag = true;
                        for (int y3 = occupiedCell.y; y3 < occupiedCell.y + y1; ++y3)
                        {
                            if (!this.m_occupiedCells.Contains(new IntVector2(x2, y3)))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                            ++x1;
                        else
                            break;
                    }
                    for (int x3 = occupiedCell.x; x3 < occupiedCell.x + x1; ++x3)
                    {
                        for (int y4 = occupiedCell.y; y4 < occupiedCell.y + y1; ++y4)
                        {
                            IntVector2 intVector2 = new IntVector2(x3, y4);
                            intVector2Set.Add(intVector2);
                        }
                    }
                    this.m_rectangleDecomposition.Add(new Tuple<IntVector2, IntVector2>(occupiedCell, new IntVector2(x1, y1)));
                }
            }
            this.m_rectangleDecomposition = this.m_rectangleDecomposition.OrderByDescending<Tuple<IntVector2, IntVector2>, int>((Func<Tuple<IntVector2, IntVector2>, int>) (a => a.Second.x * a.Second.y)).ToList<Tuple<IntVector2, IntVector2>>();
        }

        public void OnDestroy()
        {
            this.m_occupiedCells.Clear();
            if (SemioticLayoutManager.PooledResizedHashsets.Count > 10)
            {
                int index1 = 0;
                for (int index2 = 0; index2 < SemioticLayoutManager.PooledResizedHashsets.Count; ++index2)
                {
                    if (SemioticLayoutManager.PooledResizedHashsets[index1].Count > SemioticLayoutManager.PooledResizedHashsets[index2].Count)
                        index1 = index2;
                }
                SemioticLayoutManager.PooledResizedHashsets.RemoveAt(index1);
            }
            SemioticLayoutManager.PooledResizedHashsets.Add(this.m_occupiedCells);
        }

        public void DebugListLengths()
        {
            UnityEngine.Debug.Log((object) $"SLayoutManager list sizes: {(object) this.m_allRooms.Count}|{(object) this.m_occupiedCells.Count}");
        }

        public void DebugDrawOccupiedCells(Vector2 positionOffset)
        {
            foreach (IntVector2 occupiedCell in this.m_occupiedCells)
                BraveUtility.DrawDebugSquare(occupiedCell.ToVector2() + positionOffset, Color.red, 1000f);
        }

        public void DebugDrawBoundingBox(Vector2 positionOffset, Color c)
        {
            Vector2 vector2_1 = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 vector2_2 = new Vector2(float.MinValue, float.MinValue);
            foreach (IntVector2 occupiedCell in this.m_occupiedCells)
            {
                vector2_1.x = Mathf.Min((float) occupiedCell.x, vector2_1.x);
                vector2_1.y = Mathf.Min((float) occupiedCell.y, vector2_1.x);
                vector2_2.x = Mathf.Max((float) occupiedCell.x, vector2_2.x);
                vector2_2.y = Mathf.Max((float) occupiedCell.y, vector2_2.x);
            }
            BraveUtility.DrawDebugSquare(vector2_1 + positionOffset, vector2_2 + Vector2.one + positionOffset, c, 1000f);
        }

        public void ClearTemporary() => this.m_temporaryPathfindingWalls.Clear();

        public void StampComplexExitTemporary(RuntimeRoomExitData exit, CellArea area)
        {
            PrototypeRoomExit referencedExit = exit.referencedExit;
            IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(referencedExit.exitDirection);
            int num = !exit.jointedExit || referencedExit.exitDirection == DungeonData.Direction.WEST ? 0 : 1;
            for (int index1 = 0; index1 < referencedExit.containedCells.Count; ++index1)
            {
                for (int index2 = 0; index2 < exit.TotalExitLength + num; ++index2)
                {
                    IntVector2 intVector2 = referencedExit.containedCells[index1].ToIntVector2() - IntVector2.One + area.basePosition + vector2FromDirection * index2;
                    this.m_temporaryPathfindingWalls.Add(intVector2);
                    for (int index3 = 0; index3 < SemioticLayoutManager.SimpleCardinals.Length; ++index3)
                        this.m_temporaryPathfindingWalls.Add(intVector2 + SemioticLayoutManager.LayoutCardinals[index3]);
                }
            }
        }

        public void StampComplexExitToLayout(RuntimeRoomExitData exit, CellArea area, bool unstamp = false)
        {
            PrototypeRoomExit referencedExit = exit.referencedExit;
            IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(referencedExit.exitDirection);
            int num = !exit.jointedExit || referencedExit.exitDirection == DungeonData.Direction.WEST ? 0 : 1;
            for (int index1 = 0; index1 < referencedExit.containedCells.Count; ++index1)
            {
                for (int index2 = 0; index2 < exit.TotalExitLength + num; ++index2)
                {
                    IntVector2 intVector2 = referencedExit.containedCells[index1].ToIntVector2() - IntVector2.One + area.basePosition + vector2FromDirection * index2;
                    if (unstamp)
                    {
                        this.m_occupiedCells.Remove(intVector2);
                        for (int index3 = 0; index3 < SemioticLayoutManager.LayoutCardinals.Length; ++index3)
                            this.m_occupiedCells.Remove(intVector2 + SemioticLayoutManager.LayoutCardinals[index3]);
                    }
                    else
                    {
                        this.m_occupiedCells.Add(intVector2);
                        for (int index4 = 0; index4 < SemioticLayoutManager.LayoutCardinals.Length; ++index4)
                            this.m_occupiedCells.Add(intVector2 + SemioticLayoutManager.LayoutCardinals[index4]);
                    }
                }
            }
            IntVector2 intVector2_1 = referencedExit.containedCells[0].ToIntVector2() - IntVector2.One + area.basePosition + vector2FromDirection * (exit.TotalExitLength + num - 1);
            if (unstamp)
                this.m_exitTestPoints.Remove(intVector2_1);
            else
                this.m_exitTestPoints.Add(intVector2_1);
            if (this.m_rectangleDecomposition == null)
                return;
            this.m_rectangleDecomposition.Clear();
        }

        public void DebugDrawComplexit(RuntimeRoomExitData exit, CellArea area, Color c)
        {
            PrototypeRoomExit referencedExit = exit.referencedExit;
            IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(referencedExit.exitDirection);
            int num = !exit.jointedExit || referencedExit.exitDirection == DungeonData.Direction.WEST ? 0 : 1;
            for (int index1 = 0; index1 < referencedExit.containedCells.Count; ++index1)
            {
                for (int index2 = 0; index2 < exit.TotalExitLength + num; ++index2)
                {
                    IntVector2 intVector2 = referencedExit.containedCells[index1].ToIntVector2() - IntVector2.One + area.basePosition + vector2FromDirection * index2;
                    this.m_occupiedCells.Add(intVector2);
                    for (int index3 = 0; index3 < SemioticLayoutManager.LayoutCardinals.Length; ++index3)
                        BraveUtility.DrawDebugSquare((intVector2 + SemioticLayoutManager.LayoutCardinals[index3]).ToVector2(), c, 1000f);
                }
            }
        }

        public SemioticLayoutManager.BBoxPrepassResults CheckRoomBoundingBoxCollisions(
            SemioticLayoutManager otherCanvas,
            IntVector2 otherCanvasOffset)
        {
            SemioticLayoutManager.BBoxPrepassResults bboxPrepassResults = new SemioticLayoutManager.BBoxPrepassResults();
            bboxPrepassResults.overlapping = false;
            for (int index1 = 0; index1 < this.m_allRooms.Count; ++index1)
            {
                RoomHandler allRoom1 = this.m_allRooms[index1];
                for (int index2 = 0; index2 < otherCanvas.m_allRooms.Count; ++index2)
                {
                    RoomHandler allRoom2 = otherCanvas.m_allRooms[index2];
                    ++bboxPrepassResults.numPairs;
                    int cellsOverlapping = 0;
                    if (IntVector2.AABBOverlapWithArea(allRoom1.area.basePosition, allRoom1.area.dimensions, allRoom2.area.basePosition + otherCanvasOffset, allRoom2.area.dimensions, out cellsOverlapping))
                    {
                        bboxPrepassResults.overlapping = true;
                        ++bboxPrepassResults.numPairsOverlapping;
                        bboxPrepassResults.totalOverlapArea += cellsOverlapping;
                    }
                }
            }
            return bboxPrepassResults;
        }

        private bool CheckRectangleDecompositionCollisions(
            SemioticLayoutManager otherCanvas,
            IntVector2 otherCanvasOffset)
        {
            for (int index1 = 0; index1 < otherCanvas.m_rectangleDecomposition.Count; ++index1)
            {
                Tuple<IntVector2, IntVector2> tuple1 = otherCanvas.m_rectangleDecomposition[index1];
                for (int index2 = 0; index2 < this.m_rectangleDecomposition.Count; ++index2)
                {
                    Tuple<IntVector2, IntVector2> tuple2 = this.m_rectangleDecomposition[index2];
                    if (IntVector2.AABBOverlap(tuple1.First + otherCanvasOffset, tuple1.Second, tuple2.First, tuple2.Second))
                        return false;
                }
            }
            return true;
        }

        [DebuggerHidden]
        public IEnumerable FindNearestValidLocationForLayout2(
            SemioticLayoutManager canvas,
            RuntimeRoomExitData staticExit,
            RuntimeRoomExitData newExit,
            IntVector2 staticAreaBasePosition,
            IntVector2 newAreaBasePosition)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            SemioticLayoutManager__FindNearestValidLocationForLayout2c__Iterator0 locationForLayout2 = new SemioticLayoutManager__FindNearestValidLocationForLayout2c__Iterator0()
            {
                newAreaBasePosition = newAreaBasePosition,
                staticAreaBasePosition = staticAreaBasePosition,
                staticExit = staticExit,
                newExit = newExit,
                canvas = canvas,
                _this = this
            };
            // ISSUE: reference to a compiler-generated field
            locationForLayout2._PC = -2;
            return (IEnumerable) locationForLayout2;
        }

        public bool FindNearestValidLocationForLayout(
            SemioticLayoutManager canvas,
            RuntimeRoomExitData staticExit,
            RuntimeRoomExitData newExit,
            IntVector2 staticAreaBasePosition,
            IntVector2 newAreaBasePosition,
            out IntVector2 idealPosition)
        {
            IntVector2 intVector2_1 = newAreaBasePosition;
            idealPosition = intVector2_1;
            int x = 0;
            int y = 0;
            int num1 = 0;
            int num2 = -1;
            int num3 = 50000;
            while (num3 > 0)
            {
                --num3;
                IntVector2 intVector2_2 = newAreaBasePosition + new IntVector2(x, y);
                IntVector2 intVector2_3 = staticAreaBasePosition + staticExit.ExitOrigin - IntVector2.One - (intVector2_2 + newExit.ExitOrigin - IntVector2.One);
                bool flag = true;
                foreach (IntVector2 occupiedCell in canvas.m_occupiedCells)
                {
                    if (this.m_occupiedCells.Contains(occupiedCell + intVector2_3))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    idealPosition = intVector2_2;
                    return true;
                }
                if (x == y || x < 0 && x == -y || x > 0 && x == 1 - y)
                {
                    int num4 = num1;
                    num1 = -num2;
                    num2 = num4;
                }
                x += num1;
                y += num2;
            }
            BraveUtility.Log("Failed iterations on find nearest valid location for layout.", Color.red, BraveUtility.LogVerbosity.IMPORTANT);
            idealPosition = IntVector2.Zero;
            return false;
        }

        public bool FindNearestValidLocationForRoom(
            PrototypeDungeonRoom prototype,
            IntVector2 startPosition,
            out IntVector2 idealPosition)
        {
            int x1 = 0;
            int y1 = 0;
            int num1 = 0;
            int num2 = -1;
            int num3 = 10000;
            while (num3 > 0)
            {
                --num3;
                bool flag = true;
                IntVector2 intVector2 = startPosition + new IntVector2(x1, y1);
                for (int x2 = -1; x2 < prototype.Width + 1; ++x2)
                {
                    for (int y2 = -1; y2 < prototype.Height + 1; ++y2)
                    {
                        if (this.m_occupiedCells.Contains(intVector2 + new IntVector2(x2, y2)))
                        {
                            flag = false;
                            goto label_9;
                        }
                    }
                }
    label_9:
                if (flag)
                {
                    idealPosition = intVector2;
                    return true;
                }
                if (x1 == y1 || x1 < 0 && x1 == -y1 || x1 > 0 && x1 == 1 - y1)
                {
                    int num4 = num1;
                    num1 = -num2;
                    num2 = num4;
                }
                x1 += num1;
                y1 += num2;
            }
            idealPosition = IntVector2.Zero;
            return false;
        }

        public void StampCellAreaToLayout(RoomHandler newRoom, bool unstamp = false)
        {
            CellArea area = newRoom.area;
            if (!unstamp)
                this.m_allRooms.Add(newRoom);
            else
                this.m_allRooms.Remove(newRoom);
            if ((UnityEngine.Object) area.prototypeRoom != (UnityEngine.Object) null)
            {
                foreach (IntVector2 representationIncFacewall in area.prototypeRoom.GetCellRepresentationIncFacewalls())
                {
                    if (unstamp)
                        this.m_occupiedCells.Remove(representationIncFacewall + area.basePosition);
                    else
                        this.m_occupiedCells.Add(representationIncFacewall + area.basePosition);
                }
            }
            else if (area.proceduralCells != null && area.proceduralCells.Count > 0)
            {
                for (int index1 = 0; index1 < area.proceduralCells.Count; ++index1)
                {
                    this.m_occupiedCells.Add(area.proceduralCells[index1] + area.basePosition);
                    for (int index2 = 0; index2 < SemioticLayoutManager.LayoutCardinals.Length; ++index2)
                        this.m_occupiedCells.Add(area.proceduralCells[index1] + SemioticLayoutManager.LayoutCardinals[index2] + area.basePosition);
                }
            }
            else
            {
                for (int index3 = 0; index3 < area.dimensions.x; ++index3)
                {
                    for (int index4 = 0; index4 < area.dimensions.y; ++index4)
                    {
                        IntVector2 intVector2 = new IntVector2(area.basePosition.x + index3, area.basePosition.y + index4);
                        if (unstamp)
                        {
                            this.m_occupiedCells.Remove(intVector2);
                            for (int index5 = 0; index5 < SemioticLayoutManager.LayoutCardinals.Length; ++index5)
                                this.m_occupiedCells.Remove(intVector2 + SemioticLayoutManager.LayoutCardinals[index5]);
                        }
                        else
                        {
                            this.m_occupiedCells.Add(intVector2);
                            for (int index6 = 0; index6 < SemioticLayoutManager.LayoutCardinals.Length; ++index6)
                                this.m_occupiedCells.Add(intVector2 + SemioticLayoutManager.LayoutCardinals[index6]);
                        }
                    }
                }
            }
            if (this.m_rectangleDecomposition == null)
                return;
            this.m_rectangleDecomposition.Clear();
        }

        public void HandleOffsetRooms(IntVector2 offset)
        {
            this.m_currentOffset += offset;
            for (int index = 0; index < this.m_allRooms.Count; ++index)
            {
                RoomHandler allRoom = this.m_allRooms[index];
                allRoom.area.basePosition += offset;
                this.m_allRooms[index] = allRoom;
            }
        }

        public IntVector2 GetSafelyBoundedMinimumCellPosition()
        {
            IntVector2 lhs = this.GetMinimumCellPosition();
            for (int index = 0; index < this.m_allRooms.Count; ++index)
                lhs = IntVector2.Min(lhs, this.m_allRooms[index].area.basePosition);
            return lhs;
        }

        public IntVector2 GetSafelyBoundedMaximumCellPosition()
        {
            IntVector2 lhs = this.GetMaximumCellPosition();
            for (int index = 0; index < this.m_allRooms.Count; ++index)
                lhs = IntVector2.Max(lhs, this.m_allRooms[index].area.basePosition + this.m_allRooms[index].area.dimensions);
            return lhs;
        }

        public Tuple<IntVector2, IntVector2> GetMinAndMaxCellPositions()
        {
            int num1 = int.MaxValue;
            int num2 = int.MaxValue;
            int num3 = int.MinValue;
            int num4 = int.MinValue;
            foreach (IntVector2 occupiedCell in this.m_occupiedCells)
            {
                num1 = Math.Min(num1, occupiedCell.x);
                num2 = Math.Min(num2, occupiedCell.y);
                num3 = Math.Max(num3, occupiedCell.x);
                num4 = Math.Max(num4, occupiedCell.y);
            }
            return new Tuple<IntVector2, IntVector2>(new IntVector2(num1, num2), new IntVector2(num3, num4));
        }

        public IntVector2 GetMinimumCellPosition()
        {
            int num1 = int.MaxValue;
            int num2 = int.MaxValue;
            foreach (IntVector2 occupiedCell in this.m_occupiedCells)
            {
                num1 = Math.Min(num1, occupiedCell.x);
                num2 = Math.Min(num2, occupiedCell.y);
            }
            return new IntVector2(num1, num2);
        }

        public IntVector2 GetMaximumCellPosition()
        {
            int num1 = int.MinValue;
            int num2 = int.MinValue;
            foreach (IntVector2 occupiedCell in this.m_occupiedCells)
            {
                num1 = Math.Max(num1, occupiedCell.x);
                num2 = Math.Max(num2, occupiedCell.y);
            }
            return new IntVector2(num1, num2);
        }

        public bool CanPlaceCellBounds(CellArea newArea)
        {
            for (int index = 0; index < this.m_allRooms.Count; ++index)
            {
                if (this.m_allRooms[index].area.OverlapsWithUnitBorder(newArea))
                    return false;
            }
            return true;
        }

        private bool CheckExitsClearForPlacement(
            PrototypeDungeonRoom newRoom,
            RuntimeRoomExitData exitToTest,
            IntVector2 attachPoint)
        {
            IntVector2 areaBasePosition = attachPoint - (exitToTest.ExitOrigin - IntVector2.One);
            return this.CheckExitClearForPlacement(exitToTest, areaBasePosition);
        }

        private Tuple<IntVector2, IntVector2> GetExitRectCells(
            RuntimeRoomExitData exit,
            IntVector2 areaBasePosition)
        {
            PrototypeRoomExit referencedExit = exit.referencedExit;
            IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(referencedExit.exitDirection);
            int num1 = !exit.jointedExit || referencedExit.exitDirection == DungeonData.Direction.WEST ? 0 : 1;
            if (exit.jointedExit)
                ++num1;
            int num2 = Mathf.Max(4, exit.TotalExitLength + num1);
            int num3 = int.MaxValue;
            int num4 = int.MaxValue;
            int num5 = int.MinValue;
            int num6 = int.MinValue;
            for (int index = 0; index < referencedExit.containedCells.Count; ++index)
            {
                num3 = Mathf.Min((int) referencedExit.containedCells[index].x, num3);
                num4 = Mathf.Min((int) referencedExit.containedCells[index].y, num4);
                num5 = Mathf.Max((int) referencedExit.containedCells[index].x, num5);
                num6 = Mathf.Max((int) referencedExit.containedCells[index].y, num6);
            }
            IntVector2 intVector2_1 = new IntVector2(num3, num4) - IntVector2.One;
            IntVector2 intVector2_2 = new IntVector2(num5, num6) + IntVector2.One;
            IntVector2 intVector2_3 = intVector2_1 + areaBasePosition + vector2FromDirection * 3;
            IntVector2 intVector2_4 = intVector2_2 + areaBasePosition + vector2FromDirection * num2;
            IntVector2 intVector2_5 = IntVector2.Min(intVector2_3, intVector2_4);
            IntVector2 intVector2_6 = IntVector2.Max(intVector2_4, intVector2_3);
            IntVector2 second;
            IntVector2 first;
            if (!exit.jointedExit && (referencedExit.exitDirection == DungeonData.Direction.NORTH || referencedExit.exitDirection == DungeonData.Direction.SOUTH))
            {
                second = intVector2_6 + new IntVector2(1, 0);
                first = intVector2_5 - new IntVector2(1, 0);
            }
            else
            {
                second = intVector2_6 + new IntVector2(2, 3);
                first = intVector2_5 - new IntVector2(2, 2);
            }
            return new Tuple<IntVector2, IntVector2>(first, second);
        }

        private bool CheckRectAgainstLayout(
            Tuple<IntVector2, IntVector2> rectTuple,
            SemioticLayoutManager layout)
        {
            for (int x = rectTuple.First.x; x < rectTuple.Second.x; ++x)
            {
                for (int y = rectTuple.First.y; y < rectTuple.Second.y; ++y)
                {
                    IntVector2 intVector2 = new IntVector2(x, y);
                    if (layout.m_occupiedCells.Contains(intVector2))
                        return false;
                }
            }
            return true;
        }

        [DebuggerHidden]
        public IEnumerable<ProcessStatus> CheckExitsAgainstDisparateLayouts(
            SemioticLayoutManager otherLayout,
            RuntimeRoomExitData staticExit,
            IntVector2 staticAreaBasePosition,
            RuntimeRoomExitData newExit,
            IntVector2 newAreaBasePosition)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            SemioticLayoutManager__CheckExitsAgainstDisparateLayoutsc__Iterator1 layoutsCIterator1 = new SemioticLayoutManager__CheckExitsAgainstDisparateLayoutsc__Iterator1()
            {
                staticAreaBasePosition = staticAreaBasePosition,
                staticExit = staticExit,
                newAreaBasePosition = newAreaBasePosition,
                newExit = newExit,
                otherLayout = otherLayout,
                _this = this
            };
            // ISSUE: reference to a compiler-generated field
            layoutsCIterator1._PC = -2;
            return (IEnumerable<ProcessStatus>) layoutsCIterator1;
        }

        private bool CheckExitClearForPlacement(
            RuntimeRoomExitData exit,
            IntVector2 areaBasePosition,
            bool debugMode = false,
            SemioticLayoutManager debugManager = null)
        {
            PrototypeRoomExit referencedExit = exit.referencedExit;
            IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(referencedExit.exitDirection);
            int num = !exit.jointedExit || referencedExit.exitDirection == DungeonData.Direction.WEST ? 0 : 1;
            for (int index1 = 0; index1 < referencedExit.containedCells.Count; ++index1)
            {
                for (int index2 = 3; index2 < exit.TotalExitLength + num; ++index2)
                {
                    IntVector2 intVector2 = referencedExit.containedCells[index1].ToIntVector2() - IntVector2.One + areaBasePosition + vector2FromDirection * index2;
                    if (this.m_occupiedCells.Contains(intVector2))
                        return false;
                    for (int index3 = 0; index3 < SemioticLayoutManager.LayoutCardinals.Length; ++index3)
                    {
                        if (this.m_occupiedCells.Contains(intVector2 + SemioticLayoutManager.LayoutCardinals[index3]))
                            return false;
                    }
                }
            }
            return true;
        }

        private bool CheckExitsClearForPlacement(RuntimeRoomExitData exitToTest, IntVector2 attachPoint)
        {
            IntVector2 areaBasePosition = attachPoint - (exitToTest.ExitOrigin - IntVector2.One);
            return this.CheckExitClearForPlacement(exitToTest, areaBasePosition);
        }

        private bool CheckExitsClearForPlacement(
            PrototypeDungeonRoom newRoom,
            RuntimeRoomExitData exitToTest,
            IntVector2 basePositionOfPreviousRoom,
            RuntimeRoomExitData previousExit,
            IntVector2 attachPoint)
        {
            IntVector2 areaBasePosition = attachPoint - (exitToTest.ExitOrigin - IntVector2.One);
            return this.CheckExitClearForPlacement(exitToTest, areaBasePosition);
        }

        private bool CheckExitsClearForPlacement2(
            PrototypeDungeonRoom newRoom,
            RuntimeRoomExitData exitToTest,
            IntVector2 basePositionOfPreviousRoom,
            RuntimeRoomExitData previousExit,
            IntVector2 attachPoint)
        {
            IntVector2 areaBasePosition1 = attachPoint - (exitToTest.ExitOrigin - IntVector2.One);
            IntVector2 areaBasePosition2 = attachPoint - (previousExit.ExitOrigin - IntVector2.One);
            Tuple<IntVector2, IntVector2> exitRectCells1 = this.GetExitRectCells(exitToTest, areaBasePosition1);
            Tuple<IntVector2, IntVector2> exitRectCells2 = this.GetExitRectCells(previousExit, areaBasePosition2);
            return this.CheckRectAgainstLayout(exitRectCells1, this) && this.CheckRectAgainstLayout(exitRectCells2, this);
        }

        [DebuggerHidden]
        public IEnumerable CanPlaceLayoutAtPoint(
            SemioticLayoutManager layout,
            RuntimeRoomExitData staticExit,
            RuntimeRoomExitData newExit,
            IntVector2 staticAreaBasePosition,
            IntVector2 newAreaBasePosition)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            SemioticLayoutManager__CanPlaceLayoutAtPointc__Iterator2 atPointCIterator2 = new SemioticLayoutManager__CanPlaceLayoutAtPointc__Iterator2()
            {
                staticExit = staticExit,
                newExit = newExit,
                staticAreaBasePosition = staticAreaBasePosition,
                newAreaBasePosition = newAreaBasePosition,
                layout = layout,
                _this = this
            };
            // ISSUE: reference to a compiler-generated field
            atPointCIterator2._PC = -2;
            return (IEnumerable) atPointCIterator2;
        }

        public bool CanPlaceRoomAtAttachPointByExit2(
            PrototypeDungeonRoom newRoom,
            RuntimeRoomExitData exitToTest,
            IntVector2 basePositionOfPreviousRoom,
            RuntimeRoomExitData previousExit)
        {
            exitToTest.additionalExitLength = 0;
            previousExit.additionalExitLength = 0;
            Tuple<PrototypeRoomExit, PrototypeRoomExit> tuple = new Tuple<PrototypeRoomExit, PrototypeRoomExit>(exitToTest.referencedExit, previousExit.referencedExit);
            bool flag1 = false;
            if ((tuple.First.exitDirection == DungeonData.Direction.NORTH || tuple.First.exitDirection == DungeonData.Direction.SOUTH) && (tuple.Second.exitDirection == DungeonData.Direction.EAST || tuple.Second.exitDirection == DungeonData.Direction.WEST) || (tuple.Second.exitDirection == DungeonData.Direction.NORTH || tuple.Second.exitDirection == DungeonData.Direction.SOUTH) && (tuple.First.exitDirection == DungeonData.Direction.EAST || tuple.First.exitDirection == DungeonData.Direction.WEST))
                flag1 = true;
            if (flag1)
                exitToTest.additionalExitLength = 3;
            IntVector2 intVector2_1 = new IntVector2(exitToTest.additionalExitLength, previousExit.additionalExitLength);
            for (int t = 0; t < 7; ++t)
            {
                int num1 = Mathf.RoundToInt(Mathf.PingPong((float) t, 4f));
                IntVector2 intVector2_2 = new IntVector2(Mathf.Clamp(t - 4, 0, 3), Mathf.Clamp(t, 0, 3));
                for (int index1 = 0; index1 < num1; ++index1)
                {
                    IntVector2 intVector2_3 = intVector2_2 + new IntVector2(1, -1) * index1;
                    exitToTest.additionalExitLength = intVector2_1.x + intVector2_3.x;
                    previousExit.additionalExitLength = intVector2_1.y + intVector2_3.y;
                    IntVector2 attachPoint = basePositionOfPreviousRoom + previousExit.ExitOrigin - IntVector2.One;
                    IntVector2 intVector2_4 = attachPoint - (exitToTest.ExitOrigin - IntVector2.One);
                    int num2 = intVector2_4.x - 1;
                    int num3 = intVector2_4.x + newRoom.Width + 2;
                    int num4 = intVector2_4.y - 1;
                    int num5 = intVector2_4.y + newRoom.Height + 4;
                    bool flag2 = false;
                    if (this.CheckExitsClearForPlacement2(newRoom, exitToTest, basePositionOfPreviousRoom, previousExit, attachPoint))
                    {
                        bool flag3 = true;
                        for (int index2 = 0; index2 < this.m_allRooms.Count; ++index2)
                        {
                            CellArea area = this.m_allRooms[index2].area;
                            int num6 = area.basePosition.x - 1;
                            int num7 = area.basePosition.x + area.dimensions.x + 2;
                            int num8 = area.basePosition.y - 1;
                            int num9 = area.basePosition.y + area.dimensions.y + 4;
                            if (num2 < num7 && num3 > num6 && num4 < num9 && num5 > num8)
                            {
                                flag3 = false;
                                break;
                            }
                        }
                        if (!flag3 || true)
                        {
                            List<IntVector2> representationIncFacewalls = newRoom.GetCellRepresentationIncFacewalls();
                            for (int index3 = 0; index3 < representationIncFacewalls.Count && !flag2; ++index3)
                            {
                                if (this.m_occupiedCells.Contains(intVector2_4 + representationIncFacewalls[index3]))
                                {
                                    flag2 = true;
                                    break;
                                }
                            }
                        }
                        if (!flag2)
                        {
                            if (flag1)
                            {
                                exitToTest.jointedExit = true;
                                previousExit.jointedExit = true;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CanPlaceRawCellPositions(List<IntVector2> positions)
        {
            for (int index1 = 0; index1 < positions.Count; ++index1)
            {
                IntVector2 position = positions[index1];
                if (this.m_occupiedCells.Contains(position))
                    return false;
                for (int index2 = 0; index2 < SemioticLayoutManager.LayoutPathCardinals.Length; ++index2)
                {
                    if (this.m_occupiedCells.Contains(position + SemioticLayoutManager.LayoutPathCardinals[index2]))
                        return false;
                }
            }
            return true;
        }

        [DebuggerHidden]
        public IEnumerable<ProcessStatus> CanPlaceRoomAtAttachPointByExit(
            PrototypeDungeonRoom newRoom,
            RuntimeRoomExitData exitToTest,
            IntVector2 basePositionOfPreviousRoom,
            RuntimeRoomExitData previousExit)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            SemioticLayoutManager__CanPlaceRoomAtAttachPointByExitc__Iterator3 byExitCIterator3 = new SemioticLayoutManager__CanPlaceRoomAtAttachPointByExitc__Iterator3()
            {
                exitToTest = exitToTest,
                previousExit = previousExit,
                basePositionOfPreviousRoom = basePositionOfPreviousRoom,
                newRoom = newRoom,
                _this = this
            };
            // ISSUE: reference to a compiler-generated field
            byExitCIterator3._PC = -2;
            return (IEnumerable<ProcessStatus>) byExitCIterator3;
        }

        public bool CanPlaceRoomAtAttachPointByExit(
            PrototypeDungeonRoom newRoom,
            PrototypeRoomExit exitToTest,
            IntVector2 attachPoint)
        {
            IntVector2 intVector2 = attachPoint - (exitToTest.GetExitOrigin(exitToTest.exitLength) - IntVector2.One);
            int num1 = intVector2.x - 1;
            int num2 = intVector2.x + newRoom.Width + 2;
            int num3 = intVector2.y - 1;
            int num4 = intVector2.y + newRoom.Height + 4;
            bool flag1 = false;
            if (!this.CheckExitsClearForPlacement(newRoom, new RuntimeRoomExitData(exitToTest), attachPoint))
                return false;
            bool flag2 = true;
            for (int index = 0; index < this.m_allRooms.Count; ++index)
            {
                CellArea area = this.m_allRooms[index].area;
                int num5 = area.basePosition.x - 1;
                int num6 = area.basePosition.x + area.dimensions.x + 2;
                int num7 = area.basePosition.y - 1;
                int num8 = area.basePosition.y + area.dimensions.y + 4;
                if (num1 < num6 && num2 > num5 && num3 < num8 && num4 > num7)
                {
                    flag2 = false;
                    break;
                }
            }
            if (!flag2)
            {
                List<IntVector2> representationIncFacewalls = newRoom.GetCellRepresentationIncFacewalls();
                for (int index = 0; index < representationIncFacewalls.Count && !flag1; ++index)
                {
                    if (this.m_occupiedCells.Contains(intVector2 + representationIncFacewalls[index]))
                    {
                        flag1 = true;
                        break;
                    }
                }
            }
            return !flag1;
        }

        public void ReinitializeFromLayout(SemioticLayoutManager snapshot)
        {
            this.m_allRooms = new List<RoomHandler>((IEnumerable<RoomHandler>) snapshot.m_allRooms);
            this.m_occupiedCells = new HashSet<IntVector2>((IEnumerable<IntVector2>) snapshot.m_occupiedCells);
            this.m_currentOffset = snapshot.m_currentOffset;
        }

        public void MergeLayout(SemioticLayoutManager other)
        {
            for (int index = 0; index < other.Rooms.Count; ++index)
                this.m_allRooms.Add(other.Rooms[index]);
            foreach (IntVector2 occupiedCell in other.m_occupiedCells)
                this.m_occupiedCells.Add(other.m_currentOffset + occupiedCell);
            if (this.m_rectangleDecomposition == null)
                return;
            this.m_rectangleDecomposition.Clear();
        }

        public bool CanPlacePathHallway(List<IntVector2> cellPositions)
        {
            for (int index = 0; index < cellPositions.Count; ++index)
            {
                if (this.m_occupiedCells.Contains(cellPositions[index]))
                    return false;
            }
            return true;
        }

        public bool CanPlaceRectangle(IntRect rectangle)
        {
            for (int x = rectangle.Left - 1; x < rectangle.Right + 1; ++x)
            {
                for (int y = rectangle.Bottom - 1; y < rectangle.Top + 1; ++y)
                {
                    if (this.m_occupiedCells.Contains(new IntVector2(x, y)))
                        return false;
                }
            }
            return true;
        }

        private bool CheckCellAndNeighborsOccupied(IntVector2 position)
        {
            if (this.m_occupiedCells.Contains(position))
                return true;
            for (int index = 0; index < 4; ++index)
            {
                if (this.m_occupiedCells.Contains(position + IntVector2.Cardinals[index]))
                    return true;
            }
            return false;
        }

        public List<IntVector2> PathfindHallway(IntVector2 startPosition, IntVector2 endPosition)
        {
            return this.PathfindHallwayCompact(startPosition, IntVector2.Zero, endPosition);
        }

        public List<IntVector2> PathfindHallwayCompact(
            IntVector2 startPosition,
            IntVector2 startDirection,
            IntVector2 endPosition)
        {
            IntVector2 intVector2_1 = IntVector2.Min(startPosition, endPosition);
            IntVector2 intVector2_2 = IntVector2.Max(startPosition, endPosition) - intVector2_1;
            IntVector2 intVector2_3 = intVector2_1 * -1;
            IntVector2 intVector2_4 = new IntVector2(4, 4);
            int a = intVector2_2.x + intVector2_4.x * 2;
            int b = intVector2_2.y + intVector2_4.y * 2;
            int length = Mathf.NextPowerOfTwo(Mathf.Max(a, b));
            byte[,] grid = new byte[length, length];
            byte num1 = 0;
            byte num2 = 1;
            for (int index1 = 0; index1 < grid.GetLength(0); ++index1)
            {
                for (int index2 = 0; index2 < grid.GetLength(1); ++index2)
                    grid[index1, index2] = index1 > a || index2 > b ? num1 : num2;
            }
            foreach (IntVector2 occupiedCell in this.m_occupiedCells)
            {
                int num3 = occupiedCell.x + intVector2_3.x + intVector2_4.x;
                int num4 = occupiedCell.y + intVector2_3.y + intVector2_4.y;
                if (num3 >= 3 && num3 < a - 3 && num4 >= 3 && num4 < b - 3)
                {
                    for (int index3 = -3; index3 < 4; ++index3)
                    {
                        for (int index4 = -3; index4 < 4; ++index4)
                            grid[num3 + index3, num4 + index4] = num1;
                    }
                }
            }
            foreach (IntVector2 temporaryPathfindingWall in this.m_temporaryPathfindingWalls)
            {
                IntVector2 intVector2_5 = temporaryPathfindingWall + intVector2_3 + intVector2_4;
                if (intVector2_5.x >= 1 && intVector2_5.x <= grid.GetLength(0) - 2 && intVector2_5.y >= 1 && intVector2_5.y <= grid.GetLength(1) - 4)
                    grid[intVector2_5.x, intVector2_5.y] = num1;
            }
            FastDungeonLayoutPathfinder layoutPathfinder = new FastDungeonLayoutPathfinder(grid);
            layoutPathfinder.Diagonals = false;
            layoutPathfinder.PunishChangeDirection = true;
            layoutPathfinder.TieBreaker = true;
            IntVector2 start = startPosition + intVector2_3 + intVector2_4;
            IntVector2 end = endPosition + intVector2_3 + intVector2_4;
            List<PathFinderNode> path = layoutPathfinder.FindPath(start, startDirection, end);
            if (path == null || path.Count == 0)
                return (List<IntVector2>) null;
            List<IntVector2> intVector2List = new List<IntVector2>();
            for (int index = 0; index < path.Count; ++index)
            {
                IntVector2 intVector2_6 = new IntVector2(path[index].X, path[index].Y) - intVector2_3 - intVector2_4;
                intVector2List.Add(intVector2_6);
            }
            return intVector2List;
        }

        public List<IntVector2> PathfindHallway(
            IntVector2 startPosition,
            IntVector2 startDirection,
            IntVector2 endPosition)
        {
            Tuple<IntVector2, IntVector2> maxCellPositions = this.GetMinAndMaxCellPositions();
            IntVector2 intVector2_1 = maxCellPositions.Second - maxCellPositions.First;
            IntVector2 intVector2_2 = IntVector2.Max(IntVector2.Zero, IntVector2.Zero - maxCellPositions.First);
            IntVector2 intVector2_3 = new IntVector2(8, 8);
            int length = Mathf.NextPowerOfTwo(Mathf.Max(intVector2_1.x + intVector2_3.x * 2, intVector2_1.y + intVector2_3.y * 2));
            byte[,] grid = new byte[length, length];
            byte num1 = 0;
            byte num2 = 1;
            for (int index1 = 0; index1 < grid.GetLength(0); ++index1)
            {
                for (int index2 = 0; index2 < grid.GetLength(1); ++index2)
                    grid[index1, index2] = num2;
            }
            foreach (IntVector2 occupiedCell in this.m_occupiedCells)
            {
                int num3 = occupiedCell.x + intVector2_2.x + intVector2_3.x;
                int num4 = occupiedCell.y + intVector2_2.y + intVector2_3.y;
                for (int index3 = -3; index3 < 4; ++index3)
                {
                    for (int index4 = -3; index4 < 4; ++index4)
                        grid[num3 + index3, num4 + index4] = num1;
                }
            }
            foreach (IntVector2 temporaryPathfindingWall in this.m_temporaryPathfindingWalls)
            {
                IntVector2 intVector2_4 = temporaryPathfindingWall + intVector2_2 + intVector2_3;
                if (intVector2_4.x >= 1 && intVector2_4.x <= grid.GetLength(0) - 2 && intVector2_4.y >= 1 && intVector2_4.y <= grid.GetLength(1) - 4)
                    grid[intVector2_4.x, intVector2_4.y] = num1;
            }
            FastDungeonLayoutPathfinder layoutPathfinder = new FastDungeonLayoutPathfinder(grid);
            layoutPathfinder.Diagonals = false;
            layoutPathfinder.PunishChangeDirection = true;
            layoutPathfinder.TieBreaker = true;
            IntVector2 start = startPosition + intVector2_2 + intVector2_3;
            IntVector2 end = endPosition + intVector2_2 + intVector2_3;
            List<PathFinderNode> path = layoutPathfinder.FindPath(start, startDirection, end);
            if (path == null || path.Count == 0)
                return (List<IntVector2>) null;
            List<IntVector2> intVector2List = new List<IntVector2>();
            for (int index = 0; index < path.Count; ++index)
            {
                IntVector2 intVector2_5 = new IntVector2(path[index].X, path[index].Y) - intVector2_2 - intVector2_3;
                intVector2List.Add(intVector2_5);
            }
            return intVector2List;
        }

        public List<IntVector2> TraceHallway(
            IntVector2 startPosition,
            IntVector2 endPosition,
            DungeonData.Direction currentHallwayDirection,
            DungeonData.Direction endHallwayDirection)
        {
            IntVector2 intVector2_1 = startPosition;
            HashSet<IntVector2> source = new HashSet<IntVector2>();
            for (int index = 0; index < 3; ++index)
            {
                source.Add(intVector2_1);
                source.Add(intVector2_1 + IntVector2.Up);
                source.Add(intVector2_1 + IntVector2.Right);
                source.Add(intVector2_1 + IntVector2.Up + IntVector2.Right);
                intVector2_1 += DungeonData.GetIntVector2FromDirection(currentHallwayDirection);
                source.Add(endPosition);
                source.Add(endPosition + IntVector2.Up);
                source.Add(endPosition + IntVector2.Right);
                source.Add(endPosition + IntVector2.Up + IntVector2.Right);
                endPosition += DungeonData.GetIntVector2FromDirection(endHallwayDirection);
            }
            IntVector2 intVector2_2 = endPosition - intVector2_1;
            DungeonData.Direction dir1 = intVector2_2.x <= 0 ? DungeonData.Direction.WEST : DungeonData.Direction.EAST;
            DungeonData.Direction dir2 = intVector2_2.y <= 0 ? DungeonData.Direction.SOUTH : DungeonData.Direction.NORTH;
            IntVector2 vector2FromDirection1 = DungeonData.GetIntVector2FromDirection(dir1);
            IntVector2 vector2FromDirection2 = DungeonData.GetIntVector2FromDirection(dir2);
            if (currentHallwayDirection != dir1 && currentHallwayDirection != dir2)
                return (List<IntVector2>) null;
            bool flag1 = true;
            DungeonData.Direction direction = currentHallwayDirection;
            int num = 0;
            while (intVector2_1 != endPosition && num < 200)
            {
                ++num;
                bool flag2 = dir1 == direction;
                intVector2_2 = endPosition - intVector2_1;
                if (flag2)
                {
                    bool flag3 = true;
                    bool flag4 = true;
                    if (intVector2_2.x == 0 || (double) Mathf.Sign((float) intVector2_2.x) != (double) Mathf.Sign((float) vector2FromDirection1.x))
                        flag3 = false;
                    else if (this.CheckCellAndNeighborsOccupied(intVector2_1 + vector2FromDirection1) || this.CheckCellAndNeighborsOccupied(intVector2_1 + vector2FromDirection1 + IntVector2.Up))
                        flag3 = false;
                    if (flag3)
                    {
                        intVector2_1 += vector2FromDirection1;
                        source.Add(intVector2_1);
                        source.Add(intVector2_1 + IntVector2.Right);
                        source.Add(intVector2_1 + IntVector2.Up);
                        source.Add(intVector2_1 + IntVector2.Right + IntVector2.Up);
                        direction = dir1;
                        continue;
                    }
                    if (intVector2_2.y == 0 || (double) Mathf.Sign((float) intVector2_2.y) != (double) Mathf.Sign((float) vector2FromDirection2.y))
                        flag4 = false;
                    else if (this.CheckCellAndNeighborsOccupied(intVector2_1 + vector2FromDirection2) || this.CheckCellAndNeighborsOccupied(intVector2_1 + vector2FromDirection2 + IntVector2.Right))
                        flag4 = false;
                    if (flag4)
                    {
                        intVector2_1 += vector2FromDirection2;
                        source.Add(intVector2_1);
                        source.Add(intVector2_1 + IntVector2.Right);
                        source.Add(intVector2_1 + IntVector2.Up);
                        source.Add(intVector2_1 + IntVector2.Right + IntVector2.Up);
                        direction = dir2;
                        continue;
                    }
                }
                else
                {
                    bool flag5 = true;
                    bool flag6 = true;
                    if (intVector2_2.y == 0 || (double) Mathf.Sign((float) intVector2_2.y) != (double) Mathf.Sign((float) vector2FromDirection2.y))
                        flag6 = false;
                    else if (this.CheckCellAndNeighborsOccupied(intVector2_1 + vector2FromDirection2) || this.CheckCellAndNeighborsOccupied(intVector2_1 + vector2FromDirection2 + IntVector2.Right))
                        flag6 = false;
                    if (flag6)
                    {
                        intVector2_1 += vector2FromDirection2;
                        source.Add(intVector2_1);
                        source.Add(intVector2_1 + IntVector2.Right);
                        source.Add(intVector2_1 + IntVector2.Up);
                        source.Add(intVector2_1 + IntVector2.Right + IntVector2.Up);
                        direction = dir2;
                        continue;
                    }
                    if (intVector2_2.x == 0 || (double) Mathf.Sign((float) intVector2_2.x) != (double) Mathf.Sign((float) vector2FromDirection1.x))
                        flag5 = false;
                    else if (this.CheckCellAndNeighborsOccupied(intVector2_1 + vector2FromDirection1) || this.CheckCellAndNeighborsOccupied(intVector2_1 + vector2FromDirection1 + IntVector2.Up))
                        flag5 = false;
                    if (flag5)
                    {
                        intVector2_1 += vector2FromDirection1;
                        source.Add(intVector2_1);
                        source.Add(intVector2_1 + IntVector2.Right);
                        source.Add(intVector2_1 + IntVector2.Up);
                        source.Add(intVector2_1 + IntVector2.Right + IntVector2.Up);
                        direction = dir1;
                        continue;
                    }
                }
                flag1 = false;
                break;
            }
            if (num > 10000)
                UnityEngine.Debug.LogError((object) "FUCK FUCK FUCK");
            return flag1 ? source.ToList<IntVector2>() : (List<IntVector2>) null;
        }

        public Dictionary<int, int> DetermineViableExpanse(
            IntVector2 connectionCenter,
            DungeonData.Direction direction)
        {
            Dictionary<int, int> viableExpanse = new Dictionary<int, int>();
            int val1 = int.MaxValue;
            for (int index = 1; index <= this.MAXIMUM_ROOM_DIMENSION; ++index)
            {
                int num = 0;
                for (int halfWidth = 1; halfWidth <= this.MAXIMUM_ROOM_DIMENSION / 2; ++halfWidth)
                {
                    IntVector2 cellToCheck1 = this.GetCellToCheck(connectionCenter, index, halfWidth, false, direction);
                    IntVector2 cellToCheck2 = this.GetCellToCheck(connectionCenter, index, halfWidth, true, direction);
                    if (this.m_occupiedCells.Contains(cellToCheck1) || this.m_occupiedCells.Contains(cellToCheck2))
                    {
                        if (viableExpanse.ContainsKey(index - 1))
                        {
                            viableExpanse.Remove(index - 1);
                            break;
                        }
                        break;
                    }
                    num = halfWidth - 1;
                }
                val1 = Math.Min(val1, num * 2);
                if (num != 0)
                    viableExpanse.Add(index, val1);
                else
                    break;
            }
            return viableExpanse;
        }

        private IntVector2 GetCellToCheck(
            IntVector2 start,
            int extendMagnitude,
            int halfWidth,
            bool invert,
            DungeonData.Direction dir)
        {
            bool flag = false;
            int x = 0;
            int y = 0;
            switch (dir)
            {
                case DungeonData.Direction.NORTH:
                    flag = true;
                    y = start.y + extendMagnitude;
                    break;
                case DungeonData.Direction.EAST:
                    x = start.x + extendMagnitude;
                    break;
                case DungeonData.Direction.SOUTH:
                    flag = true;
                    y = start.y - extendMagnitude;
                    break;
                case DungeonData.Direction.WEST:
                    x = start.x - extendMagnitude;
                    break;
                default:
                    UnityEngine.Debug.LogError((object) "Switching on invalid direction in SemioticLayoutManager!");
                    break;
            }
            if (flag)
                x = !invert ? start.x - halfWidth : start.x + halfWidth;
            else
                y = !invert ? start.y - halfWidth : start.y + halfWidth;
            return new IntVector2(x, y);
        }

        public struct BBoxPrepassResults
        {
            public bool overlapping;
            public int numPairs;
            public int numPairsOverlapping;
            public int totalOverlapArea;
        }
    }
}
