using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Vector3)]
  [HutongGames.PlayMaker.Tooltip("Performs most possible operations on 2 Vector3: Dot product, Cross product, Distance, Angle, Project, Reflect, Add, Subtract, Multiply, Divide, Min, Max")]
  public class Vector3Operator : FsmStateAction
  {
    [RequiredField]
    public FsmVector3 vector1;
    [RequiredField]
    public FsmVector3 vector2;
    public Vector3Operator.Vector3Operation operation = Vector3Operator.Vector3Operation.Add;
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeVector3Result;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeFloatResult;
    public bool everyFrame;

    public override void Reset()
    {
      this.vector1 = (FsmVector3) null;
      this.vector2 = (FsmVector3) null;
      this.operation = Vector3Operator.Vector3Operation.Add;
      this.storeVector3Result = (FsmVector3) null;
      this.storeFloatResult = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoVector3Operator();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoVector3Operator();

    private void DoVector3Operator()
    {
      Vector3 vector3_1 = this.vector1.Value;
      Vector3 vector3_2 = this.vector2.Value;
      switch (this.operation)
      {
        case Vector3Operator.Vector3Operation.DotProduct:
          this.storeFloatResult.Value = Vector3.Dot(vector3_1, vector3_2);
          break;
        case Vector3Operator.Vector3Operation.CrossProduct:
          this.storeVector3Result.Value = Vector3.Cross(vector3_1, vector3_2);
          break;
        case Vector3Operator.Vector3Operation.Distance:
          this.storeFloatResult.Value = Vector3.Distance(vector3_1, vector3_2);
          break;
        case Vector3Operator.Vector3Operation.Angle:
          this.storeFloatResult.Value = Vector3.Angle(vector3_1, vector3_2);
          break;
        case Vector3Operator.Vector3Operation.Project:
          this.storeVector3Result.Value = Vector3.Project(vector3_1, vector3_2);
          break;
        case Vector3Operator.Vector3Operation.Reflect:
          this.storeVector3Result.Value = Vector3.Reflect(vector3_1, vector3_2);
          break;
        case Vector3Operator.Vector3Operation.Add:
          this.storeVector3Result.Value = vector3_1 + vector3_2;
          break;
        case Vector3Operator.Vector3Operation.Subtract:
          this.storeVector3Result.Value = vector3_1 - vector3_2;
          break;
        case Vector3Operator.Vector3Operation.Multiply:
          this.storeVector3Result.Value = Vector3.zero with
          {
            x = vector3_1.x * vector3_2.x,
            y = vector3_1.y * vector3_2.y,
            z = vector3_1.z * vector3_2.z
          };
          break;
        case Vector3Operator.Vector3Operation.Divide:
          this.storeVector3Result.Value = Vector3.zero with
          {
            x = vector3_1.x / vector3_2.x,
            y = vector3_1.y / vector3_2.y,
            z = vector3_1.z / vector3_2.z
          };
          break;
        case Vector3Operator.Vector3Operation.Min:
          this.storeVector3Result.Value = Vector3.Min(vector3_1, vector3_2);
          break;
        case Vector3Operator.Vector3Operation.Max:
          this.storeVector3Result.Value = Vector3.Max(vector3_1, vector3_2);
          break;
      }
    }

    public enum Vector3Operation
    {
      DotProduct,
      CrossProduct,
      Distance,
      Angle,
      Project,
      Reflect,
      Add,
      Subtract,
      Multiply,
      Divide,
      Min,
      Max,
    }
  }
}
