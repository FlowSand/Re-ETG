// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AddTorque2d
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Adds a 2d torque (rotational force) to a Game Object.")]
[ActionCategory(ActionCategory.Physics2D)]
public class AddTorque2d : ComponentAction<Rigidbody2D>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject to add torque to.")]
  [RequiredField]
  [CheckForComponent(typeof (Rigidbody2D))]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Option for applying the force")]
  public ForceMode2D forceMode;
  [HutongGames.PlayMaker.Tooltip("Torque")]
  public FsmFloat torque;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
  public bool everyFrame;

  public override void OnPreprocess() => this.Fsm.HandleFixedUpdate = true;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.torque = (FsmFloat) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoAddTorque();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnFixedUpdate() => this.DoAddTorque();

  private void DoAddTorque()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    this.rigidbody2d.AddTorque(this.torque.Value, this.forceMode);
  }
}
