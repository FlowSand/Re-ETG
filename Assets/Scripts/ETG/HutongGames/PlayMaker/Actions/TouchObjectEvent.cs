using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionTarget(typeof (GameObject), "gameObject", false)]
  [ActionCategory(ActionCategory.Device)]
  [HutongGames.PlayMaker.Tooltip("Sends events when an object is touched. Optionally filter by a fingerID. NOTE: Uses the MainCamera!")]
  public class TouchObjectEvent : FsmStateAction
  {
    [CheckForComponent(typeof (Collider))]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Game Object to detect touches on.")]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("How far from the camera is the Game Object pickable.")]
    [RequiredField]
    public FsmFloat pickDistance;
    [HutongGames.PlayMaker.Tooltip("Only detect touches that match this fingerID, or set to None.")]
    public FsmInt fingerId;
    [HutongGames.PlayMaker.Tooltip("Event to send on touch began.")]
    [ActionSection("Events")]
    public FsmEvent touchBegan;
    [HutongGames.PlayMaker.Tooltip("Event to send on touch moved.")]
    public FsmEvent touchMoved;
    [HutongGames.PlayMaker.Tooltip("Event to send on stationary touch.")]
    public FsmEvent touchStationary;
    [HutongGames.PlayMaker.Tooltip("Event to send on touch ended.")]
    public FsmEvent touchEnded;
    [HutongGames.PlayMaker.Tooltip("Event to send on touch cancel.")]
    public FsmEvent touchCanceled;
    [HutongGames.PlayMaker.Tooltip("Store the fingerId of the touch.")]
    [UIHint(UIHint.Variable)]
    [ActionSection("Store Results")]
    public FsmInt storeFingerId;
    [HutongGames.PlayMaker.Tooltip("Store the world position where the object was touched.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeHitPoint;
    [HutongGames.PlayMaker.Tooltip("Store the surface normal vector where the object was touched.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeHitNormal;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.pickDistance = (FsmFloat) 100f;
      FsmInt fsmInt = new FsmInt();
      fsmInt.UseVariable = true;
      this.fingerId = fsmInt;
      this.touchBegan = (FsmEvent) null;
      this.touchMoved = (FsmEvent) null;
      this.touchStationary = (FsmEvent) null;
      this.touchEnded = (FsmEvent) null;
      this.touchCanceled = (FsmEvent) null;
      this.storeFingerId = (FsmInt) null;
      this.storeHitPoint = (FsmVector3) null;
      this.storeHitNormal = (FsmVector3) null;
    }

    public override void OnUpdate()
    {
      if ((Object) Camera.main == (Object) null)
      {
        this.LogError("No MainCamera defined!");
        this.Finish();
      }
      else
      {
        if (Input.touchCount <= 0)
          return;
        GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
        if ((Object) ownerDefaultTarget == (Object) null)
          return;
        foreach (Touch touch in Input.touches)
        {
          if (this.fingerId.IsNone || touch.fingerId == this.fingerId.Value)
          {
            RaycastHit hitInfo;
            Physics.Raycast(Camera.main.ScreenPointToRay((Vector3) touch.position), out hitInfo, this.pickDistance.Value);
            this.Fsm.RaycastHitInfo = hitInfo;
            if ((Object) hitInfo.transform != (Object) null && (Object) hitInfo.transform.gameObject == (Object) ownerDefaultTarget)
            {
              this.storeFingerId.Value = touch.fingerId;
              this.storeHitPoint.Value = hitInfo.point;
              this.storeHitNormal.Value = hitInfo.normal;
              switch (touch.phase)
              {
                case TouchPhase.Began:
                  this.Fsm.Event(this.touchBegan);
                  return;
                case TouchPhase.Moved:
                  this.Fsm.Event(this.touchMoved);
                  return;
                case TouchPhase.Stationary:
                  this.Fsm.Event(this.touchStationary);
                  return;
                case TouchPhase.Ended:
                  this.Fsm.Event(this.touchEnded);
                  return;
                case TouchPhase.Canceled:
                  this.Fsm.Event(this.touchCanceled);
                  return;
                default:
                  continue;
              }
            }
          }
        }
      }
    }
  }
}
