// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ToggleAllSimpleTurrets
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

public class ToggleAllSimpleTurrets : FsmStateAction
{
  public FsmBool toggle;

  public override void Reset() => this.toggle = (FsmBool) false;

  public override void OnEnter()
  {
    List<SimpleTurretController> componentsAbsoluteInRoom = this.Owner.GetComponent<TalkDoerLite>().ParentRoom.GetComponentsAbsoluteInRoom<SimpleTurretController>();
    for (int index = 0; index < componentsAbsoluteInRoom.Count; ++index)
    {
      if (this.toggle.Value)
        componentsAbsoluteInRoom[index].ActivateManual();
      else
        componentsAbsoluteInRoom[index].DeactivateManual();
    }
    this.Finish();
  }
}
