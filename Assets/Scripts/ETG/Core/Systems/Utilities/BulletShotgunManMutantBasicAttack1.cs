using System.Collections;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("BulletShotgunMan/MutantBasicAttack1")]
public class BulletShotgunManMutantBasicAttack1 : Script
    {
        protected override IEnumerator Top()
        {
            for (int index = -2; index <= 2; ++index)
                this.Fire(new Brave.BulletScript.Direction((float) (index * 6), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) null);
            return (IEnumerator) null;
        }
    }

