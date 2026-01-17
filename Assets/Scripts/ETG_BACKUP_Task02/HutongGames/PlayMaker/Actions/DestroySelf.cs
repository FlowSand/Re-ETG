// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DestroySelf
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Destroys the Owner of the Fsm! Useful for spawned Prefabs that need to kill themselves, e.g., a projectile that explodes on impact.")]
[ActionCategory(ActionCategory.GameObject)]
public class DestroySelf : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Detach children before destroying the Owner.")]
  public FsmBool detachChildren;

  public override void Reset() => this.detachChildren = (FsmBool) false;

  public override void OnEnter()
  {
    if ((Object) this.Owner != (Object) null)
    {
      if (this.detachChildren.Value)
        this.Owner.transform.DetachChildren();
      Object.Destroy((Object) this.Owner);
    }
    this.Finish();
  }
}
