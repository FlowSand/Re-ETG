using UnityEngine;

#nullable disable

public class HealPlayerItem : PlayerItem
    {
        public float healingAmount = 1f;
        public GameObject healVFX;
        public bool HealsBothPlayers;
        public bool DoesRevive;
        public bool ProvidesTemporaryDamageBuff;
        public float TemporaryDamageMultiplier = 2f;
        public bool IsOrange;
        public bool HasHealingSynergy;
        [LongNumericEnum]
        public CustomSynergyType HealingSynergyRequired;
        [ShowInInspectorIf("HasHealingSynergy", false)]
        public float synergyHealingAmount = 5f;
        protected PlayerController m_buffedTarget;
        protected StatModifier m_temporaryModifier;

        public override bool CanBeUsed(PlayerController user)
        {
            return (!this.DoesRevive || GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER || !GameManager.Instance.PrimaryPlayer.healthHaver.IsAlive || !GameManager.Instance.SecondaryPlayer.healthHaver.IsAlive) && base.CanBeUsed(user);
        }

        protected override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);
            if (this.transform.childCount <= 0)
                return;
            SimpleSpriteRotator[] componentsInChildren = this.GetComponentsInChildren<SimpleSpriteRotator>(true);
            if (componentsInChildren.Length <= 0)
                return;
            componentsInChildren[0].gameObject.SetActive(true);
        }

        public override void Pickup(PlayerController player)
        {
            if (this.transform.childCount > 0)
            {
                SimpleSpriteRotator componentInChildren = this.GetComponentInChildren<SimpleSpriteRotator>();
                if ((bool) (Object) componentInChildren)
                    componentInChildren.gameObject.SetActive(false);
            }
            base.Pickup(player);
        }

        private void RemoveTemporaryBuff(
            float resultValue,
            float maxValue,
            CoreDamageTypes damageTypes,
            DamageCategory damageCategory,
            Vector2 damageDirection)
        {
            this.m_buffedTarget.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.RemoveTemporaryBuff);
            this.m_buffedTarget.ownerlessStatModifiers.Remove(this.m_temporaryModifier);
            this.m_buffedTarget.stats.RecalculateStats(this.m_buffedTarget);
            this.m_temporaryModifier = (StatModifier) null;
            this.m_buffedTarget = (PlayerController) null;
        }

        private float GetHealingAmount(PlayerController user)
        {
            return this.HasHealingSynergy && user.HasActiveBonusSynergy(this.HealingSynergyRequired) ? this.synergyHealingAmount : this.healingAmount;
        }

        protected override void DoEffect(PlayerController user)
        {
            if (this.DoesRevive && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(user);
                if (otherPlayer.healthHaver.IsDead)
                    otherPlayer.ResurrectFromBossKill();
            }
            if (this.IsOrange)
            {
                user.ownerlessStatModifiers.Add(new StatModifier()
                {
                    amount = 1f,
                    modifyType = StatModifier.ModifyMethod.ADDITIVE,
                    statToBoost = PlayerStats.StatType.Health
                });
                user.stats.RecalculateStats(user);
                int num = (int) AkSoundEngine.PostEvent("Play_OBJ_orange_love_01", this.gameObject);
            }
            if (this.ProvidesTemporaryDamageBuff && this.m_temporaryModifier == null)
            {
                this.m_buffedTarget = user;
                this.m_temporaryModifier = new StatModifier();
                this.m_temporaryModifier.statToBoost = PlayerStats.StatType.Damage;
                this.m_temporaryModifier.amount = this.TemporaryDamageMultiplier;
                this.m_temporaryModifier.modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE;
                this.m_temporaryModifier.isMeatBunBuff = true;
                user.ownerlessStatModifiers.Add(this.m_temporaryModifier);
                user.stats.RecalculateStats(user);
                user.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.RemoveTemporaryBuff);
            }
            float healingAmount = this.GetHealingAmount(user);
            if ((double) healingAmount <= 0.0)
                return;
            if (this.HealsBothPlayers)
            {
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                {
                    if (GameManager.Instance.AllPlayers[index].healthHaver.IsAlive)
                    {
                        GameManager.Instance.AllPlayers[index].healthHaver.ApplyHealing(healingAmount);
                        GameManager.Instance.AllPlayers[index].PlayEffectOnActor(this.healVFX, Vector3.zero);
                    }
                }
            }
            else
            {
                user.healthHaver.ApplyHealing(healingAmount);
                if ((Object) this.healVFX != (Object) null)
                    user.PlayEffectOnActor(this.healVFX, Vector3.zero);
            }
            int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", this.gameObject);
        }

        private void LateUpdate()
        {
            if (!this.IsOrange)
                return;
            this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
            this.sprite.renderer.enabled = false;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

