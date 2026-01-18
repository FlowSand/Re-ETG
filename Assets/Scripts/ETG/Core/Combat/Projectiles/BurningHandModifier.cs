using System;

using UnityEngine;

#nullable disable

public class BurningHandModifier : MonoBehaviour
    {
        public float MinDamageMultiplier = 1f;
        public float MaxDamageMultiplier = 10f;
        public float MinScale = 0.5f;
        public float MaxScale = 2.5f;
        public float MaxRoll = 13f;
        private Gun m_gun;

        private void Awake()
        {
            this.m_gun = this.GetComponent<Gun>();
            this.m_gun.PostProcessProjectile += new Action<Projectile>(this.HandleProjectileMod);
        }

        private void HandleProjectileMod(Projectile p)
        {
            int num1 = UnityEngine.Random.Range(1, 7) + UnityEngine.Random.Range(1, 7);
            int num2 = 0;
            if (this.m_gun.CurrentOwner is PlayerController)
            {
                switch ((this.m_gun.CurrentOwner as PlayerController).characterIdentity)
                {
                    case PlayableCharacters.Robot:
                        num2 = 1;
                        break;
                    case PlayableCharacters.Bullet:
                        num2 = -1;
                        break;
                }
            }
            int b = Mathf.Clamp(num1 + num2, 1, 100);
            int count = 0;
            if (PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.LOADED_DICE, out count))
                b = Mathf.Max(12, b);
            float num3 = Mathf.Lerp(this.MinScale, this.MaxScale, Mathf.Clamp01((float) b / this.MaxRoll));
            float num4 = Mathf.Lerp(this.MinDamageMultiplier, this.MaxDamageMultiplier, Mathf.Clamp01((float) b / this.MaxRoll));
            p.AdditionalScaleMultiplier *= num3;
            p.baseData.damage *= num4;
        }
    }

