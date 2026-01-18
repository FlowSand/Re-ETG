using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Check that inline styling can indeed be used ( the font needs to have texture gradients for inline styling to work). \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  [ActionCategory("2D Toolkit/TextMesh")]
  public class Tk2dTextMeshGetInlineStylingIsAvailable : FsmStateAction
  {
    [RequiredField]
    [CheckForComponent(typeof (tk2dTextMesh))]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Is inline styling available? true if inlineStyling is true AND the font texturGradients is true")]
    public FsmBool InlineStylingAvailable;
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
      this.InlineStylingAvailable = (FsmBool) null;
      this.everyframe = false;
    }

    public override void OnEnter()
    {
      this._getTextMesh();
      this.DoGetInlineStylingAvailable();
      if (this.everyframe)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetInlineStylingAvailable();

    private void DoGetInlineStylingAvailable()
    {
      if ((Object) this._textMesh == (Object) null)
        this.LogWarning("Missing tk2dTextMesh component: ");
      else
        this.InlineStylingAvailable.Value = this._textMesh.inlineStyling && this._textMesh.font.textureGradients;
    }
  }
}
