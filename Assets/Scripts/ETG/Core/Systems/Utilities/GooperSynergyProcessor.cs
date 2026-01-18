using System;

using UnityEngine;

using Dungeonator;

#nullable disable

public class GooperSynergyProcessor : MonoBehaviour
    {
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        public GoopDefinition goopDefinition;
        public float goopRadius;
        public DamageTypeModifier[] modifiers;
        private PassiveItem m_item;
        private PlayerController m_player;
        private DeadlyDeadlyGoopManager m_manager;
        private bool m_initialized;

        public void Awake()
        {
            this.m_item = this.GetComponent<PassiveItem>();
            this.m_manager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinition);
        }

        private void Initialize(PlayerController p)
        {
            if (this.m_initialized)
                return;
            this.m_initialized = true;
            p.OnIsRolling += new Action<PlayerController>(this.HandleRollFrame);
            for (int index = 0; index < this.modifiers.Length; ++index)
                p.healthHaver.damageTypeModifiers.Add(this.modifiers[index]);
            this.m_player = p;
        }

        private void Uninitialize()
        {
            if (!this.m_initialized)
                return;
            this.m_initialized = false;
            this.m_player.OnIsRolling -= new Action<PlayerController>(this.HandleRollFrame);
            for (int index = 0; index < this.modifiers.Length; ++index)
                this.m_player.healthHaver.damageTypeModifiers.Remove(this.modifiers[index]);
            this.m_player = (PlayerController) null;
        }

        private void Update()
        {
            if (Dungeon.IsGenerating)
                this.m_manager = (DeadlyDeadlyGoopManager) null;
            else if (!GameManager.HasInstance || !(bool) (UnityEngine.Object) GameManager.Instance.Dungeon)
            {
                this.m_manager = (DeadlyDeadlyGoopManager) null;
            }
            else
            {
                if (!(bool) (UnityEngine.Object) this.m_manager)
                    this.m_manager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinition);
                if (this.m_initialized)
                {
                    if ((!(bool) (UnityEngine.Object) this.m_item || (bool) (UnityEngine.Object) this.m_item.Owner) && this.m_item.Owner.HasActiveBonusSynergy(this.RequiredSynergy))
                        return;
                    this.Uninitialize();
                }
                else
                {
                    if (!(bool) (UnityEngine.Object) this.m_item || !(bool) (UnityEngine.Object) this.m_item.Owner || !this.m_item.Owner.HasActiveBonusSynergy(this.RequiredSynergy))
                        return;
                    this.Initialize(this.m_item.Owner);
                }
            }
        }

        private void HandleRollFrame(PlayerController p)
        {
            if (GameManager.Instance.IsFoyer || GameManager.Instance.Dungeon.IsEndTimes)
                return;
            this.m_manager.AddGoopCircle(p.specRigidbody.UnitCenter, this.goopRadius);
        }
    }

