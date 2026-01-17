// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.InverseTransformPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Transforms position from world space to a Game Object's local space. The opposite of TransformPoint.")]
[ActionCategory(ActionCategory.Transform)]
public class InverseTransformPoint : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  public FsmVector3 worldPosition;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmVector3 storeResult;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.worldPosition = (FsmVector3) null;
    this.storeResult = (FsmVector3) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoInverseTransformPoint();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoInverseTransformPoint();

  private void DoInverseTransformPoint()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    this.storeResult.Value = ownerDefaultTarget.transform.InverseTransformPoint(this.worldPosition.Value);
  }
}
