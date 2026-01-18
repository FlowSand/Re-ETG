using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [HutongGames.PlayMaker.Tooltip("Sets a Game Object's Tag.")]
    public class SetTag : FsmStateAction
    {
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Tag)]
        public FsmString tag;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.tag = (FsmString) "Untagged";
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget != (Object) null)
                ownerDefaultTarget.tag = this.tag.Value;
            this.Finish();
        }
    }
}
