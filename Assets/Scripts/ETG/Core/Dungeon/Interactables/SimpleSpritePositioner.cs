using UnityEngine;

#nullable disable

public class SimpleSpritePositioner : DungeonPlaceableBehaviour
    {
        public float Rotation;

        public void Start()
        {
            this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, this.Rotation);
            if (!(bool) (Object) this.sprite)
                return;
            this.sprite.UpdateZDepth();
            this.sprite.ForceRotationRebuild();
        }
    }

