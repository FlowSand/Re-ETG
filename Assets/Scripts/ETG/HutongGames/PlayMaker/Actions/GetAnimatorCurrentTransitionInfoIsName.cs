using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("Check the active Transition name on a specified layer. Format is 'CURRENT_STATE -> NEXT_STATE'.")]
    public class GetAnimatorCurrentTransitionInfoIsName : FsmStateActionAnimatorBase
    {
        [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The layer's index")]
        [RequiredField]
        public FsmInt layerIndex;
        [HutongGames.PlayMaker.Tooltip("The name to check the transition against.")]
        public FsmString name;
        [HutongGames.PlayMaker.Tooltip("True if name matches")]
        [UIHint(UIHint.Variable)]
        [ActionSection("Results")]
        public FsmBool nameMatch;
        [HutongGames.PlayMaker.Tooltip("Event send if name matches")]
        public FsmEvent nameMatchEvent;
        [HutongGames.PlayMaker.Tooltip("Event send if name doesn't match")]
        public FsmEvent nameDoNotMatchEvent;
        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = (FsmOwnerDefault) null;
            this.layerIndex = (FsmInt) null;
            this.name = (FsmString) null;
            this.nameMatch = (FsmBool) null;
            this.nameMatchEvent = (FsmEvent) null;
            this.nameDoNotMatchEvent = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
            {
                this.Finish();
            }
            else
            {
                this._animator = ownerDefaultTarget.GetComponent<Animator>();
                if ((Object) this._animator == (Object) null)
                {
                    this.Finish();
                }
                else
                {
                    this.IsName();
                    if (this.everyFrame)
                        return;
                    this.Finish();
                }
            }
        }

        public override void OnActionUpdate() => this.IsName();

        private void IsName()
        {
            if (!((Object) this._animator != (Object) null))
                return;
            if (this._animator.GetAnimatorTransitionInfo(this.layerIndex.Value).IsName(this.name.Value))
            {
                this.nameMatch.Value = true;
                this.Fsm.Event(this.nameMatchEvent);
            }
            else
            {
                this.nameMatch.Value = false;
                this.Fsm.Event(this.nameDoNotMatchEvent);
            }
        }
    }
}
