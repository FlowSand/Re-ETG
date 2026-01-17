// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetDrag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=4734.0")]
[HutongGames.PlayMaker.Tooltip("Sets the Drag of a Game Object's Rigid Body.")]
[ActionCategory(ActionCategory.Physics)]
public class SetDrag : ComponentAction<Rigidbody>
{
  [CheckForComponent(typeof (Rigidbody))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HasFloatSlider(0.0f, 10f)]
  [RequiredField]
  public FsmFloat drag;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame. Typically this would be set to True.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.drag = (FsmFloat) 1f;
  }

  public override void OnEnter()
  {
    this.DoSetDrag();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetDrag();

  private void DoSetDrag()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    this.rigidbody.drag = this.drag.Value;
  }
}
