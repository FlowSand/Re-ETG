using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Iterate through a list of all colliders that fall within a circular area.The colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
    [ActionCategory(ActionCategory.Physics2D)]
    public class GetNextOverlapCircle2d : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Center of the circle area. \nOr use From Position parameter.")]
        [ActionSection("Setup")]
        public FsmOwnerDefault fromGameObject;
        [HutongGames.PlayMaker.Tooltip("CEnter of the circle area as a world position. \nOr use fromGameObject parameter. If both define, will add fromPosition to the fromGameObject position")]
        public FsmVector2 fromPosition;
        [HutongGames.PlayMaker.Tooltip("The circle radius")]
        public FsmFloat radius;
        [HutongGames.PlayMaker.Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
        public FsmInt minDepth;
        [HutongGames.PlayMaker.Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
        public FsmInt maxDepth;
        [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        [UIHint(UIHint.Layer)]
        [ActionSection("Filter")]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [ActionSection("Result")]
        [HutongGames.PlayMaker.Tooltip("Store the number of colliders found for this overlap.")]
        [UIHint(UIHint.Variable)]
        public FsmInt collidersCount;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Store the next collider in a GameObject variable.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeNextCollider;
        [HutongGames.PlayMaker.Tooltip("Event to send to get the next collider.")]
        public FsmEvent loopEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send when there are no more colliders to iterate.")]
        public FsmEvent finishedEvent;
        private Collider2D[] colliders;
        private int colliderCount;
        private int nextColliderIndex;

        public override void Reset()
        {
            this.fromGameObject = (FsmOwnerDefault) null;
            FsmVector2 fsmVector2 = new FsmVector2();
            fsmVector2.UseVariable = true;
            this.fromPosition = fsmVector2;
            this.radius = (FsmFloat) 10f;
            FsmInt fsmInt1 = new FsmInt();
            fsmInt1.UseVariable = true;
            this.minDepth = fsmInt1;
            FsmInt fsmInt2 = new FsmInt();
            fsmInt2.UseVariable = true;
            this.maxDepth = fsmInt2;
            this.layerMask = new FsmInt[0];
            this.invertMask = (FsmBool) false;
            this.collidersCount = (FsmInt) null;
            this.storeNextCollider = (FsmGameObject) null;
            this.loopEvent = (FsmEvent) null;
            this.finishedEvent = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            if (this.colliders == null)
            {
                this.colliders = this.GetOverlapCircleAll();
                this.colliderCount = this.colliders.Length;
                this.collidersCount.Value = this.colliderCount;
            }
            this.DoGetNextCollider();
            this.Finish();
        }

        private void DoGetNextCollider()
        {
            if (this.nextColliderIndex >= this.colliderCount)
            {
                this.nextColliderIndex = 0;
                this.colliders = (Collider2D[]) null;
                this.Fsm.Event(this.finishedEvent);
            }
            else
            {
                this.storeNextCollider.Value = this.colliders[this.nextColliderIndex].gameObject;
                if (this.nextColliderIndex >= this.colliderCount)
                {
                    this.nextColliderIndex = 0;
                    this.Fsm.Event(this.finishedEvent);
                }
                else
                {
                    ++this.nextColliderIndex;
                    if (this.loopEvent == null)
                        return;
                    this.Fsm.Event(this.loopEvent);
                }
            }
        }

        private Collider2D[] GetOverlapCircleAll()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
            Vector2 point = this.fromPosition.Value;
            if ((Object) ownerDefaultTarget != (Object) null)
            {
                point.x += ownerDefaultTarget.transform.position.x;
                point.y += ownerDefaultTarget.transform.position.y;
            }
            if (this.minDepth.IsNone && this.maxDepth.IsNone)
                return Physics2D.OverlapCircleAll(point, this.radius.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            float minDepth = !this.minDepth.IsNone ? (float) this.minDepth.Value : float.NegativeInfinity;
            float maxDepth = !this.maxDepth.IsNone ? (float) this.maxDepth.Value : float.PositiveInfinity;
            return Physics2D.OverlapCircleAll(point, this.radius.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, maxDepth);
        }
    }
}
