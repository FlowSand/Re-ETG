using UnityEngine;

#nullable disable

public class ComplexLightColorAnimator : MonoBehaviour
  {
    public Gradient colorGradient;
    public float period = 3f;
    public float timeOffset;
    private Light m_light;

    private void Start() => this.m_light = this.GetComponent<Light>();

    private void Update()
    {
      this.m_light.color = this.colorGradient.Evaluate((UnityEngine.Time.realtimeSinceStartup + this.timeOffset) % this.period / this.period);
    }
  }

