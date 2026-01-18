using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Gets the Mass of a Game Object's Rigid Body 2D.")]
    [ActionCategory(ActionCategory.Physics2D)]
    public class GetMass2d : ComponentAction<Rigidbody2D>
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody2D attached.")]
        [RequiredField]
        [CheckForComponent(typeof (Rigidbody2D))]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Store the mass of gameObject.")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeResult;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.storeResult = (FsmFloat) null;
        }

        public override void OnEnter()
        {
            this.DoGetMass();
            this.Finish();
        }

        private void DoGetMass()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            this.storeResult.Value = this.rigidbody2d.mass;
        }
    }
}
