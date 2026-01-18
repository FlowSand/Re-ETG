using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionTarget(typeof (Component), "targetProperty", false)]
  [ActionCategory(ActionCategory.UnityObject)]
  [ActionTarget(typeof (GameObject), "targetProperty", false)]
  [HutongGames.PlayMaker.Tooltip("Sets the value of any public property or field on the targeted Unity Object. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
  public class SetProperty : FsmStateAction
  {
    public FsmProperty targetProperty;
    public bool everyFrame;

    public override void Reset()
    {
      this.targetProperty = new FsmProperty()
      {
        setProperty = true
      };
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.targetProperty.SetValue();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.targetProperty.SetValue();
  }
}
