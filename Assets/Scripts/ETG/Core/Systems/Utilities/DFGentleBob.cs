using UnityEngine;

#nullable disable

public class DFGentleBob : MonoBehaviour
  {
    public int upPixels = 6;
    public int downPixels = 6;
    public float bounceSpeed = 1f;
    public bool Quantized;
    private dfControl m_control;
    private SpeculativeRigidbody m_rigidbody;
    private Transform m_transform;
    public bool BobDuringBossIntros;
    private Vector3 m_startAbsolutePosition;
    private Vector3 m_startRelativePosition;
    private float t;

    public Vector3 AbsoluteStartPosition
    {
      get => this.m_startAbsolutePosition;
      set => this.m_startAbsolutePosition = value;
    }

    private void Start()
    {
      this.m_transform = this.transform;
      this.m_control = this.GetComponent<dfControl>();
      this.m_rigidbody = this.GetComponent<SpeculativeRigidbody>();
      this.m_startAbsolutePosition = this.m_transform.position;
      if ((Object) this.m_control != (Object) null)
        this.m_startRelativePosition = this.m_control.RelativePosition;
      this.t = Random.value;
    }

    private void Update()
    {
      if ((double) this.t == 0.0)
        this.t = Random.value;
      float num = !this.BobDuringBossIntros || !GameManager.IsBossIntro ? BraveTime.DeltaTime : GameManager.INVARIANT_DELTA_TIME;
      this.t += num * this.bounceSpeed;
      if ((Object) this.m_control != (Object) null)
        this.m_control.RelativePosition = this.m_startRelativePosition + new Vector3(0.0f, Mathf.Lerp((float) this.upPixels, (float) this.downPixels, Mathf.SmoothStep(0.0f, 1f, Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(this.t, 1f)))), 0.0f);
      else if ((Object) this.m_rigidbody != (Object) null)
      {
        this.m_rigidbody.Velocity = ((this.m_startAbsolutePosition + new Vector3(0.0f, 1f / 16f * Mathf.Lerp((float) this.upPixels, (float) -this.downPixels, Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(this.t, 1f))), 0.0f).Quantize(1f / 16f)).XY() - this.transform.position.XY()) / num;
      }
      else
      {
        this.m_transform.position = this.m_startAbsolutePosition + new Vector3(0.0f, 1f / 16f * Mathf.Lerp((float) this.upPixels, (float) -this.downPixels, Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(this.t, 1f))), 0.0f);
        if (!this.Quantized)
          return;
        this.m_transform.position = this.m_transform.position.Quantize(1f / 16f);
      }
    }

    private void OnDisable()
    {
      if (!(bool) (Object) this.m_rigidbody)
        return;
      this.m_rigidbody.Velocity = Vector2.zero;
    }
  }

