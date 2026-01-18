using System;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class TilemapDecoSettings
  {
    public WeightedIntCollection standardRoomVisualSubtypes;
    public TilemapDecoSettings.DecoStyle decalLayerStyle;
    public int decalSize = 1;
    public int decalSpacing = 1;
    public int decalExpansion;
    public TilemapDecoSettings.DecoStyle patternLayerStyle;
    public int patternSize = 1;
    public int patternSpacing = 1;
    public int patternExpansion;
    public float decoPatchFrequency = 0.01f;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    [Header("Lights")]
    public Color ambientLightColor = Color.black;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color ambientLightColorTwo = Color.clear;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color lowQualityAmbientLightColor = Color.white;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color lowQualityAmbientLightColorTwo = Color.white;
    public Vector4 lowQualityCheapLightVector = new Vector4(1f, 0.0f, -1f, 0.0f);
    public bool UsesAlienFXFloorColor;
    public Color AlienFXFloorColor = Color.black;
    public bool generateLights = true;
    public float lightCullingPercentage = 0.2f;
    public int lightOverlapRadius = 5;
    public float nearestAllowedLight = 5f;
    public int minLightExpanseWidth = 4;
    public float lightHeight = 1.5f;
    public Texture2D[] lightCookies;
    public bool debug_view;

    public Texture2D GetRandomLightCookie()
    {
      return this.lightCookies[UnityEngine.Random.Range(0, this.lightCookies.Length)];
    }

    public enum DecoStyle
    {
      GROW_FROM_WALLS = 0,
      PERLIN_NOISE = 1,
      HORIZONTAL_STRIPES = 2,
      VERTICAL_STRIPES = 3,
      AROUND_LIGHTS = 4,
      WATER_CHANNELS = 5,
      PATCHES = 6,
      NONE = 99, // 0x00000063
    }
  }
}
