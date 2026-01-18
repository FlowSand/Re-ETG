// Decompiled with JetBrains decompiler
// Type: TogglesPropertyAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class TogglesPropertyAttribute : PropertyAttribute
  {
    public string PropertyName;
    public string Label;

    public TogglesPropertyAttribute(string propertyName, string label = null)
    {
      this.PropertyName = propertyName;
      this.Label = label;
    }
  }

