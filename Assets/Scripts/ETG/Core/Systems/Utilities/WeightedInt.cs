// Decompiled with JetBrains decompiler
// Type: WeightedInt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class WeightedInt
    {
      public string annotation;
      public int value;
      public float weight;
      public DungeonPrerequisite[] additionalPrerequisites;
    }

}
