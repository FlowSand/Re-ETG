// Decompiled with JetBrains decompiler
// Type: BulletKingIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class BulletKingIntroDoer : SpecificIntroDoer
  {
    protected override void OnDestroy() => base.OnDestroy();

    public override void StartIntro(List<tk2dSpriteAnimator> animators)
    {
      BulletKingToadieController[] objectsOfType = Object.FindObjectsOfType<BulletKingToadieController>();
      for (int index = 0; index < objectsOfType.Length; ++index)
      {
        animators.Add(objectsOfType[index].spriteAnimator);
        if ((bool) (Object) objectsOfType[index].scepterAnimator)
          animators.Add(objectsOfType[index].scepterAnimator);
      }
    }
  }

