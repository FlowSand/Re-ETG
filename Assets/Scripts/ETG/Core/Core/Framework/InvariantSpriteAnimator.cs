// Decompiled with JetBrains decompiler
// Type: InvariantSpriteAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    [RequireComponent(typeof (tk2dSpriteAnimator))]
    public class InvariantSpriteAnimator : BraveBehaviour
    {
      public void Awake() => this.spriteAnimator.ignoreTimeScale = true;
    }

}
