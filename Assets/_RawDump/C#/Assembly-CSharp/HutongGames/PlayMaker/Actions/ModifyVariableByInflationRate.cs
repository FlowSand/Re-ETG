// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ModifyVariableByInflationRate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

public class ModifyVariableByInflationRate : FsmStateAction
{
  public FsmInt TargetVariable;
  public FsmFloat AdditionalMultiplier = (FsmFloat) 1f;

  public override void Reset()
  {
  }

  public override string ErrorCheck() => string.Empty;

  public override void OnEnter()
  {
    GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
    this.TargetVariable.Value = Mathf.RoundToInt((float) this.TargetVariable.Value * (loadedLevelDefinition == null ? 1f : loadedLevelDefinition.priceMultiplier) * this.AdditionalMultiplier.Value);
    if ((bool) (Object) this.Owner)
    {
      RoomHandler absoluteRoom = this.Owner.transform.position.GetAbsoluteRoom();
      if (absoluteRoom != null && absoluteRoom.connectedRooms != null && absoluteRoom.connectedRooms.Count == 1 && absoluteRoom.connectedRooms[0].area.PrototypeRoomName.Contains("Black Market"))
        this.TargetVariable.Value = Mathf.RoundToInt((float) this.TargetVariable.Value * 0.5f);
    }
    this.Finish();
  }
}
