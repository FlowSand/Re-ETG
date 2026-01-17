// Decompiled with JetBrains decompiler
// Type: ChainRule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class ChainRule
    {
      public string form;
      public string target;
      public float weight = 0.1f;
      public bool mandatory;
    }

}
