using System;

#nullable disable

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class dfCategoryAttribute : Attribute
  {
    public dfCategoryAttribute(string category) => this.Category = category;

    public string Category { get; private set; }
  }

