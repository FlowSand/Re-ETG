// Decompiled with JetBrains decompiler
// Type: TriggerCollisionData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

