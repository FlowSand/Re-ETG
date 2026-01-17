// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetPosition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Transform)]
[HutongGames.PlayMaker.Tooltip("Sets the Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
public class SetPosition : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject to position.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Use a stored Vector3 position, and/or set individual axis below.")]
  [UIHint(UIHint.Variable)]
  public FsmVector3 vector;
  public FsmFloat x;
  public FsmFloat y;
  public FsmFloat z;
  [HutongGames.PlayMaker.Tooltip("Use local or world space.")]
  public Space space;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;
  [HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
  public bool lateUpdate;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.vector = (FsmVector3) null;
    FsmFloat fsmFloat1 = new FsmFloat();
    fsmFloat1.UseVariable = true;
    this.x = fsmFloat1;
    FsmFloat fsmFloat2 = new FsmFloat();
    fsmFloat2.UseVariable = true;
    this.y = fsmFloat2;
    FsmFloat fsmFloat3 = new FsmFloat();
    fsmFloat3.UseVariable = true;
    this.z = fsmFloat3;
    this.space = Space.Self;
    this.everyFrame = false;
    this.lateUpdate = false;
  }

  public override void OnEnter()
  {
    if (this.everyFrame || this.lateUpdate)
      return;
    this.DoSetPosition();
    this.Finish();
  }

  public override void OnUpdate()
  {
    if (this.lateUpdate)
      return;
    this.DoSetPosition();
  }

  public override void OnLateUpdate()
  {
    if (this.lateUpdate)
      this.DoSetPosition();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  private void DoSetPosition()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    Vector3 vector3 = !this.vector.IsNone ? this.vector.Value : (this.space != Space.World ? ownerDefaultTarget.transform.localPosition : ownerDefaultTarget.transform.position);
    if (!this.x.IsNone)
      vector3.x = this.x.Value;
    if (!this.y.IsNone)
      vector3.y = this.y.Value;
    if (!this.z.IsNone)
      vector3.z = this.z.Value;
    if (this.space == Space.World)
      ownerDefaultTarget.transform.position = vector3;
    else
      ownerDefaultTarget.transform.localPosition = vector3;
  }
}
