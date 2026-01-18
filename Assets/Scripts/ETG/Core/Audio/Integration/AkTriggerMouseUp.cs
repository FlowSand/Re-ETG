using UnityEngine;

#nullable disable

public class AkTriggerMouseUp : AkTriggerBase
    {
        private void OnMouseUp()
        {
            if (this.triggerDelegate == null)
                return;
            this.triggerDelegate((GameObject) null);
        }
    }

