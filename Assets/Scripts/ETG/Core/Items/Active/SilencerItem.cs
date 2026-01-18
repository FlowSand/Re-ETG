using UnityEngine;

using Dungeonator;

#nullable disable

public class SilencerItem : PlayerItem
    {
        public bool destroysEnemyBullets = true;
        public bool destroysPlayerBullets;
        public float silencerRadius = 25f;
        public float silencerSpeed = 50f;
        public float additionalTimeAtMaxRadius = 1f;
        public float distortionIntensity = 0.1f;
        public float distortionRadius = 0.1f;
        public float pushForce = 12f;
        public float pushRadius = 10f;
        public float knockbackForce = 12f;
        public float knockbackRadius = 7f;
        public GameObject silencerVFXPrefab;

        protected override void Start()
        {
            this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.TriggerWasEntered);
            this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTrigger);
            base.Start();
        }

        private void OnTrigger(
            SpeculativeRigidbody otherRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody,
            CollisionData collisionData)
        {
            if (this.m_pickedUp)
                return;
            PlayerController component = otherRigidbody.GetComponent<PlayerController>();
            if (!((Object) component != (Object) null))
                return;
            this.Pickup(component);
        }

        private void TriggerWasEntered(
            SpeculativeRigidbody otherRigidbody,
            SpeculativeRigidbody selfRigidbody,
            CollisionData collisionData)
        {
            if (this.m_pickedUp)
                return;
            PlayerController component = otherRigidbody.GetComponent<PlayerController>();
            if (!((Object) component != (Object) null))
                return;
            this.Pickup(component);
        }

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            if (GameManager.Instance.InTutorial)
                GameManager.BroadcastRoomTalkDoerFsmEvent("playerAcquiredPlayerItem");
            this.m_pickedUp = true;
            if (!GameManager.Instance.InTutorial && GameStatsManager.Instance.QueryEncounterable(this.encounterTrackable) < 3)
                this.HandleEncounterable(player);
            if (RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
                RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
            this.GetRidOfMinimapIcon();
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_pickup_01", this.gameObject);
            tk2dSprite component1 = Object.Instantiate<GameObject>((GameObject) ResourceCache.Acquire("Global VFX/VFX_Item_Pickup")).GetComponent<tk2dSprite>();
            component1.PlaceAtPositionByAnchor((Vector3) this.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            component1.UpdateZDepth();
            DebrisObject component2 = this.GetComponent<DebrisObject>();
            if (this.ForceAsExtant || (Object) component2 != (Object) null)
            {
                ++player.Blanks;
                Object.Destroy((Object) this.gameObject);
                this.m_pickedUp = true;
                this.m_pickedUpThisRun = true;
            }
            else
            {
                ++player.Blanks;
                this.m_pickedUp = true;
                this.m_pickedUpThisRun = true;
            }
            this.GetRidOfMinimapIcon();
        }

        protected override void DoEffect(PlayerController user)
        {
            int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", this.gameObject);
            int num2 = (int) AkSoundEngine.PostEvent("Stop_ENM_attack_cancel_01", this.gameObject);
            new GameObject("silencer").AddComponent<SilencerInstance>().TriggerSilencer(user.CenterPosition, this.silencerSpeed, this.silencerRadius, this.silencerVFXPrefab, this.distortionIntensity, this.distortionRadius, this.pushForce, this.pushRadius, this.knockbackForce, this.knockbackRadius, this.additionalTimeAtMaxRadius, user);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

