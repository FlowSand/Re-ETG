using UnityEngine;

#nullable disable

public class TiltWorldHelper : BraveBehaviour
  {
    public float HeightOffGround;
    public bool DoForceLayer;
    public string ForceLayer = "Unoccluded";

    private void Update()
    {
      this.transform.position = this.transform.position.WithZ(this.transform.position.y - this.HeightOffGround);
      this.transform.rotation = Quaternion.identity;
      if (!this.DoForceLayer)
        return;
      this.gameObject.layer = LayerMask.NameToLayer(this.ForceLayer);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

