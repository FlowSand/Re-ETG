// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetMainCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Camera)]
[HutongGames.PlayMaker.Tooltip("Gets the GameObject tagged MainCamera from the scene")]
[ActionTarget(typeof (Camera), "storeGameObject", false)]
public class GetMainCamera : FsmStateAction
{
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmGameObject storeGameObject;

  public override void Reset() => this.storeGameObject = (FsmGameObject) null;

  public override void OnEnter()
  {
    this.storeGameObject.Value = !((Object) Camera.main != (Object) null) ? (GameObject) null : Camera.main.gameObject;
    this.Finish();
  }
}
