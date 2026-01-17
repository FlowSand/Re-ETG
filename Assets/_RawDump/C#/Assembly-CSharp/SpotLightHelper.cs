// Decompiled with JetBrains decompiler
// Type: SpotLightHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SpotLightHelper : TimeInvariantMonoBehaviour
{
  [Header("Inital Position and Intensity")]
  public bool pointDirectlyAtFloor;
  public float zHeightOffset = -30f;
  [Header("Cookie Rotation")]
  public bool randomStartingRotation;
  public bool swayRotation;
  public float swaySpeed = 0.18f;
  public float rotationMin;
  public float rotationMax;
  [Header("Constant Rotation")]
  public float rotationSpeed;
  [Header("Inital Spot/Cookie Angle")]
  public bool randomStartingCookieAngle;
  public bool pulseCookieAngle;
  public float pulseCookieAngleHang = 1f;
  public float pulseCookieAngleSpeed4Real = 1f;
  public float cookieAngleMin;
  public float cookieAngleMax;
  [Header("Light Intensity")]
  public bool randomIntensity;
  public bool pulseIntensity;
  public float pulseIntensityHang;
  public float pulseIntensitySpeed4Real = 10f;
  public float intensityMin;
  public float intensityMax;
  [Header("Ambient Light Ping Pong")]
  public bool doPingPong;
  public Color startColor = Color.blue;
  public Color endColor = Color.red;
  public float pingPongTime = 2f;
  public float otherNumber = 1.5f;
  protected Transform m_transform;
  protected float magicNumberAngle;
  protected Light m_light;

  private void Start()
  {
    this.m_transform = this.transform;
    this.m_transform.position = this.m_transform.position.WithZ(this.m_transform.position.z + this.zHeightOffset);
    this.m_light = this.GetComponent<Light>();
    if (this.randomStartingRotation)
      this.m_transform.rotation = Quaternion.Euler(this.m_transform.rotation.x, Random.Range(this.rotationMin, this.rotationMax), this.m_transform.rotation.z);
    if (this.pointDirectlyAtFloor)
    {
      this.magicNumberAngle = 45f;
      this.m_transform.rotation = Quaternion.Euler(this.magicNumberAngle, this.m_transform.rotation.y, this.m_transform.rotation.z);
    }
    if (this.randomStartingCookieAngle)
      this.m_light.spotAngle = Random.Range(this.cookieAngleMin, this.cookieAngleMax);
    if (!this.randomIntensity)
      return;
    this.m_light.intensity = Random.Range(this.intensityMin, this.intensityMax);
  }

  protected override void InvariantUpdate(float realDeltaTime)
  {
    if ((double) this.rotationSpeed != 0.0)
      this.m_transform.Rotate(0.0f, 0.0f, this.rotationSpeed * realDeltaTime);
    if (this.swayRotation)
      this.m_transform.rotation = Quaternion.Lerp(Quaternion.Euler(this.magicNumberAngle, this.rotationMin, this.m_transform.rotation.z), Quaternion.Euler(this.magicNumberAngle, this.rotationMax, this.m_transform.rotation.z), (float) (0.5 * (1.0 + (double) Mathf.Sin((float) (3.1415927410125732 * (double) UnityEngine.Time.realtimeSinceStartup * ((double) this.swaySpeed / 10.0))))));
    if (this.pulseCookieAngle)
      this.m_light.spotAngle = Mathf.SmoothStep(this.cookieAngleMin, this.cookieAngleMax, Mathf.PingPong(UnityEngine.Time.time / this.pulseCookieAngleSpeed4Real, this.pulseCookieAngleHang));
    if (this.pulseIntensity)
      this.m_light.intensity = Mathf.SmoothStep(this.intensityMin, this.intensityMax, Mathf.PingPong(UnityEngine.Time.time / this.pulseIntensitySpeed4Real, this.pulseIntensityHang));
    if (!this.doPingPong)
      return;
    RenderSettings.ambientLight = Color.Lerp(this.startColor, this.endColor, Mathf.PingPong(UnityEngine.Time.time * this.otherNumber, this.pingPongTime));
  }

  protected override void OnDestroy() => base.OnDestroy();
}
