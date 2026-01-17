// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetProperty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionTarget(typeof (Component), "targetProperty", false)]
[ActionCategory(ActionCategory.UnityObject)]
[ActionTarget(typeof (GameObject), "targetProperty", false)]
[HutongGames.PlayMaker.Tooltip("Sets the value of any public property or field on the targeted Unity Object. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
public class SetProperty : FsmStateAction
{
  public FsmProperty targetProperty;
  public bool everyFrame;

  public override void Reset()
  {
    this.targetProperty = new FsmProperty()
    {
      setProperty = true
    };
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.targetProperty.SetValue();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.targetProperty.SetValue();
}
