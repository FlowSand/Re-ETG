// Decompiled with JetBrains decompiler
// Type: SpriteAnimatorSync
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class SpriteAnimatorSync : BraveBehaviour
{
  public tk2dBaseSprite otherSprite;

  public void Start()
  {
    if ((bool) (UnityEngine.Object) this.otherSprite.spriteAnimator)
      this.otherSprite.spriteAnimator.alwaysUpdateOffscreen = true;
    this.otherSprite.SpriteChanged += new Action<tk2dBaseSprite>(this.OtherSpriteChanged);
    this.sprite.SetSprite(this.otherSprite.Collection, this.otherSprite.spriteId);
  }

  protected override void OnDestroy() => base.OnDestroy();

  private void OtherSpriteChanged(tk2dBaseSprite tk2DBaseSprite)
  {
    this.sprite.SetSprite(this.otherSprite.spriteId);
  }
}
