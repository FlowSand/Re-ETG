using System;

using UnityEngine;

#nullable disable

public class FriendlyFireChallengeModifier : ChallengeModifier
    {
        private void Start()
        {
            GameManager.PVP_ENABLED = true;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                GameManager.Instance.AllPlayers[index].PostProcessProjectile += new Action<Projectile, float>(this.ModifyProjectile);
        }

        private void ModifyProjectile(Projectile proj, float somethin)
        {
            if (!(bool) (UnityEngine.Object) proj || proj.TreatedAsNonProjectileForChallenge)
                return;
            tk2dBaseSprite componentInChildren1 = proj.GetComponentInChildren<tk2dBaseSprite>();
            Renderer componentInChildren2 = proj.GetComponentInChildren<Renderer>();
            if (!(bool) (UnityEngine.Object) componentInChildren1 || (bool) (UnityEngine.Object) componentInChildren1.GetComponent<TrailController>() || !(bool) (UnityEngine.Object) componentInChildren2 || !componentInChildren2.enabled)
                return;
            BounceProjModifier bounceProjModifier = proj.GetComponent<BounceProjModifier>();
            if (!(bool) (UnityEngine.Object) bounceProjModifier)
            {
                bounceProjModifier = proj.gameObject.AddComponent<BounceProjModifier>();
                bounceProjModifier.numberOfBounces = 1;
                bounceProjModifier.onlyBounceOffTiles = true;
            }
            bounceProjModifier.OnBounceContext += new Action<BounceProjModifier, SpeculativeRigidbody>(this.OnFirstBounce);
        }

        private void OnFirstBounce(BounceProjModifier mod, SpeculativeRigidbody otherRigidbody)
        {
            if (!(bool) (UnityEngine.Object) mod)
                return;
            mod.OnBounceContext -= new Action<BounceProjModifier, SpeculativeRigidbody>(this.OnFirstBounce);
            Projectile component = mod.GetComponent<Projectile>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            if ((bool) (UnityEngine.Object) otherRigidbody && (bool) (UnityEngine.Object) otherRigidbody.minorBreakable)
            {
                component.DieInAir();
            }
            else
            {
                component.MakeLookLikeEnemyBullet(false);
                component.baseData.speed = Mathf.Min(component.baseData.speed, 10f);
                component.Speed = Mathf.Min(component.Speed, 10f);
                component.allowSelfShooting = true;
                component.ForcePlayerBlankable = true;
                if (!(bool) (UnityEngine.Object) component.Shooter)
                    return;
                component.specRigidbody.DeregisterSpecificCollisionException(component.Shooter);
                component.specRigidbody.RegisterGhostCollisionException(component.Shooter);
            }
        }

        private void OnDestroy()
        {
            GameManager.PVP_ENABLED = false;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                GameManager.Instance.AllPlayers[index].PostProcessProjectile -= new Action<Projectile, float>(this.ModifyProjectile);
        }
    }

