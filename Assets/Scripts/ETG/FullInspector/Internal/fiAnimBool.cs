using System;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
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
}
