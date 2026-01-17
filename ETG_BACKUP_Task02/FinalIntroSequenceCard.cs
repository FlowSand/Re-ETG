// Decompiled with JetBrains decompiler
// Type: FinalIntroSequenceCard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class FinalIntroSequenceCard : MonoBehaviour
{
  public Transform StartCameraTransform;
  public Transform EndCameraTransform;
  public Renderer BGRenderer;
  public Renderer[] SpriteRenderers;
  public Renderer GunRenderer;
  public float StartHoldTime = 3f;
  public float PanTime = 5f;
  public float EndHoldTime = 3f;
  public float GunFadeDelay = 3f;
  public float GunFadeTime = 2.5f;
  public float CustomTextFadeInTime = -1f;
  public float CustomTextFadeOutTime = -1f;
  public string[] AssociatedKeys;
  public float[] AssociatedKeyTimes;
  public tk2dBaseSprite borderSprite;
  public AdditionalBraveLight[] additionalBraveLights;
  private float[] blIntensities;
  private float[] blRadii;
  private float m_elapsed;
  public float LightingFadeInDuration = 6f;
  public float LightingFadeOutDuration = 6f;
  public bool LightingReturnToNeutralGray;
  private bool m_hasLightingBeenEnabled;
  private bool m_gunBurn;
  public Transform clockhand1;
  public Transform clockhand2;
  private bool m_clockhandsInitialized;

  public string[] GetTargetKeys(float cardElapsed)
  {
    string[] targetKeys = new string[this.AssociatedKeys.Length];
    for (int index = 0; index < this.AssociatedKeyTimes.Length; ++index)
      targetKeys[index] = (double) cardElapsed <= (double) this.AssociatedKeyTimes[index] ? string.Empty : this.AssociatedKeys[index];
    return targetKeys;
  }

  public float TotalTime => this.StartHoldTime + this.PanTime + this.EndHoldTime;

  private void Start()
  {
    this.blIntensities = new float[this.additionalBraveLights.Length];
    this.blRadii = new float[this.additionalBraveLights.Length];
    for (int index = 0; index < this.additionalBraveLights.Length; ++index)
    {
      this.blIntensities[index] = this.additionalBraveLights[index].LightIntensity;
      this.blRadii[index] = this.additionalBraveLights[index].LightRadius;
    }
    for (int index = 0; index < this.SpriteRenderers.Length; ++index)
      this.SpriteRenderers[index].transform.localPosition = this.SpriteRenderers[index].transform.localPosition + CameraController.PLATFORM_CAMERA_OFFSET.WithZ(0.0f);
  }

  public void SetVisibility(float v)
  {
    if (this.BGRenderer.material.HasProperty("_AlphaMod"))
      this.BGRenderer.material.SetFloat("_AlphaMod", v);
    for (int index = 0; index < this.SpriteRenderers.Length; ++index)
    {
      if ((bool) (Object) this.SpriteRenderers[index].GetComponent<tk2dSprite>())
        this.SpriteRenderers[index].GetComponent<tk2dSprite>().usesOverrideMaterial = true;
      this.SpriteRenderers[index].material.SetFloat("_AlphaMod", v);
    }
    if ((double) v <= 0.5)
      return;
    this.InitializeClockhands();
    this.StartCoroutine(this.HandleGunBurn());
  }

  public void ToggleLighting(bool togglon) => this.StartCoroutine(this.ToggleLightingCR(togglon));

  [DebuggerHidden]
  private IEnumerator ToggleLightingCR(bool togglon)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new FinalIntroSequenceCard.\u003CToggleLightingCR\u003Ec__Iterator0()
    {
      togglon = togglon,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleGunBurn()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new FinalIntroSequenceCard.\u003CHandleGunBurn\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  private void InitializeClockhands()
  {
    if (this.m_clockhandsInitialized)
      return;
    this.m_clockhandsInitialized = true;
    if (!(bool) (Object) this.clockhand1 || !(bool) (Object) this.clockhand2)
      return;
    this.clockhand1.GetComponent<SimpleSpriteRotator>().enabled = true;
    this.clockhand2.GetComponent<SimpleSpriteRotator>().enabled = true;
    this.clockhand1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 135f);
    this.clockhand2.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 315f);
  }

  private void Update()
  {
    this.m_elapsed += GameManager.INVARIANT_DELTA_TIME;
    int num = 0;
    while (num < this.additionalBraveLights.Length)
      ++num;
  }
}
