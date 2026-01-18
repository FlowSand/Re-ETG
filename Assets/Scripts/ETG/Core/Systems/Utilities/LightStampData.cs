using System;

#nullable disable

[Serializable]
public class LightStampData : ObjectStampData
    {
        public bool CanBeTopWallLight = true;
        public bool CanBeCenterLight = true;
        public int FallbackIndex;
    }

