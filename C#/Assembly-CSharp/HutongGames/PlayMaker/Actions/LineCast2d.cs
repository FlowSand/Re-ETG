// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.LineCast2d
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Casts a Ray against all Colliders in the scene.A linecast is an imaginary line between two points in world space. Any object making contact with the beam can be detected and reported. This differs from the similar raycast in that raycasting specifies the line using an origin and direction.Use GetRaycastHit2dInfo to get more detailed info.")]
[ActionCategory(ActionCategory.Physics2D)]
public class LineCast2d : FsmStateAction
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
  private Transform _fromTrans;
  private Transform _toTrans;
  private int repeat;

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
    this.hitEvent = (FsmEvent) null;
    this.storeDidHit = (FsmBool) null;
    this.storeHitObject = (FsmGameObject) null;
    this.storeHitPoint = (FsmVector2) null;
    this.storeHitNormal = (FsmVector2) null;
    this.storeHitDistance = (FsmFloat) null;
    this.repeatInterval = (FsmInt) 1;
    this.layerMask = new FsmInt[0];
    this.invertMask = (FsmBool) false;
    this.debugColor = (FsmColor) Color.yellow;
    this.debug = (FsmBool) false;
  }

  public override void OnEnter()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
    if ((Object) ownerDefaultTarget != (Object) null)
      this._fromTrans = ownerDefaultTarget.transform;
    GameObject gameObject = this.toGameObject.Value;
    if ((Object) gameObject != (Object) null)
      this._toTrans = gameObject.transform;
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
    Vector2 start = this.fromPosition.Value;
    if ((Object) this._fromTrans != (Object) null)
    {
      start.x += this._fromTrans.position.x;
      start.y += this._fromTrans.position.y;
    }
    Vector2 end = this.toPosition.Value;
    if ((Object) this._toTrans != (Object) null)
    {
      end.x += this._toTrans.position.x;
      end.y += this._toTrans.position.y;
    }
    RaycastHit2D info;
    if (this.minDepth.IsNone && this.maxDepth.IsNone)
    {
      info = Physics2D.Linecast(start, end, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
    }
    else
    {
      float minDepth = !this.minDepth.IsNone ? (float) this.minDepth.Value : float.NegativeInfinity;
      float maxDepth = !this.maxDepth.IsNone ? (float) this.maxDepth.Value : float.PositiveInfinity;
      info = Physics2D.Linecast(start, end, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, maxDepth);
    }
    Fsm.RecordLastRaycastHit2DInfo(this.Fsm, info);
    bool flag = (Object) info.collider != (Object) null;
    this.storeDidHit.Value = flag;
    if (flag)
    {
      this.storeHitObject.Value = info.collider.gameObject;
      this.storeHitPoint.Value = info.point;
      this.storeHitNormal.Value = info.normal;
      this.storeHitDistance.Value = info.fraction;
      this.Fsm.Event(this.hitEvent);
    }
    if (!this.debug.Value)
      return;
    Debug.DrawLine(new Vector3(start.x, start.y, 0.0f), new Vector3(end.x, end.y, 0.0f), this.debugColor.Value);
  }
}
