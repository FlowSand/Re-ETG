using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the colors of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  [ActionCategory("2D Toolkit/TextMesh")]
  public class Tk2dTextMeshGetColors : FsmStateAction
  {
    [RequiredField]
    [CheckForComponent(typeof (tk2dTextMesh))]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Main color")]
    public FsmColor mainColor;
    [HutongGames.PlayMaker.Tooltip("Gradient color. Only used if gradient is true")]
    [UIHint(UIHint.Variable)]
    public FsmColor gradientColor;
    [HutongGames.PlayMaker.Tooltip("Use gradient.")]
    [UIHint(UIHint.Variable)]
    public FsmBool useGradient;
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
      this.mainColor = (FsmColor) null;
      this.gradientColor = (FsmColor) null;
      this.useGradient = (FsmBool) false;
      this.everyframe = false;
    }

    public override void OnEnter()
    {
      this._getTextMesh();
      this.DoGetColors();
      if (this.everyframe)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetColors();

    private void DoGetColors()
    {
      if ((Object) this._textMesh == (Object) null)
      {
        this.LogWarning("Missing tk2dTextMesh component: ");
      }
      else
      {
        this.useGradient.Value = this._textMesh.useGradient;
        this.mainColor.Value = this._textMesh.color;
        this.gradientColor.Value = this._textMesh.color2;
      }
    }
  }
}
