// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.LookAt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Rotates a Game Object so its forward vector points at a Target. The Target can be specified as a GameObject or a world Position. If you specify both, then Position specifies a local offset from the target object's Position.")]
  [ActionCategory(ActionCategory.Transform)]
  public class LookAt : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The GameObject to Look At.")]
    public FsmGameObject targetObject;
    [HutongGames.PlayMaker.Tooltip("World position to look at, or local offset from Target Object if specified.")]
    public FsmVector3 targetPosition;
    [HutongGames.PlayMaker.Tooltip("Rotate the GameObject to point its up direction vector in the direction hinted at by the Up Vector. See Unity Look At docs for more details.")]
    public FsmVector3 upVector;
    [HutongGames.PlayMaker.Tooltip("Don't rotate vertically.")]
    public FsmBool keepVertical;
    [HutongGames.PlayMaker.Tooltip("Draw a debug line from the GameObject to the Target.")]
    [Title("Draw Debug Line")]
    public FsmBool debug;
    [HutongGames.PlayMaker.Tooltip("Color to use for the debug line.")]
    public FsmColor debugLineColor;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame = true;
    private GameObject go;
    private GameObject goTarget;
    private Vector3 lookAtPos;
    private Vector3 lookAtPosWithVertical;

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
      this.debugLineColor = (FsmColor) Color.yellow;
      this.everyFrame = true;
    }

    public override void OnEnter()
    {
      this.DoLookAt();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnLateUpdate() => this.DoLookAt();

    private void DoLookAt()
    {
      if (!this.UpdateLookAtPosition())
        return;
      this.go.transform.LookAt(this.lookAtPos, !this.upVector.IsNone ? this.upVector.Value : Vector3.up);
      if (!this.debug.Value)
        return;
      Debug.DrawLine(this.go.transform.position, this.lookAtPos, this.debugLineColor.Value);
    }

    public bool UpdateLookAtPosition()
    {
      if (this.Fsm == null)
        return false;
      this.go = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) this.go == (Object) null)
        return false;
      this.goTarget = this.targetObject.Value;
      if ((Object) this.goTarget == (Object) null && this.targetPosition.IsNone)
        return false;
      this.lookAtPos = !((Object) this.goTarget != (Object) null) ? this.targetPosition.Value : (this.targetPosition.IsNone ? this.goTarget.transform.position : this.goTarget.transform.TransformPoint(this.targetPosition.Value));
      this.lookAtPosWithVertical = this.lookAtPos;
      if (this.keepVertical.Value)
        this.lookAtPos.y = this.go.transform.position.y;
      return true;
    }

    public Vector3 GetLookAtPosition() => this.lookAtPos;

    public Vector3 GetLookAtPositionWithVertical() => this.lookAtPosWithVertical;
  }
}
