// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.WakeUp2d
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics2D)]
[HutongGames.PlayMaker.Tooltip("Forces a Game Object's Rigid Body 2D to wake up.")]
public class WakeUp2d : ComponentAction<Rigidbody2D>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody2d attached")]
  [CheckForComponent(typeof (Rigidbody2D))]
  [RequiredField]
  public FsmOwnerDefault gameObject;

  public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

  public override void OnEnter()
  {
    this.DoWakeUp();
    this.Finish();
  }

  private void DoWakeUp()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    this.rigidbody2d.WakeUp();
  }
}
