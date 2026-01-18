using UnityEngine;

#nullable disable

public class AkTriggerMouseExit : AkTriggerBase
    {
        private void OnMouseExit()
        {
            if (this.triggerDelegate == null)
                return;
            this.triggerDelegate((GameObject) null);
        }
    }

