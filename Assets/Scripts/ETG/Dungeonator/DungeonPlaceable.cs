using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace Dungeonator
{
    public class DungeonPlaceable : ScriptableObject
    {
        public int width;
        public int height;
        [SerializeField]
        public bool isPassable = true;
        [SerializeField]
        public bool roomSequential;
        [SerializeField]
        public bool respectsEncounterableDifferentiator;
        [SerializeField]
        public bool UsePrefabTransformOffset;
        [SerializeField]
        public bool MarkSpawnedItemsAsRatIgnored;
        [SerializeField]
        public bool DebugThisPlaceable;
        [SerializeField]
        public bool IsAnnexTable;
        [SerializeField]
        public List<DungeonPlaceableVariant> variantTiers;

        public int GetHeight() => this.height;

        public int GetWidth() => this.width;

        public bool ContainsEnemy
        {
            get
            {
                for (int index = 0; index < this.variantTiers.Count; ++index)
                {
                    if (!string.IsNullOrEmpty(this.variantTiers[index].enemyPlaceableGuid) && EnemyDatabase.GetEntry(this.variantTiers[index].enemyPlaceableGuid) != null)
                        return true;
                }
                return false;
            }
        }

        public bool ContainsEnemylikeObjectForReinforcement
        {
            get
            {
                for (int index = 0; index < this.variantTiers.Count; ++index)
                {
                    if ((bool) (UnityEngine.Object) this.variantTiers[index].nonDatabasePlaceable && (bool) (UnityEngine.Object) this.variantTiers[index].nonDatabasePlaceable.GetComponent<ForgeHammerController>())
                        return true;
                }
                return false;
            }
        }

        public bool IsValidMirrorPlaceable()
        {
            for (int index = 0; index < this.variantTiers.Count; ++index)
            {
                if ((bool) (UnityEngine.Object) this.variantTiers[index].nonDatabasePlaceable && !PrototypeDungeonRoom.GameObjectCanBeMirrored(this.variantTiers[index].nonDatabasePlaceable))
                    return false;
            }
            return true;
        }

        public bool HasValidPlaceable()
        {
            for (int index1 = 0; index1 < this.variantTiers.Count; ++index1)
            {
                bool flag = true;
                if (this.variantTiers[index1] != null)
                {
                    if (this.variantTiers[index1].prerequisites == null)
                        return true;
                    for (int index2 = 0; index2 < this.variantTiers[index1].prerequisites.Length; ++index2)
                    {
                        if (!this.variantTiers[index1].prerequisites[index2].CheckConditionsFulfilled())
                            flag = false;
                    }
                    if (flag)
                        return true;
                }
            }
            return false;
        }

        private GameObject InstantiateInternal(
            DungeonPlaceableVariant selectedVariant,
            RoomHandler targetRoom,
            IntVector2 location,
            bool deferConfiguration)
        {
            if (selectedVariant == null || !((UnityEngine.Object) selectedVariant.GetOrLoadPlaceableObject != (UnityEngine.Object) null))
                return (GameObject) null;
            GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(selectedVariant.GetOrLoadPlaceableObject, targetRoom, location, deferConfiguration);
            if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null && selectedVariant.unitOffset != Vector2.zero)
            {
                gameObject.transform.position += selectedVariant.unitOffset.ToVector3ZUp();
                SpeculativeRigidbody componentInChildren = gameObject.GetComponentInChildren<SpeculativeRigidbody>();
                if ((bool) (UnityEngine.Object) componentInChildren)
                    componentInChildren.Reinitialize();
            }
            if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null && this.UsePrefabTransformOffset)
                gameObject.transform.position += selectedVariant.GetOrLoadPlaceableObject.transform.position;
            if (selectedVariant.forceBlackPhantom && (bool) (UnityEngine.Object) gameObject)
            {
                AIActor component = gameObject.GetComponent<AIActor>();
                if ((bool) (UnityEngine.Object) component)
                    component.ForceBlackPhantom = true;
            }
            if (selectedVariant.addDebrisObject && (bool) (UnityEngine.Object) gameObject)
            {
                DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                orAddComponent.shouldUseSRBMotion = true;
                orAddComponent.angularVelocity = 0.0f;
                orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                orAddComponent.canRotate = false;
            }
            if (this.MarkSpawnedItemsAsRatIgnored && (bool) (UnityEngine.Object) gameObject)
            {
                PickupObject component = gameObject.GetComponent<PickupObject>();
                if ((bool) (UnityEngine.Object) component)
                    component.IgnoredByRat = true;
            }
            return gameObject;
        }

        private GameObject InstantiateInternalOnlyActors(
            DungeonPlaceableVariant selectedVariant,
            RoomHandler targetRoom,
            IntVector2 location,
            bool deferConfiguration)
        {
            if (selectedVariant == null || !((UnityEngine.Object) selectedVariant.GetOrLoadPlaceableObject != (UnityEngine.Object) null))
                return (GameObject) null;
            GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceableOnlyActors(selectedVariant.GetOrLoadPlaceableObject, targetRoom, location, deferConfiguration);
            if (selectedVariant.forceBlackPhantom && (bool) (UnityEngine.Object) gameObject)
            {
                AIActor component = gameObject.GetComponent<AIActor>();
                if ((bool) (UnityEngine.Object) component)
                    component.ForceBlackPhantom = true;
            }
            return gameObject;
        }

        public GameObject InstantiateObjectDirectional(
            RoomHandler targetRoom,
            IntVector2 location,
            DungeonData.Direction direction)
        {
            List<DungeonPlaceableVariant> variants = new List<DungeonPlaceableVariant>();
            if (this.variantTiers.Count == 4)
            {
                switch (direction)
                {
                    case DungeonData.Direction.NORTH:
                        variants.Add(this.variantTiers[0]);
                        break;
                    case DungeonData.Direction.EAST:
                        variants.Add(this.variantTiers[1]);
                        break;
                    case DungeonData.Direction.SOUTH:
                        variants.Add(this.variantTiers[2]);
                        break;
                    case DungeonData.Direction.WEST:
                        variants.Add(this.variantTiers[3]);
                        break;
                }
                return this.InstantiateInternal(this.SelectVariantByWeighting(variants), targetRoom, location, false);
            }
            foreach (DungeonPlaceableVariant variantTier in this.variantTiers)
            {
                variantTier.percentChanceMultiplier = 1f;
                if (this.ProcessVariantPrerequisites(variantTier, new IntVector2?(location), targetRoom))
                {
                    DungeonDoorController component1 = variantTier.nonDatabasePlaceable.GetComponent<DungeonDoorController>();
                    DungeonDoorSubsidiaryBlocker component2 = variantTier.nonDatabasePlaceable.GetComponent<DungeonDoorSubsidiaryBlocker>();
                    if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
                    {
                        if (component1.northSouth && (direction == DungeonData.Direction.NORTH || direction == DungeonData.Direction.SOUTH))
                            variants.Add(variantTier);
                        else if (!component1.northSouth && (direction == DungeonData.Direction.EAST || direction == DungeonData.Direction.WEST))
                            variants.Add(variantTier);
                    }
                    else if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
                    {
                        if (component2.northSouth && (direction == DungeonData.Direction.NORTH || direction == DungeonData.Direction.SOUTH))
                            variants.Add(variantTier);
                        else if (!component2.northSouth && (direction == DungeonData.Direction.EAST || direction == DungeonData.Direction.WEST))
                            variants.Add(variantTier);
                    }
                    else
                        variants.Add(variantTier);
                }
            }
            return this.InstantiateInternal(this.SelectVariantByWeighting(variants), targetRoom, location, false);
        }

        public GameObject InstantiateObject(
            RoomHandler targetRoom,
            IntVector2 location,
            bool onlyActors = false,
            bool deferConfiguration = false)
        {
            int variantIndex = -1;
            return this.InstantiateObject(targetRoom, location, out variantIndex, onlyActors: onlyActors, deferConfiguration: deferConfiguration);
        }

        public void ModifyWeightsByDifficulty(List<DungeonPlaceableVariant> validVariants)
        {
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                return;
            if ((UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null || (UnityEngine.Object) GameManager.Instance.PrimaryPlayer.stats == (UnityEngine.Object) null)
            {
                Debug.LogError((object) "No player yet--can't check curse stat in DungeonPlaceable.");
            }
            else
            {
                Dictionary<DungeonPlaceableBehaviour.PlaceableDifficulty, float> dictionary = new Dictionary<DungeonPlaceableBehaviour.PlaceableDifficulty, float>();
                float num1 = 0.0f;
                for (int index = 0; index < validVariants.Count; ++index)
                {
                    DungeonPlaceableBehaviour.PlaceableDifficulty difficulty = validVariants[index].difficulty;
                    if (!dictionary.ContainsKey(difficulty))
                        dictionary.Add(difficulty, 0.0f);
                    dictionary[difficulty] += validVariants[index].GetPercentChance();
                    num1 += validVariants[index].GetPercentChance();
                }
                if (dictionary.Count <= 1)
                    return;
                float num2 = Mathf.Clamp01((float) PlayerStats.GetTotalCurse() / 10f);
                float num3 = (!dictionary.ContainsKey(DungeonPlaceableBehaviour.PlaceableDifficulty.BASE) ? 0.0f : dictionary[DungeonPlaceableBehaviour.PlaceableDifficulty.BASE]) / num1;
                float num4 = (!dictionary.ContainsKey(DungeonPlaceableBehaviour.PlaceableDifficulty.HARD) ? 0.0f : dictionary[DungeonPlaceableBehaviour.PlaceableDifficulty.HARD]) / num1;
                float num5 = (!dictionary.ContainsKey(DungeonPlaceableBehaviour.PlaceableDifficulty.HARDER) ? 0.0f : dictionary[DungeonPlaceableBehaviour.PlaceableDifficulty.HARDER]) / num1;
                float num6 = (!dictionary.ContainsKey(DungeonPlaceableBehaviour.PlaceableDifficulty.HARDEST) ? 0.0f : dictionary[DungeonPlaceableBehaviour.PlaceableDifficulty.HARDEST]) / num1;
                if ((double) num3 > (double) num2)
                {
                    float num7 = num3 - num2;
                    for (int index = 0; index < validVariants.Count; ++index)
                    {
                        if (validVariants[index].difficultyRating == 0)
                            validVariants[index].percentChanceMultiplier = num7 / num3;
                    }
                }
                else if ((double) num3 + (double) num4 > (double) num2)
                {
                    float num8 = num3 + num4 - num2;
                    for (int index = 0; index < validVariants.Count; ++index)
                    {
                        if (validVariants[index].difficultyRating <= 0)
                        {
                            validVariants.RemoveAt(index);
                            --index;
                        }
                        else if (validVariants[index].difficultyRating == 1)
                            validVariants[index].percentChanceMultiplier = num8 / num4;
                    }
                }
                else if ((double) num3 + (double) num4 + (double) num5 > (double) num2)
                {
                    float num9 = num3 + num4 + num5 - num2;
                    for (int index = 0; index < validVariants.Count; ++index)
                    {
                        if (validVariants[index].difficultyRating <= 1)
                        {
                            validVariants.RemoveAt(index);
                            --index;
                        }
                        else if (validVariants[index].difficultyRating == 2)
                            validVariants[index].percentChanceMultiplier = num9 / num5;
                    }
                }
                else
                {
                    if ((double) num3 + (double) num4 + (double) num5 + (double) num6 < (double) num2)
                        return;
                    for (int index = 0; index < validVariants.Count; ++index)
                    {
                        if (validVariants[index].difficultyRating <= 2)
                        {
                            validVariants.RemoveAt(index);
                            --index;
                        }
                    }
                }
            }
        }

        public GameObject InstantiateObject(
            RoomHandler targetRoom,
            IntVector2 location,
            out int variantIndex,
            int forceVariant = -1,
            bool onlyActors = false,
            bool deferConfiguration = false)
        {
            variantIndex = -1;
            List<DungeonPlaceableVariant> placeableVariantList = new List<DungeonPlaceableVariant>();
            int maxValue = int.MaxValue;
            for (int index = 0; index < this.variantTiers.Count; ++index)
            {
                DungeonPlaceableVariant variantTier = this.variantTiers[index];
                variantTier.percentChanceMultiplier = 1f;
                if (this.ProcessVariantPrerequisites(variantTier, new IntVector2?(location), targetRoom))
                {
                    if (this.respectsEncounterableDifferentiator)
                    {
                        int? nullable = new int?();
                        if ((UnityEngine.Object) variantTier.nonDatabasePlaceable != (UnityEngine.Object) null)
                        {
                            EncounterTrackable component = variantTier.nonDatabasePlaceable.GetComponent<EncounterTrackable>();
                            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                                nullable = new int?(GameStatsManager.Instance.QueryEncounterableDifferentiator(component));
                        }
                        else if (!string.IsNullOrEmpty(variantTier.enemyPlaceableGuid))
                        {
                            EnemyDatabaseEntry entry1 = EnemyDatabase.GetEntry(variantTier.enemyPlaceableGuid);
                            if (entry1 != null && !string.IsNullOrEmpty(entry1.encounterGuid))
                            {
                                EncounterDatabaseEntry entry2 = EncounterDatabase.GetEntry(entry1.encounterGuid);
                                if (entry2 != null)
                                    nullable = new int?(GameStatsManager.Instance.QueryEncounterableDifferentiator(entry2));
                            }
                        }
                        if (nullable.HasValue)
                        {
                            if (nullable.Value < maxValue)
                            {
                                placeableVariantList.Clear();
                                maxValue = nullable.Value;
                            }
                            else if (nullable.Value > maxValue)
                                continue;
                        }
                    }
                    if (targetRoom == null || !this.roomSequential || index <= targetRoom.distanceFromEntrance / 2)
                        placeableVariantList.Add(variantTier);
                }
            }
            DungeonPlaceableVariant selectedVariant = (DungeonPlaceableVariant) null;
            this.ModifyWeightsByDifficulty(placeableVariantList);
            if (forceVariant == -1)
                selectedVariant = this.SelectVariantByWeighting(placeableVariantList);
            else if (placeableVariantList.Count > forceVariant)
                selectedVariant = placeableVariantList[forceVariant];
            if (selectedVariant != null)
                variantIndex = this.variantTiers.IndexOf(selectedVariant);
            if (this.respectsEncounterableDifferentiator && selectedVariant != null && (UnityEngine.Object) selectedVariant.GetOrLoadPlaceableObject != (UnityEngine.Object) null)
            {
                EncounterTrackable component = selectedVariant.GetOrLoadPlaceableObject.GetComponent<EncounterTrackable>();
                if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                    component.HandleEncounter_GeneratedObjects();
            }
            if (selectedVariant != null && (UnityEngine.Object) selectedVariant.GetOrLoadPlaceableObject != (UnityEngine.Object) null)
            {
                DungeonPlaceableBehaviour component1 = selectedVariant.GetOrLoadPlaceableObject.GetComponent<DungeonPlaceableBehaviour>();
                if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
                {
                    GameObject gameObject = !onlyActors ? component1.InstantiateObject(targetRoom, location, deferConfiguration) : component1.InstantiateObjectOnlyActors(targetRoom, location, deferConfiguration);
                    if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null && selectedVariant.unitOffset != Vector2.zero)
                    {
                        gameObject.transform.position += selectedVariant.unitOffset.ToVector3ZUp();
                        SpeculativeRigidbody componentInChildren = gameObject.GetComponentInChildren<SpeculativeRigidbody>();
                        if ((bool) (UnityEngine.Object) componentInChildren)
                            componentInChildren.Reinitialize();
                    }
                    if (selectedVariant.forceBlackPhantom && (bool) (UnityEngine.Object) gameObject)
                    {
                        AIActor component2 = gameObject.GetComponent<AIActor>();
                        if ((bool) (UnityEngine.Object) component2)
                            component2.ForceBlackPhantom = true;
                    }
                    return gameObject;
                }
            }
            return onlyActors ? this.InstantiateInternalOnlyActors(selectedVariant, targetRoom, location, deferConfiguration) : this.InstantiateInternal(selectedVariant, targetRoom, location, deferConfiguration);
        }

        private bool ProcessVariantPrerequisites(
            DungeonPlaceableVariant dpv,
            IntVector2? targetPosition = null,
            RoomHandler targetRoom = null)
        {
            if (targetRoom != null && targetPosition.HasValue && dpv.materialRequirements != null && dpv.materialRequirements.Length > 0)
            {
                bool flag = true;
                for (int index = 0; index < dpv.materialRequirements.Length; ++index)
                {
                    if (dpv.materialRequirements[index].TargetTileset != GameManager.Instance.Dungeon.tileIndices.tilesetId)
                    {
                        if (dpv.materialRequirements[index].RequireMaterial)
                            return false;
                    }
                    else
                    {
                        int roomVisualTypeIndex = GameManager.Instance.Dungeon.data[targetRoom.area.basePosition + targetPosition.Value].cellVisualData.roomVisualTypeIndex;
                        if ((!dpv.materialRequirements[index].RequireMaterial || dpv.materialRequirements[index].RoomMaterial != roomVisualTypeIndex) && (dpv.materialRequirements[index].RequireMaterial || dpv.materialRequirements[index].RoomMaterial == roomVisualTypeIndex))
                            flag = false;
                    }
                }
                if (!flag)
                    return false;
            }
            if (dpv.prerequisites == null || dpv.prerequisites.Length <= 0)
                return true;
            bool flag1 = true;
            for (int index = 0; index < dpv.prerequisites.Length; ++index)
            {
                if (!dpv.prerequisites[index].CheckConditionsFulfilled())
                {
                    flag1 = false;
                    break;
                }
            }
            return flag1;
        }

        public DungeonPlaceableVariant SelectFromTiersFull()
        {
            List<DungeonPlaceableVariant> placeableVariantList = new List<DungeonPlaceableVariant>();
            int maxValue = int.MaxValue;
            for (int index = 0; index < this.variantTiers.Count; ++index)
            {
                DungeonPlaceableVariant variantTier = this.variantTiers[index];
                variantTier.percentChanceMultiplier = 1f;
                if (this.ProcessVariantPrerequisites(variantTier))
                {
                    if (this.respectsEncounterableDifferentiator)
                    {
                        int? nullable = new int?();
                        if ((UnityEngine.Object) variantTier.nonDatabasePlaceable != (UnityEngine.Object) null)
                        {
                            EncounterTrackable component = variantTier.nonDatabasePlaceable.GetComponent<EncounterTrackable>();
                            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                                nullable = new int?(GameStatsManager.Instance.QueryEncounterableDifferentiator(component));
                        }
                        else if (!string.IsNullOrEmpty(variantTier.enemyPlaceableGuid))
                        {
                            EnemyDatabaseEntry entry1 = EnemyDatabase.GetEntry(variantTier.enemyPlaceableGuid);
                            if (entry1 != null && !string.IsNullOrEmpty(entry1.encounterGuid))
                            {
                                EncounterDatabaseEntry entry2 = EncounterDatabase.GetEntry(entry1.encounterGuid);
                                if (entry2 != null)
                                    nullable = new int?(GameStatsManager.Instance.QueryEncounterableDifferentiator(entry2));
                            }
                        }
                        if (nullable.HasValue)
                        {
                            if (nullable.Value < maxValue)
                            {
                                placeableVariantList.Clear();
                                maxValue = nullable.Value;
                            }
                            else if (nullable.Value > maxValue)
                                continue;
                        }
                    }
                    placeableVariantList.Add(variantTier);
                }
            }
            this.ModifyWeightsByDifficulty(placeableVariantList);
            return this.SelectVariantByWeighting(placeableVariantList);
        }

        private DungeonPlaceableVariant SelectVariantByWeighting(List<DungeonPlaceableVariant> variants)
        {
            float num1 = 0.0f;
            float num2 = 0.0f;
            bool flag = this.IsAnnexTable;
            if (flag)
                flag = GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON && (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.MasteryTokensCollectedThisRun > 0;
            for (int index = 0; index < variants.Count; ++index)
            {
                if (!flag || !(bool) (UnityEngine.Object) variants[index].nonDatabasePlaceable || !(variants[index].nonDatabasePlaceable.name == "SellPit"))
                    num2 += variants[index].GetPercentChance() * variants[index].percentChanceMultiplier;
            }
            float num3 = UnityEngine.Random.value * num2;
            DungeonPlaceableVariant placeableVariant = (DungeonPlaceableVariant) null;
            for (int index = 0; index < variants.Count; ++index)
            {
                if (!flag || !(bool) (UnityEngine.Object) variants[index].nonDatabasePlaceable || !(variants[index].nonDatabasePlaceable.name == "SellPit"))
                {
                    num1 += variants[index].GetPercentChance() * variants[index].percentChanceMultiplier;
                    if ((double) num1 >= (double) num3)
                    {
                        placeableVariant = variants[index];
                        break;
                    }
                }
            }
            return placeableVariant;
        }

        public int GetMinimumDifficulty()
        {
            int val1 = int.MaxValue;
            foreach (DungeonPlaceableVariant variantTier in this.variantTiers)
                val1 = Math.Min(val1, variantTier.difficultyRating);
            return val1;
        }

        public int GetMaximumDifficulty()
        {
            int val1 = int.MinValue;
            foreach (DungeonPlaceableVariant variantTier in this.variantTiers)
                val1 = Math.Max(val1, variantTier.difficultyRating);
            return val1;
        }
    }
}
