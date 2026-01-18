// Decompiled with JetBrains decompiler
// Type: LongEnumShowInInspectorIfAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class LongEnumShowInInspectorIfAttribute : PropertyAttribute
  {
    public LongEnumShowInInspectorIfAttribute(string propertyName, bool value = true, bool indent = false)
    {
    }

    public LongEnumShowInInspectorIfAttribute(string propertyName, int value, bool indent = false)
    {
    }

    public LongEnumShowInInspectorIfAttribute(string propertyName, Object value, bool indent = false)
    {
    }
  }

