using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Add-Remove Controls/Create Child Control")]
public class DemoCreateChildControls : MonoBehaviour
    {
        public dfScrollPanel target;
        private int colorNum;
        private Color32[] colors = new Color32[4]
        {
            (Color32) Color.white,
            (Color32) Color.red,
            (Color32) Color.green,
            (Color32) Color.black
        };

        public void Start()
        {
            if (!((Object) this.target == (Object) null))
                return;
            this.target = this.GetComponent<dfScrollPanel>();
        }

        public void OnClick()
        {
            for (int index = 0; index < 10; ++index)
            {
                dfButton dfButton = this.target.AddControl<dfButton>();
                dfButton.NormalBackgroundColor = this.colors[this.colorNum % this.colors.Length];
                dfButton.BackgroundSprite = "button-normal";
                dfButton.Text = $"Button {dfButton.ZOrder}";
                dfButton.Anchor = dfAnchorStyle.Left | dfAnchorStyle.Right;
                dfButton.Width = this.target.Width - (float) this.target.ScrollPadding.horizontal;
            }
            ++this.colorNum;
        }

        public void OnDoubleClick() => this.OnClick();
    }

