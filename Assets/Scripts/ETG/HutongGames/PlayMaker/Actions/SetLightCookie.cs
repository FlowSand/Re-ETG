using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the Texture projected by a Light.")]
  [ActionCategory(ActionCategory.Lights)]
  public class SetLightCookie : ComponentAction<Light>
  {
    [CheckForComponent(typeof (Light))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    public FsmTexture lightCookie;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.lightCookie = (FsmTexture) null;
    }

    public override void OnEnter()
    {
      this.DoSetLightCookie();
      this.Finish();
    }

    private void DoSetLightCookie()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.light.cookie = this.lightCookie.Value;
    }
  }
}
