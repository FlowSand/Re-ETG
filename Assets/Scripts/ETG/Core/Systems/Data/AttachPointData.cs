using System;

#nullable disable

[Serializable]
public class AttachPointData
    {
        public tk2dSpriteDefinition.AttachPoint[] attachPoints;

        public AttachPointData(tk2dSpriteDefinition.AttachPoint[] bcs) => this.attachPoints = bcs;
    }

