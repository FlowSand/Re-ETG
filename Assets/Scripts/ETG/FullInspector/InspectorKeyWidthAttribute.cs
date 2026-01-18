using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class InspectorKeyWidthAttribute : Attribute
  {
    public float WidthPercentage;

    public InspectorKeyWidthAttribute(float widthPercentage)
    {
      this.WidthPercentage = (double) widthPercentage >= 0.0 && (double) widthPercentage < 1.0 ? widthPercentage : throw new ArgumentException("widthPercentage must be between [0,1]");
    }
  }
}
