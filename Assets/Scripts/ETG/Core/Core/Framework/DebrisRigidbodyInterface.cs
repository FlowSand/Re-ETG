// Decompiled with JetBrains decompiler
// Type: DebrisRigidbodyInterface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

