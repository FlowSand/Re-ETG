// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiAnimBool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal;

[Serializable]
public class fiAnimBool : fiBaseAnimValue<bool>
{
  [SerializeField]
  private float m_Value;

  public fiAnimBool()
    : base(false)
  {
  }

  public fiAnimBool(bool value)
    : base(value)
  {
  }

  public float faded
  {
    get
    {
      this.GetValue();
      return this.m_Value;
    }
  }

  protected override bool GetValue()
  {
    float a = this.target ? 0.0f : 1f;
    float b = 1f - a;
    this.m_Value = Mathf.Lerp(a, b, this.lerpPosition);
    return (double) this.m_Value > 0.5;
  }

  public float Fade(float from, float to) => Mathf.Lerp(from, to, this.faded);
}
