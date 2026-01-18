using System.Collections.Generic;

#nullable disable

public interface ICollidableObject
  {
    PixelCollider PrimaryPixelCollider { get; }

    bool CanCollideWith(SpeculativeRigidbody rigidbody);

    List<PixelCollider> GetPixelColliders();
  }

