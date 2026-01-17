// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TransformDirection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Transform)]
[HutongGames.PlayMaker.Tooltip("Transforms a Direction from a Game Object's local space to world space.")]
public class TransformDirection : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  public FsmVector3 localDirection;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmVector3 storeResult;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.localDirection = (FsmVector3) null;
    this.storeResult = (FsmVector3) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoTransformDirection();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoTransformDirection();

  private void DoTransformDirection()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    this.storeResult.Value = ownerDefaultTarget.transform.TransformDirection(this.localDirection.Value);
  }
}
