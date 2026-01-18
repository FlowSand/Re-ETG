using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [HutongGames.PlayMaker.Tooltip("Forces a Game Object's Rigid Body 2D to Sleep at least one frame.")]
    public class Sleep2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof (Rigidbody2D))]
        [HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody2d attached")]
        public FsmOwnerDefault gameObject;

        public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

        public override void OnEnter()
        {
            this.DoSleep();
            this.Finish();
        }

        private void DoSleep()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            this.rigidbody2d.Sleep();
        }
    }
}
