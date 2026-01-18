using System.Collections;

using Brave.BulletScript;

#nullable disable

public class TridentShot1 : Script
    {
        protected override IEnumerator Top()
        {
            this.Fire(new Brave.BulletScript.Direction(-12f, Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(10f), (Bullet) null);
            this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(10f), (Bullet) null);
            this.Fire(new Brave.BulletScript.Direction(12f, Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(10f), (Bullet) null);
            return (IEnumerator) null;
        }
    }

