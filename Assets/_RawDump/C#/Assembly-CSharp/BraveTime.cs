// Decompiled with JetBrains decompiler
// Type: BraveTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public static class BraveTime
{
  private static List<GameObject> m_sources = new List<GameObject>();
  private static List<float> m_multipliers = new List<float>();
  private static int s_lastScaledTimeFrameUpdate = -1;
  private static float s_scaledTimeSinceStartup = 0.0f;
  private static float m_cachedDeltaTime;

  public static float DeltaTime => BraveTime.m_cachedDeltaTime;

  public static void CacheDeltaTimeForFrame()
  {
    BraveTime.m_cachedDeltaTime = Mathf.Min(0.1f, UnityEngine.Time.deltaTime);
  }

  public static float ScaledTimeSinceStartup
  {
    get
    {
      BraveTime.UpdateScaledTimeSinceStartup();
      return BraveTime.s_scaledTimeSinceStartup;
    }
  }

  public static void UpdateScaledTimeSinceStartup()
  {
    if (BraveTime.s_lastScaledTimeFrameUpdate == UnityEngine.Time.frameCount)
      return;
    BraveTime.s_scaledTimeSinceStartup += BraveTime.DeltaTime;
    BraveTime.s_lastScaledTimeFrameUpdate = UnityEngine.Time.frameCount;
  }

  public static void RegisterTimeScaleMultiplier(float multiplier, GameObject source)
  {
    if (!BraveTime.m_sources.Contains(source))
    {
      BraveTime.m_sources.Add(source);
      BraveTime.m_multipliers.Add(1f);
    }
    int index = BraveTime.m_sources.IndexOf(source);
    BraveTime.m_multipliers[index] *= multiplier;
    BraveTime.UpdateTimeScale();
  }

  public static void SetTimeScaleMultiplier(float multiplier, GameObject source)
  {
    if (!BraveTime.m_sources.Contains(source))
    {
      BraveTime.m_sources.Add(source);
      BraveTime.m_multipliers.Add(1f);
    }
    int index = BraveTime.m_sources.IndexOf(source);
    BraveTime.m_multipliers[index] = multiplier;
    BraveTime.UpdateTimeScale();
  }

  public static void ClearMultiplier(GameObject source)
  {
    int index = BraveTime.m_sources.IndexOf(source);
    if (index >= 0)
    {
      BraveTime.m_sources.RemoveAt(index);
      BraveTime.m_multipliers.RemoveAt(index);
    }
    BraveTime.UpdateTimeScale();
  }

  public static void ClearAllMultipliers()
  {
    BraveTime.m_sources.Clear();
    BraveTime.m_multipliers.Clear();
    BraveTime.UpdateTimeScale();
  }

  private static void UpdateTimeScale()
  {
    float f = 1f;
    for (int index = 0; index < BraveTime.m_multipliers.Count; ++index)
      f = BraveTime.m_multipliers[index] * f;
    if (float.IsNaN(f))
    {
      Debug.LogError((object) "TIMESCALE WAS MY NAN ALL ALONG");
      f = 1f;
    }
    UnityEngine.Time.timeScale = Mathf.Clamp(f, 0.0f, !ChallengeManager.CHALLENGE_MODE_ACTIVE ? 1f : 1.5f);
  }
}
