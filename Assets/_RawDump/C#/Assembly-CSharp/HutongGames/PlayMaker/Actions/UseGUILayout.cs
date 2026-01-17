// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.UseGUILayout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Turn GUILayout on/off. If you don't use GUILayout actions you can get some performace back by turning GUILayout off. This can make a difference on iOS platforms.")]
[ActionCategory(ActionCategory.GUILayout)]
public class UseGUILayout : FsmStateAction
{
  [RequiredField]
  public bool turnOffGUIlayout;

  public override void Reset() => this.turnOffGUIlayout = true;

  public override void OnEnter()
  {
    this.Fsm.Owner.useGUILayout = !this.turnOffGUIlayout;
    this.Finish();
  }
}
