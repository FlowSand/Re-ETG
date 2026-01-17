// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TouchObject2dEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends events when a 2d object is touched. Optionally filter by a fingerID. NOTE: Uses the MainCamera!")]
[ActionCategory(ActionCategory.Device)]
public class TouchObject2dEvent : FsmStateAction
{
  [RequiredField]
  [CheckForComponent(typeof (Collider2D))]
  [HutongGames.PlayMaker.Tooltip("The Game Object to detect touches on.")]
  public FsmOwnerDefault gameObject;
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
  [HutongGames.PlayMaker.Tooltip("Store the 2d position where the object was touched.")]
  [UIHint(UIHint.Variable)]
  public FsmVector2 storeHitPoint;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    FsmInt fsmInt = new FsmInt();
    fsmInt.UseVariable = true;
    this.fingerId = fsmInt;
    this.touchBegan = (FsmEvent) null;
    this.touchMoved = (FsmEvent) null;
    this.touchStationary = (FsmEvent) null;
    this.touchEnded = (FsmEvent) null;
    this.touchCanceled = (FsmEvent) null;
    this.storeFingerId = (FsmInt) null;
    this.storeHitPoint = (FsmVector2) null;
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
          RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay((Vector3) touch.position), float.PositiveInfinity);
          Fsm.RecordLastRaycastHit2DInfo(this.Fsm, rayIntersection);
          if ((Object) rayIntersection.transform != (Object) null && (Object) rayIntersection.transform.gameObject == (Object) ownerDefaultTarget)
          {
            this.storeFingerId.Value = touch.fingerId;
            this.storeHitPoint.Value = rayIntersection.point;
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
