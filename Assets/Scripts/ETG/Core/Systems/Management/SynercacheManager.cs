using System.Collections.Generic;

using Dungeonator;

#nullable disable

public class SynercacheManager : BraveBehaviour
    {
        public static bool UseCachedSynergyIDs = false;
        public static List<int> LastCachedSynergyIDs = new List<int>();
        public bool TriggersOnMinimapVisibility;
        private bool m_synercached;
        private RoomHandler m_room;

        public static void ClearPerLevelData()
        {
            SynercacheManager.UseCachedSynergyIDs = false;
            SynercacheManager.LastCachedSynergyIDs.Clear();
        }

        private void Start()
        {
            this.m_room = this.transform.position.GetAbsoluteRoom();
            if (this.TriggersOnMinimapVisibility)
                this.m_room.OnRevealedOnMap += new System.Action(this.Cache);
            this.m_room.BecameVisible += new RoomHandler.OnBecameVisibleEventHandler(this.HandleBecameVisible);
        }

        private void HandleBecameVisible(float delay) => this.Cache();

        private void Cache()
        {
            if (this.m_synercached)
                return;
            this.m_synercached = true;
            SynercacheManager.LastCachedSynergyIDs.Clear();
            this.m_room.BecameVisible -= new RoomHandler.OnBecameVisibleEventHandler(this.HandleBecameVisible);
            this.m_room.OnRevealedOnMap -= new System.Action(this.Cache);
            for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
            {
                PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
                for (int index2 = 0; index2 < allPlayer.passiveItems.Count; ++index2)
                {
                    PickupObject passiveItem = (PickupObject) allPlayer.passiveItems[index2];
                    if ((bool) (UnityEngine.Object) passiveItem)
                        SynercacheManager.LastCachedSynergyIDs.Add(passiveItem.PickupObjectId);
                }
                for (int index3 = 0; index3 < allPlayer.activeItems.Count; ++index3)
                {
                    PickupObject activeItem = (PickupObject) allPlayer.activeItems[index3];
                    if ((bool) (UnityEngine.Object) activeItem)
                        SynercacheManager.LastCachedSynergyIDs.Add(activeItem.PickupObjectId);
                }
                for (int index4 = 0; index4 < allPlayer.inventory.AllGuns.Count; ++index4)
                {
                    PickupObject allGun = (PickupObject) allPlayer.inventory.AllGuns[index4];
                    if ((bool) (UnityEngine.Object) allGun)
                        SynercacheManager.LastCachedSynergyIDs.Add(allGun.PickupObjectId);
                }
            }
        }
    }

