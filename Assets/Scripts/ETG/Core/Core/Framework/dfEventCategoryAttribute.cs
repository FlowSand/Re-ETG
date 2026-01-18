using System;

#nullable disable

[AttributeUsage(AttributeTargets.Delegate, Inherited = true, AllowMultiple = false)]
public class dfEventCategoryAttribute : Attribute
  {
    public dfEventCategoryAttribute(string category) => this.Category = category;

    public string Category { get; private set; }
  }

