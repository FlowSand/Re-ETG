using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Adds a XY values to Vector2 Variable.")]
    [ActionCategory(ActionCategory.Vector2)]
    public class Vector2AddXY : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The vector2 target")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmVector2 vector2Variable;
        [HutongGames.PlayMaker.Tooltip("The x component to add")]
        public FsmFloat addX;
        [HutongGames.PlayMaker.Tooltip("The y component to add")]
        public FsmFloat addY;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Add the value on a per second bases.")]
        public bool perSecond;

        public override void Reset()
        {
            this.vector2Variable = (FsmVector2) null;
            this.addX = (FsmFloat) 0.0f;
            this.addY = (FsmFloat) 0.0f;
            this.everyFrame = false;
            this.perSecond = false;
        }

        public override void OnEnter()
        {
            this.DoVector2AddXYZ();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoVector2AddXYZ();

        private void DoVector2AddXYZ()
        {
            Vector2 vector2 = new Vector2(this.addX.Value, this.addY.Value);
            if (this.perSecond)
                this.vector2Variable.Value += vector2 * Time.deltaTime;
            else
                this.vector2Variable.Value += vector2;
        }
    }
}
