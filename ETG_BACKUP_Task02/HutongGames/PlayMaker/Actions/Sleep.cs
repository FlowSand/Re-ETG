// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Sleep
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics)]
[HutongGames.PlayMaker.Tooltip("Forces a Game Object's Rigid Body to Sleep at least one frame.")]
public class Sleep : ComponentAction<Rigidbody>
{
  [RequiredField]
  [CheckForComponent(typeof (Rigidbody))]
  public FsmOwnerDefault gameObject;

  public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

  public override void OnEnter()
  {
    this.DoSleep();
    this.Finish();
  }

  private void DoSleep()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    this.rigidbody.Sleep();
  }
}
