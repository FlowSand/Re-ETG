#nullable disable

public class HelicopterController : BraveBehaviour
    {
        public void Start()
        {
            this.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox));
        }
    }

