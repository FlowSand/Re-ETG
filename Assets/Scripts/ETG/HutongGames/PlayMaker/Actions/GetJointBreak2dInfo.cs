using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Gets info on the last joint break 2D event.")]
    [ActionCategory(ActionCategory.Physics2D)]
    public class GetJointBreak2dInfo : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Get the broken joint.")]
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof (Joint2D))]
        public FsmObject brokenJoint;
        [HutongGames.PlayMaker.Tooltip("Get the reaction force exerted by the broken joint. Unity 5.3+")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 reactionForce;
        [HutongGames.PlayMaker.Tooltip("Get the magnitude of the reaction force exerted by the broken joint. Unity 5.3+")]
        [UIHint(UIHint.Variable)]
        public FsmFloat reactionForceMagnitude;
        [HutongGames.PlayMaker.Tooltip("Get the reaction torque exerted by the broken joint. Unity 5.3+")]
        [UIHint(UIHint.Variable)]
        public FsmFloat reactionTorque;

        public override void Reset()
        {
            this.brokenJoint = (FsmObject) null;
            this.reactionForce = (FsmVector2) null;
            this.reactionTorque = (FsmFloat) null;
        }

        private void StoreInfo()
        {
            if ((Object) this.Fsm.BrokenJoint2D == (Object) null)
                return;
            this.brokenJoint.Value = (Object) this.Fsm.BrokenJoint2D;
            this.reactionForce.Value = this.Fsm.BrokenJoint2D.reactionForce;
            this.reactionForceMagnitude.Value = this.Fsm.BrokenJoint2D.reactionForce.magnitude;
            this.reactionTorque.Value = this.Fsm.BrokenJoint2D.reactionTorque;
        }

        public override void OnEnter()
        {
            this.StoreInfo();
            this.Finish();
        }
    }
}
