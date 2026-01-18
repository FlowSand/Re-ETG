using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Set the font of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  [ActionCategory("2D Toolkit/TextMesh")]
  public class Tk2dTextMeshSetFont : FsmStateAction
  {
    [CheckForComponent(typeof (tk2dTextMesh))]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmGameObject)]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The font gameObject")]
    [CheckForComponent(typeof (tk2dFont))]
    public FsmGameObject font;
    [HutongGames.PlayMaker.Tooltip("Commit changes")]
    [UIHint(UIHint.FsmString)]
    public FsmBool commit;
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
      this.font = (FsmGameObject) null;
      this.commit = (FsmBool) true;
    }

    public override void OnEnter()
    {
      this._getTextMesh();
      this.DoSetFont();
      this.Finish();
    }

    private void DoSetFont()
    {
      if ((Object) this._textMesh == (Object) null)
      {
        this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
      }
      else
      {
        GameObject gameObject = this.font.Value;
        if ((Object) gameObject == (Object) null)
          return;
        tk2dFont component = gameObject.GetComponent<tk2dFont>();
        if ((Object) component == (Object) null)
          return;
        this._textMesh.font = component.data;
        this._textMesh.GetComponent<Renderer>().material = component.material;
        this._textMesh.Init(true);
      }
    }
  }
}
