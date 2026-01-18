using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    public class PrepareTakePickup : FsmStateAction
    {
        public int TargetPickupIndex;

        public override void OnEnter()
        {
            PickupObject byId = PickupObjectDatabase.GetById(this.TargetPickupIndex);
            FsmString fsmString = this.Fsm.Variables.GetFsmString("npcReplacementString");
            EncounterTrackable component = byId.GetComponent<EncounterTrackable>();
            if (fsmString != null && (Object) component != (Object) null)
                fsmString.Value = component.journalData.GetPrimaryDisplayName();
            this.Finish();
        }
    }
}
