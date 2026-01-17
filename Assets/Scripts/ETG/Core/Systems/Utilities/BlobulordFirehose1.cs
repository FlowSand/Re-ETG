// Decompiled with JetBrains decompiler
// Type: BlobulordFirehose1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/Blobulord/Firehose1")]
    public class BlobulordFirehose1 : Script
    {
      private const float SpawnVariance = 0.5f;
      private const float WobbleRange = 35f;
      private const float BreakAwayChance = 0.2f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BlobulordFirehose1.<Top>c__Iterator0()
        {
          $this = this
        };
      }

      public class FirehoseBullet : Bullet
      {
        private float m_direction;

        public FirehoseBullet(float direction)
          : base("firehose")
        {
          this.m_direction = direction;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BlobulordFirehose1.FirehoseBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
