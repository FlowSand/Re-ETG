// Decompiled with JetBrains decompiler
// Type: AkObstructionOcclusion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public abstract class AkObstructionOcclusion : MonoBehaviour
{
  private readonly List<AkAudioListener> listenersToRemove = new List<AkAudioListener>();
  private readonly Dictionary<AkAudioListener, AkObstructionOcclusion.ObstructionOcclusionValue> ObstructionOcclusionValues = new Dictionary<AkAudioListener, AkObstructionOcclusion.ObstructionOcclusionValue>();
  protected float fadeRate;
  [Tooltip("Fade time in seconds")]
  public float fadeTime = 0.5f;
  [Tooltip("Layers of obstructers/occluders")]
  public LayerMask LayerMask = (LayerMask) -1;
  [Tooltip("Maximum distance to perform the obstruction/occlusion. Negative values mean infinite")]
  public float maxDistance = -1f;
  [Tooltip("The number of seconds between raycasts")]
  public float refreshInterval = 1f;
  private float refreshTime;

  protected void InitIntervalsAndFadeRates()
  {
    this.refreshTime = Random.Range(0.0f, this.refreshInterval);
    this.fadeRate = 1f / this.fadeTime;
  }

  protected void UpdateObstructionOcclusionValues(List<AkAudioListener> listenerList)
  {
    for (int index = 0; index < listenerList.Count; ++index)
    {
      if (!this.ObstructionOcclusionValues.ContainsKey(listenerList[index]))
        this.ObstructionOcclusionValues.Add(listenerList[index], new AkObstructionOcclusion.ObstructionOcclusionValue());
    }
    foreach (KeyValuePair<AkAudioListener, AkObstructionOcclusion.ObstructionOcclusionValue> obstructionOcclusionValue in this.ObstructionOcclusionValues)
    {
      if (!listenerList.Contains(obstructionOcclusionValue.Key))
        this.listenersToRemove.Add(obstructionOcclusionValue.Key);
    }
    for (int index = 0; index < this.listenersToRemove.Count; ++index)
      this.ObstructionOcclusionValues.Remove(this.listenersToRemove[index]);
  }

  protected void UpdateObstructionOcclusionValues(AkAudioListener listener)
  {
    if (!(bool) (Object) listener)
      return;
    if (!this.ObstructionOcclusionValues.ContainsKey(listener))
      this.ObstructionOcclusionValues.Add(listener, new AkObstructionOcclusion.ObstructionOcclusionValue());
    foreach (KeyValuePair<AkAudioListener, AkObstructionOcclusion.ObstructionOcclusionValue> obstructionOcclusionValue in this.ObstructionOcclusionValues)
    {
      if ((Object) listener != (Object) obstructionOcclusionValue.Key)
        this.listenersToRemove.Add(obstructionOcclusionValue.Key);
    }
    for (int index = 0; index < this.listenersToRemove.Count; ++index)
      this.ObstructionOcclusionValues.Remove(this.listenersToRemove[index]);
  }

  private void CastRays()
  {
    if ((double) this.refreshTime > (double) this.refreshInterval)
    {
      this.refreshTime -= this.refreshInterval;
      foreach (KeyValuePair<AkAudioListener, AkObstructionOcclusion.ObstructionOcclusionValue> obstructionOcclusionValue1 in this.ObstructionOcclusionValues)
      {
        AkAudioListener key = obstructionOcclusionValue1.Key;
        AkObstructionOcclusion.ObstructionOcclusionValue obstructionOcclusionValue2 = obstructionOcclusionValue1.Value;
        Vector3 vector3 = key.transform.position - this.transform.position;
        float magnitude = vector3.magnitude;
        obstructionOcclusionValue2.targetValue = (double) this.maxDistance <= 0.0 || (double) magnitude <= (double) this.maxDistance ? (!Physics.Raycast(this.transform.position, vector3 / magnitude, magnitude, this.LayerMask.value) ? 0.0f : 1f) : obstructionOcclusionValue2.currentValue;
      }
    }
    this.refreshTime += UnityEngine.Time.deltaTime;
  }

  protected abstract void UpdateObstructionOcclusionValuesForListeners();

  protected abstract void SetObstructionOcclusion(
    KeyValuePair<AkAudioListener, AkObstructionOcclusion.ObstructionOcclusionValue> ObsOccPair);

  private void Update()
  {
    this.UpdateObstructionOcclusionValuesForListeners();
    this.CastRays();
    foreach (KeyValuePair<AkAudioListener, AkObstructionOcclusion.ObstructionOcclusionValue> obstructionOcclusionValue in this.ObstructionOcclusionValues)
    {
      if (obstructionOcclusionValue.Value.Update(this.fadeRate))
        this.SetObstructionOcclusion(obstructionOcclusionValue);
    }
  }

  protected class ObstructionOcclusionValue
  {
    public float currentValue;
    public float targetValue;

    public bool Update(float fadeRate)
    {
      if (Mathf.Approximately(this.targetValue, this.currentValue))
        return false;
      this.currentValue += fadeRate * Mathf.Sign(this.targetValue - this.currentValue) * UnityEngine.Time.deltaTime;
      this.currentValue = Mathf.Clamp(this.currentValue, 0.0f, 1f);
      return true;
    }
  }
}
