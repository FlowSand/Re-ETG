// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dTextMeshSetMaxChars
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("2D Toolkit/TextMesh")]
[HutongGames.PlayMaker.Tooltip("Set the maximum characters number of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
public class Tk2dTextMeshSetMaxChars : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
  [RequiredField]
  [CheckForComponent(typeof (tk2dTextMesh))]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The max number of characters")]
  [UIHint(UIHint.FsmInt)]
  public FsmInt maxChars;
  [HutongGames.PlayMaker.Tooltip("Commit changes")]
  [UIHint(UIHint.FsmString)]
  public FsmBool commit;
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
    this.maxChars = (FsmInt) 30;
    this.commit = (FsmBool) true;
    this.everyframe = false;
  }

  public override void OnEnter()
  {
    this._getTextMesh();
    this.DoSetMaxChars();
    if (this.everyframe)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetMaxChars();

  private void DoSetMaxChars()
  {
    if ((Object) this._textMesh == (Object) null)
    {
      this.LogWarning("Missing tk2dTextMesh component: ");
    }
    else
    {
      if (this._textMesh.maxChars == this.maxChars.Value)
        return;
      this._textMesh.maxChars = this.maxChars.Value;
      if (!this.commit.Value)
        return;
      this._textMesh.Commit();
    }
  }
}
