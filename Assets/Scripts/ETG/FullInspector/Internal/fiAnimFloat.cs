// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiAnimFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
  [Serializable]
  public class fiAnimFloat(float value) : fiBaseAnimValue<float>(value)
  {
    [SerializeField]
    private float m_Value;

    protected override float GetValue()
    {
      this.m_Value = Mathf.Lerp(this.start, this.target, this.lerpPosition);
      return this.m_Value;
    }
  }
}
