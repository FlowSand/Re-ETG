using System;
using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class RagePassiveItem : PassiveItem
    {
        public float Duration = 3f;
        public float DamageMultiplier = 2f;
        public Color flatColorOverride = new Color(0.5f, 0.0f, 0.0f, 0.75f);
        public GameObject OverheadVFX;
        private bool m_isRaged;
        private float m_elapsed;
        private GameObject instanceVFX;
        private PlayerController m_player;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            base.Pickup(player);
            player.OnReceivedDamage += new Action<PlayerController>(this.HandleReceivedDamage);
            this.m_player = player;
        }

        private void HandleReceivedDamage(PlayerController obj)
        {
            if (this.m_isRaged)
            {
                if ((bool) (UnityEngine.Object) this.OverheadVFX && !(bool) (UnityEngine.Object) this.instanceVFX)
                    this.instanceVFX = this.m_player.PlayEffectOnActor(this.OverheadVFX, new Vector3(0.0f, 1.375f, 0.0f), alreadyMiddleCenter: true);
                this.m_elapsed = 0.0f;
            }
            else
                obj.StartCoroutine(this.HandleRage());
        }

        [DebuggerHidden]
        private IEnumerator HandleRage()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new RagePassiveItem__HandleRagec__Iterator0()
            {
                _this = this
            };
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            debrisObject.GetComponent<RagePassiveItem>().m_pickedUpThisRun = true;
            player.OnReceivedDamage -= new Action<PlayerController>(this.HandleReceivedDamage);
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            if ((UnityEngine.Object) this.m_player != (UnityEngine.Object) null)
                this.m_player.OnReceivedDamage -= new Action<PlayerController>(this.HandleReceivedDamage);
            base.OnDestroy();
        }
    }

