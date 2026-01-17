// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.InverseTransformDirection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Transforms a Direction from world space to a Game Object's local space. The opposite of TransformDirection.")]
[ActionCategory(ActionCategory.Transform)]
public class InverseTransformDirection : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  public FsmVector3 worldDirection;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmVector3 storeResult;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.worldDirection = (FsmVector3) null;
    this.storeResult = (FsmVector3) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoInverseTransformDirection();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoInverseTransformDirection();

  private void DoInverseTransformDirection()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    this.storeResult.Value = ownerDefaultTarget.transform.InverseTransformDirection(this.worldDirection.Value);
  }
}
