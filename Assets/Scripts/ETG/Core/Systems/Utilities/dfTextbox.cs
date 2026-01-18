using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;

#nullable disable

[dfCategory("Basic Controls")]
[dfTooltip("Implements a text entry control")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_textbox.html")]
[AddComponentMenu("Daikon Forge/User Interface/Textbox")]
[ExecuteInEditMode]
[Serializable]
public class dfTextbox : dfInteractiveBase, IDFMultiRender, IRendersText
    {
        [SerializeField]
        protected dfFontBase font;
        [SerializeField]
        protected bool acceptsTab;
        [SerializeField]
        protected bool displayAsPassword;
        [SerializeField]
        protected string passwordChar = "*";
        [SerializeField]
        protected bool readOnly;
        [SerializeField]
        protected string text = string.Empty;
        [SerializeField]
        protected Color32 textColor = (Color32) UnityEngine.Color.white;
        [SerializeField]
        protected Color32 selectionBackground = new Color32((byte) 0, (byte) 105, (byte) 210, byte.MaxValue);
        [SerializeField]
        protected Color32 cursorColor = (Color32) UnityEngine.Color.white;
        [SerializeField]
        protected string selectionSprite = string.Empty;
        [SerializeField]
        protected float textScale = 1f;
        [SerializeField]
        protected dfTextScaleMode textScaleMode;
        [SerializeField]
        protected RectOffset padding = new RectOffset();
        [SerializeField]
        protected float cursorBlinkTime = 0.45f;
        [SerializeField]
        protected int cursorWidth = 1;
        [SerializeField]
        protected int maxLength = 1024 /*0x0400*/;
        [SerializeField]
        protected bool selectOnFocus;
        [SerializeField]
        protected bool shadow;
        [SerializeField]
        protected Color32 shadowColor = (Color32) UnityEngine.Color.black;
        [SerializeField]
        protected Vector2 shadowOffset = new Vector2(1f, -1f);
        [SerializeField]
        protected bool useMobileKeyboard;
        [SerializeField]
        protected int mobileKeyboardType;
        [SerializeField]
        protected bool mobileAutoCorrect;
        [SerializeField]
        protected bool mobileHideInputField;
        [SerializeField]
        protected dfMobileKeyboardTrigger mobileKeyboardTrigger;
        [SerializeField]
        protected TextAlignment textAlign;
        private Vector2 startSize = Vector2.zero;
        private int selectionStart;
        private int selectionEnd;
        private int mouseSelectionAnchor;
        private int scrollIndex;
        private int cursorIndex;
        private float leftOffset;
        private bool cursorShown;
        private float[] charWidths;
        private float whenGotFocus;
        private string undoText = string.Empty;
        private float tripleClickTimer;
        private bool isFontCallbackAssigned;
        private dfRenderData textRenderData;
        private dfList<dfRenderData> buffers = dfList<dfRenderData>.Obtain();

        public event PropertyChangedEventHandler<bool> ReadOnlyChanged;

        public event PropertyChangedEventHandler<string> PasswordCharacterChanged;

        public event PropertyChangedEventHandler<string> TextChanged;

        public event PropertyChangedEventHandler<string> TextSubmitted;

        public event PropertyChangedEventHandler<string> TextCancelled;

        public dfFontBase Font
        {
            get
            {
                if ((UnityEngine.Object) this.font == (UnityEngine.Object) null)
                {
                    dfGUIManager manager = this.GetManager();
                    if ((UnityEngine.Object) manager != (UnityEngine.Object) null)
                        this.font = manager.DefaultFont;
                }
                return this.font;
            }
            set
            {
                if (!((UnityEngine.Object) value != (UnityEngine.Object) this.font))
                    return;
                this.unbindTextureRebuildCallback();
                this.font = value;
                this.bindTextureRebuildCallback();
                this.Invalidate();
            }
        }

        public int SelectionStart
        {
            get => this.selectionStart;
            set
            {
                if (value == this.selectionStart)
                    return;
                this.selectionStart = Mathf.Max(0, Mathf.Min(value, this.text.Length));
                this.selectionEnd = Mathf.Max(this.selectionEnd, this.selectionStart);
                this.Invalidate();
            }
        }

        public int SelectionEnd
        {
            get => this.selectionEnd;
            set
            {
                if (value == this.selectionEnd)
                    return;
                this.selectionEnd = Mathf.Max(0, Mathf.Min(value, this.text.Length));
                this.selectionStart = Mathf.Max(this.selectionStart, this.selectionEnd);
                this.Invalidate();
            }
        }

        public int SelectionLength => this.selectionEnd - this.selectionStart;

        public string SelectedText
        {
            get
            {
                return this.selectionEnd == this.selectionStart ? string.Empty : this.text.Substring(this.selectionStart, this.selectionEnd - this.selectionStart);
            }
        }

        public bool SelectOnFocus
        {
            get => this.selectOnFocus;
            set => this.selectOnFocus = value;
        }

        public RectOffset Padding
        {
            get
            {
                if (this.padding == null)
                    this.padding = new RectOffset();
                return this.padding;
            }
            set
            {
                value = value.ConstrainPadding();
                if (object.Equals((object) value, (object) this.padding))
                    return;
                this.padding = value;
                this.Invalidate();
            }
        }

        public bool IsPasswordField
        {
            get => this.displayAsPassword;
            set
            {
                if (value == this.displayAsPassword)
                    return;
                this.displayAsPassword = value;
                this.Invalidate();
            }
        }

        public string PasswordCharacter
        {
            get => this.passwordChar;
            set
            {
                this.passwordChar = string.IsNullOrEmpty(value) ? value : value[0].ToString();
                this.OnPasswordCharacterChanged();
                this.Invalidate();
            }
        }

        public float CursorBlinkTime
        {
            get => this.cursorBlinkTime;
            set => this.cursorBlinkTime = value;
        }

        public int CursorWidth
        {
            get => this.cursorWidth;
            set => this.cursorWidth = value;
        }

        public int CursorIndex
        {
            get => this.cursorIndex;
            set => this.setCursorPos(value);
        }

        public bool ReadOnly
        {
            get => this.readOnly;
            set
            {
                if (value == this.readOnly)
                    return;
                this.readOnly = value;
                this.OnReadOnlyChanged();
                this.Invalidate();
            }
        }

        public string Text
        {
            get => this.text;
            set
            {
                value = value ?? string.Empty;
                if (value.Length > this.MaxLength)
                    value = value.Substring(0, this.MaxLength);
                value = value.Replace("\t", " ");
                if (!(value != this.text))
                    return;
                this.text = value;
                this.scrollIndex = this.cursorIndex = 0;
                this.OnTextChanged();
                this.Invalidate();
            }
        }

        public Color32 TextColor
        {
            get => this.textColor;
            set
            {
                this.textColor = value;
                this.Invalidate();
            }
        }

        public string SelectionSprite
        {
            get => this.selectionSprite;
            set
            {
                if (!(value != this.selectionSprite))
                    return;
                this.selectionSprite = value;
                this.Invalidate();
            }
        }

        public Color32 SelectionBackgroundColor
        {
            get => this.selectionBackground;
            set
            {
                this.selectionBackground = value;
                this.Invalidate();
            }
        }

        public Color32 CursorColor
        {
            get => this.cursorColor;
            set
            {
                this.cursorColor = value;
                this.Invalidate();
            }
        }

        public float TextScale
        {
            get => this.textScale;
            set
            {
                value = Mathf.Max(0.1f, value);
                if (Mathf.Approximately(this.textScale, value))
                    return;
                dfFontManager.Invalidate(this.Font);
                this.textScale = value;
                this.Invalidate();
            }
        }

        public dfTextScaleMode TextScaleMode
        {
            get => this.textScaleMode;
            set
            {
                this.textScaleMode = value;
                this.Invalidate();
            }
        }

        public int MaxLength
        {
            get => this.maxLength;
            set
            {
                if (value == this.maxLength)
                    return;
                this.maxLength = Mathf.Max(0, value);
                if (this.maxLength < this.text.Length)
                    this.Text = this.text.Substring(0, this.maxLength);
                this.Invalidate();
            }
        }

        public TextAlignment TextAlignment
        {
            get => this.textAlign;
            set
            {
                if (value == this.textAlign)
                    return;
                this.textAlign = value;
                this.Invalidate();
            }
        }

        public bool Shadow
        {
            get => this.shadow;
            set
            {
                if (value == this.shadow)
                    return;
                this.shadow = value;
                this.Invalidate();
            }
        }

        public Color32 ShadowColor
        {
            get => this.shadowColor;
            set
            {
                if (value.Equals((object) this.shadowColor))
                    return;
                this.shadowColor = value;
                this.Invalidate();
            }
        }

        public Vector2 ShadowOffset
        {
            get => this.shadowOffset;
            set
            {
                if (!(value != this.shadowOffset))
                    return;
                this.shadowOffset = value;
                this.Invalidate();
            }
        }

        public bool UseMobileKeyboard
        {
            get => this.useMobileKeyboard;
            set => this.useMobileKeyboard = value;
        }

        public bool MobileAutoCorrect
        {
            get => this.mobileAutoCorrect;
            set => this.mobileAutoCorrect = value;
        }

        public bool HideMobileInputField
        {
            get => this.mobileHideInputField;
            set => this.mobileHideInputField = value;
        }

        public dfMobileKeyboardTrigger MobileKeyboardTrigger
        {
            get => this.mobileKeyboardTrigger;
            set => this.mobileKeyboardTrigger = value;
        }

        protected override void OnTabKeyPressed(dfKeyEventArgs args)
        {
            if (this.acceptsTab)
            {
                base.OnKeyPress(args);
                if (args.Used)
                    return;
                args.Character = '\t';
                this.processKeyPress(args);
            }
            else
                base.OnTabKeyPressed(args);
        }

        protected internal override void OnKeyPress(dfKeyEventArgs args)
        {
            if (this.ReadOnly || char.IsControl(args.Character))
            {
                base.OnKeyPress(args);
            }
            else
            {
                base.OnKeyPress(args);
                if (args.Used)
                    return;
                this.processKeyPress(args);
            }
        }

        private void processKeyPress(dfKeyEventArgs args)
        {
            this.DeleteSelection();
            if (this.text.Length < this.MaxLength)
            {
                if (this.cursorIndex == this.text.Length)
                    this.text += (string) (object) args.Character;
                else
                    this.text = this.text.Insert(this.cursorIndex, args.Character.ToString());
                ++this.cursorIndex;
                this.OnTextChanged();
                this.Invalidate();
            }
            args.Use();
        }

        protected internal override void OnKeyDown(dfKeyEventArgs args)
        {
            if (this.ReadOnly)
                return;
            base.OnKeyDown(args);
            if (args.Used)
                return;
            KeyCode keyCode = args.KeyCode;
            switch (keyCode)
            {
                case KeyCode.A:
                    if (args.Control)
                    {
                        this.SelectAll();
                        break;
                    }
                    break;
                case KeyCode.C:
                    if (args.Control)
                    {
                        this.CopySelectionToClipboard();
                        break;
                    }
                    break;
                case KeyCode.RightArrow:
                    if (args.Control)
                    {
                        if (args.Shift)
                        {
                            this.moveSelectionPointRightWord();
                            break;
                        }
                        this.MoveCursorToNextWord();
                        break;
                    }
                    if (args.Shift)
                    {
                        this.moveSelectionPointRight();
                        break;
                    }
                    this.MoveCursorToNextChar();
                    break;
                case KeyCode.LeftArrow:
                    if (args.Control)
                    {
                        if (args.Shift)
                        {
                            this.moveSelectionPointLeftWord();
                            break;
                        }
                        this.MoveCursorToPreviousWord();
                        break;
                    }
                    if (args.Shift)
                    {
                        this.moveSelectionPointLeft();
                        break;
                    }
                    this.MoveCursorToPreviousChar();
                    break;
                case KeyCode.Insert:
                    if (args.Shift)
                    {
                        string clipBoard = dfClipboardHelper.clipBoard;
                        if (!string.IsNullOrEmpty(clipBoard))
                        {
                            this.PasteAtCursor(clipBoard);
                            break;
                        }
                        break;
                    }
                    break;
                case KeyCode.Home:
                    if (args.Shift)
                    {
                        this.SelectToStart();
                        break;
                    }
                    this.MoveCursorToStart();
                    break;
                case KeyCode.End:
                    if (args.Shift)
                    {
                        this.SelectToEnd();
                        break;
                    }
                    this.MoveCursorToEnd();
                    break;
                default:
                    switch (keyCode - 118)
                    {
                        case KeyCode.None:
                            if (args.Control)
                            {
                                string clipBoard = dfClipboardHelper.clipBoard;
                                if (!string.IsNullOrEmpty(clipBoard))
                                {
                                    this.PasteAtCursor(clipBoard);
                                    break;
                                }
                                break;
                            }
                            break;
                        case (KeyCode) 2:
                            if (args.Control)
                            {
                                this.CutSelectionToClipboard();
                                break;
                            }
                            break;
                        default:
                            if (keyCode != KeyCode.Backspace)
                            {
                                if (keyCode != KeyCode.Return)
                                {
                                    if (keyCode != KeyCode.Escape)
                                    {
                                        if (keyCode == KeyCode.Delete)
                                        {
                                            if (this.selectionStart != this.selectionEnd)
                                            {
                                                this.DeleteSelection();
                                                break;
                                            }
                                            if (args.Control)
                                            {
                                                this.DeleteNextWord();
                                                break;
                                            }
                                            this.DeleteNextChar();
                                            break;
                                        }
                                        base.OnKeyDown(args);
                                        return;
                                    }
                                    this.ClearSelection();
                                    this.cursorIndex = this.scrollIndex = 0;
                                    this.Invalidate();
                                    this.OnCancel();
                                    break;
                                }
                                this.OnSubmit();
                                break;
                            }
                            if (args.Control)
                            {
                                this.DeletePreviousWord();
                                break;
                            }
                            this.DeletePreviousChar();
                            break;
                    }
                    break;
            }
            args.Use();
        }

        public override void OnEnable()
        {
            if (this.padding == null)
                this.padding = new RectOffset();
            base.OnEnable();
            if ((double) this.size.magnitude == 0.0)
                this.Size = new Vector2(100f, 20f);
            this.cursorShown = false;
            this.cursorIndex = this.scrollIndex = 0;
            bool flag = (UnityEngine.Object) this.Font != (UnityEngine.Object) null && this.Font.IsValid;
            if (Application.isPlaying && !flag)
                this.Font = this.GetManager().DefaultFont;
            this.bindTextureRebuildCallback();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            this.unbindTextureRebuildCallback();
        }

        public override void Awake()
        {
            base.Awake();
            this.startSize = this.Size;
        }

        protected internal override void OnEnterFocus(dfFocusEventArgs args)
        {
            base.OnEnterFocus(args);
            this.undoText = this.Text;
            if (!this.ReadOnly)
            {
                this.whenGotFocus = UnityEngine.Time.realtimeSinceStartup;
                this.StopAllCoroutines();
                this.StartCoroutine(this.doCursorBlink());
                if (this.selectOnFocus)
                {
                    this.selectionStart = 0;
                    this.selectionEnd = this.text.Length;
                }
                else
                    this.selectionStart = this.selectionEnd = 0;
            }
            this.Invalidate();
        }

        protected internal override void OnLeaveFocus(dfFocusEventArgs args)
        {
            base.OnLeaveFocus(args);
            this.StopAllCoroutines();
            this.cursorShown = false;
            this.ClearSelection();
            this.Invalidate();
            this.whenGotFocus = 0.0f;
        }

        protected internal override void OnDoubleClick(dfMouseEventArgs args)
        {
            this.tripleClickTimer = UnityEngine.Time.realtimeSinceStartup;
            if ((UnityEngine.Object) args.Source != (UnityEngine.Object) this)
            {
                base.OnDoubleClick(args);
            }
            else
            {
                if (!this.ReadOnly && this.HasFocus && args.Buttons.IsSet(dfMouseButtons.Left) && (double) UnityEngine.Time.realtimeSinceStartup - (double) this.whenGotFocus > 0.5)
                    this.SelectWordAtIndex(this.getCharIndexOfMouse(args));
                base.OnDoubleClick(args);
            }
        }

        protected internal override void OnMouseDown(dfMouseEventArgs args)
        {
            if ((UnityEngine.Object) args.Source != (UnityEngine.Object) this)
            {
                base.OnMouseDown(args);
            }
            else
            {
                if (!this.ReadOnly && args.Buttons.IsSet(dfMouseButtons.Left) && (!this.HasFocus && !this.SelectOnFocus || (double) UnityEngine.Time.realtimeSinceStartup - (double) this.whenGotFocus > 0.25))
                {
                    int charIndexOfMouse = this.getCharIndexOfMouse(args);
                    if (charIndexOfMouse != this.cursorIndex)
                    {
                        this.cursorIndex = charIndexOfMouse;
                        this.cursorShown = true;
                        this.Invalidate();
                        args.Use();
                    }
                    this.mouseSelectionAnchor = this.cursorIndex;
                    this.selectionStart = this.selectionEnd = this.cursorIndex;
                    if ((double) UnityEngine.Time.realtimeSinceStartup - (double) this.tripleClickTimer < 0.25)
                    {
                        this.SelectAll();
                        this.tripleClickTimer = 0.0f;
                    }
                }
                base.OnMouseDown(args);
            }
        }

        protected internal override void OnMouseMove(dfMouseEventArgs args)
        {
            if ((UnityEngine.Object) args.Source != (UnityEngine.Object) this)
            {
                base.OnMouseMove(args);
            }
            else
            {
                if (!this.ReadOnly && this.HasFocus && args.Buttons.IsSet(dfMouseButtons.Left))
                {
                    int charIndexOfMouse = this.getCharIndexOfMouse(args);
                    if (charIndexOfMouse != this.cursorIndex)
                    {
                        this.cursorIndex = charIndexOfMouse;
                        this.cursorShown = true;
                        this.Invalidate();
                        args.Use();
                        this.selectionStart = Mathf.Min(this.mouseSelectionAnchor, charIndexOfMouse);
                        this.selectionEnd = Mathf.Max(this.mouseSelectionAnchor, charIndexOfMouse);
                        return;
                    }
                }
                base.OnMouseMove(args);
            }
        }

        protected internal virtual void OnTextChanged()
        {
            this.SignalHierarchy(nameof (OnTextChanged), (object) this, (object) this.text);
            if (this.TextChanged == null)
                return;
            this.TextChanged((dfControl) this, this.text);
        }

        protected internal virtual void OnReadOnlyChanged()
        {
            if (this.ReadOnlyChanged == null)
                return;
            this.ReadOnlyChanged((dfControl) this, this.readOnly);
        }

        protected internal virtual void OnPasswordCharacterChanged()
        {
            if (this.PasswordCharacterChanged == null)
                return;
            this.PasswordCharacterChanged((dfControl) this, this.passwordChar);
        }

        protected internal virtual void OnSubmit()
        {
            this.SignalHierarchy("OnTextSubmitted", (object) this, (object) this.text);
            if (this.TextSubmitted == null)
                return;
            this.TextSubmitted((dfControl) this, this.text);
        }

        protected internal virtual void OnCancel()
        {
            this.text = this.undoText;
            this.SignalHierarchy("OnTextCancelled", (object) this, (object) this.text);
            if (this.TextCancelled == null)
                return;
            this.TextCancelled((dfControl) this, this.text);
        }

        public void ClearSelection()
        {
            this.selectionStart = 0;
            this.selectionEnd = 0;
            this.mouseSelectionAnchor = 0;
        }

        public void SelectAll()
        {
            this.selectionStart = 0;
            this.selectionEnd = this.text.Length;
            this.scrollIndex = 0;
            this.setCursorPos(0);
        }

        private void CutSelectionToClipboard()
        {
            this.CopySelectionToClipboard();
            this.DeleteSelection();
        }

        private void CopySelectionToClipboard()
        {
            if (this.selectionStart == this.selectionEnd)
                return;
            dfClipboardHelper.clipBoard = this.text.Substring(this.selectionStart, this.selectionEnd - this.selectionStart);
        }

        public void PasteAtCursor(string clipData)
        {
            this.DeleteSelection();
            StringBuilder stringBuilder = new StringBuilder(this.text.Length + clipData.Length);
            stringBuilder.Append(this.text);
            for (int index = 0; index < clipData.Length; ++index)
            {
                char ch = clipData[index];
                if (ch >= ' ')
                    stringBuilder.Insert(this.cursorIndex++, ch);
            }
            stringBuilder.Length = Mathf.Min(stringBuilder.Length, this.maxLength);
            this.text = stringBuilder.ToString();
            this.setCursorPos(this.cursorIndex);
            this.OnTextChanged();
            this.Invalidate();
        }

        public void SelectWordAtIndex(int index)
        {
            if (string.IsNullOrEmpty(this.text))
                return;
            index = Mathf.Max(Mathf.Min(this.text.Length - 1, index), 0);
            if (!char.IsLetterOrDigit(this.text[index]))
            {
                this.selectionStart = index;
                this.selectionEnd = index + 1;
                this.mouseSelectionAnchor = 0;
            }
            else
            {
                this.selectionStart = index;
                for (int index1 = index; index1 > 0 && char.IsLetterOrDigit(this.text[index1 - 1]); --index1)
                    --this.selectionStart;
                this.selectionEnd = index;
                for (int index2 = index; index2 < this.text.Length && char.IsLetterOrDigit(this.text[index2]); ++index2)
                    this.selectionEnd = index2 + 1;
            }
            this.cursorIndex = this.selectionStart;
            this.Invalidate();
        }

        public void DeletePreviousChar()
        {
            if (this.selectionStart != this.selectionEnd)
            {
                int selectionStart = this.selectionStart;
                this.DeleteSelection();
                this.setCursorPos(selectionStart);
            }
            else
            {
                this.ClearSelection();
                if (this.cursorIndex == 0)
                    return;
                this.text = this.text.Remove(this.cursorIndex - 1, 1);
                --this.cursorIndex;
                this.cursorShown = true;
                this.OnTextChanged();
                this.Invalidate();
            }
        }

        public void DeletePreviousWord()
        {
            this.ClearSelection();
            if (this.cursorIndex == 0)
                return;
            int num = this.findPreviousWord(this.cursorIndex);
            if (num == this.cursorIndex)
                num = 0;
            this.text = this.text.Remove(num, this.cursorIndex - num);
            this.setCursorPos(num);
            this.OnTextChanged();
            this.Invalidate();
        }

        public void DeleteSelection()
        {
            if (this.selectionStart == this.selectionEnd)
                return;
            this.text = this.text.Remove(this.selectionStart, this.selectionEnd - this.selectionStart);
            this.setCursorPos(this.selectionStart);
            this.ClearSelection();
            this.OnTextChanged();
            this.Invalidate();
        }

        public void DeleteNextChar()
        {
            this.ClearSelection();
            if (this.cursorIndex >= this.text.Length)
                return;
            this.text = this.text.Remove(this.cursorIndex, 1);
            this.cursorShown = true;
            this.OnTextChanged();
            this.Invalidate();
        }

        public void DeleteNextWord()
        {
            this.ClearSelection();
            if (this.cursorIndex == this.text.Length)
                return;
            int num = this.findNextWord(this.cursorIndex);
            if (num == this.cursorIndex)
                num = this.text.Length;
            this.text = this.text.Remove(this.cursorIndex, num - this.cursorIndex);
            this.OnTextChanged();
            this.Invalidate();
        }

        public void SelectToStart()
        {
            if (this.cursorIndex == 0)
                return;
            if (this.selectionEnd == this.selectionStart)
                this.selectionEnd = this.cursorIndex;
            else if (this.selectionEnd == this.cursorIndex)
                this.selectionEnd = this.selectionStart;
            this.selectionStart = 0;
            this.setCursorPos(0);
        }

        public void SelectToEnd()
        {
            if (this.cursorIndex == this.text.Length)
                return;
            if (this.selectionEnd == this.selectionStart)
                this.selectionStart = this.cursorIndex;
            else if (this.selectionStart == this.cursorIndex)
                this.selectionStart = this.selectionEnd;
            this.selectionEnd = this.text.Length;
            this.setCursorPos(this.text.Length);
        }

        public void MoveCursorToNextWord()
        {
            this.ClearSelection();
            if (this.cursorIndex == this.text.Length)
                return;
            this.setCursorPos(this.findNextWord(this.cursorIndex));
        }

        public void MoveCursorToPreviousWord()
        {
            this.ClearSelection();
            if (this.cursorIndex == 0)
                return;
            this.setCursorPos(this.findPreviousWord(this.cursorIndex));
        }

        public void MoveCursorToEnd()
        {
            this.ClearSelection();
            this.setCursorPos(this.text.Length);
        }

        public void MoveCursorToStart()
        {
            this.ClearSelection();
            this.setCursorPos(0);
        }

        public void MoveCursorToNextChar()
        {
            this.ClearSelection();
            this.setCursorPos(this.cursorIndex + 1);
        }

        public void MoveCursorToPreviousChar()
        {
            this.ClearSelection();
            this.setCursorPos(this.cursorIndex - 1);
        }

        private void moveSelectionPointRightWord()
        {
            if (this.cursorIndex == this.text.Length)
                return;
            int nextWord = this.findNextWord(this.cursorIndex);
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionStart = this.cursorIndex;
                this.selectionEnd = nextWord;
            }
            else if (this.selectionEnd == this.cursorIndex)
                this.selectionEnd = nextWord;
            else if (this.selectionStart == this.cursorIndex)
                this.selectionStart = nextWord;
            this.setCursorPos(nextWord);
        }

        private void moveSelectionPointLeftWord()
        {
            if (this.cursorIndex == 0)
                return;
            int previousWord = this.findPreviousWord(this.cursorIndex);
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionEnd = this.cursorIndex;
                this.selectionStart = previousWord;
            }
            else if (this.selectionEnd == this.cursorIndex)
                this.selectionEnd = previousWord;
            else if (this.selectionStart == this.cursorIndex)
                this.selectionStart = previousWord;
            this.setCursorPos(previousWord);
        }

        private void moveSelectionPointRight()
        {
            if (this.cursorIndex == this.text.Length)
                return;
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionEnd = this.cursorIndex + 1;
                this.selectionStart = this.cursorIndex;
            }
            else if (this.selectionEnd == this.cursorIndex)
                ++this.selectionEnd;
            else if (this.selectionStart == this.cursorIndex)
                ++this.selectionStart;
            this.setCursorPos(this.cursorIndex + 1);
        }

        private void moveSelectionPointLeft()
        {
            if (this.cursorIndex == 0)
                return;
            if (this.selectionEnd == this.selectionStart)
            {
                this.selectionEnd = this.cursorIndex;
                this.selectionStart = this.cursorIndex - 1;
            }
            else if (this.selectionEnd == this.cursorIndex)
                --this.selectionEnd;
            else if (this.selectionStart == this.cursorIndex)
                --this.selectionStart;
            this.setCursorPos(this.cursorIndex - 1);
        }

        private void setCursorPos(int index)
        {
            index = Mathf.Max(0, Mathf.Min(this.text.Length, index));
            if (index == this.cursorIndex)
                return;
            this.cursorIndex = index;
            this.cursorShown = this.HasFocus;
            this.scrollIndex = Mathf.Min(this.scrollIndex, this.cursorIndex);
            this.Invalidate();
        }

        private int findPreviousWord(int startIndex)
        {
            int previousWord;
            for (previousWord = startIndex; previousWord > 0; --previousWord)
            {
                char c = this.text[previousWord - 1];
                if (!char.IsWhiteSpace(c) && !char.IsSeparator(c) && !char.IsPunctuation(c))
                    break;
            }
            for (int index = previousWord; index >= 0; --index)
            {
                if (index == 0)
                {
                    previousWord = 0;
                    break;
                }
                char c = this.text[index - 1];
                if (char.IsWhiteSpace(c) || char.IsSeparator(c) || char.IsPunctuation(c))
                {
                    previousWord = index;
                    break;
                }
            }
            return previousWord;
        }

        private int findNextWord(int startIndex)
        {
            int length = this.text.Length;
            int index1 = startIndex;
            for (int index2 = index1; index2 < length; ++index2)
            {
                char c = this.text[index2];
                if (char.IsWhiteSpace(c) || char.IsSeparator(c) || char.IsPunctuation(c))
                {
                    index1 = index2;
                    break;
                }
            }
            for (; index1 < length; ++index1)
            {
                char c = this.text[index1];
                if (!char.IsWhiteSpace(c) && !char.IsSeparator(c) && !char.IsPunctuation(c))
                    break;
            }
            return index1;
        }

        [DebuggerHidden]
        private IEnumerator doCursorBlink()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new dfTextbox__doCursorBlinkc__Iterator0()
            {
                _this = this
            };
        }

        private void renderText(dfRenderData textBuffer)
        {
            float units = this.PixelsToUnits();
            Vector2 vector2_1 = new Vector2(this.size.x - (float) this.padding.horizontal, this.size.y - (float) this.padding.vertical);
            Vector3 upperLeft = this.pivot.TransformToUpperLeft(this.Size);
            Vector3 vector3 = new Vector3(upperLeft.x + (float) this.padding.left, upperLeft.y - (float) this.padding.top, 0.0f) * units;
            string text = !this.IsPasswordField || string.IsNullOrEmpty(this.passwordChar) ? this.text : this.passwordDisplayText();
            Color32 color32 = !this.IsEnabled ? this.DisabledColor : this.TextColor;
            float textScaleMultiplier = this.getTextScaleMultiplier();
            using (dfFontRendererBase renderer = this.font.ObtainRenderer())
            {
                renderer.WordWrap = false;
                renderer.MaxSize = vector2_1;
                renderer.PixelRatio = units;
                renderer.TextScale = this.TextScale * textScaleMultiplier;
                renderer.VectorOffset = vector3;
                renderer.MultiLine = false;
                renderer.TextAlign = TextAlignment.Left;
                renderer.ProcessMarkup = false;
                renderer.DefaultColor = color32;
                renderer.BottomColor = new Color32?(color32);
                renderer.OverrideMarkupColors = false;
                renderer.Opacity = this.CalculateOpacity();
                renderer.Shadow = this.Shadow;
                renderer.ShadowColor = this.ShadowColor;
                renderer.ShadowOffset = this.ShadowOffset;
                this.cursorIndex = Mathf.Min(this.cursorIndex, text.Length);
                this.scrollIndex = Mathf.Min(Mathf.Min(this.scrollIndex, this.cursorIndex), text.Length);
                this.charWidths = renderer.GetCharacterWidths(text);
                Vector2 vector2_2 = vector2_1 * units;
                this.leftOffset = 0.0f;
                if (this.textAlign == TextAlignment.Left)
                {
                    float num = 0.0f;
                    for (int scrollIndex = this.scrollIndex; scrollIndex < this.cursorIndex; ++scrollIndex)
                        num += this.charWidths[scrollIndex];
                    while ((double) num >= (double) vector2_2.x && this.scrollIndex < this.cursorIndex)
                        num -= this.charWidths[this.scrollIndex++];
                }
                else
                {
                    this.scrollIndex = Mathf.Max(0, Mathf.Min(this.cursorIndex, text.Length - 1));
                    float num1 = 0.0f;
                    float num2 = (float) this.font.FontSize * 1.25f * units;
                    while (this.scrollIndex > 0 && (double) num1 < (double) vector2_2.x - (double) num2)
                        num1 += this.charWidths[this.scrollIndex--];
                    float num3 = text.Length <= 0 ? 0.0f : ((IEnumerable<float>) renderer.GetCharacterWidths(text.Substring(this.scrollIndex))).Sum();
                    switch (this.textAlign)
                    {
                        case TextAlignment.Center:
                            this.leftOffset = Mathf.Max(0.0f, (float) (((double) vector2_2.x - (double) num3) * 0.5));
                            break;
                        case TextAlignment.Right:
                            this.leftOffset = Mathf.Max(0.0f, vector2_2.x - num3);
                            break;
                    }
                    vector3.x += this.leftOffset;
                    renderer.VectorOffset = vector3;
                }
                if (this.selectionEnd != this.selectionStart)
                    this.renderSelection(this.scrollIndex, this.charWidths, this.leftOffset);
                else if (this.cursorShown)
                    this.renderCursor(this.scrollIndex, this.cursorIndex, this.charWidths, this.leftOffset);
                renderer.Render(text.Substring(this.scrollIndex), textBuffer);
            }
        }

        private float getTextScaleMultiplier()
        {
            if (this.textScaleMode == dfTextScaleMode.None || !Application.isPlaying)
                return 1f;
            return this.textScaleMode == dfTextScaleMode.ScreenResolution ? (float) Screen.height / (float) this.cachedManager.FixedHeight : this.Size.y / this.startSize.y;
        }

        private string passwordDisplayText() => new string(this.passwordChar[0], this.text.Length);

        private void renderSelection(int scrollIndex, float[] charWidths, float leftOffset)
        {
            if (string.IsNullOrEmpty(this.SelectionSprite) || (UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
                return;
            float units = this.PixelsToUnits();
            float b1 = (this.size.x - (float) this.padding.horizontal) * units;
            int b2 = scrollIndex;
            float num1 = 0.0f;
            for (int index = scrollIndex; index < this.text.Length; ++index)
            {
                ++b2;
                num1 += charWidths[index];
                if ((double) num1 > (double) b1)
                    break;
            }
            if (this.selectionStart > b2 || this.selectionEnd < scrollIndex)
                return;
            int num2 = Mathf.Max(scrollIndex, this.selectionStart);
            if (num2 > b2)
                return;
            int num3 = Mathf.Min(this.selectionEnd, b2);
            if (num3 <= scrollIndex)
                return;
            float num4 = 0.0f;
            float num5 = 0.0f;
            float num6 = 0.0f;
            for (int index = scrollIndex; index <= b2; ++index)
            {
                if (index == num2)
                    num4 = num6;
                if (index == num3)
                {
                    num5 = num6;
                    break;
                }
                num6 += charWidths[index];
            }
            float num7 = this.Size.y * units;
            this.addQuadIndices(this.renderData.Vertices, this.renderData.Triangles);
            RectOffset selectionPadding = this.getSelectionPadding();
            float x1 = (float) ((double) num4 + (double) leftOffset + (double) this.padding.left * (double) units);
            float x2 = x1 + Mathf.Min(num5 - num4, b1);
            float y1 = (float) -(selectionPadding.top + 1) * units;
            float y2 = (float) ((double) y1 - (double) num7 + (double) (selectionPadding.vertical + 2) * (double) units);
            Vector3 vector3_1 = this.pivot.TransformToUpperLeft(this.Size) * units;
            Vector3 vector3_2 = new Vector3(x1, y1) + vector3_1;
            Vector3 vector3_3 = new Vector3(x2, y1) + vector3_1;
            Vector3 vector3_4 = new Vector3(x1, y2) + vector3_1;
            Vector3 vector3_5 = new Vector3(x2, y2) + vector3_1;
            this.renderData.Vertices.Add(vector3_2);
            this.renderData.Vertices.Add(vector3_3);
            this.renderData.Vertices.Add(vector3_5);
            this.renderData.Vertices.Add(vector3_4);
            Color32 color32 = this.ApplyOpacity(this.SelectionBackgroundColor);
            this.renderData.Colors.Add(color32);
            this.renderData.Colors.Add(color32);
            this.renderData.Colors.Add(color32);
            this.renderData.Colors.Add(color32);
            dfAtlas.ItemInfo atla = this.Atlas[this.SelectionSprite];
            Rect region = atla.region;
            float num8 = region.width / atla.sizeInPixels.x;
            float num9 = region.height / atla.sizeInPixels.y;
            this.renderData.UV.Add(new Vector2(region.x + num8, region.yMax - num9));
            this.renderData.UV.Add(new Vector2(region.xMax - num8, region.yMax - num9));
            this.renderData.UV.Add(new Vector2(region.xMax - num8, region.y + num9));
            this.renderData.UV.Add(new Vector2(region.x + num8, region.y + num9));
        }

        private RectOffset getSelectionPadding()
        {
            if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
                return this.padding;
            dfAtlas.ItemInfo backgroundSprite = this.getBackgroundSprite();
            return backgroundSprite == (dfAtlas.ItemInfo) null ? this.padding : backgroundSprite.border;
        }

        private void renderCursor(int startIndex, int cursorIndex, float[] charWidths, float leftOffset)
        {
            if (string.IsNullOrEmpty(this.SelectionSprite) || (UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
                return;
            float num1 = 0.0f;
            for (int index = startIndex; index < cursorIndex; ++index)
                num1 += charWidths[index];
            float units = this.PixelsToUnits();
            float x = ((float) ((double) num1 + (double) leftOffset + (double) this.padding.left * (double) units)).Quantize(units);
            float y = (float) -this.padding.top * units;
            float num2 = units * (float) this.cursorWidth;
            float num3 = (this.size.y - (float) this.padding.vertical) * units;
            Vector3 vector3_1 = new Vector3(x, y);
            Vector3 vector3_2 = new Vector3(x + num2, y);
            Vector3 vector3_3 = new Vector3(x + num2, y - num3);
            Vector3 vector3_4 = new Vector3(x, y - num3);
            dfList<Vector3> vertices = this.renderData.Vertices;
            dfList<int> triangles = this.renderData.Triangles;
            dfList<Vector2> uv = this.renderData.UV;
            dfList<Color32> colors = this.renderData.Colors;
            Vector3 vector3_5 = this.pivot.TransformToUpperLeft(this.size) * units;
            this.addQuadIndices(vertices, triangles);
            vertices.Add(vector3_1 + vector3_5);
            vertices.Add(vector3_2 + vector3_5);
            vertices.Add(vector3_3 + vector3_5);
            vertices.Add(vector3_4 + vector3_5);
            Color32 color32 = this.ApplyOpacity(this.CursorColor);
            colors.Add(color32);
            colors.Add(color32);
            colors.Add(color32);
            colors.Add(color32);
            Rect region = this.Atlas[this.SelectionSprite].region;
            uv.Add(new Vector2(region.x, region.yMax));
            uv.Add(new Vector2(region.xMax, region.yMax));
            uv.Add(new Vector2(region.xMax, region.y));
            uv.Add(new Vector2(region.x, region.y));
        }

        private void addQuadIndices(dfList<Vector3> verts, dfList<int> triangles)
        {
            int count = verts.Count;
            int[] numArray = new int[6]{ 0, 1, 3, 3, 1, 2 };
            foreach (int num in numArray)
                triangles.Add(count + num);
        }

        private int getCharIndexOfMouse(dfMouseEventArgs args)
        {
            Vector2 hitPosition = this.GetHitPosition(args);
            float units = this.PixelsToUnits();
            int scrollIndex1 = this.scrollIndex;
            float num = this.leftOffset / units;
            for (int scrollIndex2 = this.scrollIndex; scrollIndex2 < this.charWidths.Length; ++scrollIndex2)
            {
                num += this.charWidths[scrollIndex2] / units;
                if ((double) num < (double) hitPosition.x)
                    ++scrollIndex1;
            }
            return scrollIndex1;
        }

        public dfList<dfRenderData> RenderMultiple()
        {
            if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null || (UnityEngine.Object) this.Font == (UnityEngine.Object) null)
                return (dfList<dfRenderData>) null;
            if (!this.isVisible)
                return (dfList<dfRenderData>) null;
            if (this.renderData == null)
            {
                this.renderData = dfRenderData.Obtain();
                this.textRenderData = dfRenderData.Obtain();
                this.isControlInvalidated = true;
            }
            Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
            if (!this.isControlInvalidated)
            {
                for (int index = 0; index < this.buffers.Count; ++index)
                    this.buffers[index].Transform = localToWorldMatrix;
                return this.buffers;
            }
            this.buffers.Clear();
            this.renderData.Clear();
            this.renderData.Material = this.Atlas.Material;
            this.renderData.Transform = localToWorldMatrix;
            this.buffers.Add(this.renderData);
            this.textRenderData.Clear();
            this.textRenderData.Material = this.Atlas.Material;
            this.textRenderData.Transform = localToWorldMatrix;
            this.buffers.Add(this.textRenderData);
            this.renderBackground();
            this.renderText(this.textRenderData);
            this.isControlInvalidated = false;
            this.updateCollider();
            return this.buffers;
        }

        private void bindTextureRebuildCallback()
        {
            if (this.isFontCallbackAssigned || (UnityEngine.Object) this.Font == (UnityEngine.Object) null || !(this.Font is dfDynamicFont))
                return;
            UnityEngine.Font.textureRebuilt += new Action<UnityEngine.Font>(this.onFontTextureRebuilt);
            this.isFontCallbackAssigned = true;
        }

        private void unbindTextureRebuildCallback()
        {
            if (!this.isFontCallbackAssigned || (UnityEngine.Object) this.Font == (UnityEngine.Object) null)
                return;
            if (this.Font is dfDynamicFont)
                UnityEngine.Font.textureRebuilt -= new Action<UnityEngine.Font>(this.onFontTextureRebuilt);
            this.isFontCallbackAssigned = false;
        }

        private void requestCharacterInfo()
        {
            dfDynamicFont font = this.Font as dfDynamicFont;
            if ((UnityEngine.Object) font == (UnityEngine.Object) null || !dfFontManager.IsDirty(this.Font) || string.IsNullOrEmpty(this.text))
                return;
            int fontSize = Mathf.CeilToInt((float) this.font.FontSize * (this.TextScale * this.getTextScaleMultiplier()));
            font.AddCharacterRequest(this.text, fontSize, FontStyle.Normal);
        }

        private void onFontTextureRebuilt(UnityEngine.Font font)
        {
            if (!(this.Font is dfDynamicFont) || !((UnityEngine.Object) font == (UnityEngine.Object) (this.Font as dfDynamicFont).BaseFont))
                return;
            this.requestCharacterInfo();
            this.Invalidate();
        }

        public void UpdateFontInfo() => this.requestCharacterInfo();
    }

