using System.Collections;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Infinilich/BasicSwingLeft1")]
public class InfinilichBasicSwingLeft1 : Script
    {
        private const float EnemyBulletSpeedItem = 12f;
        private static int[] ShootPoints = new int[11]
        {
            4,
            9,
            13,
            18,
            20,
            21,
            22,
            23,
            24,
            25,
            26
        };

        protected override IEnumerator Top()
        {
            for (int index = 0; index < InfinilichBasicSwingLeft1.ShootPoints.Length; ++index)
            {
                string transform = "bullet limb " + (object) InfinilichBasicSwingLeft1.ShootPoints[index];
                float leadAmount = Mathf.Lerp(0.0f, 2f, (float) index / ((float) InfinilichBasicSwingLeft1.ShootPoints.Length - 1f));
                float aimDirection = this.GetAimDirection(transform, leadAmount, 12f);
                this.Fire(new Offset(transform), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(12f));
            }
            return (IEnumerator) null;
        }
    }

