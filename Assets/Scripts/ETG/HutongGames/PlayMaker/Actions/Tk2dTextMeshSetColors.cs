using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Set the colors of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  [ActionCategory("2D Toolkit/TextMesh")]
  public class Tk2dTextMeshSetColors : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    [CheckForComponent(typeof (tk2dTextMesh))]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmColor)]
    [HutongGames.PlayMaker.Tooltip("Main color")]
    public FsmColor mainColor;
    [UIHint(UIHint.FsmColor)]
    [HutongGames.PlayMaker.Tooltip("Gradient color. Only used if gradient is true")]
    public FsmColor gradientColor;
    [UIHint(UIHint.FsmBool)]
    [HutongGames.PlayMaker.Tooltip("Use gradient.")]
    public FsmBool useGradient;
    [UIHint(UIHint.FsmString)]
    [HutongGames.PlayMaker.Tooltip("Commit changes")]
    public FsmBool commit;
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
      this.commit = (FsmBool) true;
      this.everyframe = false;
    }

    public override void OnEnter()
    {
      this._getTextMesh();
      this.DoSetColors();
      if (this.everyframe)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetColors();

    private void DoSetColors()
    {
      if ((Object) this._textMesh == (Object) null)
      {
        this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
      }
      else
      {
        bool flag = false;
        if (this._textMesh.useGradient != this.useGradient.Value)
        {
          this._textMesh.useGradient = this.useGradient.Value;
          flag = true;
        }
        if (this._textMesh.color != this.mainColor.Value)
        {
          this._textMesh.color = this.mainColor.Value;
          flag = true;
        }
        if (this._textMesh.color2 != this.gradientColor.Value)
        {
          this._textMesh.color2 = this.gradientColor.Value;
          flag = true;
        }
        if (!this.commit.Value || !flag)
          return;
        this._textMesh.Commit();
      }
    }
  }
}
