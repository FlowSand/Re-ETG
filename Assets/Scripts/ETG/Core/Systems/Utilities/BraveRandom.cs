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

