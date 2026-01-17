// Decompiled with JetBrains decompiler
// Type: BlobulordBouncingRings1
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
    [InspectorDropdownName("Bosses/Blobulord/BouncingRings1")]
    public class BlobulordBouncingRings1 : Script
    {
      private const int NumBlobs = 8;
      private const int NumBullets = 18;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BlobulordBouncingRings1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
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
          return (IEnumerator) new BlobulordBouncingRings1.BouncingRingBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
