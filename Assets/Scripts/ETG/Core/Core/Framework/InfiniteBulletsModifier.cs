using System;

#nullable disable

public class InfiniteBulletsModifier : BraveBehaviour
    {
        public void Start()
        {
            this.projectile.OnDestruction += new Action<Projectile>(this.HandleDestruction);
        }

        private void HandleDestruction(Projectile p)
        {
            if (p.HasImpactedEnemy || !(bool) (UnityEngine.Object) p.PossibleSourceGun || !p.PossibleSourceGun.gameObject.activeSelf)
                return;
            p.PossibleSourceGun.GainAmmo(1);
            p.PossibleSourceGun.ForceFireProjectile(p);
        }
    }

