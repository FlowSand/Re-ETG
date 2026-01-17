// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dTextMeshMakePixelPerfect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Make a TextMesh pixelPerfect. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  [ActionCategory("2D Toolkit/TextMesh")]
  public class Tk2dTextMeshMakePixelPerfect : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    [RequiredField]
    [CheckForComponent(typeof (tk2dTextMesh))]
    public FsmOwnerDefault gameObject;
    private tk2dTextMesh _textMesh;

    private void _getTextMesh()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
    }

    public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

    public override void OnEnter()
    {
      this._getTextMesh();
      this.MakePixelPerfect();
      this.Finish();
    }

    private void MakePixelPerfect()
    {
      if ((Object) this._textMesh == (Object) null)
        this.LogWarning("Missing tk2dTextMesh component ");
      else
        this._textMesh.MakePixelPerfect();
    }
  }
}
