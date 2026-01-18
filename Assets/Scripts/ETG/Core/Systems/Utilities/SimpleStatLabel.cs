using UnityEngine;

#nullable disable

public class SimpleStatLabel : MonoBehaviour
    {
        public TrackedStats stat;
        protected dfLabel m_label;

        private void Start() => this.m_label = this.GetComponent<dfLabel>();

        private void Update()
        {
            if (!(bool) (Object) this.m_label || !this.m_label.IsVisible)
                return;
            this.m_label.Text = IntToStringSansGarbage.GetStringForInt(Mathf.FloorToInt(GameStatsManager.Instance.GetPlayerStatValue(this.stat)));
        }
    }

