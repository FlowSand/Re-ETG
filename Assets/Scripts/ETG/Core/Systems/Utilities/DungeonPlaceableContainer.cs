using UnityEngine;

using Dungeonator;

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

