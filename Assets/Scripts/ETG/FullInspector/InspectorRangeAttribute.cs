using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class InspectorRangeAttribute : Attribute
  {
    public float Min;
    public float Max;
    public float Step = float.NaN;

    public InspectorRangeAttribute(float min, float max)
    {
      this.Min = min;
      this.Max = max;
    }
  }
}
