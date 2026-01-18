using System;

#nullable disable

[Serializable]
public class BulletScriptSettings
    {
        public bool overrideMotion;
        public bool preventPooling;
        public bool surviveRigidbodyCollisions;
        public bool surviveTileCollisions;

        public BulletScriptSettings()
        {
        }

        public BulletScriptSettings(BulletScriptSettings other) => this.SetAll(other);

        public void SetAll(BulletScriptSettings other)
        {
            this.overrideMotion = other.overrideMotion;
            this.preventPooling = other.preventPooling;
            this.surviveRigidbodyCollisions = other.surviveRigidbodyCollisions;
            this.surviveTileCollisions = other.surviveTileCollisions;
        }
    }

