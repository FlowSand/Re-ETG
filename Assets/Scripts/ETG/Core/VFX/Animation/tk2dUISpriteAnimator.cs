// Decompiled with JetBrains decompiler
// Type: tk2dUISpriteAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    [AddComponentMenu("2D Toolkit/UI/Core/tk2dUISpriteAnimator")]
    public class tk2dUISpriteAnimator : tk2dSpriteAnimator
    {
      public override void LateUpdate() => this.UpdateAnimation(tk2dUITime.deltaTime);

      protected override void OnDestroy() => base.OnDestroy();
    }

}
