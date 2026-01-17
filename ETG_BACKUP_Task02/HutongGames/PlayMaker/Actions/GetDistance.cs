// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable.")]
[ActionCategory(ActionCategory.GameObject)]
public class GetDistance : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Measure distance from this GameObject.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Target GameObject.")]
  [RequiredField]
  public FsmGameObject target;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("Store the distance in a float variable.")]
  public FsmFloat storeResult;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.target = (FsmGameObject) null;
    this.storeResult = (FsmFloat) null;
    this.everyFrame = true;
  }

  public override void OnEnter()
  {
    this.DoGetDistance();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetDistance();

  private void DoGetDistance()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null || (Object) this.target.Value == (Object) null || this.storeResult == null)
      return;
    this.storeResult.Value = Vector3.Distance(ownerDefaultTarget.transform.position, this.target.Value.transform.position);
  }
}
