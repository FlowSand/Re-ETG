// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetName
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Gets the name of a Game Object and stores it in a String Variable.")]
[ActionCategory(ActionCategory.GameObject)]
public class GetName : FsmStateAction
{
  [RequiredField]
  public FsmGameObject gameObject;
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmString storeName;
  public bool everyFrame;

  public override void Reset()
  {
    FsmGameObject fsmGameObject = new FsmGameObject();
    fsmGameObject.UseVariable = true;
    this.gameObject = fsmGameObject;
    this.storeName = (FsmString) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoGetGameObjectName();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetGameObjectName();

  private void DoGetGameObjectName()
  {
    GameObject gameObject = this.gameObject.Value;
    this.storeName.Value = !((Object) gameObject != (Object) null) ? string.Empty : gameObject.name;
  }
}
