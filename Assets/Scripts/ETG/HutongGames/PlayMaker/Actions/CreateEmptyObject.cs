using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Creates a Game Object at a spawn point.\nUse a Game Object and/or Position/Rotation for the Spawn Point. If you specify a Game Object, Position is used as a local offset, and Rotation will override the object's rotation.")]
    [ActionCategory(ActionCategory.GameObject)]
    public class CreateEmptyObject : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Optional GameObject to create. Usually a Prefab.")]
        public FsmGameObject gameObject;
        [HutongGames.PlayMaker.Tooltip("Optional Spawn Point.")]
        public FsmGameObject spawnPoint;
        [HutongGames.PlayMaker.Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
        public FsmVector3 position;
        [HutongGames.PlayMaker.Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
        public FsmVector3 rotation;
        [HutongGames.PlayMaker.Tooltip("Optionally store the created object.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeObject;

        public override void Reset()
        {
            this.gameObject = (FsmGameObject) null;
            this.spawnPoint = (FsmGameObject) null;
            FsmVector3 fsmVector3_1 = new FsmVector3();
            fsmVector3_1.UseVariable = true;
            this.position = fsmVector3_1;
            FsmVector3 fsmVector3_2 = new FsmVector3();
            fsmVector3_2.UseVariable = true;
            this.rotation = fsmVector3_2;
            this.storeObject = (FsmGameObject) null;
        }

        public override void OnEnter()
        {
            GameObject original = this.gameObject.Value;
            Vector3 vector3_1 = Vector3.zero;
            Vector3 vector3_2 = Vector3.zero;
            if ((Object) this.spawnPoint.Value != (Object) null)
            {
                vector3_1 = this.spawnPoint.Value.transform.position;
                if (!this.position.IsNone)
                    vector3_1 += this.position.Value;
                vector3_2 = this.rotation.IsNone ? this.spawnPoint.Value.transform.eulerAngles : this.rotation.Value;
            }
            else
            {
                if (!this.position.IsNone)
                    vector3_1 = this.position.Value;
                if (!this.rotation.IsNone)
                    vector3_2 = this.rotation.Value;
            }
            GameObject gameObject1 = this.storeObject.Value;
            GameObject gameObject2;
            if ((Object) original != (Object) null)
            {
                gameObject2 = Object.Instantiate<GameObject>(original);
                this.storeObject.Value = gameObject2;
            }
            else
            {
                gameObject2 = new GameObject("EmptyObjectFromNull");
                this.storeObject.Value = gameObject2;
            }
            if ((Object) gameObject2 != (Object) null)
            {
                gameObject2.transform.position = vector3_1;
                gameObject2.transform.eulerAngles = vector3_2;
            }
            this.Finish();
        }
    }
}
