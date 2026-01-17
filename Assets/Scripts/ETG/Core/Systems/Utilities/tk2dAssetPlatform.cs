// Decompiled with JetBrains decompiler
// Type: tk2dAssetPlatform
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class tk2dAssetPlatform
    {
      public string name = string.Empty;
      public float scale = 1f;

      public tk2dAssetPlatform(string name, float scale)
      {
        this.name = name;
        this.scale = scale;
      }
    }

}
