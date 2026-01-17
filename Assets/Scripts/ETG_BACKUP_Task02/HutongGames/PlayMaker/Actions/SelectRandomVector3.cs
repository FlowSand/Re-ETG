// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SelectRandomVector3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Select a Random Vector3 from a Vector3 array.")]
[ActionCategory(ActionCategory.Vector3)]
public class SelectRandomVector3 : FsmStateAction
{
  [CompoundArray("Vectors", "Vector", "Weight")]
  public FsmVector3[] vector3Array;
  [HasFloatSlider(0.0f, 1f)]
  public FsmFloat[] weights;
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmVector3 storeVector3;

  public override void Reset()
  {
    this.vector3Array = new FsmVector3[3];
    this.weights = new FsmFloat[3]
    {
      (FsmFloat) 1f,
      (FsmFloat) 1f,
      (FsmFloat) 1f
    };
    this.storeVector3 = (FsmVector3) null;
  }

  public override void OnEnter()
  {
    this.DoSelectRandomColor();
    this.Finish();
  }

  private void DoSelectRandomColor()
  {
    if (this.vector3Array == null || this.vector3Array.Length == 0 || this.storeVector3 == null)
      return;
    int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
    if (randomWeightedIndex == -1)
      return;
    this.storeVector3.Value = this.vector3Array[randomWeightedIndex].Value;
  }
}
