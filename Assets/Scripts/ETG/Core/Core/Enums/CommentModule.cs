// Decompiled with JetBrains decompiler
// Type: CommentModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Core.Enums
{
    [Serializable]
    public struct CommentModule
    {
      public string stringKey;
      public float duration;
      public CommentModule.CommentTarget target;
      public float delay;

      public enum CommentTarget
      {
        PRIMARY,
        SECONDARY,
        DOG,
      }
    }

}
