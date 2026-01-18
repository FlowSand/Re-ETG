using System;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [HutongGames.PlayMaker.Tooltip("Iterate through a list of all colliders detected by a RayCastThe colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
    public class GetNextRayCast2d : FsmStateAction
    {
        [ActionSection("Setup")]
        [HutongGames.PlayMaker.Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
        public FsmOwnerDefault fromGameObject;
        [HutongGames.PlayMaker.Tooltip("Start ray at a vector2 world position. \nOr use Game Object parameter.")]
        public FsmVector2 fromPosition;
        [HutongGames.PlayMaker.Tooltip("A vector2 direction vector")]
        public FsmVector2 direction;
        [HutongGames.PlayMaker.Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("The length of the ray. Set to -1 for infinity.")]
        public FsmFloat distance;
        [HutongGames.PlayMaker.Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
        public FsmInt minDepth;
        [HutongGames.PlayMaker.Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
        public FsmInt maxDepth;
        [ActionSection("Filter")]
        [UIHint(UIHint.Layer)]
        [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [UIHint(UIHint.Variable)]
        [ActionSection("Result")]
        [HutongGames.PlayMaker.Tooltip("Store the number of colliders found for this overlap.")]
        public FsmInt collidersCount;
        [HutongGames.PlayMaker.Tooltip("Store the next collider in a GameObject variable.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeNextCollider;
        [HutongGames.PlayMaker.Tooltip("Get the 2d position of the next ray hit point and store it in a variable.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 storeNextHitPoint;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Get the 2d normal at the next hit point and store it in a variable.")]
        public FsmVector2 storeNextHitNormal;
        [HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the next hit point and store it in a variable.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeNextHitDistance;
        [HutongGames.PlayMaker.Tooltip("Get the fraction along the ray to the hit point and store it in a variable. If the ray's direction vector is normalised then this value is simply the distance between the origin and the hit point. If the direction is not normalised then this distance is expressed as a 'fraction' (which could be greater than 1) of the vector's magnitude.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeNextHitFraction;
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
            FsmVector2 fsmVector2_2 = new FsmVector2();
            fsmVector2_2.UseVariable = true;
            this.direction = fsmVector2_2;
            this.space = Space.Self;
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
            this.storeNextHitFraction = (FsmFloat) null;
            this.loopEvent = (FsmEvent) null;
            this.finishedEvent = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            if (this.hits == null)
            {
                this.hits = this.GetRayCastAll();
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
                this.storeNextHitDistance.Value = this.hits[this.nextColliderIndex].distance;
                this.storeNextHitFraction.Value = this.hits[this.nextColliderIndex].fraction;
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

        private RaycastHit2D[] GetRayCastAll()
        {
            if ((double) Math.Abs(this.distance.Value) < (double) Mathf.Epsilon)
                return new RaycastHit2D[0];
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
            Vector2 origin = this.fromPosition.Value;
            if ((UnityEngine.Object) ownerDefaultTarget != (UnityEngine.Object) null)
            {
                origin.x += ownerDefaultTarget.transform.position.x;
                origin.y += ownerDefaultTarget.transform.position.y;
            }
            float distance = float.PositiveInfinity;
            if ((double) this.distance.Value > 0.0)
                distance = this.distance.Value;
            Vector2 normalized = this.direction.Value.normalized;
            if ((UnityEngine.Object) ownerDefaultTarget != (UnityEngine.Object) null && this.space == Space.Self)
            {
                Vector3 vector3 = ownerDefaultTarget.transform.TransformDirection(new Vector3(this.direction.Value.x, this.direction.Value.y, 0.0f));
                normalized.x = vector3.x;
                normalized.y = vector3.y;
            }
            if (this.minDepth.IsNone && this.maxDepth.IsNone)
                return Physics2D.RaycastAll(origin, normalized, distance, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            float minDepth = !this.minDepth.IsNone ? (float) this.minDepth.Value : float.NegativeInfinity;
            float maxDepth = !this.maxDepth.IsNone ? (float) this.maxDepth.Value : float.PositiveInfinity;
            return Physics2D.RaycastAll(origin, normalized, distance, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, maxDepth);
        }
    }
}
