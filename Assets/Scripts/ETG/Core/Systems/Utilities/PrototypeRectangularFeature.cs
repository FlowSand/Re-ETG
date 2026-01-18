using System;

#nullable disable

[Serializable]
public struct PrototypeRectangularFeature
    {
        public IntVector2 basePosition;
        public IntVector2 dimensions;

        public static PrototypeRectangularFeature CreateMirror(
            PrototypeRectangularFeature source,
            IntVector2 roomDimensions)
        {
            PrototypeRectangularFeature mirror = new PrototypeRectangularFeature()
            {
                dimensions = source.dimensions,
                basePosition = source.basePosition
            };
            mirror.basePosition.x = roomDimensions.x - (mirror.basePosition.x + mirror.dimensions.x);
            return mirror;
        }
    }

