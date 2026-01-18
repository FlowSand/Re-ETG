using System.Collections;
using System.Diagnostics;

using Brave.BulletScript;

#nullable disable

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
            return (IEnumerator) new WizardBlueSlam1__Topc__Iterator0()
            {
                _this = this
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
                return (IEnumerator) new WizardBlueSlam1.ClusterBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

