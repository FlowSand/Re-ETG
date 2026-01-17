// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetLightType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
