// Decompiled with JetBrains decompiler
// Type: WizardBlueSlam1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class WizardBlueSlam1 : Script
    {
      private const int BulletClusters = 22;
      private const int BulletsPerCluster = 3;
      private const float ClusterRadius = 0.4f;
      private const float ClusterRotationDegPerFrame = -8f;
      public float aimDirection;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WizardBlueSlam1.<Top>c__Iterator0()
        {
          $this = this
        };
      }

      public class ClusterBullet : Bullet
      {
        private WizardBlueSlam1 parent;
        private float clusterAngle;

        public ClusterBullet(WizardBlueSlam1 parent, float clusterAngle)
          : base("Trio")
        {
          this.parent = parent;
          this.clusterAngle = clusterAngle;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new WizardBlueSlam1.ClusterBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
