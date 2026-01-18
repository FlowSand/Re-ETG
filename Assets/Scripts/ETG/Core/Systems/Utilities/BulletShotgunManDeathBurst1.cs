using System.Collections;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("BulletShotgunMan/DeathBurst1")]
public class BulletShotgunManDeathBurst1 : Script
    {
        protected override IEnumerator Top()
        {
            for (int index = 0; index <= 6; ++index)
                this.Fire(new Brave.BulletScript.Direction((float) (index * 60)), new Brave.BulletScript.Speed(6.5f), new Bullet("flashybullet"));
            return (IEnumerator) null;
        }
    }

