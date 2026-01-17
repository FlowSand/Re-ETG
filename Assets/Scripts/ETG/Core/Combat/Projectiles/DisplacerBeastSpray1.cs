// Decompiled with JetBrains decompiler
// Type: DisplacerBeastSpray1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    [InspectorDropdownName("DisplacerBeastSpray1")]
    public class DisplacerBeastSpray1 : Script
    {
      private const int NumBullets = 20;
      private const float BulletSpread = 27f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DisplacerBeastSpray1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private string[] GetTransformNames()
      {
        Transform transform = this.BulletBank.transform.Find("bullet limbs").Find("back tip 1");
        return (bool) (Object) transform && transform.gameObject.activeSelf ? new string[2]
        {
          "bullet tip 1",
          "back tip 1"
        } : new string[2]{ "bullet tip 1", "bullet tip 2" };
      }

      public class DisplacerBullet : Bullet
      {
        public DisplacerBullet()
          : base()
        {
        }

        protected override IEnumerator Top()
        {
          if ((bool) (Object) this.Projectile)
            this.Projectile.IgnoreTileCollisionsFor(0.25f);
          return (IEnumerator) null;
        }
      }
    }

}
