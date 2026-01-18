#nullable disable

public class DebrisRigidbodyInterface : BraveBehaviour
  {
    public bool IsWall;
    public bool IsPit;

    private void Start()
    {
      if (this.IsWall)
        DebrisObject.SRB_Walls.Add(this.specRigidbody);
      if (!this.IsPit)
        return;
      this.specRigidbody.PrimaryPixelCollider.IsTrigger = true;
      DebrisObject.SRB_Pits.Add(this.specRigidbody);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

