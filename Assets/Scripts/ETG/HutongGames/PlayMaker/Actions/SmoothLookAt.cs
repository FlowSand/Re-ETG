// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SmoothLookAt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Smoothly Rotates a Game Object so its forward vector points at a Target. The target can be defined as a Game Object or a world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
  [ActionCategory(ActionCategory.Transform)]
  public class SmoothLookAt : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to rotate to face a target.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("A target GameObject.")]
    public FsmGameObject targetObject;
    [HutongGames.PlayMaker.Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
    public FsmVector3 targetPosition;
    [HutongGames.PlayMaker.Tooltip("Used to keep the game object generally upright. If left undefined the world y axis is used.")]
    public FsmVector3 upVector;
    [HutongGames.PlayMaker.Tooltip("Force the game object to remain vertical. Useful for characters.")]
    public FsmBool keepVertical;
    [HutongGames.PlayMaker.Tooltip("How fast the look at moves.")]
    [HasFloatSlider(0.5f, 15f)]
    public FsmFloat speed;
    [HutongGames.PlayMaker.Tooltip("Draw a line in the Scene View to the look at position.")]
    public FsmBool debug;
    [HutongGames.PlayMaker.Tooltip("If the angle to the target is less than this, send the Finish Event below. Measured in degrees.")]
    public FsmFloat finishTolerance;
    [HutongGames.PlayMaker.Tooltip("Event to send if the angle to target is less than the Finish Tolerance.")]
    public FsmEvent finishEvent;
    private GameObject previousGo;
    private Quaternion lastRotation;
    private Quaternion desiredRotation;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.targetObject = (FsmGameObject) null;
      FsmVector3 fsmVector3_1 = new FsmVector3();
      fsmVector3_1.UseVariable = true;
      this.targetPosition = fsmVector3_1;
      FsmVector3 fsmVector3_2 = new FsmVector3();
      fsmVector3_2.UseVariable = true;
      this.upVector = fsmVector3_2;
      this.keepVertical = (FsmBool) true;
      this.debug = (FsmBool) false;
      this.speed = (FsmFloat) 5f;
      this.finishTolerance = (FsmFloat) 1f;
      this.finishEvent = (FsmEvent) null;
    }

    public override void OnEnter() => this.previousGo = (GameObject) null;

    public override void OnLateUpdate() => this.DoSmoothLookAt();

    private void DoSmoothLookAt()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      GameObject gameObject = this.targetObject.Value;
      if ((Object) gameObject == (Object) null && this.targetPosition.IsNone)
        return;
      if ((Object) this.previousGo != (Object) ownerDefaultTarget)
      {
        this.lastRotation = ownerDefaultTarget.transform.rotation;
        this.desiredRotation = this.lastRotation;
        this.previousGo = ownerDefaultTarget;
      }
      Vector3 end = !((Object) gameObject != (Object) null) ? this.targetPosition.Value : (this.targetPosition.IsNone ? gameObject.transform.position : gameObject.transform.TransformPoint(this.targetPosition.Value));
      if (this.keepVertical.Value)
        end.y = ownerDefaultTarget.transform.position.y;
      Vector3 forward = end - ownerDefaultTarget.transform.position;
      if (forward != Vector3.zero && (double) forward.sqrMagnitude > 0.0)
        this.desiredRotation = Quaternion.LookRotation(forward, !this.upVector.IsNone ? this.upVector.Value : Vector3.up);
      this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
      ownerDefaultTarget.transform.rotation = this.lastRotation;
      if (this.debug.Value)
        Debug.DrawLine(ownerDefaultTarget.transform.position, end, Color.grey);
      if (this.finishEvent == null || (double) Mathf.Abs(Vector3.Angle(end - ownerDefaultTarget.transform.position, ownerDefaultTarget.transform.forward)) > (double) this.finishTolerance.Value)
        return;
      this.Fsm.Event(this.finishEvent);
    }
  }
}
