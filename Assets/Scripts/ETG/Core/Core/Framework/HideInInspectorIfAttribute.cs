// Decompiled with JetBrains decompiler
// Type: HideInInspectorIfAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class HideInInspectorIfAttribute : ShowInInspectorIfAttribute
    {
      public HideInInspectorIfAttribute(string propertyName, bool indent = false)
        : base(propertyName, indent)
      {
      }

      public HideInInspectorIfAttribute(string propertyName, int value, bool indent = false)
        : base(propertyName, value, indent)
      {
      }
    }

}
