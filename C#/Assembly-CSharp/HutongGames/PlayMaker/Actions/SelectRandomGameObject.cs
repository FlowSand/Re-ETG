// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SelectRandomGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Selects a Random Game Object from an array of Game Objects.")]
[ActionCategory(ActionCategory.GameObject)]
public class SelectRandomGameObject : FsmStateAction
{
  [CompoundArray("Game Objects", "Game Object", "Weight")]
  public FsmGameObject[] gameObjects;
  [HasFloatSlider(0.0f, 1f)]
  public FsmFloat[] weights;
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmGameObject storeGameObject;

  public override void Reset()
  {
    this.gameObjects = new FsmGameObject[3];
    this.weights = new FsmFloat[3]
    {
      (FsmFloat) 1f,
      (FsmFloat) 1f,
      (FsmFloat) 1f
    };
    this.storeGameObject = (FsmGameObject) null;
  }

  public override void OnEnter()
  {
    this.DoSelectRandomGameObject();
    this.Finish();
  }

  private void DoSelectRandomGameObject()
  {
    if (this.gameObjects == null || this.gameObjects.Length == 0 || this.storeGameObject == null)
      return;
    int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
    if (randomWeightedIndex == -1)
      return;
    this.storeGameObject.Value = this.gameObjects[randomWeightedIndex].Value;
  }
}
