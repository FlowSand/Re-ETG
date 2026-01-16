// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dTextMeshGetInlineStyling
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("2D Toolkit/TextMesh")]
[HutongGames.PlayMaker.Tooltip("Get the inline styling flag of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
public class Tk2dTextMeshGetInlineStyling : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
  [RequiredField]
  [CheckForComponent(typeof (tk2dTextMesh))]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The max number of characters")]
  [UIHint(UIHint.Variable)]
  public FsmBool inlineStyling;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  [ActionSection("")]
  public bool everyframe;
  private tk2dTextMesh _textMesh;

  private void _getTextMesh()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
  }

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.inlineStyling = (FsmBool) null;
    this.everyframe = false;
  }

  public override void OnEnter()
  {
    this._getTextMesh();
    this.DoGetInlineStyling();
    if (this.everyframe)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetInlineStyling();

  private void DoGetInlineStyling()
  {
    if ((Object) this._textMesh == (Object) null)
      this.LogWarning("Missing tk2dTextMesh component: ");
    else
      this.inlineStyling.Value = this._textMesh.inlineStyling;
  }
}
