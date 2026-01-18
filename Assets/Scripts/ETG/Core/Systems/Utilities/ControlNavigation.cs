using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Control Navigation")]
public class ControlNavigation : MonoBehaviour
    {
        public bool FocusOnStart;
        public bool FocusOnMouseEnter;
        public dfControl SelectOnLeft;
        public dfControl SelectOnRight;
        public dfControl SelectOnUp;
        public dfControl SelectOnDown;
        public dfControl SelectOnTab;
        public dfControl SelectOnShiftTab;
        public dfControl SelectOnClick;

        private void OnMouseEnter(dfControl sender, dfMouseEventArgs args)
        {
            if (!this.FocusOnMouseEnter)
                return;
            dfControl component = this.GetComponent<dfControl>();
            if (!((Object) component != (Object) null))
                return;
            component.Focus(true);
        }

        private void OnClick(dfControl sender, dfMouseEventArgs args)
        {
            if (!((Object) this.SelectOnClick != (Object) null))
                return;
            this.SelectOnClick.Focus(true);
        }

        private void OnKeyDown(dfControl sender, dfKeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case KeyCode.Tab:
                    if (args.Shift)
                    {
                        if (!((Object) this.SelectOnShiftTab != (Object) null))
                            break;
                        this.SelectOnShiftTab.Focus(true);
                        args.Use();
                        break;
                    }
                    if (!((Object) this.SelectOnTab != (Object) null))
                        break;
                    this.SelectOnTab.Focus(true);
                    args.Use();
                    break;
                case KeyCode.UpArrow:
                    if (!((Object) this.SelectOnUp != (Object) null))
                        break;
                    this.SelectOnUp.Focus(true);
                    args.Use();
                    break;
                case KeyCode.DownArrow:
                    if (!((Object) this.SelectOnDown != (Object) null))
                        break;
                    this.SelectOnDown.Focus(true);
                    args.Use();
                    break;
                case KeyCode.RightArrow:
                    if (!((Object) this.SelectOnRight != (Object) null))
                        break;
                    this.SelectOnRight.Focus(true);
                    args.Use();
                    break;
                case KeyCode.LeftArrow:
                    if (!((Object) this.SelectOnLeft != (Object) null))
                        break;
                    this.SelectOnLeft.Focus(true);
                    args.Use();
                    break;
            }
        }

        private void Awake()
        {
        }

        private void OnEnable()
        {
        }

        private void Start()
        {
            if (!this.FocusOnStart)
                return;
            dfControl component = this.GetComponent<dfControl>();
            if (!((Object) component != (Object) null))
                return;
            component.Focus(true);
        }
    }

