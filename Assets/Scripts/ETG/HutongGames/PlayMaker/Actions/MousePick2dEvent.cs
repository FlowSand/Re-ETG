using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sends Events based on mouse interactions with a 2d Game Object: MouseOver, MouseDown, MouseUp, MouseOff.")]
    [ActionCategory(ActionCategory.Input)]
    public class MousePick2dEvent : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject with a Collider2D attached.")]
        [CheckForComponent(typeof (Collider2D))]
        public FsmOwnerDefault GameObject;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is over the GameObject.")]
        public FsmEvent mouseOver;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is pressed while over the GameObject.")]
        public FsmEvent mouseDown;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is released while over the GameObject.")]
        public FsmEvent mouseUp;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse moves off the GameObject.")]
        public FsmEvent mouseOff;
        [UIHint(UIHint.Layer)]
        [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.GameObject = (FsmOwnerDefault) null;
            this.mouseOver = (FsmEvent) null;
            this.mouseDown = (FsmEvent) null;
            this.mouseUp = (FsmEvent) null;
            this.mouseOff = (FsmEvent) null;
            this.layerMask = new FsmInt[0];
            this.invertMask = (FsmBool) false;
            this.everyFrame = true;
        }

        public override void OnEnter()
        {
            this.DoMousePickEvent();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoMousePickEvent();

        private void DoMousePickEvent()
        {
            if (this.DoRaycast())
            {
                if (this.mouseDown != null && Input.GetMouseButtonDown(0))
                    this.Fsm.Event(this.mouseDown);
                if (this.mouseOver != null)
                    this.Fsm.Event(this.mouseOver);
                if (this.mouseUp == null || !Input.GetMouseButtonUp(0))
                    return;
                this.Fsm.Event(this.mouseUp);
            }
            else
            {
                if (this.mouseOff == null)
                    return;
                this.Fsm.Event(this.mouseOff);
            }
        }

        private bool DoRaycast()
        {
            UnityEngine.GameObject gameObject = this.GameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.GameObject.GameObject.Value : this.Owner;
            RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            Fsm.RecordLastRaycastHit2DInfo(this.Fsm, rayIntersection);
            return (Object) rayIntersection.transform != (Object) null && (Object) rayIntersection.transform.gameObject == (Object) gameObject;
        }
    }
}
