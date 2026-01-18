using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [HutongGames.PlayMaker.Tooltip("Gets info on the last Trigger event and store in variables.")]
    public class GetTriggerInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public FsmGameObject gameObjectHit;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Useful for triggering different effects. Audio, particles...")]
        public FsmString physicsMaterialName;

        public override void Reset()
        {
            this.gameObjectHit = (FsmGameObject) null;
            this.physicsMaterialName = (FsmString) null;
        }

        private void StoreTriggerInfo()
        {
            if ((Object) this.Fsm.TriggerCollider == (Object) null)
                return;
            this.gameObjectHit.Value = this.Fsm.TriggerCollider.gameObject;
            this.physicsMaterialName.Value = this.Fsm.TriggerCollider.material.name;
        }

        public override void OnEnter()
        {
            this.StoreTriggerInfo();
            this.Finish();
        }
    }
}
