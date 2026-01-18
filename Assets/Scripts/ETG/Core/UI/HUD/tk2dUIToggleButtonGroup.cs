using System;

using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/tk2dUIToggleButtonGroup")]
public class tk2dUIToggleButtonGroup : MonoBehaviour
    {
        [SerializeField]
        private tk2dUIToggleButton[] toggleBtns;
        public GameObject sendMessageTarget;
        [SerializeField]
        private int selectedIndex;
        private tk2dUIToggleButton selectedToggleButton;
        public string SendMessageOnChangeMethodName = string.Empty;

        public tk2dUIToggleButton[] ToggleBtns => this.toggleBtns;

        public int SelectedIndex
        {
            get => this.selectedIndex;
            set
            {
                if (this.selectedIndex == value)
                    return;
                this.selectedIndex = value;
                this.SetToggleButtonUsingSelectedIndex();
            }
        }

        public tk2dUIToggleButton SelectedToggleButton
        {
            get => this.selectedToggleButton;
            set => this.ButtonToggle(value);
        }

        public event Action<tk2dUIToggleButtonGroup> OnChange;

        protected virtual void Awake() => this.Setup();

        protected void Setup()
        {
            foreach (tk2dUIToggleButton toggleBtn in this.toggleBtns)
            {
                if ((UnityEngine.Object) toggleBtn != (UnityEngine.Object) null)
                {
                    toggleBtn.IsInToggleGroup = true;
                    toggleBtn.IsOn = false;
                    toggleBtn.OnToggle += new Action<tk2dUIToggleButton>(this.ButtonToggle);
                }
            }
            this.SetToggleButtonUsingSelectedIndex();
        }

        public void AddNewToggleButtons(tk2dUIToggleButton[] newToggleBtns)
        {
            this.ClearExistingToggleBtns();
            this.toggleBtns = newToggleBtns;
            this.Setup();
        }

        private void ClearExistingToggleBtns()
        {
            if (this.toggleBtns == null || this.toggleBtns.Length <= 0)
                return;
            foreach (tk2dUIToggleButton toggleBtn in this.toggleBtns)
            {
                toggleBtn.IsInToggleGroup = false;
                toggleBtn.OnToggle -= new Action<tk2dUIToggleButton>(this.ButtonToggle);
                toggleBtn.IsOn = false;
            }
        }

        private void SetToggleButtonUsingSelectedIndex()
        {
            if (this.selectedIndex >= 0 && this.selectedIndex < this.toggleBtns.Length)
            {
                this.toggleBtns[this.selectedIndex].IsOn = true;
            }
            else
            {
                tk2dUIToggleButton toggleButton = (tk2dUIToggleButton) null;
                this.selectedIndex = -1;
                this.ButtonToggle(toggleButton);
            }
        }

        private void ButtonToggle(tk2dUIToggleButton toggleButton)
        {
            if (!((UnityEngine.Object) toggleButton == (UnityEngine.Object) null) && !toggleButton.IsOn)
                return;
            foreach (tk2dUIToggleButton toggleBtn in this.toggleBtns)
            {
                if ((UnityEngine.Object) toggleBtn != (UnityEngine.Object) toggleButton)
                    toggleBtn.IsOn = false;
            }
            if (!((UnityEngine.Object) toggleButton != (UnityEngine.Object) this.selectedToggleButton))
                return;
            this.selectedToggleButton = toggleButton;
            this.SetSelectedIndexFromSelectedToggleButton();
            if (this.OnChange != null)
                this.OnChange(this);
            if (!((UnityEngine.Object) this.sendMessageTarget != (UnityEngine.Object) null) || this.SendMessageOnChangeMethodName.Length <= 0)
                return;
            this.sendMessageTarget.SendMessage(this.SendMessageOnChangeMethodName, (object) this, SendMessageOptions.RequireReceiver);
        }

        private void SetSelectedIndexFromSelectedToggleButton()
        {
            this.selectedIndex = -1;
            for (int index = 0; index < this.toggleBtns.Length; ++index)
            {
                if ((UnityEngine.Object) this.toggleBtns[index] == (UnityEngine.Object) this.selectedToggleButton)
                {
                    this.selectedIndex = index;
                    break;
                }
            }
        }
    }

