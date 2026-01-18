using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [HutongGames.PlayMaker.Tooltip("Rigid bodies 2D start sleeping when they come to rest. This action wakes up all rigid bodies 2D in the scene. E.g., if you Set Gravity 2D and want objects at rest to respond.")]
    public class WakeAllRigidBodies2d : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Note: This would be very expensive!")]
        public bool everyFrame;

        public override void Reset() => this.everyFrame = false;

        public override void OnEnter()
        {
            this.DoWakeAll();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoWakeAll();

        private void DoWakeAll()
        {
            if (!(Object.FindObjectsOfType(typeof (Rigidbody2D)) is Rigidbody2D[] objectsOfType))
                return;
            for (int index = 0; index < objectsOfType.Length; ++index)
                objectsOfType[index].WakeUp();
        }
    }
}
