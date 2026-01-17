// Decompiled with JetBrains decompiler
// Type: tk2dSpriteCollectionDefault
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    [Serializable]
    public class tk2dSpriteCollectionDefault
    {
      public bool additive;
      public Vector3 scale = new Vector3(1f, 1f, 1f);
      public tk2dSpriteCollectionDefinition.Anchor anchor = tk2dSpriteCollectionDefinition.Anchor.MiddleCenter;
      public tk2dSpriteCollectionDefinition.Pad pad;
      public tk2dSpriteCollectionDefinition.ColliderType colliderType;
    }

}
