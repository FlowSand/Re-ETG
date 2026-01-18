using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [HutongGames.PlayMaker.Tooltip("Sets the Mass of a Game Object's Rigid Body 2D.")]
    public class SetMass2d : ComponentAction<Rigidbody2D>
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
        [CheckForComponent(typeof (Rigidbody2D))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Mass")]
        [HasFloatSlider(0.1f, 10f)]
        [RequiredField]
        public FsmFloat mass;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.mass = (FsmFloat) 1f;
        }

        public override void OnEnter()
        {
            this.DoSetMass();
            this.Finish();
        }

        private void DoSetMass()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            this.rigidbody2d.mass = this.mass.Value;
        }
    }
}
