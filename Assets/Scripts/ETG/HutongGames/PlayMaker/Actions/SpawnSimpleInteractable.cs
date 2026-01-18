using UnityEngine;

using Dungeonator;

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
