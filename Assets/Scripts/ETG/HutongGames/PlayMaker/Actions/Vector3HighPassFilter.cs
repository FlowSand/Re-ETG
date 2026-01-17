// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vector3HighPassFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Use a high pass filter to isolate sudden changes in a Vector3 Variable. Useful when working with Get Device Acceleration to remove the constant effect of gravity.")]
[ActionCategory(ActionCategory.Vector3)]
public class Vector3HighPassFilter : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Vector3 Variable to filter. Should generally come from some constantly updated input, e.g., acceleration.")]
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmVector3 vector3Variable;
  [HutongGames.PlayMaker.Tooltip("Determines how much influence new changes have.")]
  public FsmFloat filteringFactor;
  private Vector3 filteredVector;

  public override void Reset()
  {
    this.vector3Variable = (FsmVector3) null;
    this.filteringFactor = (FsmFloat) 0.1f;
  }

  public override void OnEnter()
  {
    this.filteredVector = new Vector3(this.vector3Variable.Value.x, this.vector3Variable.Value.y, this.vector3Variable.Value.z);
  }

  public override void OnUpdate()
  {
    this.filteredVector.x = this.vector3Variable.Value.x - (float) ((double) this.vector3Variable.Value.x * (double) this.filteringFactor.Value + (double) this.filteredVector.x * (1.0 - (double) this.filteringFactor.Value));
    this.filteredVector.y = this.vector3Variable.Value.y - (float) ((double) this.vector3Variable.Value.y * (double) this.filteringFactor.Value + (double) this.filteredVector.y * (1.0 - (double) this.filteringFactor.Value));
    this.filteredVector.z = this.vector3Variable.Value.z - (float) ((double) this.vector3Variable.Value.z * (double) this.filteringFactor.Value + (double) this.filteredVector.z * (1.0 - (double) this.filteringFactor.Value));
    this.vector3Variable.Value = new Vector3(this.filteredVector.x, this.filteredVector.y, this.filteredVector.z);
  }
}
