using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [HutongGames.PlayMaker.Tooltip("Gets the Position of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
    public class GetPosition : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable)]
        public FsmVector3 vector;
        [UIHint(UIHint.Variable)]
        public FsmFloat x;
        [UIHint(UIHint.Variable)]
        public FsmFloat y;
        [UIHint(UIHint.Variable)]
        public FsmFloat z;
        public Space space;
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.vector = (FsmVector3) null;
            this.x = (FsmFloat) null;
            this.y = (FsmFloat) null;
            this.z = (FsmFloat) null;
            this.space = Space.World;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetPosition();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetPosition();

        private void DoGetPosition()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            Vector3 vector3 = this.space != Space.World ? ownerDefaultTarget.transform.localPosition : ownerDefaultTarget.transform.position;
            this.vector.Value = vector3;
            this.x.Value = vector3.x;
            this.y.Value = vector3.y;
            this.z.Value = vector3.z;
        }
    }
}
