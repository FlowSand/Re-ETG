using System;

#nullable disable

[Serializable]
public class BagelColliderData
  {
    public BagelCollider[] bagelColliders;

    public BagelColliderData(BagelCollider[] bcs) => this.bagelColliders = bcs;
  }

