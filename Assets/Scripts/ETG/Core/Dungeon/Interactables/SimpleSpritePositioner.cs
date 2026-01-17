// Decompiled with JetBrains decompiler
// Type: SimpleSpritePositioner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class SimpleSpritePositioner : DungeonPlaceableBehaviour
    {
      public float Rotation;

      public void Start()
      {
        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, this.Rotation);
        if (!(bool) (Object) this.sprite)
          return;
        this.sprite.UpdateZDepth();
        this.sprite.ForceRotationRebuild();
      }
    }

}
