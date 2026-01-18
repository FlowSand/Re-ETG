using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Touch/Drag Source Events")]
public class TouchDragSourceEvents : MonoBehaviour
    {
        private dfLabel _label;
        private bool isDragging;

        public void Start() => this._label = this.GetComponent<dfLabel>();

        public void OnGUI()
        {
            if (!this.isDragging)
                return;
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.y = (float) Screen.height - mousePosition.y;
            GUI.Box(new Rect(mousePosition.x - 100f, mousePosition.y - 50f, 200f, 100f), this._label.name);
        }

        public void OnDragEnd(dfControl control, dfDragEventArgs dragEvent)
        {
            this._label.Text = dragEvent.State != dfDragDropState.Dropped ? "Drag Ended: " + (object) dragEvent.State : "Dropped on " + dragEvent.Target.name;
            this.isDragging = false;
        }

        public void OnDragStart(dfControl control, dfDragEventArgs dragEvent)
        {
            this._label.Text = "Dragging...";
            dragEvent.Data = (object) this.name;
            dragEvent.State = dfDragDropState.Dragging;
            dragEvent.Use();
            this.isDragging = true;
        }
    }

