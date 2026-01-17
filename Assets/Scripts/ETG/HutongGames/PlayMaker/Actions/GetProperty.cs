// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetProperty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Gets the value of any public property or field on the targeted Unity Object and stores it in a variable. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
[ActionTarget(typeof (GameObject), "targetProperty", false)]
[ActionCategory(ActionCategory.UnityObject)]
[ActionTarget(typeof (Component), "targetProperty", false)]
public class GetProperty : FsmStateAction
{
  public FsmProperty targetProperty;
  public bool everyFrame;

  public override void Reset()
  {
    this.targetProperty = new FsmProperty()
    {
      setProperty = false
    };
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.targetProperty.GetValue();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.targetProperty.GetValue();
}
