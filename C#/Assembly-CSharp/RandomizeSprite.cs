// Decompiled with JetBrains decompiler
// Type: RandomizeSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
public class RandomizeSprite : BraveBehaviour
{
  [CheckSprite(null)]
  public List<string> spriteNames;
  [CheckAnimation(null)]
  public List<string> animationNames;
  public bool UseStaticIndex;
  private static int s_index;

  public void Start()
  {
    if (this.UseStaticIndex)
    {
      if (this.spriteNames.Count > 0)
        this.sprite.SetSprite(this.spriteNames[RandomizeSprite.s_index % this.spriteNames.Count]);
      if (this.animationNames.Count > 0)
        this.spriteAnimator.Play(this.animationNames[RandomizeSprite.s_index % this.animationNames.Count]);
      ++RandomizeSprite.s_index;
      if (RandomizeSprite.s_index >= 0)
        return;
      RandomizeSprite.s_index = 0;
    }
    else
    {
      if (this.spriteNames.Count > 0)
        this.sprite.SetSprite(BraveUtility.RandomElement<string>(this.spriteNames));
      if (this.animationNames.Count <= 0)
        return;
      this.spriteAnimator.Play(BraveUtility.RandomElement<string>(this.animationNames));
    }
  }

  protected override void OnDestroy() => base.OnDestroy();
}
