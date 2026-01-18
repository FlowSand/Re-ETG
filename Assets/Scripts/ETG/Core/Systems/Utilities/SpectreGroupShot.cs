using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Brave.BulletScript;

#nullable disable

public class SpectreGroupShot : Script
    {
        private const int NumBullets = 4;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new SpectreGroupShot__Topc__Iterator0()
            {
                _this = this
            };
        }

        private void FireFrom(string transform)
        {
            float aimDirection = this.GetAimDirection(transform, (float) Random.Range(0, 2), 8f);
            Vector2 unit = PhysicsEngine.PixelToUnit(new IntVector2(4, 0));
            this.Fire(new Offset(unit, transform: transform), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(8f), new Bullet("eyeBullet"));
            this.Fire(new Offset(-unit, transform: transform), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(8f), new Bullet("eyeBullet"));
        }
    }

