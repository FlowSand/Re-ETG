using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

[Serializable]
public class TileIndexGrid : ScriptableObject
    {
        public int roomTypeRestriction = -1;
        [TileIndexList]
        public TileIndexList topLeftIndices;
        [TileIndexList]
        public TileIndexList topIndices;
        [TileIndexList]
        public TileIndexList topRightIndices;
        [TileIndexList]
        public TileIndexList leftIndices;
        [TileIndexList]
        public TileIndexList centerIndices;
        [TileIndexList]
        public TileIndexList rightIndices;
        [TileIndexList]
        public TileIndexList bottomLeftIndices;
        [TileIndexList]
        public TileIndexList bottomIndices;
        [TileIndexList]
        public TileIndexList bottomRightIndices;
        [TileIndexList]
        public TileIndexList horizontalIndices;
        [TileIndexList]
        public TileIndexList verticalIndices;
        [TileIndexList]
        public TileIndexList topCapIndices;
        [TileIndexList]
        public TileIndexList rightCapIndices;
        [TileIndexList]
        public TileIndexList bottomCapIndices;
        [TileIndexList]
        public TileIndexList leftCapIndices;
        [TileIndexList]
        public TileIndexList allSidesIndices;
        [TileIndexList]
        public TileIndexList topLeftNubIndices;
        [TileIndexList]
        public TileIndexList topRightNubIndices;
        [TileIndexList]
        public TileIndexList bottomLeftNubIndices;
        [TileIndexList]
        public TileIndexList bottomRightNubIndices;
        public bool extendedSet;
        [TileIndexList]
        [Header("Extended Set")]
        public TileIndexList topCenterLeftIndices;
        [TileIndexList]
        public TileIndexList topCenterIndices;
        [TileIndexList]
        public TileIndexList topCenterRightIndices;
        [TileIndexList]
        public TileIndexList thirdTopRowLeftIndices;
        [TileIndexList]
        public TileIndexList thirdTopRowCenterIndices;
        [TileIndexList]
        public TileIndexList thirdTopRowRightIndices;
        [TileIndexList]
        public TileIndexList internalBottomLeftCenterIndices;
        [TileIndexList]
        public TileIndexList internalBottomCenterIndices;
        [TileIndexList]
        public TileIndexList internalBottomRightCenterIndices;
        [Header("Additional Borders")]
        [TileIndexList]
        public TileIndexList borderTopNubLeftIndices;
        [TileIndexList]
        public TileIndexList borderTopNubRightIndices;
        [TileIndexList]
        public TileIndexList borderTopNubBothIndices;
        [TileIndexList]
        public TileIndexList borderRightNubTopIndices;
        [TileIndexList]
        public TileIndexList borderRightNubBottomIndices;
        [TileIndexList]
        public TileIndexList borderRightNubBothIndices;
        [TileIndexList]
        public TileIndexList borderBottomNubLeftIndices;
        [TileIndexList]
        public TileIndexList borderBottomNubRightIndices;
        [TileIndexList]
        public TileIndexList borderBottomNubBothIndices;
        [TileIndexList]
        public TileIndexList borderLeftNubTopIndices;
        [TileIndexList]
        public TileIndexList borderLeftNubBottomIndices;
        [TileIndexList]
        public TileIndexList borderLeftNubBothIndices;
        [TileIndexList]
        public TileIndexList diagonalNubsTopLeftBottomRight;
        [TileIndexList]
        public TileIndexList diagonalNubsTopRightBottomLeft;
        [TileIndexList]
        public TileIndexList doubleNubsTop;
        [TileIndexList]
        public TileIndexList doubleNubsRight;
        [TileIndexList]
        public TileIndexList doubleNubsBottom;
        [TileIndexList]
        public TileIndexList doubleNubsLeft;
        [TileIndexList]
        public TileIndexList quadNubs;
        [TileIndexList]
        public TileIndexList topRightWithNub;
        [TileIndexList]
        public TileIndexList topLeftWithNub;
        [TileIndexList]
        public TileIndexList bottomRightWithNub;
        [TileIndexList]
        public TileIndexList bottomLeftWithNub;
        [Header("Diagonals--For Borders Only")]
        [TileIndexList]
        public TileIndexList diagonalBorderNE;
        [TileIndexList]
        public TileIndexList diagonalBorderSE;
        [TileIndexList]
        public TileIndexList diagonalBorderSW;
        [TileIndexList]
        public TileIndexList diagonalBorderNW;
        [TileIndexList]
        public TileIndexList diagonalCeilingNE;
        [TileIndexList]
        public TileIndexList diagonalCeilingSE;
        [TileIndexList]
        public TileIndexList diagonalCeilingSW;
        [TileIndexList]
        public TileIndexList diagonalCeilingNW;
        [Header("Carpet Options")]
        public bool CenterCheckerboard;
        public int CheckerboardDimension = 1;
        public bool CenterIndicesAreStrata;
        [Header("Weirdo Options")]
        [Space(5f)]
        public List<TileIndexGrid> PitInternalSquareGrids;
        [Space(5f)]
        public PitSquarePlacementOptions PitInternalSquareOptions;
        [Space(5f)]
        public bool PitBorderIsInternal;
        [Space(5f)]
        public bool PitBorderOverridesFloorTile;
        [Space(5f)]
        public bool CeilingBorderUsesDistancedCenters;
        [Header("For Rat Chunk Borders")]
        [Space(5f)]
        public bool UsesRatChunkBorders;
        [TileIndexList]
        public TileIndexList RatChunkNormalSet;
        [TileIndexList]
        public TileIndexList RatChunkBottomSet;
        [Header("Path Options")]
        [Space(5f)]
        public GameObject PathFacewallStamp;
        public GameObject PathSidewallStamp;
        [Space(5f)]
        [TileIndexList]
        public TileIndexList PathPitPosts;
        [TileIndexList]
        public TileIndexList PathPitPostsBL;
        [TileIndexList]
        public TileIndexList PathPitPostsBR;
        [Space(5f)]
        public GameObject PathStubNorth;
        public GameObject PathStubEast;
        public GameObject PathStubSouth;
        public GameObject PathStubWest;

        protected virtual int ProcessBenubbedTiles(
            bool isNorthBorder,
            bool isNortheastBorder,
            bool isEastBorder,
            bool isSoutheastBorder,
            bool isSouthBorder,
            bool isSouthwestBorder,
            bool isWestBorder,
            bool isNorthwestBorder)
        {
            if (!isNorthBorder && !isEastBorder && !isWestBorder && !isSouthBorder)
            {
                if (isNortheastBorder && isNorthwestBorder && isSoutheastBorder && isSouthwestBorder && this.quadNubs.ContainsValid())
                    return this.quadNubs.GetIndexByWeight();
                if (isNortheastBorder)
                {
                    if (isNorthwestBorder && this.doubleNubsTop.ContainsValid())
                        return this.doubleNubsTop.GetIndexByWeight();
                    if (isSoutheastBorder && this.doubleNubsRight.ContainsValid())
                        return this.doubleNubsRight.GetIndexByWeight();
                    if (isSouthwestBorder && this.diagonalNubsTopRightBottomLeft.ContainsValid())
                        return this.diagonalNubsTopRightBottomLeft.GetIndexByWeight();
                    if (this.topRightNubIndices.ContainsValid())
                        return this.topRightNubIndices.GetIndexByWeight();
                }
                if (isNorthwestBorder)
                {
                    if (isSouthwestBorder && this.doubleNubsLeft.ContainsValid())
                        return this.doubleNubsLeft.GetIndexByWeight();
                    if (isSoutheastBorder && this.diagonalNubsTopLeftBottomRight.ContainsValid())
                        return this.diagonalNubsTopLeftBottomRight.GetIndexByWeight();
                    if (this.topLeftNubIndices.ContainsValid())
                        return this.topLeftNubIndices.GetIndexByWeight();
                }
                if (isSoutheastBorder)
                {
                    if (isSouthwestBorder && this.doubleNubsBottom.ContainsValid())
                        return this.doubleNubsBottom.GetIndexByWeight();
                    if (this.bottomRightNubIndices.ContainsValid())
                        return this.bottomRightNubIndices.GetIndexByWeight();
                }
                if (isSouthwestBorder && this.bottomLeftNubIndices.ContainsValid())
                    return this.bottomLeftNubIndices.GetIndexByWeight();
            }
            if (isNorthBorder && !isEastBorder && !isSouthBorder && !isWestBorder)
            {
                if (isSoutheastBorder && isSouthwestBorder && this.borderTopNubBothIndices.ContainsValid())
                    return this.borderTopNubBothIndices.GetIndexByWeight();
                if (isSoutheastBorder && this.borderTopNubRightIndices.ContainsValid())
                    return this.borderTopNubRightIndices.GetIndexByWeight();
                if (isSouthwestBorder && this.borderTopNubLeftIndices.ContainsValid())
                    return this.borderTopNubLeftIndices.GetIndexByWeight();
            }
            if (!isNorthBorder && isEastBorder && !isSouthBorder && !isWestBorder)
            {
                if (isNorthwestBorder && isSouthwestBorder && this.borderRightNubBothIndices.ContainsValid())
                    return this.borderRightNubBothIndices.GetIndexByWeight();
                if (isSouthwestBorder && this.borderRightNubBottomIndices.ContainsValid())
                    return this.borderRightNubBottomIndices.GetIndexByWeight();
                if (isNorthwestBorder && this.borderRightNubTopIndices.ContainsValid())
                    return this.borderRightNubTopIndices.GetIndexByWeight();
            }
            if (!isNorthBorder && !isEastBorder && isSouthBorder && !isWestBorder)
            {
                if (isNortheastBorder && isNorthwestBorder && this.borderBottomNubBothIndices.ContainsValid())
                    return this.borderBottomNubBothIndices.GetIndexByWeight();
                if (isNorthwestBorder && this.borderBottomNubLeftIndices.ContainsValid())
                    return this.borderBottomNubLeftIndices.GetIndexByWeight();
                if (isNortheastBorder && this.borderBottomNubRightIndices.ContainsValid())
                    return this.borderBottomNubRightIndices.GetIndexByWeight();
            }
            if (!isNorthBorder && !isEastBorder && !isSouthBorder && isWestBorder)
            {
                if (isNortheastBorder && isSoutheastBorder && this.borderLeftNubBothIndices.ContainsValid())
                    return this.borderLeftNubBothIndices.GetIndexByWeight();
                if (isNortheastBorder && this.borderLeftNubTopIndices.ContainsValid())
                    return this.borderLeftNubTopIndices.GetIndexByWeight();
                if (isSoutheastBorder && this.borderLeftNubBottomIndices.ContainsValid())
                    return this.borderLeftNubBottomIndices.GetIndexByWeight();
            }
            if (isNorthBorder && isEastBorder && !isSouthBorder && !isWestBorder && isSouthwestBorder)
                return this.topRightWithNub.GetIndexByWeight();
            if (!isNorthBorder && isEastBorder && isSouthBorder && !isWestBorder && isNorthwestBorder)
                return this.bottomRightWithNub.GetIndexByWeight();
            if (!isNorthBorder && !isEastBorder && isSouthBorder && isWestBorder && isNortheastBorder)
                return this.bottomLeftWithNub.GetIndexByWeight();
            return isNorthBorder && !isEastBorder && !isSouthBorder && isWestBorder && isSoutheastBorder ? this.topLeftWithNub.GetIndexByWeight() : -1;
        }

        public virtual int GetIndexGivenEightSides(bool[] eightSides)
        {
            return this.GetIndexGivenSides(eightSides[0], eightSides[1], eightSides[2], eightSides[3], eightSides[4], eightSides[5], eightSides[6], eightSides[7]);
        }

        public virtual int GetStaticIndexGivenSides(
            bool isNorthBorder,
            bool isNortheastBorder,
            bool isEastBorder,
            bool isSoutheastBorder,
            bool isSouthBorder,
            bool isSouthwestBorder,
            bool isWestBorder,
            bool isNorthwestBorder)
        {
            UnityEngine.Random.InitState(147);
            int num = this.ProcessBenubbedTiles(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
            return num != -1 ? num : this.GetIndexGivenSides(isNorthBorder, isEastBorder, isSouthBorder, isWestBorder);
        }

        public virtual int GetIndexGivenSides(
            bool isNorthBorder,
            bool isNortheastBorder,
            bool isEastBorder,
            bool isSoutheastBorder,
            bool isSouthBorder,
            bool isSouthwestBorder,
            bool isWestBorder,
            bool isNorthwestBorder)
        {
            int num = this.ProcessBenubbedTiles(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
            return num != -1 ? num : this.GetIndexGivenSides(isNorthBorder, isEastBorder, isSouthBorder, isWestBorder);
        }

        public virtual int GetIndexGivenSides(List<CellData> cells, Func<CellData, bool> evalFunc)
        {
            return this.GetIndexGivenSides(evalFunc(cells[0]), evalFunc(cells[1]), evalFunc(cells[2]), evalFunc(cells[3]), evalFunc(cells[4]), evalFunc(cells[5]), evalFunc(cells[6]), evalFunc(cells[7]));
        }

        public virtual int GetRatChunkIndexGivenSidesStatic(
            bool isNorthBorder,
            bool isNortheastBorder,
            bool isEastBorder,
            bool isSoutheastBorder,
            bool isSouthBorder,
            bool isSouthwestBorder,
            bool isWestBorder,
            bool isNorthwestBorder,
            bool isTwoSouthEmpty,
            out TileIndexGrid.RatChunkResult result)
        {
            result = TileIndexGrid.RatChunkResult.NONE;
            if (isNorthBorder || isEastBorder || isSouthBorder || isWestBorder)
            {
                if (isNorthBorder && isTwoSouthEmpty || isEastBorder && isTwoSouthEmpty || isWestBorder && isTwoSouthEmpty)
                {
                    result = TileIndexGrid.RatChunkResult.BOTTOM;
                    return this.RatChunkBottomSet.indices[0];
                }
                result = TileIndexGrid.RatChunkResult.NORMAL;
                return this.RatChunkNormalSet.indices[0];
            }
            if (!isNortheastBorder && !isNorthwestBorder && !isSoutheastBorder && !isSouthwestBorder)
                return this.centerIndices.indices[0];
            result = TileIndexGrid.RatChunkResult.CORNER;
            return this.bottomRightNubIndices.indices[0];
        }

        public virtual int GetRatChunkIndexGivenSides(
            bool isNorthBorder,
            bool isNortheastBorder,
            bool isEastBorder,
            bool isSoutheastBorder,
            bool isSouthBorder,
            bool isSouthwestBorder,
            bool isWestBorder,
            bool isNorthwestBorder,
            bool isTwoSouthEmpty,
            out TileIndexGrid.RatChunkResult result)
        {
            result = TileIndexGrid.RatChunkResult.NONE;
            if (isNorthBorder || isEastBorder || isSouthBorder || isWestBorder)
            {
                if (isNorthBorder && isTwoSouthEmpty || isEastBorder && isTwoSouthEmpty || isWestBorder && isTwoSouthEmpty)
                {
                    result = TileIndexGrid.RatChunkResult.BOTTOM;
                    return this.RatChunkBottomSet.GetIndexByWeight();
                }
                result = TileIndexGrid.RatChunkResult.NORMAL;
                return this.RatChunkNormalSet.GetIndexByWeight();
            }
            if (!isNortheastBorder && !isNorthwestBorder && !isSoutheastBorder && !isSouthwestBorder)
                return this.centerIndices.GetIndexByWeight();
            result = TileIndexGrid.RatChunkResult.CORNER;
            return this.bottomRightNubIndices.GetIndexByWeight();
        }

        public virtual int GetIndexGivenSides(
            bool isNorthBorder,
            bool isEastBorder,
            bool isSouthBorder,
            bool isWestBorder)
        {
            return isNorthBorder && isEastBorder && isSouthBorder && isWestBorder ? (this.allSidesIndices.ContainsValid() ? this.allSidesIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isNorthBorder && isEastBorder && isWestBorder ? (this.topCapIndices.ContainsValid() ? this.topCapIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isEastBorder && isNorthBorder && isSouthBorder ? (this.rightCapIndices.ContainsValid() ? this.rightCapIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isSouthBorder && isEastBorder && isWestBorder ? (this.bottomCapIndices.ContainsValid() ? this.bottomCapIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isWestBorder && isSouthBorder && isNorthBorder ? (this.leftCapIndices.ContainsValid() ? this.leftCapIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isNorthBorder && isEastBorder ? (this.topRightIndices.ContainsValid() ? this.topRightIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isEastBorder && isSouthBorder ? (this.bottomRightIndices.ContainsValid() ? this.bottomRightIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isSouthBorder && isWestBorder ? (this.bottomLeftIndices.ContainsValid() ? this.bottomLeftIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isWestBorder && isNorthBorder ? (this.topLeftIndices.ContainsValid() ? this.topLeftIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isNorthBorder && isSouthBorder ? (this.horizontalIndices.ContainsValid() ? this.horizontalIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isEastBorder && isWestBorder ? (this.verticalIndices.ContainsValid() ? this.verticalIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isNorthBorder ? (this.topIndices.ContainsValid() ? this.topIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isEastBorder ? (this.rightIndices.ContainsValid() ? this.rightIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isSouthBorder ? (this.bottomIndices.ContainsValid() ? this.bottomIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()) : (isWestBorder && this.leftIndices.ContainsValid() ? this.leftIndices.GetIndexByWeight() : this.centerIndices.GetIndexByWeight()))))))))))))));
        }

        public virtual int GetIndexGivenSides(
            bool isNorthBorder,
            bool isSecondNorthBorder,
            bool isNortheastBorder,
            bool isEastBorder,
            bool isSoutheastBorder,
            bool isSouthBorder,
            bool isSouthwestBorder,
            bool isWestBorder,
            bool isNorthwestBorder)
        {
            if (!isNorthBorder && isSecondNorthBorder && this.extendedSet)
            {
                if (isEastBorder && this.topCenterRightIndices.ContainsValid())
                    return this.topCenterRightIndices.GetIndexByWeight();
                if (isWestBorder && this.topCenterLeftIndices.ContainsValid())
                    return this.topCenterLeftIndices.GetIndexByWeight();
                if (this.topCenterIndices.ContainsValid())
                    return this.topCenterIndices.GetIndexByWeight();
            }
            return this.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
        }

        public virtual int GetIndexGivenSides(
            bool isNorthBorder,
            bool isSecondNorthBorder,
            bool isThirdNorthBorder,
            bool isNortheastBorder,
            bool isEastBorder,
            bool isSoutheastBorder,
            bool isSouthBorder,
            bool isSouthwestBorder,
            bool isWestBorder,
            bool isNorthwestBorder)
        {
            if (!isNorthBorder && !isSecondNorthBorder && isThirdNorthBorder && this.extendedSet)
            {
                if (isEastBorder && this.thirdTopRowRightIndices.ContainsValid())
                    return this.thirdTopRowRightIndices.GetIndexByWeight();
                if (isWestBorder && this.thirdTopRowLeftIndices.ContainsValid())
                    return this.thirdTopRowLeftIndices.GetIndexByWeight();
                if (this.thirdTopRowCenterIndices.ContainsValid())
                    return this.thirdTopRowCenterIndices.GetIndexByWeight();
            }
            return this.GetIndexGivenSides(isNorthBorder, isSecondNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
        }

        public virtual int GetIndexGivenSides(
            bool isNorthBorder,
            bool isSecondNorthBorder,
            bool isEastBorder,
            bool isSouthBorder,
            bool isWestBorder)
        {
            if (!isNorthBorder && isSecondNorthBorder && this.extendedSet)
            {
                if (isEastBorder && this.topCenterRightIndices.ContainsValid())
                    return this.topCenterRightIndices.GetIndexByWeight();
                if (isWestBorder && this.topCenterLeftIndices.ContainsValid())
                    return this.topCenterLeftIndices.GetIndexByWeight();
                if (this.topCenterIndices.ContainsValid())
                    return this.topCenterIndices.GetIndexByWeight();
            }
            return this.GetIndexGivenSides(isNorthBorder, isEastBorder, isSouthBorder, isWestBorder);
        }

        public virtual int GetIndexGivenSides(
            bool isNorthBorder,
            bool isSecondNorthBorder,
            bool isThirdNorthBorder,
            bool isEastBorder,
            bool isSouthBorder,
            bool isWestBorder)
        {
            if (!isNorthBorder && !isSecondNorthBorder && isThirdNorthBorder && this.extendedSet)
            {
                if (isEastBorder && this.thirdTopRowRightIndices.ContainsValid())
                    return this.thirdTopRowRightIndices.GetIndexByWeight();
                if (isWestBorder && this.thirdTopRowLeftIndices.ContainsValid())
                    return this.thirdTopRowLeftIndices.GetIndexByWeight();
                if (this.thirdTopRowCenterIndices.ContainsValid())
                    return this.thirdTopRowCenterIndices.GetIndexByWeight();
            }
            return this.GetIndexGivenSides(isNorthBorder, isSecondNorthBorder, isEastBorder, isSouthBorder, isWestBorder);
        }

        public virtual int GetInternalIndexGivenSides(
            bool isNorthBorder,
            bool isEastBorder,
            bool isSouthBorder,
            bool isWestBorder)
        {
            if (!this.extendedSet || isSouthBorder)
                return -1;
            if (!isEastBorder)
                return this.internalBottomRightCenterIndices.GetIndexByWeight();
            return !isWestBorder ? this.internalBottomLeftCenterIndices.GetIndexByWeight() : this.internalBottomCenterIndices.GetIndexByWeight();
        }

        public enum RatChunkResult
        {
            NONE,
            NORMAL,
            BOTTOM,
            CORNER,
        }
    }

