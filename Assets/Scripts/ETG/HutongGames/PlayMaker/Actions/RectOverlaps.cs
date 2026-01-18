using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Tests if 2 Rects overlap.")]
    [ActionCategory(ActionCategory.Rect)]
    public class RectOverlaps : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("First Rectangle.")]
        public FsmRect rect1;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Second Rectangle.")]
        public FsmRect rect2;
        [HutongGames.PlayMaker.Tooltip("Event to send if the Rects overlap.")]
        public FsmEvent trueEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send if the Rects do not overlap.")]
        public FsmEvent falseEvent;
        [HutongGames.PlayMaker.Tooltip("Store the result in a variable.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            FsmRect fsmRect1 = new FsmRect();
            fsmRect1.UseVariable = true;
            this.rect1 = fsmRect1;
            FsmRect fsmRect2 = new FsmRect();
            fsmRect2.UseVariable = true;
            this.rect2 = fsmRect2;
            this.storeResult = (FsmBool) null;
            this.trueEvent = (FsmEvent) null;
            this.falseEvent = (FsmEvent) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoRectOverlap();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoRectOverlap();

        private void DoRectOverlap()
        {
            if (this.rect1.IsNone || this.rect2.IsNone)
                return;
            bool flag = RectOverlaps.Intersect(this.rect1.Value, this.rect2.Value);
            this.storeResult.Value = flag;
            this.Fsm.Event(!flag ? this.falseEvent : this.trueEvent);
        }

        public static bool Intersect(Rect a, Rect b)
        {
            RectOverlaps.FlipNegative(ref a);
            RectOverlaps.FlipNegative(ref b);
            bool flag1 = (double) a.xMin < (double) b.xMax;
            bool flag2 = (double) a.xMax > (double) b.xMin;
            bool flag3 = (double) a.yMin < (double) b.yMax;
            bool flag4 = (double) a.yMax > (double) b.yMin;
            return flag1 && flag2 && flag3 && flag4;
        }

        public static void FlipNegative(ref Rect r)
        {
            if ((double) r.width < 0.0)
                r.x -= (r.width *= -1f);
            if ((double) r.height >= 0.0)
                return;
            r.y -= (r.height *= -1f);
        }
    }
}
