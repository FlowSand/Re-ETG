using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GUIElement)]
  [HutongGames.PlayMaker.Tooltip("Sets the Texture used by the GUITexture attached to a Game Object.")]
  public class SetGUITexture : ComponentAction<GUITexture>
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the GUITexture.")]
    [CheckForComponent(typeof (GUITexture))]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Texture to apply.")]
    public FsmTexture texture;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.texture = (FsmTexture) null;
    }

    public override void OnEnter()
    {
      if (this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        this.guiTexture.texture = this.texture.Value;
      this.Finish();
    }
  }
}
