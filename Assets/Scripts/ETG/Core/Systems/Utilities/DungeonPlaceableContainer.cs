// Decompiled with JetBrains decompiler
// Type: DungeonPlaceableContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class DungeonPlaceableContainer : MonoBehaviour
  {
    public DungeonPlaceable placeable;

    private void Awake()
    {
      IntVector2 pos = this.transform.position.IntXY(VectorConversions.Floor);
      RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(pos);
      GameObject gObj = this.placeable.InstantiateObject(roomFromPosition, pos - roomFromPosition.area.basePosition);
      if (!((Object) gObj != (Object) null))
        return;
      foreach (IPlayerInteractable interfacesInChild in gObj.GetInterfacesInChildren<IPlayerInteractable>())
        roomFromPosition.RegisterInteractable(interfacesInChild);
      SurfaceDecorator component = gObj.GetComponent<SurfaceDecorator>();
      if (!((Object) component != (Object) null))
        return;
      component.Decorate(roomFromPosition);
    }
  }

