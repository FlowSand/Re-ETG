using System;
using System.Collections.Generic;

using Dungeonator;

#nullable disable

[Serializable]
public class PrototypeDungeonRoomCellData
    {
        public CellType state;
        public DiagonalWallType diagonalWallType;
        public bool breakable;
        public string str;
        public bool conditionalOnParentExit;
        public bool conditionalCellIsPit;
        public int parentExitIndex = -1;
        public bool containsManuallyPlacedLight;
        public int lightStampIndex;
        public int lightPixelsOffsetY;
        public bool doesDamage;
        public CellDamageDefinition damageDefinition;
        public int placedObjectRUBELIndex = -1;
        public List<int> additionalPlacedObjectIndices = new List<int>();
        public PrototypeDungeonRoomCellAppearance appearance;
        public bool ForceTileNonDecorated;

        public PrototypeDungeonRoomCellData()
        {
        }

        public PrototypeDungeonRoomCellData(string s, CellType st)
        {
            this.str = s;
            this.state = st;
        }

        public bool IsOccupied => this.placedObjectRUBELIndex >= 0;

        public bool HasChanges()
        {
            return this.diagonalWallType != DiagonalWallType.NONE || this.breakable || !string.IsNullOrEmpty(this.str) || this.conditionalOnParentExit || this.conditionalCellIsPit || this.parentExitIndex != -1 || this.containsManuallyPlacedLight || this.lightStampIndex != 0 || this.lightPixelsOffsetY != 0 || this.doesDamage || this.damageDefinition.HasChanges() || this.placedObjectRUBELIndex != -1 || this.additionalPlacedObjectIndices.Count != 0 || this.appearance != null && this.appearance.HasChanges() || this.ForceTileNonDecorated;
        }

        public bool IsOccupiedAtLayer(int layerIndex)
        {
            return this.state == CellType.WALL || this.additionalPlacedObjectIndices.Count > layerIndex && this.additionalPlacedObjectIndices[layerIndex] >= 0;
        }

        public void MirrorData(PrototypeDungeonRoomCellData source)
        {
            this.state = source.state;
            switch (source.diagonalWallType)
            {
                case DiagonalWallType.NONE:
                    this.diagonalWallType = DiagonalWallType.NONE;
                    break;
                case DiagonalWallType.NORTHEAST:
                    this.diagonalWallType = DiagonalWallType.NORTHWEST;
                    break;
                case DiagonalWallType.SOUTHEAST:
                    this.diagonalWallType = DiagonalWallType.SOUTHWEST;
                    break;
                case DiagonalWallType.SOUTHWEST:
                    this.diagonalWallType = DiagonalWallType.SOUTHEAST;
                    break;
                case DiagonalWallType.NORTHWEST:
                    this.diagonalWallType = DiagonalWallType.NORTHEAST;
                    break;
            }
            this.breakable = source.breakable;
            this.str = source.str;
            this.conditionalOnParentExit = source.conditionalOnParentExit;
            this.conditionalCellIsPit = source.conditionalCellIsPit;
            this.parentExitIndex = source.parentExitIndex;
            this.containsManuallyPlacedLight = source.containsManuallyPlacedLight;
            this.lightStampIndex = source.lightStampIndex;
            this.lightPixelsOffsetY = source.lightPixelsOffsetY;
            this.doesDamage = source.doesDamage;
            this.damageDefinition = source.damageDefinition;
            this.placedObjectRUBELIndex = source.placedObjectRUBELIndex;
            this.additionalPlacedObjectIndices = new List<int>();
            for (int index = 0; index < source.additionalPlacedObjectIndices.Count; ++index)
                this.additionalPlacedObjectIndices.Add(source.additionalPlacedObjectIndices[index]);
            this.appearance = new PrototypeDungeonRoomCellAppearance();
            this.appearance.MirrorData(source.appearance);
            this.ForceTileNonDecorated = source.ForceTileNonDecorated;
        }
    }

