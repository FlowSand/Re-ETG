using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("DisplacerBeastSpray1")]
public class DisplacerBeastSpray1 : Script
    {
        private const int NumBullets = 20;
        private const float BulletSpread = 27f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DisplacerBeastSpray1__Topc__Iterator0()
            {
                _this = this
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

