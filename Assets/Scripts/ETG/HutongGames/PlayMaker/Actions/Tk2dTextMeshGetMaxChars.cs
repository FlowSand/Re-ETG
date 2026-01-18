using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the maximum characters number of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  [ActionCategory("2D Toolkit/TextMesh")]
  public class Tk2dTextMeshGetMaxChars : FsmStateAction
  {
    [CheckForComponent(typeof (tk2dTextMesh))]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("The max number of characters")]
    public FsmInt maxChars;
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
      this.maxChars = (FsmInt) null;
      this.everyframe = false;
    }

    public override void OnEnter()
    {
      this._getTextMesh();
      this.DoGetMaxChars();
      if (this.everyframe)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetMaxChars();

    private void DoGetMaxChars()
    {
      if ((Object) this._textMesh == (Object) null)
        this.LogWarning("Missing tk2dTextMesh component: ");
      else
        this.maxChars.Value = this._textMesh.maxChars;
    }
  }
}
