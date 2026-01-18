using System.Collections;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Blizzbulon/BasicAttack1")]
public class BlizzbulonBasicAttack1 : Script
    {
        private const int NumBullets = 12;

        protected override IEnumerator Top()
        {
            float num = 30f;
            for (int index = 0; index < 12; ++index)
                this.Fire(new Brave.BulletScript.Direction((float) index * num), new Brave.BulletScript.Speed(6f), (Bullet) null);
            return (IEnumerator) null;
        }
    }

