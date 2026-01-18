using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Smoothly Rotates a Game Object so its forward vector points in the specified Direction. Lets you fire an event when minmagnitude is reached")]
  [ActionCategory(ActionCategory.Transform)]
  public class SmoothLookAtDirection : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The direction to smoothly rotate towards.")]
    [RequiredField]
    public FsmVector3 targetDirection;
    [HutongGames.PlayMaker.Tooltip("Only rotate if Target Direction Vector length is greater than this threshold.")]
    public FsmFloat minMagnitude;
    [HutongGames.PlayMaker.Tooltip("Keep this vector pointing up as the GameObject rotates.")]
    public FsmVector3 upVector;
    [HutongGames.PlayMaker.Tooltip("Eliminate any tilt up/down as the GameObject rotates.")]
    [RequiredField]
    public FsmBool keepVertical;
    [HutongGames.PlayMaker.Tooltip("How quickly to rotate.")]
    [HasFloatSlider(0.5f, 15f)]
    [RequiredField]
    public FsmFloat speed;
    [HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
    public bool lateUpdate;
    [HutongGames.PlayMaker.Tooltip("Event to send if the direction difference is less than Min Magnitude.")]
    public FsmEvent finishEvent;
    [HutongGames.PlayMaker.Tooltip("Stop running the action if the direction difference is less than Min Magnitude.")]
    public FsmBool finish;
    private GameObject previousGo;
    private Quaternion lastRotation;
    private Quaternion desiredRotation;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      FsmVector3 fsmVector3_1 = new FsmVector3();
      fsmVector3_1.UseVariable = true;
      this.targetDirection = fsmVector3_1;
      this.minMagnitude = (FsmFloat) 0.1f;
      FsmVector3 fsmVector3_2 = new FsmVector3();
      fsmVector3_2.UseVariable = true;
      this.upVector = fsmVector3_2;
      this.keepVertical = (FsmBool) true;
      this.speed = (FsmFloat) 5f;
      this.lateUpdate = true;
      this.finishEvent = (FsmEvent) null;
    }

    public override void OnEnter() => this.previousGo = (GameObject) null;

    public override void OnUpdate()
    {
      if (this.lateUpdate)
        return;
      this.DoSmoothLookAtDirection();
    }

    public override void OnLateUpdate()
    {
      if (!this.lateUpdate)
        return;
      this.DoSmoothLookAtDirection();
    }

    private void DoSmoothLookAtDirection()
    {
      if (this.targetDirection.IsNone)
        return;
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if ((Object) this.previousGo != (Object) ownerDefaultTarget)
      {
        this.lastRotation = ownerDefaultTarget.transform.rotation;
        this.desiredRotation = this.lastRotation;
        this.previousGo = ownerDefaultTarget;
      }
      Vector3 forward = this.targetDirection.Value;
      if (this.keepVertical.Value)
        forward.y = 0.0f;
      bool flag = false;
      if ((double) forward.sqrMagnitude > (double) this.minMagnitude.Value)
        this.desiredRotation = Quaternion.LookRotation(forward, !this.upVector.IsNone ? this.upVector.Value : Vector3.up);
      else
        flag = true;
      this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
      ownerDefaultTarget.transform.rotation = this.lastRotation;
      if (!flag)
        return;
      this.Fsm.Event(this.finishEvent);
      if (!this.finish.Value)
        return;
      this.Finish();
    }
  }
}
