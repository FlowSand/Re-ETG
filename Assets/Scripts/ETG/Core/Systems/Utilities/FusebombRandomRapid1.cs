using System.Collections;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Fusebomb/RandomBurstsRapid1")]
public class FusebombRandomRapid1 : Script
    {
        private const int NumBullets = 8;
        private static bool s_offset;

        protected override IEnumerator Top()
        {
            float startAngle = this.RandomAngle();
            for (int i = 0; i < 8; ++i)
                this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, 360f, 8, i)), new Brave.BulletScript.Speed(9f), (Bullet) null);
            return (IEnumerator) null;
        }
    }

