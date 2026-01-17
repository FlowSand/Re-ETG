// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetSpeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics)]
[HutongGames.PlayMaker.Tooltip("Gets the Speed of a Game Object and stores it in a Float Variable. NOTE: The Game Object must have a rigid body.")]
public class GetSpeed : ComponentAction<Rigidbody>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody.")]
  [RequiredField]
  [CheckForComponent(typeof (Rigidbody))]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("Store the speed in a float variable.")]
  public FsmFloat storeResult;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.storeResult = (FsmFloat) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoGetSpeed();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetSpeed();

  private void DoGetSpeed()
  {
    if (this.storeResult == null || !this.UpdateCache(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner))
      return;
    this.storeResult.Value = this.rigidbody.velocity.magnitude;
  }
}
