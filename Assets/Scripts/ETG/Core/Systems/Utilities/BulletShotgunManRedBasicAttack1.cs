using System.Collections;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("BulletShotgunMan/RedBasicAttack1")]
public class BulletShotgunManRedBasicAttack1 : Script
    {
        protected override IEnumerator Top()
        {
            for (int index = -2; index <= 2; ++index)
                this.Fire(new Brave.BulletScript.Direction((float) (index * 6), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(5f), (Bullet) null);
            return (IEnumerator) null;
        }
    }

