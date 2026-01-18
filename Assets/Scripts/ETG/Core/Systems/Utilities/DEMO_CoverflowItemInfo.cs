using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Coverflow/Item Info")]
public class DEMO_CoverflowItemInfo : MonoBehaviour
    {
        public dfCoverflow scroller;
        public string[] descriptions;
        private dfLabel label;

        public void Start() => this.label = this.GetComponent<dfLabel>();

        private void Update()
        {
            if ((Object) this.scroller == (Object) null || this.descriptions == null || this.descriptions.Length == 0)
                return;
            this.label.Text = this.descriptions[Mathf.Max(0, Mathf.Min(this.descriptions.Length - 1, this.scroller.selectedIndex))];
        }
    }

