using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Iterate through a list of all colliders that fall within a rectangular area.The colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
    [ActionCategory(ActionCategory.Physics2D)]
    public class GetNextOverlapArea2d : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("First corner of the rectangle area using the game object position. \nOr use firstCornerPosition parameter.")]
        [ActionSection("Setup")]
        public FsmOwnerDefault firstCornerGameObject;
        [HutongGames.PlayMaker.Tooltip("First Corner of the rectangle area as a world position. \nOr use FirstCornerGameObject parameter. If both define, will add firstCornerPosition to the FirstCornerGameObject position")]
        public FsmVector2 firstCornerPosition;
        [HutongGames.PlayMaker.Tooltip("Second corner of the rectangle area using the game object position. \nOr use secondCornerPosition parameter.")]
        public FsmGameObject secondCornerGameObject;
        [HutongGames.PlayMaker.Tooltip("Second Corner rectangle area as a world position. \nOr use SecondCornerGameObject parameter. If both define, will add secondCornerPosition to the SecondCornerGameObject position")]
        public FsmVector2 secondCornerPosition;
        [HutongGames.PlayMaker.Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
        public FsmInt minDepth;
        [HutongGames.PlayMaker.Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
        public FsmInt maxDepth;
        [UIHint(UIHint.Layer)]
        [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        [ActionSection("Filter")]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [HutongGames.PlayMaker.Tooltip("Store the number of colliders found for this overlap.")]
        [UIHint(UIHint.Variable)]
        [ActionSection("Result")]
        public FsmInt collidersCount;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Store the next collider in a GameObject variable.")]
        [RequiredField]
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
            this.firstCornerGameObject = (FsmOwnerDefault) null;
            FsmVector2 fsmVector2_1 = new FsmVector2();
            fsmVector2_1.UseVariable = true;
            this.firstCornerPosition = fsmVector2_1;
            this.secondCornerGameObject = (FsmGameObject) null;
            FsmVector2 fsmVector2_2 = new FsmVector2();
            fsmVector2_2.UseVariable = true;
            this.secondCornerPosition = fsmVector2_2;
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
                this.colliders = this.GetOverlapAreaAll();
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

        private Collider2D[] GetOverlapAreaAll()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.firstCornerGameObject);
            Vector2 pointA = this.firstCornerPosition.Value;
            if ((Object) ownerDefaultTarget != (Object) null)
            {
                pointA.x += ownerDefaultTarget.transform.position.x;
                pointA.y += ownerDefaultTarget.transform.position.y;
            }
            GameObject gameObject = this.secondCornerGameObject.Value;
            Vector2 pointB = this.secondCornerPosition.Value;
            if ((Object) gameObject != (Object) null)
            {
                pointB.x += gameObject.transform.position.x;
                pointB.y += gameObject.transform.position.y;
            }
            if (this.minDepth.IsNone && this.maxDepth.IsNone)
                return Physics2D.OverlapAreaAll(pointA, pointB, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            float minDepth = !this.minDepth.IsNone ? (float) this.minDepth.Value : float.NegativeInfinity;
            float maxDepth = !this.maxDepth.IsNone ? (float) this.maxDepth.Value : float.PositiveInfinity;
            return Physics2D.OverlapAreaAll(pointA, pointB, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, maxDepth);
        }
    }
}
