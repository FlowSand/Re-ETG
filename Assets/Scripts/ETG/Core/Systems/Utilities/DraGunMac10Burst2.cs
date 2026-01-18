using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/Mac10Burst2")]
public class DraGunMac10Burst2 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunMac10Burst2__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class UziBullet : Bullet
        {
            public UziBullet()
                : base(nameof (UziBullet))
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new DraGunMac10Burst2.UziBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

