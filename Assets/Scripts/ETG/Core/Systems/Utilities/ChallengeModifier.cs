// Decompiled with JetBrains decompiler
// Type: ChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public abstract class ChallengeModifier : MonoBehaviour
  {
[SerializeField]
    public string DisplayName;
[SerializeField]
    public string AlternateLanguageDisplayName;
[SerializeField]
    public string AtlasSpriteName;
[SerializeField]
    public bool ValidInBossChambers = true;
[SerializeField]
    public List<ChallengeModifier> MutuallyExclusive;
[NonSerialized]
    public dfSprite IconSprite;
[NonSerialized]
    public dfLabel IconLabel;
[NonSerialized]
    public bool IconShattered;

    public void ShatterIcon(dfAnimationClip ChallengeBurstClip)
    {
      if (this.IconShattered || !(bool) (UnityEngine.Object) this.IconSprite)
        return;
      this.IconShattered = true;
      dfSpriteAnimation dfSpriteAnimation = this.IconSprite.gameObject.AddComponent<dfSpriteAnimation>();
      dfSpriteAnimation.Target = new dfComponentMemberInfo();
      dfComponentMemberInfo target = dfSpriteAnimation.Target;
      target.Component = (Component) this.IconSprite;
      target.MemberName = "SpriteName";
      dfSpriteAnimation.Clip = ChallengeBurstClip;
      dfSpriteAnimation.Length = 0.2f;
      dfSpriteAnimation.LoopType = dfTweenLoopType.Once;
      dfSpriteAnimation.Play();
      UnityEngine.Object.Destroy((UnityEngine.Object) this.IconLabel.gameObject, 0.2f);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.IconSprite.gameObject, 0.2f);
    }

    public virtual bool IsValid(RoomHandler room) => true;
  }

