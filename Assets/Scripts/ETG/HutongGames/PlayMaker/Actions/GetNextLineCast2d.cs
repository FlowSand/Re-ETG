using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Iterate through a list of all colliders detected by a LineCastThe colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
    [ActionCategory(ActionCategory.Physics2D)]
    public class GetNextLineCast2d : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
        [ActionSection("Setup")]
        public FsmOwnerDefault fromGameObject;
        [HutongGames.PlayMaker.Tooltip("Start ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add fromPosition to the fromGameObject position")]
        public FsmVector2 fromPosition;
        [HutongGames.PlayMaker.Tooltip("End ray at game object position. \nOr use From Position parameter.")]
        public FsmGameObject toGameObject;
        [HutongGames.PlayMaker.Tooltip("End ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add toPosition to the ToGameObject position")]
        public FsmVector2 toPosition;
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
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Store the number of colliders found for this overlap.")]
        [ActionSection("Result")]
        public FsmInt collidersCount;
        [HutongGames.PlayMaker.Tooltip("Store the next collider in a GameObject variable.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeNextCollider;
        [HutongGames.PlayMaker.Tooltip("Get the 2d position of the next ray hit point and store it in a variable.")]
        public FsmVector2 storeNextHitPoint;
        [HutongGames.PlayMaker.Tooltip("Get the 2d normal at the next hit point and store it in a variable.")]
        public FsmVector2 storeNextHitNormal;
        [HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the next hit point and store it in a variable.")]
        public FsmFloat storeNextHitDistance;
        [HutongGames.PlayMaker.Tooltip("Event to send to get the next collider.")]
        public FsmEvent loopEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send when there are no more colliders to iterate.")]
        public FsmEvent finishedEvent;
        private RaycastHit2D[] hits;
        private int colliderCount;
        private int nextColliderIndex;

        public override void Reset()
        {
            this.fromGameObject = (FsmOwnerDefault) null;
            FsmVector2 fsmVector2_1 = new FsmVector2();
            fsmVector2_1.UseVariable = true;
            this.fromPosition = fsmVector2_1;
            this.toGameObject = (FsmGameObject) null;
            FsmVector2 fsmVector2_2 = new FsmVector2();
            fsmVector2_2.UseVariable = true;
            this.toPosition = fsmVector2_2;
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
            this.storeNextHitPoint = (FsmVector2) null;
            this.storeNextHitNormal = (FsmVector2) null;
            this.storeNextHitDistance = (FsmFloat) null;
            this.loopEvent = (FsmEvent) null;
            this.finishedEvent = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            if (this.hits == null)
            {
                this.hits = this.GetLineCastAll();
                this.colliderCount = this.hits.Length;
                this.collidersCount.Value = this.colliderCount;
            }
            this.DoGetNextCollider();
            this.Finish();
        }

        private void DoGetNextCollider()
        {
            if (this.nextColliderIndex >= this.colliderCount)
            {
                this.hits = new RaycastHit2D[0];
                this.nextColliderIndex = 0;
                this.Fsm.Event(this.finishedEvent);
            }
            else
            {
                Fsm.RecordLastRaycastHit2DInfo(this.Fsm, this.hits[this.nextColliderIndex]);
                this.storeNextCollider.Value = this.hits[this.nextColliderIndex].collider.gameObject;
                this.storeNextHitPoint.Value = this.hits[this.nextColliderIndex].point;
                this.storeNextHitNormal.Value = this.hits[this.nextColliderIndex].normal;
                this.storeNextHitDistance.Value = this.hits[this.nextColliderIndex].fraction;
                if (this.nextColliderIndex >= this.colliderCount)
                {
                    this.hits = new RaycastHit2D[0];
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

        private RaycastHit2D[] GetLineCastAll()
        {
            Vector2 start = this.fromPosition.Value;
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
            if ((Object) ownerDefaultTarget != (Object) null)
            {
                start.x += ownerDefaultTarget.transform.position.x;
                start.y += ownerDefaultTarget.transform.position.y;
            }
            Vector2 end = this.toPosition.Value;
            GameObject gameObject = this.toGameObject.Value;
            if ((Object) gameObject != (Object) null)
            {
                end.x += gameObject.transform.position.x;
                end.y += gameObject.transform.position.y;
            }
            if (this.minDepth.IsNone && this.maxDepth.IsNone)
                return Physics2D.LinecastAll(start, end, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            float minDepth = !this.minDepth.IsNone ? (float) this.minDepth.Value : float.NegativeInfinity;
            float maxDepth = !this.maxDepth.IsNone ? (float) this.maxDepth.Value : float.PositiveInfinity;
            return Physics2D.LinecastAll(start, end, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, maxDepth);
        }
    }
}
