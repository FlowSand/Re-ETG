// Decompiled with JetBrains decompiler
// Type: ScreenShakeSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using UnityEngine;

#nullable disable
[Serializable]
public class ScreenShakeSettings : fiInspectorOnly
{
  public static float GLOBAL_SHAKE_MULTIPLIER = 1f;
  public float magnitude;
  public float speed;
  public float time;
  public float falloff;
  public Vector2 direction;
  public ScreenShakeSettings.VibrationType vibrationType = ScreenShakeSettings.VibrationType.Auto;
  [InspectorShowIf("ShowSimpleVibrationParams")]
  [InspectorIndent]
  public Vibration.Time simpleVibrationTime = Vibration.Time.Normal;
  [InspectorIndent]
  [InspectorShowIf("ShowSimpleVibrationParams")]
  public Vibration.Strength simpleVibrationStrength = Vibration.Strength.Medium;

  public ScreenShakeSettings()
  {
    this.magnitude = 0.35f;
    this.speed = 6f;
    this.time = 0.06f;
    this.falloff = 0.0f;
  }

  public ScreenShakeSettings(float mag, float spd, float tim, float foff)
  {
    this.magnitude = mag;
    this.speed = spd;
    this.time = tim;
    this.falloff = foff;
    this.direction = Vector2.zero;
  }

  public ScreenShakeSettings(float mag, float spd, float tim, float foff, Vector2 dir)
  {
    this.magnitude = mag;
    this.speed = spd;
    this.time = tim;
    this.falloff = foff;
    this.direction = dir;
  }

  public bool ShowSimpleVibrationParams()
  {
    return this.vibrationType == ScreenShakeSettings.VibrationType.Simple;
  }

  public enum VibrationType
  {
    None = 0,
    Auto = 10, // 0x0000000A
    Simple = 20, // 0x00000014
  }
}
