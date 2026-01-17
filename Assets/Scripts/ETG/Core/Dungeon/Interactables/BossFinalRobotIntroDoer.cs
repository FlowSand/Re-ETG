// Decompiled with JetBrains decompiler
// Type: BossFinalRobotIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    [RequireComponent(typeof (GenericIntroDoer))]
    public class BossFinalRobotIntroDoer : SpecificIntroDoer
    {
      protected override void OnDestroy() => base.OnDestroy();

      public override void EndIntro() => this.aiAnimator.StopVfx("torch_intro");
    }

}
