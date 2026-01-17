// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dTextMeshSetScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory("2D Toolkit/TextMesh")]
  [HutongGames.PlayMaker.Tooltip("Set the scale of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  public class Tk2dTextMeshSetScale : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    [CheckForComponent(typeof (tk2dTextMesh))]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmVector3)]
    [HutongGames.PlayMaker.Tooltip("The scale")]
    public FsmVector3 scale;
    [UIHint(UIHint.FsmBool)]
    [HutongGames.PlayMaker.Tooltip("Commit changes")]
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
      this.scale = (FsmVector3) null;
      this.commit = (FsmBool) true;
      this.everyframe = false;
    }

    public override void OnEnter()
    {
      this._getTextMesh();
      this.DoSetScale();
      if (this.everyframe)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetScale();

    private void DoSetScale()
    {
      if ((Object) this._textMesh == (Object) null)
      {
        this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
      }
      else
      {
        if (!(this._textMesh.scale != this.scale.Value))
          return;
        this._textMesh.scale = this.scale.Value;
        if (!this.commit.Value)
          return;
        this._textMesh.Commit();
      }
    }
  }
}
