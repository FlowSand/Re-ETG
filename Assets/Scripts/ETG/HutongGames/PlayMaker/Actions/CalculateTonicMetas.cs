using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(".NPCs")]
    public class CalculateTonicMetas : FsmStateAction
    {
        public override void OnEnter()
        {
            int num = Mathf.RoundToInt(GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY));
            FsmInt fsmInt = this.Fsm.Variables.FindFsmInt("npcNumber1");
            FsmFloat fsmFloat = this.Fsm.Variables.FindFsmFloat("costFloat");
            fsmInt.Value = Mathf.RoundToInt(((float) num * 0.9f).Quantize(50f));
            fsmFloat.Value = (float) fsmInt.Value;
            this.Finish();
        }
    }
}
