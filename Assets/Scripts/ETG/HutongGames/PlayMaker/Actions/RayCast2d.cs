using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics2D)]
  [HutongGames.PlayMaker.Tooltip("Casts a Ray against all Colliders in the scene. A raycast is conceptually like a laser beam that is fired from a point in space along a particular direction. Any object making contact with the beam can be detected and reported. Use GetRaycastHit2dInfo to get more detailed info.")]
  public class RayCast2d : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
    [ActionSection("Setup")]
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
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Event to send if the ray hits an object.")]
    [ActionSection("Result")]
    public FsmEvent hitEvent;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Set a bool variable to true if hit something, otherwise false.")]
    public FsmBool storeDidHit;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the game object hit in a variable.")]
    public FsmGameObject storeHitObject;
    [HutongGames.PlayMaker.Tooltip("Get the 2d position of the ray hit point and store it in a variable.")]
    [UIHint(UIHint.Variable)]
    public FsmVector2 storeHitPoint;
    [HutongGames.PlayMaker.Tooltip("Get the 2d normal at the hit point and store it in a variable.")]
    [UIHint(UIHint.Variable)]
    public FsmVector2 storeHitNormal;
    [HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeHitDistance;
    [HutongGames.PlayMaker.Tooltip("Get the fraction along the ray to the hit point and store it in a variable. If the ray's direction vector is normalised then this value is simply the distance between the origin and the hit point. If the direction is not normalised then this distance is expressed as a 'fraction' (which could be greater than 1) of the vector's magnitude.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeHitFraction;
    [HutongGames.PlayMaker.Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
    [ActionSection("Filter")]
    public FsmInt repeatInterval;
    [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
    [UIHint(UIHint.Layer)]
    public FsmInt[] layerMask;
    [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
    public FsmBool invertMask;
    [HutongGames.PlayMaker.Tooltip("The color to use for the debug line.")]
    [ActionSection("Debug")]
    public FsmColor debugColor;
    [HutongGames.PlayMaker.Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
    public FsmBool debug;
    private Transform _transform;
    private int repeat;

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
      this.distance = (FsmFloat) 100f;
      this.hitEvent = (FsmEvent) null;
      this.storeDidHit = (FsmBool) null;
      this.storeHitObject = (FsmGameObject) null;
      this.storeHitPoint = (FsmVector2) null;
      this.storeHitNormal = (FsmVector2) null;
      this.storeHitDistance = (FsmFloat) null;
      this.storeHitFraction = (FsmFloat) null;
      this.repeatInterval = (FsmInt) 1;
      this.layerMask = new FsmInt[0];
      this.invertMask = (FsmBool) false;
      this.debugColor = (FsmColor) Color.yellow;
      this.debug = (FsmBool) false;
    }

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
      if ((UnityEngine.Object) ownerDefaultTarget != (UnityEngine.Object) null)
        this._transform = ownerDefaultTarget.transform;
      this.DoRaycast();
      if (this.repeatInterval.Value != 0)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      --this.repeat;
      if (this.repeat != 0)
        return;
      this.DoRaycast();
    }

    private void DoRaycast()
    {
      this.repeat = this.repeatInterval.Value;
      if ((double) Math.Abs(this.distance.Value) < (double) Mathf.Epsilon)
        return;
      Vector2 origin = this.fromPosition.Value;
      if ((UnityEngine.Object) this._transform != (UnityEngine.Object) null)
      {
        origin.x += this._transform.position.x;
        origin.y += this._transform.position.y;
      }
      float num1 = float.PositiveInfinity;
      if ((double) this.distance.Value > 0.0)
        num1 = this.distance.Value;
      Vector2 normalized = this.direction.Value.normalized;
      if ((UnityEngine.Object) this._transform != (UnityEngine.Object) null && this.space == Space.Self)
      {
        Vector3 vector3 = this._transform.TransformDirection(new Vector3(this.direction.Value.x, this.direction.Value.y, 0.0f));
        normalized.x = vector3.x;
        normalized.y = vector3.y;
      }
      RaycastHit2D info;
      if (this.minDepth.IsNone && this.maxDepth.IsNone)
      {
        info = Physics2D.Raycast(origin, normalized, num1, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
      }
      else
      {
        float minDepth = !this.minDepth.IsNone ? (float) this.minDepth.Value : float.NegativeInfinity;
        float maxDepth = !this.maxDepth.IsNone ? (float) this.maxDepth.Value : float.PositiveInfinity;
        info = Physics2D.Raycast(origin, normalized, num1, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, maxDepth);
      }
      Fsm.RecordLastRaycastHit2DInfo(this.Fsm, info);
      bool flag = (UnityEngine.Object) info.collider != (UnityEngine.Object) null;
      this.storeDidHit.Value = flag;
      if (flag)
      {
        this.storeHitObject.Value = info.collider.gameObject;
        this.storeHitPoint.Value = info.point;
        this.storeHitNormal.Value = info.normal;
        this.storeHitDistance.Value = info.distance;
        this.storeHitFraction.Value = info.fraction;
        this.Fsm.Event(this.hitEvent);
      }
      if (!this.debug.Value)
        return;
      float num2 = Mathf.Min(num1, 1000f);
      Vector3 start = new Vector3(origin.x, origin.y, 0.0f);
      Vector3 vector3_1 = new Vector3(normalized.x, normalized.y, 0.0f);
      Vector3 end = start + vector3_1 * num2;
      Debug.DrawLine(start, end, this.debugColor.Value);
    }
  }
}
