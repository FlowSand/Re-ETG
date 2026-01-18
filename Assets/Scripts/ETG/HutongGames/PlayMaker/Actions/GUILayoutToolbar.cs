using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [HutongGames.PlayMaker.Tooltip("GUILayout Toolbar. NOTE: Arrays must be the same length as NumButtons or empty.")]
    public class GUILayoutToolbar : GUILayoutAction
    {
        [HutongGames.PlayMaker.Tooltip("The number of buttons in the toolbar")]
        public FsmInt numButtons;
        [HutongGames.PlayMaker.Tooltip("Store the index of the selected button in an Integer Variable")]
        [UIHint(UIHint.Variable)]
        public FsmInt selectedButton;
        [HutongGames.PlayMaker.Tooltip("Event to send when each button is pressed.")]
        public FsmEvent[] buttonEventsArray;
        [HutongGames.PlayMaker.Tooltip("Image to use on each button.")]
        public FsmTexture[] imagesArray;
        [HutongGames.PlayMaker.Tooltip("Text to use on each button.")]
        public FsmString[] textsArray;
        [HutongGames.PlayMaker.Tooltip("Tooltip to use for each button.")]
        public FsmString[] tooltipsArray;
        [HutongGames.PlayMaker.Tooltip("A named GUIStyle to use for the toolbar buttons. Default is Button.")]
        public FsmString style;
        [HutongGames.PlayMaker.Tooltip("Update the content of the buttons every frame. Useful if the buttons are using variables that change.")]
        public bool everyFrame;
        private GUIContent[] contents;

        public GUIContent[] Contents
        {
            get
            {
                if (this.contents == null)
                    this.SetButtonsContent();
                return this.contents;
            }
        }

        private void SetButtonsContent()
        {
            if (this.contents == null)
                this.contents = new GUIContent[this.numButtons.Value];
            for (int index = 0; index < this.numButtons.Value; ++index)
                this.contents[index] = new GUIContent();
            for (int index = 0; index < this.imagesArray.Length; ++index)
                this.contents[index].image = this.imagesArray[index].Value;
            for (int index = 0; index < this.textsArray.Length; ++index)
                this.contents[index].text = this.textsArray[index].Value;
            for (int index = 0; index < this.tooltipsArray.Length; ++index)
                this.contents[index].tooltip = this.tooltipsArray[index].Value;
        }

        public override void Reset()
        {
            base.Reset();
            this.numButtons = (FsmInt) 0;
            this.selectedButton = (FsmInt) null;
            this.buttonEventsArray = new FsmEvent[0];
            this.imagesArray = new FsmTexture[0];
            this.tooltipsArray = new FsmString[0];
            this.style = (FsmString) "Button";
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            string text = this.ErrorCheck();
            if (string.IsNullOrEmpty(text))
                return;
            this.LogError(text);
            this.Finish();
        }

        public override void OnGUI()
        {
            if (this.everyFrame)
                this.SetButtonsContent();
            bool changed = GUI.changed;
            GUI.changed = false;
            this.selectedButton.Value = GUILayout.Toolbar(this.selectedButton.Value, this.Contents, (GUIStyle) this.style.Value, this.LayoutOptions);
            if (GUI.changed)
            {
                if (this.selectedButton.Value >= this.buttonEventsArray.Length)
                    return;
                this.Fsm.Event(this.buttonEventsArray[this.selectedButton.Value]);
                GUIUtility.ExitGUI();
            }
            else
                GUI.changed = changed;
        }

        public override string ErrorCheck()
        {
            string empty = string.Empty;
            if (this.imagesArray.Length > 0 && this.imagesArray.Length != this.numButtons.Value)
                empty += "Images array doesn't match NumButtons.\n";
            if (this.textsArray.Length > 0 && this.textsArray.Length != this.numButtons.Value)
                empty += "Texts array doesn't match NumButtons.\n";
            if (this.tooltipsArray.Length > 0 && this.tooltipsArray.Length != this.numButtons.Value)
                empty += "Tooltips array doesn't match NumButtons.\n";
            return empty;
        }
    }
}
