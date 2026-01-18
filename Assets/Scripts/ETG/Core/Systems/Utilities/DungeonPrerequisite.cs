using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

[Serializable]
public class DungeonPrerequisite
    {
        public DungeonPrerequisite.PrerequisiteType prerequisiteType;
        [ShowInInspectorIf("UsesOperation", false)]
        public DungeonPrerequisite.PrerequisiteOperation prerequisiteOperation;
        [ShowInInspectorIf("IsStatComparison", false)]
        public TrackedStats statToCheck;
        [ShowInInspectorIf("IsMaxComparison", false)]
        public TrackedMaximums maxToCheck;
        [ShowInInspectorIf("IsComparison", false)]
        public float comparisonValue;
        [ShowInInspectorIf("IsComparison", false)]
        public bool useSessionStatValue;
        [ShowInInspectorIf("IsEncounter", false)]
        [EncounterIdentifier]
        public string encounteredObjectGuid;
        [ShowInInspectorIf("IsEncounter", false)]
        public PrototypeDungeonRoom encounteredRoom;
        [ShowInInspectorIf("IsEncounter", false)]
        public int requiredNumberOfEncounters = 1;
        [ShowInInspectorIf("IsCharacter", false)]
        public PlayableCharacters requiredCharacter;
        [ShowInInspectorIf("IsCharacter", false)]
        public bool requireCharacter = true;
        [ShowInInspectorIf("IsTileset", false)]
        public GlobalDungeonData.ValidTilesets requiredTileset;
        [ShowInInspectorIf("IsTileset", false)]
        public bool requireTileset = true;
        [LongEnum]
        public GungeonFlags saveFlagToCheck;
        [ShowInInspectorIf("IsFlag", false)]
        public bool requireFlag = true;
        [ShowInInspectorIf("IsDemoMode", false)]
        public bool requireDemoMode;

        private bool UsesOperation()
        {
            return this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.COMPARISON || this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.MAXIMUM_COMPARISON || this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.NUMBER_PASTS_COMPLETED || this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.ENCOUNTER || this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.ENCOUNTER;
        }

        private bool IsComparison()
        {
            return this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.COMPARISON || this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.MAXIMUM_COMPARISON || this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.NUMBER_PASTS_COMPLETED;
        }

        private bool IsStatComparison()
        {
            return this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.COMPARISON;
        }

        private bool IsMaxComparison()
        {
            return this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.MAXIMUM_COMPARISON;
        }

        private bool IsEncounter()
        {
            return this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.ENCOUNTER || this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.ENCOUNTER_OR_FLAG;
        }

        private bool IsCharacter()
        {
            return this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.CHARACTER;
        }

        private bool IsTileset() => this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.TILESET;

        private bool IsFlag()
        {
            return this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.FLAG || this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.ENCOUNTER_OR_FLAG;
        }

        private bool IsDemoMode()
        {
            return this.prerequisiteType == DungeonPrerequisite.PrerequisiteType.DEMO_MODE;
        }

        public bool CheckConditionsFulfilled()
        {
            EncounterDatabaseEntry et = (EncounterDatabaseEntry) null;
            if (!string.IsNullOrEmpty(this.encounteredObjectGuid))
                et = EncounterDatabase.GetEntry(this.encounteredObjectGuid);
            switch (this.prerequisiteType)
            {
                case DungeonPrerequisite.PrerequisiteType.ENCOUNTER:
                    if (et == null && (UnityEngine.Object) this.encounteredRoom == (UnityEngine.Object) null)
                        return true;
                    if (et != null)
                    {
                        int num = GameStatsManager.Instance.QueryEncounterable(et);
                        switch (this.prerequisiteOperation)
                        {
                            case DungeonPrerequisite.PrerequisiteOperation.LESS_THAN:
                                return num < this.requiredNumberOfEncounters;
                            case DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO:
                                return num == this.requiredNumberOfEncounters;
                            case DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN:
                                return num > this.requiredNumberOfEncounters;
                            default:
                                Debug.LogError((object) "Switching on invalid stat comparison operation!");
                                break;
                        }
                    }
                    else if ((UnityEngine.Object) this.encounteredRoom != (UnityEngine.Object) null)
                    {
                        int num = GameStatsManager.Instance.QueryRoomEncountered(this.encounteredRoom.GUID);
                        switch (this.prerequisiteOperation)
                        {
                            case DungeonPrerequisite.PrerequisiteOperation.LESS_THAN:
                                return num < this.requiredNumberOfEncounters;
                            case DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO:
                                return num == this.requiredNumberOfEncounters;
                            case DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN:
                                return num > this.requiredNumberOfEncounters;
                            default:
                                Debug.LogError((object) "Switching on invalid stat comparison operation!");
                                break;
                        }
                    }
                    return false;
                case DungeonPrerequisite.PrerequisiteType.COMPARISON:
                    float playerStatValue = GameStatsManager.Instance.GetPlayerStatValue(this.statToCheck);
                    switch (this.prerequisiteOperation)
                    {
                        case DungeonPrerequisite.PrerequisiteOperation.LESS_THAN:
                            return (double) playerStatValue < (double) this.comparisonValue;
                        case DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO:
                            return (double) playerStatValue == (double) this.comparisonValue;
                        case DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN:
                            return (double) playerStatValue > (double) this.comparisonValue;
                        default:
                            Debug.LogError((object) "Switching on invalid stat comparison operation!");
                            break;
                    }
                    break;
                case DungeonPrerequisite.PrerequisiteType.CHARACTER:
                    PlayableCharacters playableCharacters = ~PlayableCharacters.Pilot;
                    if (!BraveRandom.IgnoreGenerationDifferentiator)
                    {
                        if ((UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null)
                            playableCharacters = GameManager.Instance.PrimaryPlayer.characterIdentity;
                        else if ((UnityEngine.Object) GameManager.PlayerPrefabForNewGame != (UnityEngine.Object) null)
                            playableCharacters = GameManager.PlayerPrefabForNewGame.GetComponent<PlayerController>().characterIdentity;
                        else if ((UnityEngine.Object) GameManager.Instance.BestGenerationDungeonPrefab != (UnityEngine.Object) null)
                            playableCharacters = GameManager.Instance.BestGenerationDungeonPrefab.defaultPlayerPrefab.GetComponent<PlayerController>().characterIdentity;
                    }
                    return this.requireCharacter == (playableCharacters == this.requiredCharacter);
                case DungeonPrerequisite.PrerequisiteType.TILESET:
                    return (UnityEngine.Object) GameManager.Instance.BestGenerationDungeonPrefab != (UnityEngine.Object) null ? this.requireTileset == (GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == this.requiredTileset) : this.requireTileset == (GameManager.Instance.Dungeon.tileIndices.tilesetId == this.requiredTileset);
                case DungeonPrerequisite.PrerequisiteType.FLAG:
                    return GameStatsManager.Instance.GetFlag(this.saveFlagToCheck) == this.requireFlag;
                case DungeonPrerequisite.PrerequisiteType.DEMO_MODE:
                    return !this.requireDemoMode;
                case DungeonPrerequisite.PrerequisiteType.MAXIMUM_COMPARISON:
                    float playerMaximum = GameStatsManager.Instance.GetPlayerMaximum(this.maxToCheck);
                    switch (this.prerequisiteOperation)
                    {
                        case DungeonPrerequisite.PrerequisiteOperation.LESS_THAN:
                            return (double) playerMaximum < (double) this.comparisonValue;
                        case DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO:
                            return (double) playerMaximum == (double) this.comparisonValue;
                        case DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN:
                            return (double) playerMaximum > (double) this.comparisonValue;
                        default:
                            Debug.LogError((object) "Switching on invalid stat comparison operation!");
                            break;
                    }
                    break;
                case DungeonPrerequisite.PrerequisiteType.ENCOUNTER_OR_FLAG:
                    if (GameStatsManager.Instance.GetFlag(this.saveFlagToCheck) == this.requireFlag)
                        return true;
                    if (et != null)
                    {
                        int num = GameStatsManager.Instance.QueryEncounterable(et);
                        switch (this.prerequisiteOperation)
                        {
                            case DungeonPrerequisite.PrerequisiteOperation.LESS_THAN:
                                return num < this.requiredNumberOfEncounters;
                            case DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO:
                                return num == this.requiredNumberOfEncounters;
                            case DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN:
                                return num > this.requiredNumberOfEncounters;
                            default:
                                Debug.LogError((object) "Switching on invalid stat comparison operation!");
                                break;
                        }
                    }
                    else if ((UnityEngine.Object) this.encounteredRoom != (UnityEngine.Object) null)
                    {
                        int num = GameStatsManager.Instance.QueryRoomEncountered(this.encounteredRoom.GUID);
                        switch (this.prerequisiteOperation)
                        {
                            case DungeonPrerequisite.PrerequisiteOperation.LESS_THAN:
                                return num < this.requiredNumberOfEncounters;
                            case DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO:
                                return num == this.requiredNumberOfEncounters;
                            case DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN:
                                return num > this.requiredNumberOfEncounters;
                            default:
                                Debug.LogError((object) "Switching on invalid stat comparison operation!");
                                break;
                        }
                    }
                    return false;
                case DungeonPrerequisite.PrerequisiteType.NUMBER_PASTS_COMPLETED:
                    return (double) GameStatsManager.Instance.GetNumberPastsBeaten() >= (double) this.comparisonValue;
                default:
                    Debug.LogError((object) "Switching on invalid prerequisite type!!!");
                    break;
            }
            return false;
        }

        protected bool Equals(DungeonPrerequisite other)
        {
            return this.prerequisiteType == other.prerequisiteType && this.prerequisiteOperation == other.prerequisiteOperation && this.statToCheck == other.statToCheck && this.maxToCheck == other.maxToCheck && this.comparisonValue.Equals(other.comparisonValue) && this.useSessionStatValue.Equals(other.useSessionStatValue) && object.Equals((object) this.encounteredRoom, (object) other.encounteredRoom) && object.Equals((object) this.encounteredObjectGuid, (object) other.encounteredObjectGuid) && this.requiredNumberOfEncounters == other.requiredNumberOfEncounters && this.requiredCharacter == other.requiredCharacter && this.requireCharacter.Equals(other.requireCharacter) && this.requiredTileset == other.requiredTileset && this.requireTileset.Equals(other.requireTileset) && this.saveFlagToCheck == other.saveFlagToCheck && this.requireFlag.Equals(other.requireFlag) && this.requireDemoMode.Equals(other.requireDemoMode);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals((object) null, obj))
                return false;
            if (object.ReferenceEquals((object) this, obj))
                return true;
            return obj.GetType() == this.GetType() && this.Equals((DungeonPrerequisite) obj);
        }

        public override int GetHashCode()
        {
            return ((int) ((GungeonFlags) (((int) ((GlobalDungeonData.ValidTilesets) (((int) ((PlayableCharacters) (((((((int) ((TrackedStats) ((int) ((DungeonPrerequisite.PrerequisiteOperation) ((int) this.prerequisiteType * 397) ^ this.prerequisiteOperation) * 397) ^ this.statToCheck) * 397 ^ this.comparisonValue.GetHashCode()) * 397 ^ this.useSessionStatValue.GetHashCode()) * 397 ^ (!((UnityEngine.Object) this.encounteredRoom != (UnityEngine.Object) null) ? 0 : this.encounteredRoom.GetHashCode())) * 397 ^ (this.encounteredObjectGuid == null ? 0 : this.encounteredObjectGuid.GetHashCode())) * 397 ^ this.requiredNumberOfEncounters) * 397) ^ this.requiredCharacter) * 397 ^ this.requireCharacter.GetHashCode()) * 397) ^ this.requiredTileset) * 397 ^ this.requireTileset.GetHashCode()) * 397) ^ this.saveFlagToCheck) * 397 ^ this.requireFlag.GetHashCode()) * 397 ^ this.requireDemoMode.GetHashCode();
        }

        public static bool CheckConditionsFulfilled(DungeonPrerequisite[] prereqs)
        {
            if (prereqs == null)
                return true;
            for (int index = 0; index < prereqs.Length; ++index)
            {
                if (prereqs[index] != null && !prereqs[index].CheckConditionsFulfilled())
                    return false;
            }
            return true;
        }

        public static bool CheckConditionsFulfilled(List<DungeonPrerequisite> prereqs)
        {
            if (prereqs == null)
                return true;
            for (int index = 0; index < prereqs.Count; ++index)
            {
                if (prereqs[index] != null && !prereqs[index].CheckConditionsFulfilled())
                    return false;
            }
            return true;
        }

        public enum PrerequisiteType
        {
            ENCOUNTER,
            COMPARISON,
            CHARACTER,
            TILESET,
            FLAG,
            DEMO_MODE,
            MAXIMUM_COMPARISON,
            ENCOUNTER_OR_FLAG,
            NUMBER_PASTS_COMPLETED,
        }

        public enum PrerequisiteOperation
        {
            LESS_THAN,
            EQUAL_TO,
            GREATER_THAN,
        }
    }

