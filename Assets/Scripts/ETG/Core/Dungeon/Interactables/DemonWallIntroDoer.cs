// Decompiled with JetBrains decompiler
// Type: DemonWallIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    [RequireComponent(typeof (GenericIntroDoer))]
    public class DemonWallIntroDoer : SpecificIntroDoer
    {
      public string preIntro;

      protected override void OnDestroy() => base.OnDestroy();

      public override Vector2? OverrideOutroPosition
      {
        get => new Vector2?(this.GetComponent<DemonWallController>().CameraPos);
      }

      public override void OnCameraIntro() => this.aiAnimator.PlayUntilCancelled(this.preIntro);

      public override void OnCleanup() => this.aiAnimator.EndAnimation();

      public override void EndIntro() => this.GetComponent<DemonWallController>().ModifyCamera(true);
    }

}
