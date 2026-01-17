// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MoveTowards
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Transform)]
  [HutongGames.PlayMaker.Tooltip("Moves a Game Object towards a Target. Optionally sends an event when successful. The Target can be specified as a Game Object or a world Position. If you specify both, then the Position is used as a local offset from the Object's Position.")]
  public class MoveTowards : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to Move")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("A target GameObject to move towards. Or use a world Target Position below.")]
    public FsmGameObject targetObject;
    [HutongGames.PlayMaker.Tooltip("A world position if no Target Object. Otherwise used as a local offset from the Target Object.")]
    public FsmVector3 targetPosition;
    [HutongGames.PlayMaker.Tooltip("Ignore any height difference in the target.")]
    public FsmBool ignoreVertical;
    [HutongGames.PlayMaker.Tooltip("The maximum movement speed. HINT: You can make this a variable to change it over time.")]
    [HasFloatSlider(0.0f, 20f)]
    public FsmFloat maxSpeed;
    [HutongGames.PlayMaker.Tooltip("Distance at which the move is considered finished, and the Finish Event is sent.")]
    [HasFloatSlider(0.0f, 5f)]
    public FsmFloat finishDistance;
    [HutongGames.PlayMaker.Tooltip("Event to send when the Finish Distance is reached.")]
    public FsmEvent finishEvent;
    private GameObject go;
    private GameObject goTarget;
    private Vector3 targetPos;
    private Vector3 targetPosWithVertical;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.targetObject = (FsmGameObject) null;
      this.maxSpeed = (FsmFloat) 10f;
      this.finishDistance = (FsmFloat) 1f;
      this.finishEvent = (FsmEvent) null;
    }

    public override void OnUpdate() => this.DoMoveTowards();

    private void DoMoveTowards()
    {
      if (!this.UpdateTargetPos())
        return;
      this.go.transform.position = Vector3.MoveTowards(this.go.transform.position, this.targetPos, this.maxSpeed.Value * Time.deltaTime);
      if ((double) (this.go.transform.position - this.targetPos).magnitude >= (double) this.finishDistance.Value)
        return;
      this.Fsm.Event(this.finishEvent);
      this.Finish();
    }

    public bool UpdateTargetPos()
    {
      this.go = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) this.go == (Object) null)
        return false;
      this.goTarget = this.targetObject.Value;
      if ((Object) this.goTarget == (Object) null && this.targetPosition.IsNone)
        return false;
      this.targetPos = !((Object) this.goTarget != (Object) null) ? this.targetPosition.Value : (this.targetPosition.IsNone ? this.goTarget.transform.position : this.goTarget.transform.TransformPoint(this.targetPosition.Value));
      this.targetPosWithVertical = this.targetPos;
      if (this.ignoreVertical.Value)
        this.targetPos.y = this.go.transform.position.y;
      return true;
    }

    public Vector3 GetTargetPos() => this.targetPos;

    public Vector3 GetTargetPosWithVertical() => this.targetPosWithVertical;
  }
}
