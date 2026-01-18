using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HelpUrl("https://hutonggames.fogbugz.com/default.asp?W719")]
  [HutongGames.PlayMaker.Tooltip("Set the text of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  [ActionCategory("2D Toolkit/TextMesh")]
  public class Tk2dTextMeshSetText : FsmStateAction
  {
    [RequiredField]
    [CheckForComponent(typeof (tk2dTextMesh))]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmString)]
    [HutongGames.PlayMaker.Tooltip("The text")]
    public FsmString text;
    [UIHint(UIHint.FsmString)]
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
      this.text = (FsmString) string.Empty;
      this.commit = (FsmBool) true;
      this.everyframe = false;
    }

    public override void OnEnter()
    {
      this._getTextMesh();
      this.DoSetText();
      if (this.everyframe)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetText();

    private void DoSetText()
    {
      if ((Object) this._textMesh == (Object) null)
      {
        this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
      }
      else
      {
        if (!(this._textMesh.text != this.text.Value))
          return;
        this._textMesh.text = this.text.Value;
        if (!this.commit.Value)
          return;
        this._textMesh.Commit();
      }
    }
  }
}
