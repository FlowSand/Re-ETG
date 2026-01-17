// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetTag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[HutongGames.PlayMaker.Tooltip("Sets a Game Object's Tag.")]
public class SetTag : FsmStateAction
{
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Tag)]
  public FsmString tag;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.tag = (FsmString) "Untagged";
  }

  public override void OnEnter()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget != (Object) null)
      ownerDefaultTarget.tag = this.tag.Value;
    this.Finish();
  }
}
