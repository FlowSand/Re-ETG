// Decompiled with JetBrains decompiler
// Type: ChancebulonBouncingRing1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Chancebulon/BouncingRing1")]
    public class ChancebulonBouncingRing1 : Script
    {
      private const int NumBullets = 18;

      protected override IEnumerator Top()
      {
        float direction = this.GetAimDirection((double) Random.value >= 0.40000000596046448 ? 0.0f : 1f, 8f) + Random.Range(-10f, 10f);
        for (int index = 0; index < 18; ++index)
        {
          Vector2 vector = BraveMathCollege.DegreesToVector((float) index * 20f, 1.8f);
          this.Fire(new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(8f), (Bullet) new ChancebulonBouncingRing1.BouncingRingBullet("bouncingRing", vector));
        }
        this.Fire(new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(8f), (Bullet) new ChancebulonBouncingRing1.BouncingRingBullet("bouncingRing", new Vector2(-0.7f, 0.7f)));
        this.Fire(new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(8f), (Bullet) new ChancebulonBouncingRing1.BouncingRingBullet("bouncingMouth", new Vector2(0.0f, 0.4f)));
        this.Fire(new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(8f), (Bullet) new ChancebulonBouncingRing1.BouncingRingBullet("bouncingRing", new Vector2(0.7f, 0.7f)));
        return (IEnumerator) null;
      }

      public class BouncingRingBullet : Bullet
      {
        private Vector2 m_desiredOffset;

        public BouncingRingBullet(string name, Vector2 desiredOffset)
          : base(name)
        {
          this.m_desiredOffset = desiredOffset;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new ChancebulonBouncingRing1.BouncingRingBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
