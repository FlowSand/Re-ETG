using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Containers/Auto-Arrange Options")]
[ExecuteInEditMode]
public class AutoArrangeOptions : MonoBehaviour
    {
        public dfScrollPanel Panel;

        public int FlowDirection
        {
            get => (int) this.Panel.FlowDirection;
            set => this.Panel.FlowDirection = (dfScrollPanel.LayoutDirection) value;
        }

        public int PaddingLeft
        {
            get => this.Panel.FlowPadding.left;
            set
            {
                this.Panel.FlowPadding.left = value;
                this.Panel.Reset();
            }
        }

        public int PaddingRight
        {
            get => this.Panel.FlowPadding.right;
            set
            {
                this.Panel.FlowPadding.right = value;
                this.Panel.Reset();
            }
        }

        public int PaddingTop
        {
            get => this.Panel.FlowPadding.top;
            set
            {
                this.Panel.FlowPadding.top = value;
                this.Panel.Reset();
            }
        }

        public int PaddingBottom
        {
            get => this.Panel.FlowPadding.bottom;
            set
            {
                this.Panel.FlowPadding.bottom = value;
                this.Panel.Reset();
            }
        }

        private void Start()
        {
            if (!((Object) this.Panel == (Object) null))
                return;
            this.Panel = this.GetComponent<dfScrollPanel>();
        }

        public void ExpandAll()
        {
            for (int index = 0; index < this.Panel.Controls.Count; ++index)
                this.Panel.Controls[index].GetComponent<AutoArrangeDemoItem>().Expand();
        }

        public void CollapseAll()
        {
            for (int index = 0; index < this.Panel.Controls.Count; ++index)
                this.Panel.Controls[index].GetComponent<AutoArrangeDemoItem>().Collapse();
        }
    }

