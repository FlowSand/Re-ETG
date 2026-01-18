using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.RenderSettings)]
  [HutongGames.PlayMaker.Tooltip("Sets the Ambient Light Color for the scene.")]
  public class SetAmbientLight : FsmStateAction
  {
    [RequiredField]
    public FsmColor ambientColor;
    public bool everyFrame;

    public override void Reset()
    {
      this.ambientColor = (FsmColor) Color.gray;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetAmbientColor();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetAmbientColor();

    private void DoSetAmbientColor() => RenderSettings.ambientLight = this.ambientColor.Value;
  }
}
