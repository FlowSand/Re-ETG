using UnityEngine;

#nullable disable

public class SpinningTrapController : TrapController
  {
    public GameObject baseObject;
    public GameObject spinningObject;
    public float secondsPerRotation;
    public bool doQuantize;
    public float multiplesOf;
    private float m_rotation;

    public void Update()
    {
      this.m_rotation += 360f * BraveTime.DeltaTime / this.secondsPerRotation;
      float z = this.m_rotation;
      if (this.doQuantize)
        z = BraveMathCollege.QuantizeFloat(this.m_rotation, this.multiplesOf);
      this.spinningObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, z);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

