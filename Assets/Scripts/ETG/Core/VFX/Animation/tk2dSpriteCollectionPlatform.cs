// Decompiled with JetBrains decompiler
// Type: tk2dSpriteCollectionPlatform
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    [Serializable]
    public class tk2dSpriteCollectionPlatform
    {
      public string name = string.Empty;
      public tk2dSpriteCollection spriteCollection;

      public bool Valid => this.name.Length > 0 && (UnityEngine.Object) this.spriteCollection != (UnityEngine.Object) null;

      public void CopyFrom(tk2dSpriteCollectionPlatform source)
      {
        this.name = source.name;
        this.spriteCollection = source.spriteCollection;
      }
    }

}
