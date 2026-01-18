using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("GunNut/Cone1")]
public class GunNutCone : Script
    {
        private const int NumBulletsMainWave = 25;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GunNutCone__Topc__Iterator0()
            {
                _this = this
            };
        }

        private void FireCluster(float direction)
        {
            this.Fire(new Offset(0.5f, rotation: direction, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
            this.Fire(new Offset(0.275f, 0.25f, direction, string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
            this.Fire(new Offset(0.275f, -0.25f, direction, string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
            this.Fire(new Offset(y: 0.4f, rotation: direction, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
            this.Fire(new Offset(y: -0.4f, rotation: direction, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
        }
    }

