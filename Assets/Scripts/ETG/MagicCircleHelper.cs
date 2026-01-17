// Decompiled with JetBrains decompiler
// Type: MagicCircleHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MagicCircleHelper : MonoBehaviour
{
  public ParticleSystem CircleParticles;
  public float CircleStartVal = 0.75f;
  public float EmissiveColorPower = 7f;
  public float minBrightness = 0.5f;
  public float maxBrightness = 1f;
  public float minEmissivePower = 50f;
  public float maxEmissivePower = 100f;
  public float pulsePeriod = 1f;
  public float fadeInTime = 1f;
  private float elapsed;
  private Material m_materialInst;
  private MeshFilter m_mf;
  private static bool indicesInitialized;
  private static int powerIndex;
  private static int colorPowerIndex;
  private static int circlefadeIndex;
  private static int uvRangeIndex;
  private static int brightnessIndex;

  private void Start()
  {
    if (!MagicCircleHelper.indicesInitialized)
    {
      MagicCircleHelper.indicesInitialized = true;
      MagicCircleHelper.powerIndex = Shader.PropertyToID("_EmissivePower");
      MagicCircleHelper.colorPowerIndex = Shader.PropertyToID("_EmissiveColorPower");
      MagicCircleHelper.circlefadeIndex = Shader.PropertyToID("_RadialFade");
      MagicCircleHelper.uvRangeIndex = Shader.PropertyToID("_UVMinMax");
      MagicCircleHelper.brightnessIndex = Shader.PropertyToID("_Brightness");
    }
    tk2dBaseSprite component = this.GetComponent<tk2dBaseSprite>();
    if ((Object) component != (Object) null)
      component.usesOverrideMaterial = true;
    this.m_mf = this.GetComponent<MeshFilter>();
    this.m_materialInst = this.GetComponent<Renderer>().material;
    this.m_materialInst.SetFloat(MagicCircleHelper.powerIndex, this.minEmissivePower);
    this.m_materialInst.SetFloat(MagicCircleHelper.colorPowerIndex, this.EmissiveColorPower);
  }

  public void OnSpawned() => this.elapsed = 0.0f;

  private Vector4 GetMinMaxUVs()
  {
    Vector2 rhs1 = new Vector2(float.MaxValue, float.MaxValue);
    Vector2 rhs2 = new Vector2(float.MinValue, float.MinValue);
    for (int index = 0; index < this.m_mf.sharedMesh.uv.Length; ++index)
    {
      rhs1 = Vector2.Min(this.m_mf.sharedMesh.uv[index], rhs1);
      rhs2 = Vector2.Max(this.m_mf.sharedMesh.uv[index], rhs2);
    }
    return new Vector4(rhs1.x, rhs1.y, rhs2.x, rhs2.y);
  }

  private void LateUpdate()
  {
    this.m_materialInst.SetVector(MagicCircleHelper.uvRangeIndex, this.GetMinMaxUVs());
    this.elapsed += BraveTime.DeltaTime;
    this.m_materialInst.SetFloat(MagicCircleHelper.circlefadeIndex, Mathf.Lerp(1f, 0.0f, this.elapsed / this.fadeInTime));
    float t = Mathf.PingPong(this.elapsed, this.pulsePeriod) / this.pulsePeriod;
    this.m_materialInst.SetFloat(MagicCircleHelper.brightnessIndex, Mathf.Lerp(this.minBrightness, this.maxBrightness, t) * Mathf.Clamp01(this.elapsed / this.fadeInTime));
    this.m_materialInst.SetFloat(MagicCircleHelper.powerIndex, Mathf.Lerp(this.minEmissivePower, this.maxEmissivePower, t) * Mathf.Clamp01(this.elapsed / this.fadeInTime));
    if (!((Object) this.CircleParticles != (Object) null))
      return;
    BraveUtility.EnableEmission(this.CircleParticles, (double) this.elapsed / (double) this.fadeInTime >= (double) this.CircleStartVal);
  }
}
