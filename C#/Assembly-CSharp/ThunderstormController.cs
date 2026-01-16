// Decompiled with JetBrains decompiler
// Type: ThunderstormController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ThunderstormController : MonoBehaviour
{
  public bool DoLighting = true;
  public float MinTimeBetweenLightningStrikes = 5f;
  public float MaxTimeBetweenLightningStrikes = 10f;
  public Transform RainSystemTransform;
  public ScreenShakeSettings ThunderShake;
  public bool TrackCamera = true;
  public bool DecayVertical;
  public Vector2 DecayYRange;
  public bool DecayTrackPlayer;
  public Renderer[] LightningRenderers;
  public bool ModifyAmbient;
  public float AmbientBoost = 0.25f;
  public float ZOffset = -20f;
  private Transform m_mainCameraTransform;
  private Vector3 m_lastCameraPosition;
  private ParticleSystem m_system;
  private ParticleSystem.Particle[] m_particles;
  private float m_cachedEmissionRate;
  private Vector3 m_currentWindForce = Vector3.zero;
  private float m_lightningTimer;

  private void Start()
  {
    this.m_mainCameraTransform = GameManager.Instance.MainCameraController.transform;
    this.m_lastCameraPosition = this.m_mainCameraTransform.position;
    this.RainSystemTransform.position = this.m_mainCameraTransform.position + new Vector3(0.0f, 20f, 20f);
    this.m_lightningTimer = Random.Range(this.MinTimeBetweenLightningStrikes, this.MaxTimeBetweenLightningStrikes);
    this.m_system = this.RainSystemTransform.GetComponent<ParticleSystem>();
    this.m_cachedEmissionRate = this.m_system.emission.rate.constant;
    if (this.m_particles != null)
      return;
    this.m_particles = new ParticleSystem.Particle[this.m_system.maxParticles];
  }

  private void Update()
  {
    if (GameManager.Instance.IsLoadingLevel)
      return;
    if (this.TrackCamera)
    {
      Vector3 vector3 = this.m_mainCameraTransform.transform.position - this.m_lastCameraPosition;
      this.m_lastCameraPosition = this.m_mainCameraTransform.transform.position;
      this.RainSystemTransform.position += vector3;
      this.RainSystemTransform.position = this.RainSystemTransform.position.WithZ(this.RainSystemTransform.position.y + this.ZOffset);
      if (this.DecayVertical)
      {
        float y = this.m_lastCameraPosition.y;
        if (this.DecayTrackPlayer)
          y = GameManager.Instance.PrimaryPlayer.CenterPosition.y;
        BraveUtility.SetEmissionRate(this.m_system, this.m_cachedEmissionRate * Mathf.Lerp(1f, 0.0f, (float) (((double) y - (double) this.DecayYRange.x) / ((double) this.DecayYRange.y - (double) this.DecayYRange.x))));
      }
    }
    if ((double) this.m_system.emission.rate.constant > 0.0 && !TimeTubeCreditsController.IsTimeTubing && (bool) (Object) AmmonomiconController.Instance && !AmmonomiconController.Instance.IsOpen)
    {
      int num1 = (int) AkSoundEngine.PostEvent("Play_ENV_rain_loop_01", this.gameObject);
    }
    else
    {
      int num2 = (int) AkSoundEngine.PostEvent("Stop_ENV_rain_loop_01", this.gameObject);
    }
    if (this.DoLighting)
    {
      this.m_lightningTimer -= !GameManager.IsBossIntro ? BraveTime.DeltaTime : GameManager.INVARIANT_DELTA_TIME;
      if ((double) this.m_lightningTimer <= 0.0)
      {
        if (!this.DecayVertical || (double) this.m_lastCameraPosition.y < (double) this.DecayYRange.y)
          this.StartCoroutine(this.DoLightningStrike());
        for (int index = 0; index < this.LightningRenderers.Length; ++index)
          this.StartCoroutine(this.ProcessLightningRenderer(this.LightningRenderers[index]));
        if (this.ModifyAmbient)
          this.StartCoroutine(this.HandleLightningAmbientBoost());
        this.m_lightningTimer = Random.Range(this.MinTimeBetweenLightningStrikes, this.MaxTimeBetweenLightningStrikes);
      }
    }
    this.ProcessParticles();
  }

  private void ProcessParticles()
  {
    int particles = this.m_system.GetParticles(this.m_particles);
    this.m_currentWindForce = new Vector3(Mathf.Sin(UnityEngine.Time.timeSinceLevelLoad / 20f) * 7f, 0.0f, 0.0f);
    Vector3 vector3 = this.m_currentWindForce * (!GameManager.IsBossIntro ? BraveTime.DeltaTime : GameManager.INVARIANT_DELTA_TIME);
    for (int index = 0; index < particles; ++index)
      this.m_particles[index].velocity += vector3;
    this.m_system.SetParticles(this.m_particles, particles);
  }

  [DebuggerHidden]
  protected IEnumerator InvariantWait(float duration)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ThunderstormController.\u003CInvariantWait\u003Ec__Iterator0()
    {
      duration = duration
    };
  }

  [DebuggerHidden]
  protected IEnumerator HandleLightningAmbientBoost()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ThunderstormController.\u003CHandleLightningAmbientBoost\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  protected IEnumerator ProcessLightningRenderer(Renderer target)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ThunderstormController.\u003CProcessLightningRenderer\u003Ec__Iterator2()
    {
      target = target,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  protected IEnumerator DoLightningStrike()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ThunderstormController.\u003CDoLightningStrike\u003Ec__Iterator3()
    {
      \u0024this = this
    };
  }
}
