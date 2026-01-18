using System;

using UnityEngine;

using Dungeonator;

#nullable disable

public class Bloodthirst : MonoBehaviour
    {
        private int m_currentNumKillsRequired;
        private int m_currentNumKills;
        private PlayerController m_player;
        private Action<AIActor, float> AuraAction;

        private void Awake()
        {
            this.m_player = this.GetComponent<PlayerController>();
            this.m_player.specRigidbody.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.HandlePostRigidbodyMovement);
            this.m_currentNumKillsRequired = GameManager.Instance.BloodthirstOptions.NumKillsForHealRequiredBase;
            this.m_currentNumKills = 0;
        }

        private void HandlePostRigidbodyMovement(
            SpeculativeRigidbody inSrb,
            Vector2 inVec2,
            IntVector2 inPixels)
        {
            if (!(bool) (UnityEngine.Object) this.m_player || this.m_player.IsGhost || this.m_player.IsStealthed || Dungeon.IsGenerating || (double) BraveTime.DeltaTime == 0.0)
                return;
            RedMatterParticleController matterController = GlobalSparksDoer.GetRedMatterController();
            BloodthirstSettings bloodthirstOptions = GameManager.Instance.BloodthirstOptions;
            float radius = bloodthirstOptions.Radius;
            float damagePerSecond = bloodthirstOptions.DamagePerSecond;
            float percentAffected = bloodthirstOptions.PercentAffected;
            int gainPerHeal = bloodthirstOptions.NumKillsAddedPerHealthGained;
            int maxRequired = bloodthirstOptions.NumKillsRequiredCap;
            if (this.AuraAction == null)
                this.AuraAction = (Action<AIActor, float>) ((actor, dist) =>
                {
                    if (!(bool) (UnityEngine.Object) actor || !(bool) (UnityEngine.Object) actor.healthHaver)
                        return;
                    if (!actor.HasBeenBloodthirstProcessed)
                    {
                        actor.HasBeenBloodthirstProcessed = true;
                        actor.CanBeBloodthirsted = (double) UnityEngine.Random.value < (double) percentAffected;
                        if (actor.CanBeBloodthirsted && (bool) (UnityEngine.Object) actor.sprite)
                        {
                            Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(actor.sprite);
                            if ((UnityEngine.Object) outlineMaterial != (UnityEngine.Object) null)
                                outlineMaterial.SetColor("_OverrideColor", new Color(1f, 0.0f, 0.0f));
                        }
                    }
                    if ((double) dist >= (double) radius || !actor.CanBeBloodthirsted || actor.IsGone)
                        return;
                    float damage = damagePerSecond * BraveTime.DeltaTime;
                    bool isDead = actor.healthHaver.IsDead;
                    actor.healthHaver.ApplyDamage(damage, Vector2.zero, nameof (Bloodthirst));
                    if (!isDead && actor.healthHaver.IsDead)
                    {
                        ++this.m_currentNumKills;
                        if (this.m_currentNumKills >= this.m_currentNumKillsRequired)
                        {
                            this.m_currentNumKills = 0;
                            if ((double) this.m_player.healthHaver.GetCurrentHealthPercentage() < 1.0)
                            {
                                this.m_player.healthHaver.ApplyHealing(0.5f);
                                this.m_currentNumKillsRequired = Mathf.Min(maxRequired, this.m_currentNumKillsRequired + gainPerHeal);
                                GameObject effect = BraveResources.Load<GameObject>("Global VFX/VFX_Healing_Sparkles_001");
                                if ((UnityEngine.Object) effect != (UnityEngine.Object) null)
                                    this.m_player.PlayEffectOnActor(effect, Vector3.zero);
                                int num = (int) AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", this.gameObject);
                            }
                        }
                    }
                    GlobalSparksDoer.DoRadialParticleBurst(3, (Vector3) actor.specRigidbody.HitboxPixelCollider.UnitBottomLeft, (Vector3) actor.specRigidbody.HitboxPixelCollider.UnitTopRight, 90f, 4f, 0.0f, systemType: GlobalSparksDoer.SparksType.RED_MATTER);
                });
            if ((UnityEngine.Object) this.m_player != (UnityEngine.Object) null && this.m_player.CurrentRoom != null)
                this.m_player.CurrentRoom.ApplyActionToNearbyEnemies(this.m_player.CenterPosition, 100f, this.AuraAction);
            if (!(bool) (UnityEngine.Object) matterController)
                return;
            matterController.target.position = (Vector3) this.m_player.CenterPosition;
            matterController.ProcessParticles();
        }
    }

