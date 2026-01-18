using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the anchor of a TextMesh. \nThe anchor is stored as a string. tk2dTextMeshSetAnchor can work with this string. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  [ActionCategory("2D Toolkit/TextMesh")]
  public class Tk2dTextMeshGetAnchor : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    [CheckForComponent(typeof (tk2dTextMesh))]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The anchor as a string. \npossible values: LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight ")]
    public FsmString textAnchorAsString;
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
      this.textAnchorAsString = (FsmString) string.Empty;
      this.everyframe = false;
    }

    public override void OnEnter()
    {
      this._getTextMesh();
      this.DoGetAnchor();
      if (this.everyframe)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetAnchor();

    private void DoGetAnchor()
    {
      if ((Object) this._textMesh == (Object) null)
        this.LogWarning("Missing tk2dTextMesh component");
      else
        this.textAnchorAsString.Value = this._textMesh.anchor.ToString();
    }
  }
}
