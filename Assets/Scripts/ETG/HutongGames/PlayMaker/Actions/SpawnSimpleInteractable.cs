// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SpawnSimpleInteractable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Events)]
  public class SpawnSimpleInteractable : FsmStateAction
  {
    public GameObject ThingToSpawn;

    public override void Reset()
    {
    }

    public override void OnEnter()
    {
      GameObject gObj = Object.Instantiate<GameObject>(this.ThingToSpawn, this.Owner.transform.position, Quaternion.identity);
      IPlayerInteractable[] interfaces1 = gObj.GetInterfaces<IPlayerInteractable>();
      IPlaceConfigurable[] interfaces2 = gObj.GetInterfaces<IPlaceConfigurable>();
      RoomHandler absoluteRoom = this.Owner.transform.position.GetAbsoluteRoom();
      for (int index = 0; index < interfaces1.Length; ++index)
        absoluteRoom.RegisterInteractable(interfaces1[index]);
      for (int index = 0; index < interfaces2.Length; ++index)
        interfaces2[index].ConfigureOnPlacement(absoluteRoom);
      this.Finish();
    }
  }
}
