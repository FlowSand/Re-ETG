// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DetachChildren
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Unparents all children from the Game Object.")]
[ActionCategory(ActionCategory.GameObject)]
public class DetachChildren : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("GameObject to unparent children from.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;

  public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

  public override void OnEnter()
  {
    DetachChildren.DoDetachChildren(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
    this.Finish();
  }

  private static void DoDetachChildren(GameObject go)
  {
    if (!((Object) go != (Object) null))
      return;
    go.transform.DetachChildren();
  }
}
