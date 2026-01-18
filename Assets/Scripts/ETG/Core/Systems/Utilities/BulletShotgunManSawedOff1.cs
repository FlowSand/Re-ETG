using System.Collections;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("BulletShotgunMan/SawedOff1")]
public class BulletShotgunManSawedOff1 : Script
    {
        protected override IEnumerator Top()
        {
            float aimDirection = this.GetAimDirection(1f, 9f);
            for (int index = -2; index <= 2; ++index)
                this.Fire(new Brave.BulletScript.Direction(aimDirection + (float) (index * 6)), new Brave.BulletScript.Speed((float) (10.0 - (double) Mathf.Abs(index) * 0.5)), (Bullet) null);
            return (IEnumerator) null;
        }
    }

