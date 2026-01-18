using System.Collections.Generic;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Touch/Display Events")]
public class TouchStateDisplayEvents : MonoBehaviour
    {
        public dfLabel _label;
        private List<string> messages = new List<string>();

        public void Start()
        {
            if (!((Object) this._label == (Object) null))
                return;
            this._label = this.GetComponent<dfLabel>();
            this._label.Text = "Touch State";
        }

        public void OnDragDrop(dfControl control, dfDragEventArgs dragEvent)
        {
            this.display("DragDrop: " + (dragEvent.Data != null ? dragEvent.Data.ToString() : "(null)"));
            dragEvent.State = dfDragDropState.Dropped;
            dragEvent.Use();
        }

        public void OnEnterFocus(dfControl control, dfFocusEventArgs args) => this.display("EnterFocus");

        public void OnLeaveFocus(dfControl control, dfFocusEventArgs args) => this.display("LeaveFocus");

        public void OnClick(dfControl control, dfMouseEventArgs mouseEvent) => this.display("Click");

        public void OnDoubleClick(dfControl control, dfMouseEventArgs mouseEvent)
        {
            this.display("DoubleClick");
        }

        public void OnMouseDown(dfControl control, dfMouseEventArgs mouseEvent)
        {
            this.display("MouseDown");
        }

        public void OnMouseEnter(dfControl control, dfMouseEventArgs mouseEvent)
        {
            this.display("MouseEnter");
        }

        public void OnMouseLeave(dfControl control, dfMouseEventArgs mouseEvent)
        {
            this.display("MouseLeave");
        }

        public void OnMouseMove(dfControl control, dfMouseEventArgs mouseEvent)
        {
            this.display("MouseMove: " + (object) this.screenToGUI(mouseEvent.Position));
        }

        public void OnMouseUp(dfControl control, dfMouseEventArgs mouseEvent) => this.display("MouseUp");

        public void OnMultiTouch(dfControl control, dfTouchEventArgs touchData)
        {
            string text = "Multi-Touch:\n";
            for (int index = 0; index < touchData.Touches.Count; ++index)
            {
                dfTouchInfo touch = touchData.Touches[index];
                text += $"\tFinger {index + 1}: {this.screenToGUI(touch.position)}\n";
            }
            this.display(text);
        }

        private void display(string text)
        {
            this.messages.Add(text);
            if (this.messages.Count > 6)
                this.messages.RemoveAt(0);
            this._label.Text = string.Join("\n", this.messages.ToArray());
        }

        private Vector2 screenToGUI(Vector2 position)
        {
            position.y = this._label.GetManager().GetScreenSize().y - position.y;
            return position;
        }
    }

