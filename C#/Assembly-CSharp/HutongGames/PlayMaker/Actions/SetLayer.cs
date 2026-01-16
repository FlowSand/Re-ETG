// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[HutongGames.PlayMaker.Tooltip("Sets a Game Object's Layer.")]
public class SetLayer : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Layer)]
  public int layer;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.layer = 0;
  }

  public override void OnEnter()
  {
    this.DoSetLayer();
    this.Finish();
  }

  private void DoSetLayer()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    ownerDefaultTarget.layer = this.layer;
  }
}
