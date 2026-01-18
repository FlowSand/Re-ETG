using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Add-Remove Controls/Move Child Control")]
public class DemoMoveControls : MonoBehaviour
    {
        public dfScrollPanel from;
        public dfScrollPanel to;

        public void OnClick()
        {
            this.from.SuspendLayout();
            this.to.SuspendLayout();
            while (this.from.Controls.Count > 0)
            {
                dfControl control = this.from.Controls[0];
                this.from.RemoveControl(control);
                control.ZOrder = -1;
                this.to.AddControl(control);
            }
            this.from.ResumeLayout();
            this.to.ResumeLayout();
            this.from.ScrollPosition = Vector2.zero;
        }
    }

