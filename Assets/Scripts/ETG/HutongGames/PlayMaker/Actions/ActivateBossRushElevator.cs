using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Events)]
    public class ActivateBossRushElevator : FsmStateAction
    {
        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            Object.FindObjectOfType<ShortcutElevatorController>().SetBossRushPaymentValid();
            this.Finish();
        }
    }
}
