using System;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
  [Serializable]
  public class fiAnimFloat : fiBaseAnimValue<float>
  {
    public fiAnimFloat(float value)
      : base(value)
    {
    }

    [SerializeField]
    private float m_Value;

    protected override float GetValue()
    {
      this.m_Value = Mathf.Lerp(this.start, this.target, this.lerpPosition);
      return this.m_Value;
    }
  }
}
