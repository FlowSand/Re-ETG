// Decompiled with JetBrains decompiler
// Type: StringTableStringAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class StringTableStringAttribute : PropertyAttribute
    {
      public string stringTableTarget;
      public bool isInToggledState;
      public string keyToWrite = string.Empty;
      public StringTableStringAttribute.TargetStringTableType targetStringTable;

      public StringTableStringAttribute(string tableTarget = null)
      {
        this.stringTableTarget = tableTarget;
      }

      public enum TargetStringTableType
      {
        DEFAULT,
        ENEMIES,
        ITEMS,
        UI,
      }
    }

}
