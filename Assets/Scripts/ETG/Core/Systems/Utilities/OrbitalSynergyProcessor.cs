using System;

using UnityEngine;

#nullable disable

public class OrbitalSynergyProcessor : MonoBehaviour
    {
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        public bool RequiresNoSynergy;
        public PlayerOrbital OrbitalPrefab;
        public PlayerOrbitalFollower OrbitalFollowerPrefab;
        private Gun m_gun;
        private PassiveItem m_item;
        protected GameObject m_extantOrbital;
        private bool m_active;
        private PlayerController m_cachedPlayer;

        private void Awake()
        {
            this.m_gun = this.GetComponent<Gun>();
            this.m_item = this.GetComponent<PassiveItem>();
        }

        private void OnDisable()
        {
            this.DeactivateSynergy();
            this.m_active = false;
        }

        private void CreateOrbital(PlayerController owner)
        {
            this.m_extantOrbital = UnityEngine.Object.Instantiate<GameObject>(!((UnityEngine.Object) this.OrbitalPrefab != (UnityEngine.Object) null) ? this.OrbitalFollowerPrefab.gameObject : this.OrbitalPrefab.gameObject, owner.transform.position, Quaternion.identity);
            if ((UnityEngine.Object) this.OrbitalPrefab != (UnityEngine.Object) null)
            {
                this.m_extantOrbital.GetComponent<PlayerOrbital>().Initialize(owner);
            }
            else
            {
                if (!((UnityEngine.Object) this.OrbitalFollowerPrefab != (UnityEngine.Object) null))
                    return;
                this.m_extantOrbital.GetComponent<PlayerOrbitalFollower>().Initialize(owner);
            }
        }

        private void DestroyOrbital()
        {
            if (!(bool) (UnityEngine.Object) this.m_extantOrbital)
                return;
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantOrbital.gameObject);
            this.m_extantOrbital = (GameObject) null;
        }

        private void HandleNewFloor(PlayerController obj)
        {
            this.DestroyOrbital();
            this.CreateOrbital(obj);
        }

        public void ActivateSynergy(PlayerController player)
        {
            player.OnNewFloorLoaded += new Action<PlayerController>(this.HandleNewFloor);
            this.CreateOrbital(player);
        }

        public void DeactivateSynergy()
        {
            if ((UnityEngine.Object) this.m_cachedPlayer != (UnityEngine.Object) null)
            {
                this.m_cachedPlayer.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloor);
                this.m_cachedPlayer = (PlayerController) null;
            }
            this.DestroyOrbital();
        }

        public void Update()
        {
            PlayerController player = (PlayerController) null;
            if ((bool) (UnityEngine.Object) this.m_gun)
                player = this.m_gun.CurrentOwner as PlayerController;
            else if ((bool) (UnityEngine.Object) this.m_item)
                player = this.m_item.Owner;
            if ((bool) (UnityEngine.Object) player && (this.RequiresNoSynergy || player.HasActiveBonusSynergy(this.RequiredSynergy)) && !this.m_active)
            {
                this.m_active = true;
                this.m_cachedPlayer = player;
                this.ActivateSynergy(player);
            }
            else
            {
                if ((bool) (UnityEngine.Object) player && (this.RequiresNoSynergy || player.HasActiveBonusSynergy(this.RequiredSynergy) || !this.m_active))
                    return;
                this.DeactivateSynergy();
                this.m_active = false;
            }
        }
    }

