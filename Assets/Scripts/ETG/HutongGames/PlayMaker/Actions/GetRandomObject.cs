using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [HutongGames.PlayMaker.Tooltip("Gets a Random Game Object from the scene.\nOptionally filter by Tag.")]
    public class GetRandomObject : FsmStateAction
    {
        [UIHint(UIHint.Tag)]
        public FsmString withTag;
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmGameObject storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.withTag = (FsmString) "Untagged";
            this.storeResult = (FsmGameObject) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetRandomObject();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetRandomObject();

        private void DoGetRandomObject()
        {
            GameObject[] gameObjectArray = !(this.withTag.Value != "Untagged") ? (GameObject[]) Object.FindObjectsOfType(typeof (GameObject)) : GameObject.FindGameObjectsWithTag(this.withTag.Value);
            if (gameObjectArray.Length > 0)
                this.storeResult.Value = gameObjectArray[Random.Range(0, gameObjectArray.Length)];
            else
                this.storeResult.Value = (GameObject) null;
        }
    }
}
