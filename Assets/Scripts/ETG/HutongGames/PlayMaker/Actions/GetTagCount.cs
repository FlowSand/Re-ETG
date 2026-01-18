using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Gets the number of Game Objects in the scene with the specified Tag.")]
    [ActionCategory(ActionCategory.GameObject)]
    public class GetTagCount : FsmStateAction
    {
        [UIHint(UIHint.Tag)]
        public FsmString tag;
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmInt storeResult;

        public override void Reset()
        {
            this.tag = (FsmString) "Untagged";
            this.storeResult = (FsmInt) null;
        }

        public override void OnEnter()
        {
            GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag(this.tag.Value);
            if (this.storeResult != null)
                this.storeResult.Value = gameObjectsWithTag == null ? 0 : gameObjectsWithTag.Length;
            this.Finish();
        }
    }
}
