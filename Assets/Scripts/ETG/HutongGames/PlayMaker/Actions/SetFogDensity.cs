using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the density of the Fog in the scene.")]
  [ActionCategory(ActionCategory.RenderSettings)]
  public class SetFogDensity : FsmStateAction
  {
    [RequiredField]
    public FsmFloat fogDensity;
    public bool everyFrame;

    public override void Reset()
    {
      this.fogDensity = (FsmFloat) 0.5f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetFogDensity();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetFogDensity();

    private void DoSetFogDensity() => RenderSettings.fogDensity = this.fogDensity.Value;
  }
}
