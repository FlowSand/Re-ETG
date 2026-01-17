// Decompiled with JetBrains decompiler
// Type: NonActor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Enemy
{
    public class NonActor : GameActor
    {
      public override void Awake()
      {
      }

      public override Gun CurrentGun => (Gun) null;

      public override Transform GunPivot => (Transform) null;

      public override Vector3 SpriteDimensions => Vector3.zero;

      public override bool SpriteFlipped => false;

      public override void Update()
      {
      }
    }

}
