using System;

using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/tk2dUIToggleButton")]
public class tk2dUIToggleButton : tk2dUIBaseItemControl
    {
        public GameObject offStateGO;
        public GameObject onStateGO;
        public bool activateOnPress;
        [SerializeField]
        private bool isOn = true;
        private bool isInToggleGroup;
        public string SendMessageOnToggleMethodName = string.Empty;

        public event Action<tk2dUIToggleButton> OnToggle;

        public bool IsOn
        {
            get => this.isOn;
            set
            {
                if (this.isOn == value)
                    return;
                this.isOn = value;
                this.SetState();
                if (this.OnToggle == null)
                    return;
                this.OnToggle(this);
            }
        }

        public bool IsInToggleGroup
        {
            get => this.isInToggleGroup;
            set => this.isInToggleGroup = value;
        }

        private void Start() => this.SetState();

        private void OnEnable()
        {
            if (!(bool) (UnityEngine.Object) this.uiItem)
                return;
            this.uiItem.OnClick += new System.Action(this.ButtonClick);
            this.uiItem.OnDown += new System.Action(this.ButtonDown);
        }

        private void OnDisable()
        {
            if (!(bool) (UnityEngine.Object) this.uiItem)
                return;
            this.uiItem.OnClick -= new System.Action(this.ButtonClick);
            this.uiItem.OnDown -= new System.Action(this.ButtonDown);
        }

        private void ButtonClick()
        {
            if (this.activateOnPress)
                return;
            this.ButtonToggle();
        }

        private void ButtonDown()
        {
            if (!this.activateOnPress)
                return;
            this.ButtonToggle();
        }

        private void ButtonToggle()
        {
            if (this.isOn && this.isInToggleGroup)
                return;
            this.isOn = !this.isOn;
            this.SetState();
            if (this.OnToggle != null)
                this.OnToggle(this);
            this.DoSendMessage(this.SendMessageOnToggleMethodName, (object) this);
        }

        private void SetState()
        {
            tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.offStateGO, !this.isOn);
            tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.onStateGO, this.isOn);
        }
    }

