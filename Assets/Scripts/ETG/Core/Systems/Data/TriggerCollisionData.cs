#nullable disable

public class TriggerCollisionData
  {
    public PixelCollider PixelCollider;
    public SpeculativeRigidbody SpecRigidbody;
    public bool FirstFrame = true;
    public bool ContinuedCollision;
    public bool Notified;

    public TriggerCollisionData(SpeculativeRigidbody specRigidbody, PixelCollider pixelCollider)
    {
      this.SpecRigidbody = specRigidbody;
      this.PixelCollider = pixelCollider;
    }

    public void Reset()
    {
      this.FirstFrame = false;
      this.ContinuedCollision = false;
      this.Notified = false;
    }
  }

