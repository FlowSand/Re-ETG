using UnityEngine;

#nullable disable

public class BreakableSprite : BraveBehaviour
    {
        public bool animations = true;
        public BreakFrame[] breakFrames;

        public void Start()
        {
            if (!(bool) (Object) this.healthHaver)
                return;
            this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnHealthHaverDamaged);
        }

        protected override void OnDestroy() => base.OnDestroy();

        private void OnHealthHaverDamaged(
            float resultValue,
            float maxValue,
            CoreDamageTypes damageTypes,
            DamageCategory damageCategory,
            Vector2 damageDirection)
        {
            for (int index = this.breakFrames.Length - 1; index >= 0; --index)
            {
                if ((double) resultValue / (double) maxValue <= (double) this.breakFrames[index].healthPercentage / 100.0)
                {
                    string sprite = this.breakFrames[index].sprite;
                    if (this.animations)
                    {
                        this.spriteAnimator.Play(sprite);
                        break;
                    }
                    this.sprite.SetSprite(this.breakFrames[index].sprite);
                    break;
                }
            }
        }
    }

