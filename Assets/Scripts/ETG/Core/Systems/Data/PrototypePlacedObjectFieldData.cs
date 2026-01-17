// Decompiled with JetBrains decompiler
// Type: PrototypePlacedObjectFieldData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Data
{
    [Serializable]
    public class PrototypePlacedObjectFieldData
    {
      public PrototypePlacedObjectFieldData.FieldType fieldType;
      public string fieldName;
      public float floatValue;
      public bool boolValue;

      public enum FieldType
      {
        FLOAT,
        BOOL,
      }
    }

}
