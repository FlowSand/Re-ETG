using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class PickupObjectDatabase : ObjectDatabase<PickupObject>
    {
        private static PickupObjectDatabase m_instance;

        public static PickupObjectDatabase Instance
        {
            get
            {
                if ((UnityEngine.Object) PickupObjectDatabase.m_instance == (UnityEngine.Object) null)
                    PickupObjectDatabase.m_instance = BraveResources.Load<PickupObjectDatabase>(nameof (PickupObjectDatabase), ".asset");
                return PickupObjectDatabase.m_instance;
            }
        }

        public static bool HasInstance => (UnityEngine.Object) PickupObjectDatabase.m_instance != (UnityEngine.Object) null;

        public static void Unload()
        {
            PickupObjectDatabase.m_instance = (PickupObjectDatabase) null;
            Resources.UnloadAsset((UnityEngine.Object) PickupObjectDatabase.m_instance);
        }

        public static int GetId(PickupObject obj) => PickupObjectDatabase.Instance.InternalGetId(obj);

        public static PickupObject GetById(int id) => PickupObjectDatabase.Instance.InternalGetById(id);

        public static PickupObject GetByName(string name)
        {
            return PickupObjectDatabase.Instance.InternalGetByName(name);
        }

        public static Gun GetRandomGun()
        {
            List<Gun> gunList = new List<Gun>();
            for (int index = 0; index < PickupObjectDatabase.Instance.Objects.Count; ++index)
            {
                if ((UnityEngine.Object) PickupObjectDatabase.Instance.Objects[index] != (UnityEngine.Object) null && PickupObjectDatabase.Instance.Objects[index] is Gun && PickupObjectDatabase.Instance.Objects[index].quality != PickupObject.ItemQuality.EXCLUDED && PickupObjectDatabase.Instance.Objects[index].quality != PickupObject.ItemQuality.SPECIAL && PickupObjectDatabase.Instance.Objects[index].contentSource != ContentSource.EXCLUDED && !(PickupObjectDatabase.Instance.Objects[index] is ContentTeaserGun))
                {
                    EncounterTrackable component = PickupObjectDatabase.Instance.Objects[index].GetComponent<EncounterTrackable>();
                    if ((bool) (UnityEngine.Object) component && component.PrerequisitesMet())
                        gunList.Add(PickupObjectDatabase.Instance.Objects[index] as Gun);
                }
            }
            return gunList[UnityEngine.Random.Range(0, gunList.Count)];
        }

        public static Gun GetRandomStartingGun(System.Random usedRandom)
        {
            List<Gun> gunList = new List<Gun>();
            for (int index = 0; index < PickupObjectDatabase.Instance.Objects.Count; ++index)
            {
                if ((UnityEngine.Object) PickupObjectDatabase.Instance.Objects[index] != (UnityEngine.Object) null && PickupObjectDatabase.Instance.Objects[index] is Gun && PickupObjectDatabase.Instance.Objects[index].quality != PickupObject.ItemQuality.EXCLUDED && !(PickupObjectDatabase.Instance.Objects[index] is ContentTeaserGun) && (PickupObjectDatabase.Instance.Objects[index] as Gun).StarterGunForAchievement && (PickupObjectDatabase.Instance.Objects[index] as Gun).InfiniteAmmo)
                {
                    EncounterTrackable component = PickupObjectDatabase.Instance.Objects[index].GetComponent<EncounterTrackable>();
                    if ((bool) (UnityEngine.Object) component && component.PrerequisitesMet())
                        gunList.Add(PickupObjectDatabase.Instance.Objects[index] as Gun);
                }
            }
            return gunList[usedRandom.Next(gunList.Count)];
        }

        public static Gun GetRandomGunOfQualities(
            System.Random usedRandom,
            List<int> excludedIDs,
            params PickupObject.ItemQuality[] qualities)
        {
            List<Gun> gunList = new List<Gun>();
            for (int index = 0; index < PickupObjectDatabase.Instance.Objects.Count; ++index)
            {
                if ((UnityEngine.Object) PickupObjectDatabase.Instance.Objects[index] != (UnityEngine.Object) null && PickupObjectDatabase.Instance.Objects[index] is Gun && PickupObjectDatabase.Instance.Objects[index].quality != PickupObject.ItemQuality.EXCLUDED && PickupObjectDatabase.Instance.Objects[index].quality != PickupObject.ItemQuality.SPECIAL && !(PickupObjectDatabase.Instance.Objects[index] is ContentTeaserGun) && Array.IndexOf<PickupObject.ItemQuality>(qualities, PickupObjectDatabase.Instance.Objects[index].quality) != -1 && !excludedIDs.Contains(PickupObjectDatabase.Instance.Objects[index].PickupObjectId) && (PickupObjectDatabase.Instance.Objects[index].PickupObjectId != GlobalItemIds.UnfinishedGun || !GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_AMMONOMICON_COMPLETE)))
                {
                    EncounterTrackable component = PickupObjectDatabase.Instance.Objects[index].GetComponent<EncounterTrackable>();
                    if ((bool) (UnityEngine.Object) component && component.PrerequisitesMet())
                        gunList.Add(PickupObjectDatabase.Instance.Objects[index] as Gun);
                }
            }
            int index1 = usedRandom.Next(gunList.Count);
            return index1 < 0 || index1 >= gunList.Count ? (Gun) null : gunList[index1];
        }

        public static PassiveItem GetRandomPassiveOfQualities(
            System.Random usedRandom,
            List<int> excludedIDs,
            params PickupObject.ItemQuality[] qualities)
        {
            List<PassiveItem> passiveItemList = new List<PassiveItem>();
            for (int index = 0; index < PickupObjectDatabase.Instance.Objects.Count; ++index)
            {
                if ((UnityEngine.Object) PickupObjectDatabase.Instance.Objects[index] != (UnityEngine.Object) null && PickupObjectDatabase.Instance.Objects[index] is PassiveItem && PickupObjectDatabase.Instance.Objects[index].quality != PickupObject.ItemQuality.EXCLUDED && PickupObjectDatabase.Instance.Objects[index].quality != PickupObject.ItemQuality.SPECIAL && !(PickupObjectDatabase.Instance.Objects[index] is ContentTeaserItem) && Array.IndexOf<PickupObject.ItemQuality>(qualities, PickupObjectDatabase.Instance.Objects[index].quality) != -1 && !excludedIDs.Contains(PickupObjectDatabase.Instance.Objects[index].PickupObjectId))
                {
                    EncounterTrackable component = PickupObjectDatabase.Instance.Objects[index].GetComponent<EncounterTrackable>();
                    if ((bool) (UnityEngine.Object) component && component.PrerequisitesMet())
                        passiveItemList.Add(PickupObjectDatabase.Instance.Objects[index] as PassiveItem);
                }
            }
            int index1 = usedRandom.Next(passiveItemList.Count);
            return index1 < 0 || index1 >= passiveItemList.Count ? (PassiveItem) null : passiveItemList[index1];
        }

        public static PickupObject GetByEncounterName(string name)
        {
            for (int index = 0; index < PickupObjectDatabase.Instance.Objects.Count; ++index)
            {
                PickupObject byEncounterName = PickupObjectDatabase.Instance.Objects[index];
                if ((bool) (UnityEngine.Object) byEncounterName)
                {
                    EncounterTrackable encounterTrackable = byEncounterName.encounterTrackable;
                    if ((bool) (UnityEngine.Object) encounterTrackable && encounterTrackable.journalData.GetPrimaryDisplayName().Equals(name, StringComparison.OrdinalIgnoreCase))
                        return byEncounterName;
                }
            }
            return (PickupObject) null;
        }
    }

