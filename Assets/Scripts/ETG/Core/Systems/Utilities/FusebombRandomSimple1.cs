using System.Collections;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Fusebomb/RandomBurstsSimple1")]
public class FusebombRandomSimple1 : Script
    {
        protected override IEnumerator Top()
        {
            if ((double) Random.value < 0.5)
            {
                int numBullets = 10;
                float startAngle = this.RandomAngle();
                for (int i = 0; i < numBullets; ++i)
                    this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, 360f, numBullets, i)), new Brave.BulletScript.Speed(9f), (Bullet) null);
            }
            else
            {
                int numBullets = 5;
                float aimDirection = this.AimDirection;
                float num = 35f;
                bool offset = BraveUtility.RandomBool();
                for (int i = 0; i < numBullets + (!offset ? 0 : -1); ++i)
                    this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(aimDirection - num, num * 2f, numBullets, i, offset)), new Brave.BulletScript.Speed(9f), (Bullet) null);
            }
            return (IEnumerator) null;
        }
    }

