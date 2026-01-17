// Decompiled with JetBrains decompiler
// Type: DumbGunShooter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Enemy
{
    public class DumbGunShooter : GameActor
    {
      public Gun gunToUse;
      public float continueShootTime;
      public float shootPauseTime;
      public bool overridesInaccuracy = true;
      public float inaccuracyFraction;
      private GunInventory inventory;

      public override void Start()
      {
        this.inventory = new GunInventory((GameActor) this);
        this.inventory.maxGuns = 1;
        this.inventory.AddGunToInventory(this.gunToUse, true);
        SpriteOutlineManager.AddOutlineToSprite(this.inventory.CurrentGun.sprite, Color.black, 0.1f, 0.05f);
        this.StartCoroutine(this.HandleGunShoot());
      }

      [DebuggerHidden]
      private IEnumerator HandleGunShoot()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DumbGunShooter.<HandleGunShoot>c__Iterator0()
        {
          $this = this
        };
      }

      public override Gun CurrentGun => this.inventory.CurrentGun;

      public override Transform GunPivot => this.transform;

      public override bool SpriteFlipped => false;

      public override Vector3 SpriteDimensions => Vector3.one;

      protected override void OnDestroy() => base.OnDestroy();
    }

}
