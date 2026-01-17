// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
[ActionCategory(ActionCategory.Transform)]
public class SetScale : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject to scale.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Use stored Vector3 value, and/or set each axis below.")]
  public FsmVector3 vector;
  public FsmFloat x;
  public FsmFloat y;
  public FsmFloat z;
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
    this.everyFrame = false;
    this.lateUpdate = false;
  }

  public override void OnEnter()
  {
    this.DoSetScale();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate()
  {
    if (this.lateUpdate)
      return;
    this.DoSetScale();
  }

  public override void OnLateUpdate()
  {
    if (this.lateUpdate)
      this.DoSetScale();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  private void DoSetScale()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    Vector3 vector3 = !this.vector.IsNone ? this.vector.Value : ownerDefaultTarget.transform.localScale;
    if (!this.x.IsNone)
      vector3.x = this.x.Value;
    if (!this.y.IsNone)
      vector3.y = this.y.Value;
    if (!this.z.IsNone)
      vector3.z = this.z.Value;
    ownerDefaultTarget.transform.localScale = vector3;
  }
}
