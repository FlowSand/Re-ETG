using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    public static class BravePlayMakerUtility
    {
        public static FsmTransition[] CachedGlobalTransitions { get; set; }

        public static string CheckCurrentModeVariable(Fsm fsm)
        {
            if (fsm.Variables.FindFsmString("currentMode") == null)
                fsm.Variables.StringVariables = new List<FsmString>((IEnumerable<FsmString>) fsm.Variables.StringVariables)
                {
                    new FsmString("currentMode") { Value = "modeBegin" }
                }.ToArray();
            return string.Empty;
        }

        public static string CheckEventExists(Fsm fsm, string eventName)
        {
            return fsm != null && !Array.Exists<FsmEvent>(fsm.Events, (Predicate<FsmEvent>) (e => e.Name == eventName)) ? $"No event with name \"{eventName}\" exists.\n" : string.Empty;
        }

        public static string CheckGlobalTransitionExists(Fsm fsm, string eventName)
        {
            return fsm != null && !Array.Exists<FsmTransition>(fsm.GlobalTransitions, (Predicate<FsmTransition>) (t => t.EventName == eventName)) ? $"No global transition exists for the event \"{eventName}\".\n" : string.Empty;
        }

        public static bool ModeIsSetSomewhere(Fsm fsm, string eventName)
        {
            foreach (FsmState state in fsm.States)
            {
                foreach (FsmStateAction action in state.Actions)
                {
                    if (action is SetMode && (action as SetMode).mode.Value == eventName || action is TestSaveFlag && (action as TestSaveFlag).successType == TestSaveFlag.SuccessType.SetMode && (action as TestSaveFlag).mode.Value == eventName)
                        return true;
                }
            }
            return false;
        }

        public static bool AllOthersAreFinished(FsmStateAction action)
        {
            for (int index = 0; index < action.State.Actions.Length; ++index)
            {
                FsmStateAction action1 = action.State.Actions[index];
                if (action1 != action && !(action1 is INonFinishingState) && !action1.Finished)
                    return false;
            }
            return true;
        }

        public static void DisconnectState(FsmState fsmState)
        {
            Fsm fsm = fsmState.Fsm;
            for (int index = 0; index < fsm.GlobalTransitions.Length; ++index)
            {
                FsmTransition globalTransition = fsm.GlobalTransitions[index];
                if (globalTransition.ToState == fsmState.Name)
                {
                    globalTransition.FsmEvent = (FsmEvent) null;
                    globalTransition.ToState = string.Empty;
                }
            }
            for (int index1 = 0; index1 < fsm.States.Length; ++index1)
            {
                FsmState state = fsm.States[index1];
                for (int index2 = 0; index2 < state.Transitions.Length; ++index2)
                {
                    FsmTransition transition = state.Transitions[index2];
                    if (transition.ToState == fsmState.Name)
                    {
                        transition.FsmEvent = (FsmEvent) null;
                        transition.ToState = string.Empty;
                    }
                }
            }
        }

        public static float GetConsumableValue(
            PlayerController player,
            BravePlayMakerUtility.ConsumableType consumableType)
        {
            switch (consumableType)
            {
                case BravePlayMakerUtility.ConsumableType.Currency:
                    return (float) player.carriedConsumables.Currency;
                case BravePlayMakerUtility.ConsumableType.Keys:
                    return (float) player.carriedConsumables.KeyBullets;
                case BravePlayMakerUtility.ConsumableType.Hearts:
                    return player.healthHaver.GetCurrentHealth();
                case BravePlayMakerUtility.ConsumableType.HeartContainers:
                    return player.healthHaver.GetMaxHealth();
                case BravePlayMakerUtility.ConsumableType.MetaCurrency:
                    return GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY);
                case BravePlayMakerUtility.ConsumableType.Blanks:
                    return (float) player.Blanks;
                case BravePlayMakerUtility.ConsumableType.Armor:
                    return player.healthHaver.Armor;
                default:
                    Debug.LogError((object) ("Unknown consumable type: " + (object) consumableType));
                    return 0.0f;
            }
        }

        public static void SetConsumableValue(
            PlayerController player,
            BravePlayMakerUtility.ConsumableType consumableType,
            float value)
        {
            switch (consumableType)
            {
                case BravePlayMakerUtility.ConsumableType.Currency:
                    player.carriedConsumables.Currency = Mathf.RoundToInt(value);
                    break;
                case BravePlayMakerUtility.ConsumableType.Keys:
                    player.carriedConsumables.KeyBullets = Mathf.RoundToInt(value);
                    break;
                case BravePlayMakerUtility.ConsumableType.Hearts:
                    player.healthHaver.ForceSetCurrentHealth(BraveMathCollege.QuantizeFloat(value, 0.5f));
                    break;
                case BravePlayMakerUtility.ConsumableType.HeartContainers:
                    player.healthHaver.SetHealthMaximum(BraveMathCollege.QuantizeFloat(value, 0.5f));
                    break;
                case BravePlayMakerUtility.ConsumableType.MetaCurrency:
                    GameStatsManager.Instance.ClearStatValueGlobal(TrackedStats.META_CURRENCY);
                    GameStatsManager.Instance.SetStat(TrackedStats.META_CURRENCY, value);
                    break;
                case BravePlayMakerUtility.ConsumableType.Blanks:
                    player.Blanks = Mathf.FloorToInt(value);
                    break;
                case BravePlayMakerUtility.ConsumableType.Armor:
                    if (player.ForceZeroHealthState && (double) value == 0.0)
                        value = 1f;
                    player.healthHaver.Armor = (float) Mathf.RoundToInt(value);
                    break;
                default:
                    Debug.LogError((object) ("Unknown consumable type: " + (object) consumableType));
                    break;
            }
        }

        public enum ConsumableType
        {
            Currency,
            Keys,
            Hearts,
            HeartContainers,
            MetaCurrency,
            Blanks,
            Armor,
        }
    }
}
