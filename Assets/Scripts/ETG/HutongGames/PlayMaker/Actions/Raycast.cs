using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics)]
  [HutongGames.PlayMaker.Tooltip("Casts a Ray against all Colliders in the scene. Use either a Game Object or Vector3 world position as the origin of the ray. Use GetRaycastInfo to get more detailed info.")]
  public class Raycast : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
    public FsmOwnerDefault fromGameObject;
    [HutongGames.PlayMaker.Tooltip("Start ray at a vector3 world position. \nOr use Game Object parameter.")]
    public FsmVector3 fromPosition;
    [HutongGames.PlayMaker.Tooltip("A vector3 direction vector")]
    public FsmVector3 direction;
    [HutongGames.PlayMaker.Tooltip("Cast the ray in world or local space. Note if no Game Object is specfied, the direction is in world space.")]
    public Space space;
    [HutongGames.PlayMaker.Tooltip("The length of the ray. Set to -1 for infinity.")]
    public FsmFloat distance;
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
    [HutongGames.PlayMaker.Tooltip("Get the world position of the ray hit point and store it in a variable.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeHitPoint;
    [HutongGames.PlayMaker.Tooltip("Get the normal at the hit point and store it in a variable.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeHitNormal;
    [HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeHitDistance;
    [HutongGames.PlayMaker.Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
    [ActionSection("Filter")]
    public FsmInt repeatInterval;
    [UIHint(UIHint.Layer)]
    [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
    public FsmInt[] layerMask;
    [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
    public FsmBool invertMask;
    [HutongGames.PlayMaker.Tooltip("The color to use for the debug line.")]
    [ActionSection("Debug")]
    public FsmColor debugColor;
    [HutongGames.PlayMaker.Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
    public FsmBool debug;
    private int repeat;

    public override void Reset()
    {
      this.fromGameObject = (FsmOwnerDefault) null;
      FsmVector3 fsmVector3_1 = new FsmVector3();
      fsmVector3_1.UseVariable = true;
      this.fromPosition = fsmVector3_1;
      FsmVector3 fsmVector3_2 = new FsmVector3();
      fsmVector3_2.UseVariable = true;
      this.direction = fsmVector3_2;
      this.space = Space.Self;
      this.distance = (FsmFloat) 100f;
      this.hitEvent = (FsmEvent) null;
      this.storeDidHit = (FsmBool) null;
      this.storeHitObject = (FsmGameObject) null;
      this.storeHitPoint = (FsmVector3) null;
      this.storeHitNormal = (FsmVector3) null;
      this.storeHitDistance = (FsmFloat) null;
      this.repeatInterval = (FsmInt) 1;
      this.layerMask = new FsmInt[0];
      this.invertMask = (FsmBool) false;
      this.debugColor = (FsmColor) Color.yellow;
      this.debug = (FsmBool) false;
    }

    public override void OnEnter()
    {
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
      if ((double) this.distance.Value == 0.0)
        return;
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
      Vector3 vector3 = !((Object) ownerDefaultTarget != (Object) null) ? this.fromPosition.Value : ownerDefaultTarget.transform.position;
      float num1 = float.PositiveInfinity;
      if ((double) this.distance.Value > 0.0)
        num1 = this.distance.Value;
      Vector3 direction = this.direction.Value;
      if ((Object) ownerDefaultTarget != (Object) null && this.space == Space.Self)
        direction = ownerDefaultTarget.transform.TransformDirection(this.direction.Value);
      RaycastHit hitInfo;
      Physics.Raycast(vector3, direction, out hitInfo, num1, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
      this.Fsm.RaycastHitInfo = hitInfo;
      bool flag = (Object) hitInfo.collider != (Object) null;
      this.storeDidHit.Value = flag;
      if (flag)
      {
        this.storeHitObject.Value = hitInfo.collider.gameObject;
        this.storeHitPoint.Value = this.Fsm.RaycastHitInfo.point;
        this.storeHitNormal.Value = this.Fsm.RaycastHitInfo.normal;
        this.storeHitDistance.Value = this.Fsm.RaycastHitInfo.distance;
        this.Fsm.Event(this.hitEvent);
      }
      if (!this.debug.Value)
        return;
      float num2 = Mathf.Min(num1, 1000f);
      Debug.DrawLine(vector3, vector3 + direction * num2, this.debugColor.Value);
    }
  }
}
