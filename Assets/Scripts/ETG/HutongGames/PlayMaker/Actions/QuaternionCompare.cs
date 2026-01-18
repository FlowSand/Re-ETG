using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Check if two quaternions are equals or not. Takes in account inversed representations of quaternions")]
  [ActionCategory(ActionCategory.Quaternion)]
  public class QuaternionCompare : QuaternionBaseAction
  {
    [HutongGames.PlayMaker.Tooltip("First Quaternion")]
    [RequiredField]
    public FsmQuaternion Quaternion1;
    [HutongGames.PlayMaker.Tooltip("Second Quaternion")]
    [RequiredField]
    public FsmQuaternion Quaternion2;
    [HutongGames.PlayMaker.Tooltip("true if Quaternions are equal")]
    public FsmBool equal;
    [HutongGames.PlayMaker.Tooltip("Event sent if Quaternions are equal")]
    public FsmEvent equalEvent;
    [HutongGames.PlayMaker.Tooltip("Event sent if Quaternions are not equal")]
    public FsmEvent notEqualEvent;

    public override void Reset()
    {
      FsmQuaternion fsmQuaternion1 = new FsmQuaternion();
      fsmQuaternion1.UseVariable = true;
      this.Quaternion1 = fsmQuaternion1;
      FsmQuaternion fsmQuaternion2 = new FsmQuaternion();
      fsmQuaternion2.UseVariable = true;
      this.Quaternion2 = fsmQuaternion2;
      this.equal = (FsmBool) null;
      this.equalEvent = (FsmEvent) null;
      this.notEqualEvent = (FsmEvent) null;
      this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
    }

    public override void OnEnter()
    {
      this.DoQuatCompare();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
        return;
      this.DoQuatCompare();
    }

    public override void OnLateUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
        return;
      this.DoQuatCompare();
    }

    public override void OnFixedUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.DoQuatCompare();
    }

    private void DoQuatCompare()
    {
      bool flag = (double) Mathf.Abs(Quaternion.Dot(this.Quaternion1.Value, this.Quaternion2.Value)) > 0.99999898672103882;
      this.equal.Value = flag;
      if (flag)
        this.Fsm.Event(this.equalEvent);
      else
        this.Fsm.Event(this.notEqualEvent);
    }
  }
}
