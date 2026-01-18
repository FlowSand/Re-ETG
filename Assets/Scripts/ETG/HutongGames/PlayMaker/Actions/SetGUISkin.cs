using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the GUISkin used by GUI elements.")]
  [ActionCategory(ActionCategory.GUI)]
  public class SetGUISkin : FsmStateAction
  {
    [RequiredField]
    public GUISkin skin;
    public FsmBool applyGlobally;

    public override void Reset()
    {
      this.skin = (GUISkin) null;
      this.applyGlobally = (FsmBool) true;
    }

    public override void OnGUI()
    {
      if ((Object) this.skin != (Object) null)
        GUI.skin = this.skin;
      if (!this.applyGlobally.Value)
        return;
      PlayMakerGUI.GUISkin = this.skin;
      this.Finish();
    }
  }
}
