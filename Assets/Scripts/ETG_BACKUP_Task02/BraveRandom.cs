// Decompiled with JetBrains decompiler
// Type: BraveRandom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class BraveRandom
{
  public static bool IgnoreGenerationDifferentiator;
  private static System.Random m_generationRandom;

  public static System.Random GeneratorRandom => BraveRandom.m_generationRandom;

  public static bool IsInitialized() => BraveRandom.m_generationRandom != null;

  public static void InitializeRandom() => BraveRandom.m_generationRandom = new System.Random();

  public static void InitializeWithSeed(int seed)
  {
    BraveRandom.m_generationRandom = new System.Random(seed);
  }

  public static float GenerationRandomValue()
  {
    return (float) BraveRandom.m_generationRandom.NextDouble();
  }

  public static float GenerationRandomRange(float min, float max)
  {
    return (max - min) * BraveRandom.GenerationRandomValue() + min;
  }

  public static int GenerationRandomRange(int min, int max)
  {
    return Mathf.FloorToInt((float) (max - min) * BraveRandom.GenerationRandomValue()) + min;
  }
}
