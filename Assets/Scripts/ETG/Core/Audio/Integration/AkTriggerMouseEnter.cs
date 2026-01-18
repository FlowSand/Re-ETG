using UnityEngine;

#nullable disable

public class AkTriggerMouseEnter : AkTriggerBase
    {
        private void OnMouseEnter()
        {
            if (this.triggerDelegate == null)
                return;
            this.triggerDelegate((GameObject) null);
        }
    }

