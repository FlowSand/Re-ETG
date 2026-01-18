using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Adds a value to Vector3 Variable.")]
  [ActionCategory(ActionCategory.Vector3)]
  public class Vector3Add : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector3 vector3Variable;
    [RequiredField]
    public FsmVector3 addVector;
    public bool everyFrame;
    public bool perSecond;

    public override void Reset()
    {
      this.vector3Variable = (FsmVector3) null;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.addVector = fsmVector3;
      this.everyFrame = false;
      this.perSecond = false;
    }

    public override void OnEnter()
    {
      this.DoVector3Add();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoVector3Add();

    private void DoVector3Add()
    {
      if (this.perSecond)
        this.vector3Variable.Value += this.addVector.Value * Time.deltaTime;
      else
        this.vector3Variable.Value += this.addVector.Value;
    }
  }
}
