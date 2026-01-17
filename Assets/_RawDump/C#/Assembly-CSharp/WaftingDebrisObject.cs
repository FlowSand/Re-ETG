// Decompiled with JetBrains decompiler
// Type: WaftingDebrisObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WaftingDebrisObject : DebrisObject
{
  [Header("Waft Properties")]
  public string waftAnimationName;
  public Vector2 initialBurstDuration = new Vector2(0.3f, 0.45f);
  public Vector2 waftDuration = new Vector2(2f, 4.5f);
  public Vector2 waftDistance = new Vector2(1.5f, 3.5f);
  private bool m_initialized;
  private Vector3 m_cachedInitialVelocity;
  private float m_peakElapsed;
  private float m_peakDuration;
  private bool m_hasHitPeak;
  private float m_waftElapsed;
  private float m_waftPeriod;
  private float m_waftDistance;
  private int m_coplanarSign;

  protected override void UpdateVelocity(float adjustedDeltaTime)
  {
    if (!this.m_initialized)
    {
      this.m_initialized = true;
      this.m_cachedInitialVelocity = this.m_velocity;
      this.m_peakDuration = Mathf.Lerp(this.initialBurstDuration.x, this.initialBurstDuration.y, Random.value);
    }
    if ((double) this.m_currentPosition.z <= 0.0)
      return;
    if (!this.m_hasHitPeak)
    {
      this.m_peakElapsed += adjustedDeltaTime;
      float smoothStepInterpolate = BraveMathCollege.LinearToSmoothStepInterpolate(0.0f, 1f, this.m_peakElapsed / this.m_peakDuration);
      this.m_velocity = Vector3.Lerp(Vector3.Scale(this.m_cachedInitialVelocity, new Vector3(2.5f, 2.5f, 4f)), new Vector3(this.m_cachedInitialVelocity.x * 0.5f, this.m_cachedInitialVelocity.y * 0.5f, 0.0f), smoothStepInterpolate);
      if ((double) this.m_velocity.z > 0.0)
        return;
      this.m_hasHitPeak = true;
      this.m_waftPeriod = Mathf.Lerp(this.waftDuration.x, this.waftDuration.y, Random.value);
      this.m_waftDistance = Mathf.Lerp(this.waftDistance.x, this.waftDistance.y, Random.value);
      this.m_coplanarSign = (double) Random.value <= 0.5 ? -1 : 1;
      if ((double) Random.value < 0.5)
        this.m_waftElapsed = this.m_waftPeriod / 2f;
      else
        this.m_waftElapsed = 0.0f;
    }
    else
    {
      this.m_waftElapsed += adjustedDeltaTime;
      float num1 = this.m_waftElapsed % this.m_waftPeriod;
      float f = Mathf.Cos((float) ((double) num1 / (double) this.m_waftPeriod * 2.0 * 3.1415927410125732));
      float num2 = Mathf.Sin((float) ((double) num1 / (double) this.m_waftPeriod * 2.0 * 3.1415927410125732));
      float z = Mathf.Lerp(this.m_velocity.z, this.m_velocity.z + 4f * adjustedDeltaTime, Mathf.Abs(f)) + -3f * adjustedDeltaTime;
      this.m_velocity = new Vector3(this.m_waftDistance * f, this.m_waftDistance / 5f * num2 * (float) this.m_coplanarSign, z);
      if (string.IsNullOrEmpty(this.waftAnimationName))
        return;
      tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.waftAnimationName);
      if (clipByName != this.spriteAnimator.CurrentClip)
      {
        this.spriteAnimator.Play(this.waftAnimationName);
        this.spriteAnimator.Stop();
      }
      float num3 = Mathf.PingPong((float) ((double) ((this.m_waftElapsed + 0.5f * this.m_waftPeriod) % this.m_waftPeriod) / (double) this.m_waftPeriod * 2.0), 1f);
      this.spriteAnimator.SetFrame(Mathf.Clamp(Mathf.FloorToInt((float) clipByName.frames.Length * num3), 0, clipByName.frames.Length - 1));
    }
  }

  protected override void OnDestroy() => base.OnDestroy();
}
