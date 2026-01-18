using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Lights)]
  [HutongGames.PlayMaker.Tooltip("Set Spot, Directional, or Point Light type.")]
  public class SetLightType : ComponentAction<Light>
  {
    [CheckForComponent(typeof (Light))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [ObjectType(typeof (LightType))]
    public FsmEnum lightType;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.lightType = (FsmEnum) (Enum) LightType.Point;
    }

    public override void OnEnter()
    {
      this.DoSetLightType();
      this.Finish();
    }

    private void DoSetLightType()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.light.type = (LightType) this.lightType.Value;
    }
  }
}
