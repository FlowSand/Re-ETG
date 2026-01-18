using System;

using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/UI/tk2dUITextInput")]
public class tk2dUITextInput : MonoBehaviour
    {
        public tk2dUIItem selectionBtn;
        public tk2dTextMesh inputLabel;
        public tk2dTextMesh emptyDisplayLabel;
        public GameObject unSelectedStateGO;
        public GameObject selectedStateGO;
        public GameObject cursor;
        public float fieldLength = 1f;
        public int maxCharacterLength = 30;
        public string emptyDisplayText;
        public bool isPasswordField;
        public string passwordChar = "*";
        [HideInInspector]
        [SerializeField]
        private tk2dUILayout layoutItem;
        private bool isSelected;
        private bool wasStartedCalled;
        private bool wasOnAnyPressEventAttached;
        private bool listenForKeyboardText;
        private bool isDisplayTextShown;
        public Action<tk2dUITextInput> OnTextChange;
        public string SendMessageOnTextChangeMethodName = string.Empty;
        private string text = string.Empty;

        public tk2dUILayout LayoutItem
        {
            get => this.layoutItem;
            set
            {
                if (!((UnityEngine.Object) this.layoutItem != (UnityEngine.Object) value))
                    return;
                if ((UnityEngine.Object) this.layoutItem != (UnityEngine.Object) null)
                    this.layoutItem.OnReshape -= new Action<Vector3, Vector3>(this.LayoutReshaped);
                this.layoutItem = value;
                if (!((UnityEngine.Object) this.layoutItem != (UnityEngine.Object) null))
                    return;
                this.layoutItem.OnReshape += new Action<Vector3, Vector3>(this.LayoutReshaped);
            }
        }

        public GameObject SendMessageTarget
        {
            get
            {
                return (UnityEngine.Object) this.selectionBtn != (UnityEngine.Object) null ? this.selectionBtn.sendMessageTarget : (GameObject) null;
            }
            set
            {
                if (!((UnityEngine.Object) this.selectionBtn != (UnityEngine.Object) null) || !((UnityEngine.Object) this.selectionBtn.sendMessageTarget != (UnityEngine.Object) value))
                    return;
                this.selectionBtn.sendMessageTarget = value;
            }
        }

        public bool IsFocus => this.isSelected;

        public string Text
        {
            get => this.text;
            set
            {
                if (!(this.text != value))
                    return;
                this.text = value;
                if (this.text.Length > this.maxCharacterLength)
                    this.text = this.text.Substring(0, this.maxCharacterLength);
                this.FormatTextForDisplay(this.text);
                if (!this.isSelected)
                    return;
                this.SetCursorPosition();
            }
        }

        private void Awake()
        {
            this.SetState();
            this.ShowDisplayText();
        }

        private void Start()
        {
            this.wasStartedCalled = true;
            if ((UnityEngine.Object) tk2dUIManager.Instance__NoCreate != (UnityEngine.Object) null)
                tk2dUIManager.Instance.OnAnyPress += new System.Action(this.AnyPress);
            this.wasOnAnyPressEventAttached = true;
        }

        private void OnEnable()
        {
            if (this.wasStartedCalled && !this.wasOnAnyPressEventAttached && (UnityEngine.Object) tk2dUIManager.Instance__NoCreate != (UnityEngine.Object) null)
                tk2dUIManager.Instance.OnAnyPress += new System.Action(this.AnyPress);
            if ((UnityEngine.Object) this.layoutItem != (UnityEngine.Object) null)
                this.layoutItem.OnReshape += new Action<Vector3, Vector3>(this.LayoutReshaped);
            this.selectionBtn.OnClick += new System.Action(this.InputSelected);
        }

        private void OnDisable()
        {
            if ((UnityEngine.Object) tk2dUIManager.Instance__NoCreate != (UnityEngine.Object) null)
            {
                tk2dUIManager.Instance.OnAnyPress -= new System.Action(this.AnyPress);
                if (this.listenForKeyboardText)
                    tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.ListenForKeyboardTextUpdate);
            }
            this.wasOnAnyPressEventAttached = false;
            this.selectionBtn.OnClick -= new System.Action(this.InputSelected);
            this.listenForKeyboardText = false;
            if (!((UnityEngine.Object) this.layoutItem != (UnityEngine.Object) null))
                return;
            this.layoutItem.OnReshape -= new Action<Vector3, Vector3>(this.LayoutReshaped);
        }

        public void SetFocus() => this.SetFocus(true);

        public void SetFocus(bool focus)
        {
            if (!this.IsFocus && focus)
            {
                this.InputSelected();
            }
            else
            {
                if (!this.IsFocus || focus)
                    return;
                this.InputDeselected();
            }
        }

        private void FormatTextForDisplay(string modifiedText)
        {
            if (this.isPasswordField)
            {
                int length = modifiedText.Length;
                char paddingChar = this.passwordChar.Length <= 0 ? '*' : this.passwordChar[0];
                modifiedText = string.Empty;
                modifiedText = modifiedText.PadRight(length, paddingChar);
            }
            this.inputLabel.text = modifiedText;
            this.inputLabel.Commit();
            while ((double) this.inputLabel.GetComponent<Renderer>().bounds.extents.x * 2.0 > (double) this.fieldLength)
            {
                modifiedText = modifiedText.Substring(1, modifiedText.Length - 1);
                this.inputLabel.text = modifiedText;
                this.inputLabel.Commit();
            }
            if (modifiedText.Length == 0 && !this.listenForKeyboardText)
                this.ShowDisplayText();
            else
                this.HideDisplayText();
        }

        private void ListenForKeyboardTextUpdate()
        {
            bool flag = false;
            string str = this.text;
            foreach (char ch in Input.inputString)
            {
                if ((int) ch == (int) "\b"[0])
                {
                    if (this.text.Length != 0)
                    {
                        str = this.text.Substring(0, this.text.Length - 1);
                        flag = true;
                    }
                }
                else if ((int) ch != (int) "\n"[0] && (int) ch != (int) "\r"[0] && ch != '\t' && ch != '')
                {
                    str += (string) (object) ch;
                    flag = true;
                }
            }
            if (!flag)
                return;
            this.Text = str;
            if (this.OnTextChange != null)
                this.OnTextChange(this);
            if (!((UnityEngine.Object) this.SendMessageTarget != (UnityEngine.Object) null) || this.SendMessageOnTextChangeMethodName.Length <= 0)
                return;
            this.SendMessageTarget.SendMessage(this.SendMessageOnTextChangeMethodName, (object) this, SendMessageOptions.RequireReceiver);
        }

        private void InputSelected()
        {
            if (this.text.Length == 0)
                this.HideDisplayText();
            this.isSelected = true;
            if (!this.listenForKeyboardText)
                tk2dUIManager.Instance.OnInputUpdate += new System.Action(this.ListenForKeyboardTextUpdate);
            this.listenForKeyboardText = true;
            this.SetState();
            this.SetCursorPosition();
        }

        private void InputDeselected()
        {
            if (this.text.Length == 0)
                this.ShowDisplayText();
            this.isSelected = false;
            if (this.listenForKeyboardText)
                tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.ListenForKeyboardTextUpdate);
            this.listenForKeyboardText = false;
            this.SetState();
        }

        private void AnyPress()
        {
            if (!this.isSelected || !((UnityEngine.Object) tk2dUIManager.Instance.PressedUIItem != (UnityEngine.Object) this.selectionBtn))
                return;
            this.InputDeselected();
        }

        private void SetState()
        {
            tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.unSelectedStateGO, !this.isSelected);
            tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.selectedStateGO, this.isSelected);
            tk2dUIBaseItemControl.ChangeGameObjectActiveState(this.cursor, this.isSelected);
        }

        private void SetCursorPosition()
        {
            float num1 = 1f;
            float num2 = 1f / 500f;
            if (this.inputLabel.anchor == TextAnchor.MiddleLeft || this.inputLabel.anchor == TextAnchor.LowerLeft || this.inputLabel.anchor == TextAnchor.UpperLeft)
                num1 = 2f;
            else if (this.inputLabel.anchor == TextAnchor.MiddleRight || this.inputLabel.anchor == TextAnchor.LowerRight || this.inputLabel.anchor == TextAnchor.UpperRight)
            {
                num1 = -2f;
                num2 = 0.012f;
            }
            if (this.text.EndsWith(" "))
            {
                tk2dFontChar tk2dFontChar = !this.inputLabel.font.useDictionary ? this.inputLabel.font.chars[32 /*0x20*/] : this.inputLabel.font.charDict[32 /*0x20*/];
                num2 += (float) ((double) tk2dFontChar.advance * (double) this.inputLabel.scale.x / 2.0);
            }
            this.cursor.transform.localPosition = new Vector3(this.inputLabel.transform.localPosition.x + (this.inputLabel.GetComponent<Renderer>().bounds.extents.x + num2) * num1, this.cursor.transform.localPosition.y, this.cursor.transform.localPosition.z);
        }

        private void ShowDisplayText()
        {
            if (this.isDisplayTextShown)
                return;
            this.isDisplayTextShown = true;
            if ((UnityEngine.Object) this.emptyDisplayLabel != (UnityEngine.Object) null)
            {
                this.emptyDisplayLabel.text = this.emptyDisplayText;
                this.emptyDisplayLabel.Commit();
                tk2dUIBaseItemControl.ChangeGameObjectActiveState(this.emptyDisplayLabel.gameObject, true);
            }
            tk2dUIBaseItemControl.ChangeGameObjectActiveState(this.inputLabel.gameObject, false);
        }

        private void HideDisplayText()
        {
            if (!this.isDisplayTextShown)
                return;
            this.isDisplayTextShown = false;
            tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.emptyDisplayLabel.gameObject, false);
            tk2dUIBaseItemControl.ChangeGameObjectActiveState(this.inputLabel.gameObject, true);
        }

        private void LayoutReshaped(Vector3 dMin, Vector3 dMax)
        {
            this.fieldLength += dMax.x - dMin.x;
            string text = this.text;
            this.text = string.Empty;
            this.Text = text;
        }
    }

