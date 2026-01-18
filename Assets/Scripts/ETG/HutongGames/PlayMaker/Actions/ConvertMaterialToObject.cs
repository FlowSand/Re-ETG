using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Convert)]
    [HutongGames.PlayMaker.Tooltip("Converts a Material variable to an Object variable. Useful if you want to use Set Property (which only works on Object variables).")]
    public class ConvertMaterialToObject : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Material variable to convert to an Object.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmMaterial materialVariable;
        [HutongGames.PlayMaker.Tooltip("Store the result in an Object variable.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmObject objectVariable;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the Material variable is changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.materialVariable = (FsmMaterial) null;
            this.objectVariable = (FsmObject) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoConvertMaterialToObject();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoConvertMaterialToObject();

        private void DoConvertMaterialToObject()
        {
            this.objectVariable.Value = (Object) this.materialVariable.Value;
        }
    }
}
