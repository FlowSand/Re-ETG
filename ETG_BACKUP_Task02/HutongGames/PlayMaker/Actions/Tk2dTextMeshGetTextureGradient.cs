// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dTextMeshGetTextureGradient
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Set the texture gradient of the font currently applied to a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
[ActionCategory("2D Toolkit/TextMesh")]
public class Tk2dTextMeshGetTextureGradient : FsmStateAction
{
  [RequiredField]
  [CheckForComponent(typeof (tk2dTextMesh))]
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The Gradient Id")]
  public FsmInt textureGradient;
  [ActionSection("")]
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
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
    this.textureGradient = (FsmInt) 0;
    this.everyframe = false;
  }

  public override void OnEnter()
  {
    this._getTextMesh();
    this.DoGetTextureGradient();
    if (this.everyframe)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetTextureGradient();

  private void DoGetTextureGradient()
  {
    if ((Object) this._textMesh == (Object) null)
      this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
    else
      this.textureGradient.Value = this._textMesh.textureGradient;
  }
}
